using Core.DB;
using Core.Models;
using Core.ViewModels;
using Logic.IHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using static Core.DB.ECollegeEnums;

namespace Logic.Helpers
{
    public class CbtHelper : ICbtHelper
    {
        private readonly AppDbContext _context;

        public CbtHelper(AppDbContext context)
        {
            _context = context;
        }

        public List<CbtTestViewModel> GetStaffCbtTests(string userId, string? search = null, string? filter = null)
        {
            var staff = GetStaff(userId);
            if (staff?.DepartmentId == null)
            {
                return new List<CbtTestViewModel>();
            }

            var query = _context.CbtTests
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Include(x => x.Questions)
                .Where(x => x.Active && x.DepartmentId == staff.DepartmentId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }

            var now = DateTime.Now;
            if (filter == "published")
            {
                query = query.Where(x => x.IsPublished);
            }
            else if (filter == "draft")
            {
                query = query.Where(x => !x.IsPublished);
            }
            else if (filter == "active")
            {
                query = query.Where(x => x.IsPublished && x.StartDateTime <= now && x.EndDateTime >= now);
            }
            else if (filter == "upcoming")
            {
                query = query.Where(x => x.StartDateTime > now);
            }
            else if (filter == "ended")
            {
                query = query.Where(x => x.EndDateTime < now);
            }

            return query
                .OrderByDescending(x => x.DateCreated)
                .AsEnumerable()
                .Select(x => MapTest(x, staff.Id, isStaff: true))
                .ToList();
        }

        public CbtTestViewModel? GetStaffCbtTest(int testId, string userId)
        {
            var staff = GetStaff(userId);
            if (staff?.DepartmentId == null)
            {
                return null;
            }

            var test = _context.CbtTests
                .Include(x => x.Department)
                .Include(x => x.CreatedBy)
                .Include(x => x.Questions)
                .FirstOrDefault(x => x.Id == testId && x.Active && x.DepartmentId == staff.DepartmentId);

            return test == null ? null : MapTest(test, staff.Id, isStaff: true);
        }

        public async Task<(bool Success, string Message, int? TestId)> CreateCbtTestAsync(CbtTestViewModel model, string staffUserId)
        {
            var staff = GetStaff(staffUserId);
            if (staff?.DepartmentId == null)
            {
                return (false, "You must be assigned to a department before creating CBT tests.", null);
            }

            var validation = ValidateTestModel(model);
            if (!validation.Success)
            {
                return (false, validation.Message, null);
            }

            var test = new CbtTest
            {
                Title = model.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                DepartmentId = staff.DepartmentId.Value,
                CreatedByUserId = staffUserId,
                DurationMinutes = model.DurationMinutes,
                StartDateTime = model.StartDateTime,
                EndDateTime = model.EndDateTime,
                TotalMarks = model.TotalMarks,
                PassMark = model.PassMark,
                MarkPerQuestion = model.MarkPerQuestion,
                MaximumAttempts = model.MaximumAttempts <= 0 ? 1 : model.MaximumAttempts,
                ShuffleQuestions = model.ShuffleQuestions,
                ShuffleOptions = model.ShuffleOptions,
                IsPublished = model.IsPublished,
                BrowserCode = string.IsNullOrWhiteSpace(model.BrowserCode) ? null : model.BrowserCode.Trim(),
                Instructions = string.IsNullOrWhiteSpace(model.Instructions) ? null : model.Instructions.Trim(),
                Active = true,
                DateCreated = DateTime.Now
            };

            _context.CbtTests.Add(test);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return (true, "CBT test created successfully.", test.Id);
        }

