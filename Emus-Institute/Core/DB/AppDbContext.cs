using Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DB
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Assignment>()
                .HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssignmentSubmission>()
                .HasOne(x => x.Student)
                .WithMany()
                .HasForeignKey(x => x.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssignmentSubmission>()
                .HasOne(x => x.GradedBy)
                .WithMany()
                .HasForeignKey(x => x.GradedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AssignmentSubmission>()
                .HasIndex(x => new { x.AssignmentId, x.StudentUserId })
                .IsUnique();
        }
        public DbSet<CommonDropdowns> CommonDropdowns { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<StaffDocumentation> StaffDocuments { get; set; }
        public DbSet<Suspension> Suspensions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Textbooks> Textbooks { get; set; }
        public DbSet<Complains> Complain { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }
        public DbSet<EnquiryMessage> Enquiries { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PayStack> PayStackpayments { get; set; }
        public DbSet<EvaluationDetails> EvaluationDetails { get; set; }
        public DbSet<StaffEvaluationDetails> StaffEvaluationDetails { get; set; }
        public DbSet<LiveSession> LiveSessions { get; set; }
        public DbSet<CbtTest> CbtTests { get; set; }
        public DbSet<CbtQuestion> CbtQuestions { get; set; }
        public DbSet<CbtAttempt> CbtAttempts { get; set; }
        public DbSet<CbtStudentAnswer> CbtStudentAnswers { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<StudyCenter> StudyCenters { get; set; }
        public DbSet<AssessmentRegistration> AssessmentRegistrations { get; set; }


        //public DbSet<PaymentForm> PaymentForms { get; set; }
        //public DbSet<PaymentForm> PaymentForms { get; set; }
        //public DbSet<ExamQuestions> ExamQuestions { get; set; }
        //public DbSet<ExamAnswerOptions> ExamAnswerOptions { get; set; }
        //public DbSet<ExamDuration> ExamDurations { get; set; }
        //public DbSet<ExamResult> ExamResults { get; set; }
        //public DbSet<UserVerification> UserVerifications { get; set; }

    }
}