        public async Task<(bool Success, string Message)> UpdateCbtTestAsync(CbtTestViewModel model, string staffUserId)
        {
            var test = GetStaffTestEntity(model.Id, staffUserId);
            if (test == null)
            {
                return (false, "Test not found.");
            }

            if (!CanEditTest(test))
            {
                return (false, "This test has already started. Editing is no longer allowed.");
            }

            var validation = ValidateTestModel(model);
            if (!validation.Success)
            {
                return (false, validation.Message);
            }

            test.Title = model.Title.Trim();
            test.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            test.DurationMinutes = model.DurationMinutes;
            test.StartDateTime = model.StartDateTime;
            test.EndDateTime = model.EndDateTime;
            test.PassMark = model.PassMark;
            test.MarkPerQuestion = model.MarkPerQuestion;
            test.MaximumAttempts = model.MaximumAttempts <= 0 ? 1 : model.MaximumAttempts;
            test.ShuffleQuestions = model.ShuffleQuestions;
            test.ShuffleOptions = model.ShuffleOptions;
            test.IsPublished = model.IsPublished;
            test.BrowserCode = string.IsNullOrWhiteSpace(model.BrowserCode) ? null : model.BrowserCode.Trim();
            test.Instructions = string.IsNullOrWhiteSpace(model.Instructions) ? null : model.Instructions.Trim();

            await RecalculateTotalMarksAsync(test.Id).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return (true, "CBT test updated successfully.");
        }

        public (bool Success, string Message) DeleteCbtTest(int testId, string staffUserId)
        {
            var test = GetStaffTestEntity(testId, staffUserId);
            if (test == null)
            {
                return (false, "Test not found.");
            }

            if (!CanEditTest(test))
            {
                return (false, "This test has already started. Deletion is no longer allowed.");
            }

            test.Active = false;
            _context.SaveChanges();
            return (true, "CBT test deleted successfully.");
        }

        public bool CanEditTest(int testId)
        {
            var test = _context.CbtTests.FirstOrDefault(x => x.Id == testId && x.Active);
            return test != null && CanEditTest(test);
        }

        public List<CbtQuestionViewModel> GetQuestionsForTest(int testId, string userId, bool forTaking = false, int? seed = null)
        {
            CbtTest? test;
            if (forTaking)
            {
                test = _context.CbtTests
                    .Include(x => x.Questions)
                    .FirstOrDefault(x => x.Id == testId && x.Active && x.IsPublished);
            }
            else
            {
                test = GetStaffTestEntity(testId, userId, includeQuestions: true);
            }

            if (test == null)
            {
                return new List<CbtQuestionViewModel>();
            }

            var questions = test.Questions.Where(x => x.Active).OrderBy(x => x.SortOrder).ToList();
            if (forTaking && test.ShuffleQuestions)
            {
                var rng = seed.HasValue ? new Random(seed.Value) : new Random();
                questions = questions.OrderBy(_ => rng.Next()).ToList();
            }

            return questions.Select(q => MapQuestion(q, CanEditTest(test), forTaking, test.ShuffleOptions, seed)).ToList();
        }

        public async Task<(bool Success, string Message)> SaveQuestionAsync(
            CbtQuestionViewModel model,
            IFormFile? imageFile,
            string staffUserId,
            string webRootPath,
            int? questionId = null)
        {
            var test = GetStaffTestEntity(model.CbtTestId, staffUserId, includeQuestions: true);
            if (test == null)
            {
                return (false, "Test not found.");
            }

            if (!CanEditTest(test))
            {
                return (false, "This test has already started. Questions cannot be modified.");
            }

            if (string.IsNullOrWhiteSpace(model.QuestionText))
            {
                return (false, "Question text is required.");
            }

            if (!Enum.IsDefined(typeof(CbtQuestionType), model.QuestionType))
            {
                return (false, "Invalid question type.");
            }

            var normalizedAnswer = NormalizeAnswer(model.CorrectAnswer, model.QuestionType);
            if (string.IsNullOrWhiteSpace(normalizedAnswer))
            {
                return (false, "Correct answer is required.");
            }

            CbtQuestion question;
            if (questionId.HasValue && questionId.Value > 0)
            {
                question = test.Questions.FirstOrDefault(x => x.Id == questionId.Value && x.Active);
                if (question == null)
                {
                    return (false, "Question not found.");
                }
            }
            else
            {
                question = new CbtQuestion
                {
                    CbtTestId = test.Id,
                    SortOrder = test.Questions.Count(x => x.Active) + 1,
                    Active = true,
                    DateCreated = DateTime.Now
                };
                _context.CbtQuestions.Add(question);
            }

            question.QuestionType = model.QuestionType;
            question.QuestionText = model.QuestionText.Trim();
            question.Marks = model.Marks > 0 ? model.Marks : test.MarkPerQuestion;
            question.Explanation = string.IsNullOrWhiteSpace(model.Explanation) ? null : model.Explanation.Trim();
            question.CorrectAnswer = normalizedAnswer;

            if (model.QuestionType == CbtQuestionType.TrueFalse)
            {
                question.OptionA = "True";
                question.OptionB = "False";
                question.OptionC = null;
                question.OptionD = null;
            }
            else
            {
                question.OptionA = model.OptionA?.Trim();
                question.OptionB = model.OptionB?.Trim();
                question.OptionC = model.OptionC?.Trim();
                question.OptionD = model.OptionD?.Trim();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var savedPath = await SaveQuestionImageAsync(imageFile, webRootPath).ConfigureAwait(false);
                if (!string.IsNullOrWhiteSpace(savedPath))
                {
                    question.ImagePath = savedPath;
                }
            }
            else if (!string.IsNullOrWhiteSpace(model.ImagePath))
            {
                question.ImagePath = model.ImagePath;
            }

            await _context.SaveChangesAsync().ConfigureAwait(false);
            await RecalculateTotalMarksAsync(test.Id).ConfigureAwait(false);
            return (true, questionId.HasValue ? "Question updated successfully." : "Question added successfully.");
        }

        public (bool Success, string Message) DeleteQuestion(int questionId, string staffUserId)
        {
            var question = _context.CbtQuestions
                .Include(x => x.CbtTest)
                .FirstOrDefault(x => x.Id == questionId && x.Active);

            if (question?.CbtTest == null)
            {
                return (false, "Question not found.");
            }

            var test = GetStaffTestEntity(question.CbtTestId, staffUserId);
            if (test == null)
            {
                return (false, "Test not found.");
            }

            if (!CanEditTest(test))
            {
                return (false, "This test has already started. Questions cannot be deleted.");
            }

            question.Active = false;
            _context.SaveChanges();
            RecalculateTotalMarksAsync(test.Id).GetAwaiter().GetResult();
            return (true, "Question deleted successfully.");
        }

        public List<CbtAttemptViewModel> GetTestScores(int testId, string staffUserId)
        {
            if (GetStaffTestEntity(testId, staffUserId) == null)
            {
                return new List<CbtAttemptViewModel>();
            }

            return _context.CbtAttempts
                .Include(x => x.Student)
                .Include(x => x.CbtTest)
                .Where(x => x.CbtTestId == testId && x.IsSubmitted)
                .OrderByDescending(x => x.SubmittedAt)
                .AsEnumerable()
                .Select(MapAttemptSummary)
                .ToList();
        }

        public CbtAnalyticsViewModel? GetTestAnalytics(int testId, string staffUserId)
        {
            var test = GetStaffTestEntity(testId, staffUserId);
            if (test == null)
            {
                return null;
            }

            var attempts = _context.CbtAttempts
                .Include(x => x.Student)
                .Where(x => x.CbtTestId == testId && x.IsSubmitted)
                .ToList();

            var total = attempts.Count;
            var passed = attempts.Count(x => x.Passed);
            var failed = total - passed;

            return new CbtAnalyticsViewModel
            {
                CbtTestId = test.Id,
                TestTitle = test.Title,
                TotalAttempts = total,
                PassedCount = passed,
                FailedCount = failed,
                AverageScore = total == 0 ? 0 : Math.Round(attempts.Average(x => x.Score), 2),
                PassRate = total == 0 ? 0 : Math.Round((decimal)passed / total * 100, 2),
                FailRate = total == 0 ? 0 : Math.Round((decimal)failed / total * 100, 2),
                TotalMarks = test.TotalMarks,
                PassMark = test.PassMark,
                Attempts = attempts
                    .OrderByDescending(x => x.SubmittedAt)
                    .Select(MapAttemptSummary)
                    .ToList()
            };
        }

        public CbtStudentTestsViewModel GetStudentCbtTests(string userId)
        {
            var student = GetStudent(userId);
            if (student?.DepartmentId == null)
            {
                return new CbtStudentTestsViewModel();
            }

            var now = DateTime.Now;
            var tests = _context.CbtTests
                .Include(x => x.Department)
                .Include(x => x.Questions)
                .Include(x => x.Attempts)
                .Where(x => x.Active && x.IsPublished && x.DepartmentId == student.DepartmentId)
                .OrderByDescending(x => x.StartDateTime)
                .AsEnumerable()
                .Select(x => MapTest(x, student.Id, isStaff: false))
                .ToList();

            return new CbtStudentTestsViewModel
            {
                ActiveTests = tests.Where(x => !x.HasCompletedAttempt && !x.IsEnded).ToList(),
                CompletedTests = tests.Where(x => x.HasCompletedAttempt).ToList()
            };
        }

        public (bool Success, string Message, int? AttemptId) StartAttempt(int testId, string userId, string? browserCode)
        {
            var student = GetStudent(userId);
            if (student?.DepartmentId == null)
            {
                return (false, "Student profile not found.", null);
            }

            var test = _context.CbtTests
                .Include(x => x.Questions)
                .Include(x => x.Attempts)
                .FirstOrDefault(x => x.Id == testId && x.Active && x.IsPublished && x.DepartmentId == student.DepartmentId);

            if (test == null)
            {
                return (false, "Test not found.", null);
            }

            var now = DateTime.Now;
            if (now < test.StartDateTime || now > test.EndDateTime)
            {
                return (false, "This test is not available at the current time.", null);
            }

            if (!string.IsNullOrWhiteSpace(test.BrowserCode)
                && !string.Equals(test.BrowserCode.Trim(), browserCode?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Invalid browser code.", null);
            }

            var submittedAttempts = test.Attempts.Count(x => x.StudentUserId == userId && x.IsSubmitted);
            if (submittedAttempts >= test.MaximumAttempts)
            {
                return (false, "You have already completed this test.", null);
            }

            var inProgress = test.Attempts.FirstOrDefault(x => x.StudentUserId == userId && !x.IsSubmitted);
            if (inProgress != null)
            {
                if (IsAttemptExpired(inProgress, test))
                {
                    SubmitAttemptInternal(inProgress, test, new List<CbtStudentAnswerViewModel>(), autoSubmitted: true);
                    return (false, "Your previous attempt timed out and was submitted automatically.", inProgress.Id);
                }

                return (true, "Resuming your in-progress attempt.", inProgress.Id);
            }

            if (test.Questions.Count(x => x.Active) == 0)
            {
                return (false, "This test has no questions yet.", null);
            }

            var attempt = new CbtAttempt
            {
                CbtTestId = test.Id,
                StudentUserId = userId,
                StartedAt = now,
                TotalMarks = test.TotalMarks,
                Score = 0,
                Passed = false,
                IsSubmitted = false,
                AutoSubmitted = false
            };

            _context.CbtAttempts.Add(attempt);
            _context.SaveChanges();
            return (true, "Test started.", attempt.Id);
        }

        public CbtAttemptViewModel? GetAttemptForTaking(int attemptId, string userId)
        {
            var attempt = _context.CbtAttempts
                .Include(x => x.CbtTest)
                .ThenInclude(x => x!.Questions)
                .FirstOrDefault(x => x.Id == attemptId && x.StudentUserId == userId);

            if (attempt?.CbtTest == null || attempt.IsSubmitted)
            {
                return null;
            }

            var test = attempt.CbtTest;
            if (IsAttemptExpired(attempt, test))
            {
                SubmitAttemptInternal(attempt, test, new List<CbtStudentAnswerViewModel>(), autoSubmitted: true);
                return null;
            }

            var seed = attempt.Id;
            var questions = GetQuestionsForTest(test.Id, userId, forTaking: true, seed: seed);
            var remaining = GetRemainingSeconds(attempt, test);

            return new CbtAttemptViewModel
            {
                Id = attempt.Id,
                CbtTestId = test.Id,
                TestTitle = test.Title,
                StartedAt = attempt.StartedAt,
                DurationMinutes = test.DurationMinutes,
                RemainingSeconds = remaining,
                Instructions = test.Instructions,
                RequiresBrowserCode = !string.IsNullOrWhiteSpace(test.BrowserCode),
                Questions = questions
            };
        }

        public async Task<(bool Success, string Message, CbtAttemptViewModel? Result)> SubmitAttemptAsync(
            int attemptId,
            string userId,
            List<CbtStudentAnswerViewModel> answers,
            bool autoSubmitted)
        {
            var attempt = await _context.CbtAttempts
                .Include(x => x.CbtTest)
                .ThenInclude(x => x!.Questions)
                .FirstOrDefaultAsync(x => x.Id == attemptId && x.StudentUserId == userId)
                .ConfigureAwait(false);

            if (attempt?.CbtTest == null)
            {
                return (false, "Attempt not found.", null);
            }

            if (attempt.IsSubmitted)
            {
                return (false, "This attempt has already been submitted.", GetAttemptResult(attemptId, userId));
            }

            SubmitAttemptInternal(attempt, attempt.CbtTest, answers ?? new List<CbtStudentAnswerViewModel>(), autoSubmitted);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return (true, autoSubmitted ? "Test auto-submitted." : "Test submitted successfully.", GetAttemptResult(attemptId, userId));
        }

        public CbtAttemptViewModel? GetAttemptResult(int attemptId, string userId, bool isStaff = false)
        {
            var attempt = _context.CbtAttempts
                .Include(x => x.Student)
                .Include(x => x.CbtTest)
                .Include(x => x.Answers)
                .ThenInclude(x => x.CbtQuestion)
                .FirstOrDefault(x => x.Id == attemptId);

            if (attempt?.CbtTest == null || !attempt.IsSubmitted)
            {
                return null;
            }

            if (isStaff)
            {
                if (GetStaffTestEntity(attempt.CbtTestId, userId) == null)
                {
                    return null;
                }
            }
            else if (attempt.StudentUserId != userId)
            {
                return null;
            }

            var vm = MapAttemptSummary(attempt);
            vm.PassMark = attempt.CbtTest.PassMark;
            vm.Answers = attempt.Answers.Select(a => new CbtStudentAnswerViewModel
            {
                QuestionId = a.CbtQuestionId,
                SelectedAnswer = a.SelectedAnswer,
                IsCorrect = a.IsCorrect,
                MarksAwarded = a.MarksAwarded,
                CorrectAnswer = a.CbtQuestion?.CorrectAnswer,
                Explanation = a.CbtQuestion?.Explanation
            }).ToList();

            return vm;
        }

        private void SubmitAttemptInternal(CbtAttempt attempt, CbtTest test, List<CbtStudentAnswerViewModel> answers, bool autoSubmitted)
        {
            var questions = test.Questions.Where(x => x.Active).ToList();
            decimal score = 0;

            foreach (var question in questions)
            {
                var submitted = answers.FirstOrDefault(x => x.QuestionId == question.Id);
                var selected = submitted?.SelectedAnswer;
                var isCorrect = IsAnswerCorrect(selected, question);
                var marksAwarded = isCorrect ? question.Marks : 0;
                score += marksAwarded;

                _context.CbtStudentAnswers.Add(new CbtStudentAnswer
                {
                    CbtAttemptId = attempt.Id,
                    CbtQuestionId = question.Id,
                    SelectedAnswer = selected,
                    IsCorrect = isCorrect,
                    MarksAwarded = marksAwarded
                });
            }

            attempt.Score = score;
            attempt.Passed = score >= test.PassMark;
            attempt.IsSubmitted = true;
            attempt.AutoSubmitted = autoSubmitted;
            attempt.SubmittedAt = DateTime.Now;
            attempt.TotalMarks = test.TotalMarks;
        }

        private static bool IsAnswerCorrect(string? selected, CbtQuestion question)
        {
            if (string.IsNullOrWhiteSpace(selected))
            {
                return false;
            }

            if (question.QuestionType == CbtQuestionType.MultipleChoice)
            {
                return NormalizeMultiple(selected) == NormalizeMultiple(question.CorrectAnswer);
            }

            return string.Equals(
                NormalizeSingle(selected, question.QuestionType),
                NormalizeSingle(question.CorrectAnswer, question.QuestionType),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeAnswer(string? answer, CbtQuestionType type)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                return string.Empty;
            }

            return type == CbtQuestionType.MultipleChoice
                ? NormalizeMultiple(answer)
                : NormalizeSingle(answer, type);
        }

        private static string NormalizeSingle(string answer, CbtQuestionType type)
        {
            var trimmed = answer.Trim();
            if (type == CbtQuestionType.TrueFalse)
            {
                if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) || trimmed == "A")
                {
                    return "True";
                }
                if (trimmed.Equals("false", StringComparison.OrdinalIgnoreCase) || trimmed == "B")
                {
                    return "False";
                }
            }

            return trimmed.ToUpperInvariant();
        }

        private static string NormalizeMultiple(string answer)
        {
            return string.Join(",", answer.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => x.ToUpperInvariant())
                .Distinct()
                .OrderBy(x => x));
        }

        private async Task RecalculateTotalMarksAsync(int testId)
        {
            var total = await _context.CbtQuestions
                .Where(x => x.CbtTestId == testId && x.Active)
                .SumAsync(x => x.Marks)
                .ConfigureAwait(false);

            var test = await _context.CbtTests.FirstOrDefaultAsync(x => x.Id == testId).ConfigureAwait(false);
            if (test != null)
            {
                test.TotalMarks = total;
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private static async Task<string?> SaveQuestionImageAsync(IFormFile imageFile, string webRootPath)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                return null;
            }

            var folder = Path.Combine(webRootPath, "uploads", "cbt", "questions");
            Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);
            await using var stream = new FileStream(fullPath, FileMode.Create);
            await imageFile.CopyToAsync(stream).ConfigureAwait(false);
            return $"/uploads/cbt/questions/{fileName}";
        }

        private CbtTestViewModel MapTest(CbtTest test, string userId, bool isStaff)
        {
            var now = DateTime.Now;
            var hasStarted = now >= test.StartDateTime;
            var isActiveNow = test.IsPublished && now >= test.StartDateTime && now <= test.EndDateTime;
            var isUpcoming = now < test.StartDateTime;
            var isEnded = now > test.EndDateTime;

            var studentAttempts = test.Attempts?
                .Where(x => x.StudentUserId == userId)
                .OrderByDescending(x => x.StartedAt)
                .ToList() ?? new List<CbtAttempt>();

            var latestSubmitted = studentAttempts.FirstOrDefault(x => x.IsSubmitted);
            var hasCompleted = latestSubmitted != null;
            var canTake = !isStaff && test.IsPublished && isActiveNow && !hasCompleted
                && studentAttempts.Count(x => x.IsSubmitted) < test.MaximumAttempts;

            var status = !test.IsPublished ? "Draft"
                : isActiveNow ? "Active"
                : isUpcoming ? "Upcoming"
                : "Ended";

            return new CbtTestViewModel
            {
                Id = test.Id,
                Title = test.Title,
                Description = test.Description,
                DepartmentId = test.DepartmentId,
                DepartmentName = test.Department?.Name,
                CreatedByUserId = test.CreatedByUserId,
                StaffName = test.CreatedBy == null ? null : $"{test.CreatedBy.FirstName} {test.CreatedBy.LastName}".Trim(),
                DurationMinutes = test.DurationMinutes,
                StartDateTime = test.StartDateTime,
                EndDateTime = test.EndDateTime,
                TotalMarks = test.TotalMarks,
                PassMark = test.PassMark,
                MarkPerQuestion = test.MarkPerQuestion,
                MaximumAttempts = test.MaximumAttempts,
                ShuffleQuestions = test.ShuffleQuestions,
                ShuffleOptions = test.ShuffleOptions,
                IsPublished = test.IsPublished,
                BrowserCode = test.BrowserCode,
                Instructions = test.Instructions,
                Active = test.Active,
                DateCreated = test.DateCreated,
                QuestionCount = test.Questions?.Count(x => x.Active) ?? 0,
                CanEdit = CanEditTest(test),
                HasStarted = hasStarted,
                IsActiveNow = isActiveNow,
                IsUpcoming = isUpcoming,
                IsEnded = isEnded,
                CanTake = canTake,
                HasCompletedAttempt = hasCompleted,
                StatusLabel = status,
                ScheduleDisplay = $"{test.StartDateTime:ddd, MMM d yyyy · h:mm tt} – {test.EndDateTime:h:mm tt}",
                LatestAttemptId = latestSubmitted?.Id,
                LatestScore = latestSubmitted?.Score,
                LatestPassed = latestSubmitted?.Passed
            };
        }

        private CbtQuestionViewModel MapQuestion(CbtQuestion q, bool canEdit, bool forTaking, bool shuffleOptions, int? seed)
        {
            var options = BuildOptions(q);
            if (forTaking && shuffleOptions && seed.HasValue)
            {
                var rng = new Random(seed.Value + q.Id);
                options = options.OrderBy(_ => rng.Next()).ToList();
            }

            return new CbtQuestionViewModel
            {
                Id = q.Id,
                CbtTestId = q.CbtTestId,
                QuestionType = q.QuestionType,
                QuestionText = q.QuestionText,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                CorrectAnswer = q.CorrectAnswer,
                Marks = q.Marks,
                Explanation = q.Explanation,
                ImagePath = q.ImagePath,
                SortOrder = q.SortOrder,
                CanEdit = canEdit,
                DisplayOptions = options
            };
        }

        private static List<CbtOptionViewModel> BuildOptions(CbtQuestion q)
        {
            if (q.QuestionType == CbtQuestionType.TrueFalse)
            {
                return new List<CbtOptionViewModel>
                {
                    new() { Key = "True", Text = "True" },
                    new() { Key = "False", Text = "False" }
                };
            }

            var list = new List<CbtOptionViewModel>();
            if (!string.IsNullOrWhiteSpace(q.OptionA)) list.Add(new CbtOptionViewModel { Key = "A", Text = q.OptionA });
            if (!string.IsNullOrWhiteSpace(q.OptionB)) list.Add(new CbtOptionViewModel { Key = "B", Text = q.OptionB });
            if (!string.IsNullOrWhiteSpace(q.OptionC)) list.Add(new CbtOptionViewModel { Key = "C", Text = q.OptionC });
            if (!string.IsNullOrWhiteSpace(q.OptionD)) list.Add(new CbtOptionViewModel { Key = "D", Text = q.OptionD });
            return list;
        }

        private CbtAttemptViewModel MapAttemptSummary(CbtAttempt attempt)
        {
            return new CbtAttemptViewModel
            {
                Id = attempt.Id,
                CbtTestId = attempt.CbtTestId,
                TestTitle = attempt.CbtTest?.Title,
                StudentUserId = attempt.StudentUserId,
                StudentName = attempt.Student == null ? null : $"{attempt.Student.FirstName} {attempt.Student.LastName}".Trim(),
                StudentEmail = attempt.Student?.Email,
                StartedAt = attempt.StartedAt,
                SubmittedAt = attempt.SubmittedAt,
                Score = attempt.Score,
                TotalMarks = attempt.TotalMarks,
                Passed = attempt.Passed,
                IsSubmitted = attempt.IsSubmitted,
                AutoSubmitted = attempt.AutoSubmitted
            };
        }

        private static bool CanEditTest(CbtTest test) => DateTime.Now < test.StartDateTime;

        private static bool IsAttemptExpired(CbtAttempt attempt, CbtTest test)
        {
            return DateTime.Now >= attempt.StartedAt.AddMinutes(test.DurationMinutes);
        }

        private static int GetRemainingSeconds(CbtAttempt attempt, CbtTest test)
        {
            var end = attempt.StartedAt.AddMinutes(test.DurationMinutes);
            var testEnd = test.EndDateTime;
            var effectiveEnd = end < testEnd ? end : testEnd;
            var remaining = (int)Math.Max(0, (effectiveEnd - DateTime.Now).TotalSeconds);
            return remaining;
        }

        private static (bool Success, string Message) ValidateTestModel(CbtTestViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
            {
                return (false, "Test title is required.");
            }

            if (model.DurationMinutes <= 0)
            {
                return (false, "Duration must be greater than zero.");
            }

            if (model.EndDateTime <= model.StartDateTime)
            {
                return (false, "End date must be after start date.");
            }

            if (model.PassMark < 0)
            {
                return (false, "Pass mark cannot be negative.");
            }

            if (model.MarkPerQuestion <= 0)
            {
                return (false, "Mark per question must be greater than zero.");
            }

            return (true, string.Empty);
        }

        private ApplicationUser? GetStaff(string userId) =>
            _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated && x.IsAdmin);

        private ApplicationUser? GetStudent(string userId) =>
            _context.ApplicationUser.FirstOrDefault(x => x.Id == userId && !x.Deactivated && x.IsStudent && !x.IsAdmin);

        private CbtTest? GetStaffTestEntity(int testId, string staffUserId, bool includeQuestions = false)
        {
            var staff = GetStaff(staffUserId);
            if (staff?.DepartmentId == null)
            {
                return null;
            }

            IQueryable<CbtTest> query = _context.CbtTests;
            if (includeQuestions)
            {
                query = query.Include(x => x.Questions);
            }

            return query.FirstOrDefault(x => x.Id == testId && x.Active && x.DepartmentId == staff.DepartmentId);
        }
    }
}
