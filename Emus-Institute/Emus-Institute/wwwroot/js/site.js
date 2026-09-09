
function RegisterStudent() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var urlParams = new URLSearchParams(window.location.search);
    var refLink = urlParams.get('rl');

    //var selectedUserId = [];
    //$('.user-checkbox:checked').each(function () {
    //    selectedUserId.push($(this).data('user-id'));
    //});

    var data = {};
    data.DepartmentId = $('#deptId').val();
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.OtherName = $('#othername').val();
    data.Phonenumber = $('#phonenumber').val();
    data.Email = $('#email').val();
    data.Password = $('#password').val();
    data.ConfirmPassword = $('#confirmPassword').val();
    data.State = $('#state').val();
    data.Country = $('#country').val();
    data.Address = $('#address').val();
    data.DOB = $('#dateOfBirth').val();
    if (data.DOB == "") {
        data.DOB = "0001-01-01T00:00:00"
    };

    if (data.Phonenumber == "" || data.Phonenumber == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the phonenumber");
        return;
    }

    if (data.State == "" || data.State == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the State of Residence");
        return;
    }
    if (data.Country == "" || data.Country == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Country of Residence");
        return;
    }
    if (data.Address == "" || data.Address == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Residential Address");
        return;
    }

    let userDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/StudentRegistration',
        dataType: 'json',
        data:
        {
            userDetails: userDetails,
            refLink: refLink,
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })

}

function RegisterScholarshipStudent() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var urlParams = new URLSearchParams(window.location.search);
    var refLink = urlParams.get('rl');

    var data = {};
    data.DepartmentId = $('#deptId').val();
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.OtherName = $('#othername').val();
    data.Phonenumber = $('#phonenumber').val();
    data.Email = $('#email').val();
    data.Password = $('#password').val();
    data.ConfirmPassword = $('#confirmPassword').val();
    data.State = $('#state').val();
    data.Country = $('#country').val();
    data.Address = $('#address').val();
    data.DOB = $('#dateOfBirth').val();
    if (data.DOB == "") {
        data.DOB = "0001-01-01T00:00:00"
    };

    if (data.Phonenumber == "" || data.Phonenumber == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the phonenumber");
        return;
    }

    if (data.State == "" || data.State == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the State of Residence");
        return;
    }
    if (data.Country == "" || data.Country == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Country of Residence");
        return;
    }
    if (data.Address == "" || data.Address == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Residential Address");
        return;
    }

    let userDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/ScholarshipRegistration',
        dataType: 'json',
        data:
        {
            userDetails: userDetails,
            refLink: refLink,
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })
}

function RegisterCohortStudent() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var urlParams = new URLSearchParams(window.location.search);
    var refLink = urlParams.get('rl');

    var data = {};
    data.DepartmentId = $('#deptId').val();
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.OtherName = $('#othername').val();
    data.Phonenumber = $('#phonenumber').val();
    data.Email = $('#email').val();
    data.Password = $('#password').val();
    data.ConfirmPassword = $('#confirmPassword').val();
    data.State = $('#state').val();
    data.Country = $('#country').val();
    data.Address = $('#address').val();
    data.DOB = $('#dateOfBirth').val();
    data.IsCohort = true;
    if (data.DOB == "") {
        data.DOB = "0001-01-01T00:00:00"
    };

    if (data.Phonenumber == "" || data.Phonenumber == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the phonenumber");
        return;
    }

    if (data.State == "" || data.State == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the State of Residence");
        return;
    }
    if (data.Country == "" || data.Country == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Country of Residence");
        return;
    }
    if (data.Address == "" || data.Address == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in the Residential Address");
        return;
    }

    let userDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/CohortRegistration',
        dataType: 'json',
        data:
        {
            userDetails: userDetails,
            refLink: refLink,
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    })
}

//function validateForm(id, message) {
//    debugger;
//    $("#" + id + "Error").text(message).css({ color: "red" });
//    $("#" + id).css({ border: "1px solid red" });
//}

async function EvaluateStaff() {
    const defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    const userId = $('#userId').val();

    function toBase64(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = error => reject(error);
        });
    }

    try {
        const passportFile = document.getElementById("passport").files[0];
        const transcriptFile = document.getElementById("transcript").files[0];
        const highSchCertFile = document.getElementById("highSchCert").files[0];
        const waecScratchCardFile = document.getElementById("waecScratchCard").files[0];
        const anyRelevantCertFile = document.getElementById("anyRelevantCert").files[0];

        if (!passportFile || !transcriptFile || !highSchCertFile || !waecScratchCardFile || !anyRelevantCertFile) {
            errorAlert("Please attach all required files before submitting.");
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            return;
        }

        const [passport, transcript, highSchCert, waecScratchCard, anyRelevantCert] = await Promise.all([
            toBase64(passportFile),
            toBase64(transcriptFile),
            toBase64(highSchCertFile),
            toBase64(waecScratchCardFile),
            toBase64(anyRelevantCertFile)
        ]);

        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/AcademicStaff/EvaluateStaffDetails',
            data: {
                userId,
                passport,
                transcript,
                highSchCert,
                waecScratchCard,
                anyRelevantCert
            },
            success: function (result) {
                if (!result.isError) {
                    successAlertWithRedirect(result.msg, result.data);
                } else if (result.isError && result.url) {
                    window.location.href = result.url;
                } else {
                    $('#submit_btn').html(defaultBtnValue);
                    $('#submit_btn').attr("disabled", false);
                    errorAlert(result.msg);
                }
            },
            error: function () {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert("An error occurred. Please contact admin if issue persists.");
            },
        });
    } catch (error) {
        console.error("File reading error:", error);
        errorAlert("Error reading files. Please try again.");
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
    }
}

async function Evaluate() {
   
    const defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    const UserId = $('#userId').val();

    // Helper: convert file to Base64
    function toBase64(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = error => reject(error);
        });
    }

    try {
        const passportFile = document.getElementById("passport").files[0];
        const transcriptFile = document.getElementById("transcript").files[0];
        const highSchCertFile = document.getElementById("highSchCert").files[0];
        const waecScratchCardFile = document.getElementById("waecScratchCard").files[0];
        const anyRelevantCertFile = document.getElementById("anyRelevantCert").files[0];

        if (!passportFile || !transcriptFile || !highSchCertFile || !waecScratchCardFile || !anyRelevantCertFile) {
            errorAlert("Please attach all required files before submitting.");
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            return;
        }

        // Wait for all files to convert

        const [passport, transcript, highSchCert, waecScratchCard, anyRelevantCert] = await Promise.all([
            toBase64(passportFile),
            toBase64(transcriptFile),
            toBase64(highSchCertFile),
            toBase64(waecScratchCardFile),
            toBase64(anyRelevantCertFile)
        ]);

        // Now safely call your endpoint
        
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: '/Account/EvaluateUserDetails',
            data: {
                UserId,
                passport,
                transcript,
                highSchCert,
                waecScratchCard,
                anyRelevantCert
            },
            success: function (result) {

                if (!result.isError) {
                    successAlertWithRedirect(result.msg, result.data);

                } else if (result.isError && result.url) {

                    window.location.href = result.url;
                } else {

                    $('#submit_btn').html(defaultBtnValue);
                    $('#submit_btn').attr("disabled", false);
                    errorAlert(result.msg);
                }
            },
            error: function (ex) {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert("An error occurred. Please contact admin if issue persists.");
            },
        });
    } catch (error) {
        console.error("File reading error:", error);
        errorAlert("Error reading files. Please try again.");
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
    }
}



//async function Evaluate() {
//    debugger
//    var defaultBtnValue = $('#submit_btn').html();
//    $('#submit_btn').html("Please wait...");
//    $('#submit_btn').attr("disabled", true);

//    //var data = {}
//    var UserId = $('#userId').val();

//    var passport = document.getElementById("passport").files;
//    debugger
//    if (passport.length > 0 || passport[0] != null) {
//        const reader = new FileReader();
//        reader.readAsDataURL(passport[0]);
//        reader.onload = function () {
//        passport = reader.result;
//        }
//    } else {
//        $('#submit_btn').html(defaultBtnValue);
//        $('#submit_btn').attr("disabled", false);
//        errorAlert("Please attach your transcript");
//        return;
//    }

//    var transcript = document.getElementById("transcript").files;
//    debugger
//    if (transcript.length > 0 || transcript[0] != null) {
//        const reader = new FileReader();
//        reader.readAsDataURL(transcript[0]);
//        reader.onload = function () {
//        transcript = reader.result;
//        }
//    } else {
//        $('#submit_btn').html(defaultBtnValue);
//        $('#submit_btn').attr("disabled", false);
//        errorAlert("Please attach your transcript");
//        return;
//    }
//    var highSchCert = document.getElementById("highSchCert").files;
//    debugger
//    if (highSchCert.length > 0 || highSchCert[0] != null) {
//        const reader = new FileReader();
//        reader.readAsDataURL(highSchCert[0]);
//        reader.onload = function () {
//            highSchCert = reader.result;
//        }
//    } else {
//        $('#submit_btn').html(defaultBtnValue);
//        $('#submit_btn').attr("disabled", false);
//        errorAlert("Please attach your High School Certificate");
//        return;
//    }
//    var waecScratchCard = document.getElementById("waecScratchCard").files;
//    debugger
//    if (waecScratchCard.length > 0 || waecScratchCard[0] != null) {
//        const reader = new FileReader();
//        reader.readAsDataURL(waecScratchCard[0]);
//        reader.onload = function () {
//            waecScratchCard = reader.result;
//        }
//    } else {
//        $('#submit_btn').html(defaultBtnValue);
//        $('#submit_btn').attr("disabled", false);
//        errorAlert("Please attach your WAEC ScratchCard");
//        return;
//    }
//    var anyRelevantCert = document.getElementById("anyRelevantCert").files;
//    debugger
//    if (anyRelevantCert.length > 0 || anyRelevantCert[0] != null) {
//        const reader = new FileReader();
//        reader.readAsDataURL(anyRelevantCert[0]);
//        reader.onload = function () {
//            anyRelevantCert = reader.result;
//        }
//    } else {
//        $('#submit_btn').html(defaultBtnValue);
//        $('#submit_btn').attr("disabled", false);
//        errorAlert("Please attach any other relevant certificate");
//        return;
//    }
//    debugger
//    $.ajax({
//        type: 'Post',
//        dataType: 'Json',
//        url: '/Account/EvaluateUserDetails',
//        data: {
//            UserId: UserId,
//            passport: passport,
//            transcript: transcript,
//            highSchCert: highSchCert,
//            waecScratchCard: waecScratchCard,
//            anyRelevantCert: anyRelevantCert
//        },

//        success: function (result)
//        {
//            if (!result.isError) {
//                var url = result.data;
//                SuccessAlert(result.msg, url)
//            }
//            else if (result.isError == true && result.url != null) {

//                window.location.href = result.url;  
//            }
//            else {
//                ErrorAlert(result.msg)
//            }
//        },
//        error: function (ex) {
//            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
//        }
//    })
     
//}

function login() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var email = $('#email').val();
    var password = $('#password').val();
    $.ajax({
        type: 'Post',
        url: '/Account/Login',
        dataType: 'json',
        data:
        {
            email: email,
            password: password
        },
        success: function (result) {
            if (!result.isError) {
                var n = 1;
                localStorage.removeItem("on_load_counter");
                localStorage.setItem("on_load_counter", n);
                location.replace(result.dashboard);
                return;
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("An error occured, please try again.");
        }
    });
}

function toggleScholarshipAmount(prefix) {
    var isChecked = $('#' + prefix + '_IsUnderScholarship').is(':checked');
    $('#' + prefix + '_scholarship_amount_wrap').toggle(isChecked);
    if (!isChecked) {
        $('#' + prefix + '_ScholarshipAmount').val('');
    }
}

function addDept() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var data = {};
    data.Name = $('#dept_Name').val();
    data.Description = $('#dept_Desc').val();
    data.IsUnderScholarship = $('#dept_IsUnderScholarship').is(':checked');
    data.ScholarshipAmount = data.IsUnderScholarship ? $('#dept_ScholarshipAmount').val() : null;

    //if (data.Name != "") {
    //    $('#submit_btn').html(defaultBtnValue);
    //    $('#submit_btn').attr("disabled", false);
    //    errorAlert("Please fill the department name");
    //    return;
    //}

    //if (data.Description != "") {
    //    $('#submit_btn').html(defaultBtnValue);
    //    $('#submit_btn').attr("disabled", false);
    //    errorAlert("Please fill the department description");
    //    return;
    //}

    if (data.Name != "" && data.Description != "") {
        let deptDetails = JSON.stringify(data);
        $.ajax({
            type: 'Post',
            url: '/SuperAdmin/CreateDepartment',
            dataType: 'json',
            data:
            {
                deptDetails: deptDetails,
            },
            success: function (result) {
                if (!result.isError) {
                    var url = '/SuperAdmin/Departments';
                    successAlertWithRedirect(result.msg, url);
                    $('#submit_btn').html(defaultBtnValue);
                }
                else {
                    $('#submit_btn').html(defaultBtnValue);
                    $('#submit_btn').attr("disabled", false);
                    errorAlert(result.msg);
                }
            },
            error: function (ex) {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert("Please check and try again. Contact Admin if issue persists..");
            }
        });
    } else {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill the form Correctly");
    }
}

function deptToBeEdited(id) {
    $.ajax({
        type: 'Get',
        dataType: 'Json',
        url: '/SuperAdmin/EditDepartment',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                $('#dept_id').val(result.id);
                $('#edit_dept_Name').val(result.name);
                $('#edit_Desc').val(result.description);
                $('#edit_dept_IsUnderScholarship').prop('checked', result.isUnderScholarship);
                if (result.scholarshipAmount != null && result.scholarshipAmount !== '') {
                    $('#edit_dept_ScholarshipAmount').val(result.scholarshipAmount);
                } else {
                    $('#edit_dept_ScholarshipAmount').val('');
                }
                toggleScholarshipAmount('edit_dept');
                $('#edit_dept').modal('show');
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function SaveEditedDept() {
    var defaultBtnValue = $('#submit_Btn').html();
    $('#submit_Btn').html("Please wait...");
    $('#submit_Btn').attr("disabled", true);

    var data = {};
    data.Id = $("#dept_id").val();
    data.Name = $("#edit_dept_Name").val();
    data.Description = $("#edit_Desc").val();
    data.IsUnderScholarship = $('#edit_dept_IsUnderScholarship').is(':checked');
    data.ScholarshipAmount = data.IsUnderScholarship ? $('#edit_dept_ScholarshipAmount').val() : null;
    if (data.Name != "" && data.Description != "") {
        let editDept = JSON.stringify(data);
        $.ajax({
            type: 'POST',
            url: '/SuperAdmin/EditedDepartment',
            dataType: 'json',
            data:
            {
                editDept: editDept,
            },
            success: function (result) {
                if (!result.isError) {
                    var url = '/SuperAdmin/Departments'
                    successAlertWithRedirect(result.msg, url)
                    $('#submit_Btn').html(defaultBtnValue);
                }
                else {
                    $('#submit_Btn').html(defaultBtnValue);
                    $('#submit_Btn').attr("disabled", false);
                    errorAlert(result.msg);
                }
            },
            error: function (ex) {
                $('#submit_Btn').html(defaultBtnValue);
                $('#submit_Btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        });
    } else {
        $('#submit_Btn').html(defaultBtnValue);
        $('#submit_Btn').attr("disabled", false);
        errorAlert("Invalid, Please fill the form correctly.");
    }
}

function DeleteDept() {
    var id = $('#dept_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/SuperAdmin/DeleteDepartment',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/Departments'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function deptToDelete(id) {
    $('#dept_id').val(id);
    $('#delete_dept').modal('show');
}

function approveStudent(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/StudentApproval',
        dataType: 'json',
        data: {
            userId: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/RegisteredStudents';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            "Something went wrong, contact the support - " + errorAlert(ex);
        }
    });
}
function declineStudent(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/DeclineStudent', // we are calling json method
        dataType: 'json',
        data:
        {
            userId: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/RegisteredStudents';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Please, Contact the Support for --- " + ex);
        }
    });
}

function approveScholarshipStudent(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/ScholarshipStudentApproval',
        dataType: 'json',
        data: {
            userId: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/ScholarshipStudents';
                successAlertWithRedirect(result.msg, url);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Something went wrong, contact the support - " + ex);
        }
    });
}

function declineScholarshipStudent(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/DeclineScholarshipStudent',
        dataType: 'json',
        data: {
            userId: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/ScholarshipStudents';
                successAlertWithRedirect(result.msg, url);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Please, Contact the Support for --- " + ex);
        }
    });
}

function copyScholarshipRegistrationLink() {
    $.ajax({
        type: 'GET',
        url: '/SuperAdmin/ScholarshipRegistrationLink',
        success: function (data) {
            navigator.clipboard.writeText(data);
            successAlert("Scholarship registration link copied successfully");
        },
        error: function () {
            errorAlert("Unable to copy scholarship registration link. Please try again.");
        }
    });
}

async function RegisterStaff() {
  
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var data = {};
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.OtherName = $('#othername').val();
    data.Phonenumber = $('#phonenumber').val();
    data.Email = $('#email').val();
    data.State = $('#state').val();
    data.Country = $('#country').val();
    data.Address = $('#address').val();
    data.DOB = $('#dateOfBirth').val();
    if (data.DOB == "") {
        data.DOB = "0001-01-01T00:00:00"
    };
    var appLetter = $('#appLetter').val();
    data.DepartmentId = $('#deptId').val();
    var StaffPosition;
    //var validId = document.getElementById("validId").files;
    //var resume = document.getElementById("resume").files;
    if (data.DepartmentId > 0) {
        StaffPosition = "";
    } else {
       // StaffPosition = $('.user-checkbox').data('user-id');
        StaffPosition = $('.user-radio:checked').data('user-id');
    }
    
    if (data.Phonenumber == "" || data.Phonenumber == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your phonenumber");
        return;
    }
    if (data.State == "" || data.State == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your state of Residence");
        return;
    }
    if (data.Country == "" || data.Country == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill in your country of residence");
        return;
    }
    if (appLetter == "" || appLetter == undefined) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please add your cover letter");
        return;
    }

    var resumeFile = document.getElementById("resume").files[0];
    var validIdFile = document.getElementById("validId").files[0];

    if (!resumeFile) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please attach your resume");
        return;
    }
    if (!validIdFile) {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please attach your valid Id");
        return;
    }
    function toBase64(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = reject;
        });
    }

    var resume = await toBase64(resumeFile);
    var validId = await toBase64(validIdFile);

    let userDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/StaffRegistration',
        dataType: 'json',
        data:
        {
            userDetails: userDetails,
            staffPosition: StaffPosition,
            appLetter: appLetter,
            validId: validId,
            resume: resume,
        },
        success: function (result) {
          
            if (!result.isError) {
                var url = '/Account/Login';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                $('#submit_btn').html(defaultBtnValue);
                $('#submit_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            $('#submit_btn').html(defaultBtnValue);
            $('#submit_btn').attr("disabled", false);
            errorAlert("Please check and try again. Contact Admin if issue persists..");
        },
    });
}

function ReferralLink() {
    
    $.ajax({
        type: 'GET',
        url: '/AcademicStaff/ReferralLink',
        success: function (data) {
            if (!data.isError) {
                var text = data;
                navigator.clipboard.writeText(text)
                successAlert("Referral link copied successfully");
            }
            else {
                location.replace(data.dashboard);
            }
        }
    });
}
function viewCoverLetter(id) {
    if (id != null) {
        $.ajax({
            type: 'get',
            dataType: 'json',
            url: '/Account/GetCoverLetter',
            data: {
                Id: id,
            },
            success: function (result) {
                if (!result.isError) {
                    $("#viewCoverLetter").val(result.data.applicationLetter);

                } else {
                    errorAlert(result.msg);
                }
            },
            error: function (ex) {
                errorAlert("Network failure, please try again");
            }
        });
    } else {
        errorAlert("Invalid Id");
    }
}

function approveApplication(id) {
    
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/ApproveApplication',
        dataType: 'json',
        data: {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/PendingApplication';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            "Something went wrong, contact the support - " + errorAlert(ex);
        }
    });
}

function declineApplication(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/DeclineApplication', // we are calling json method
        dataType: 'json',
        data:
        {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/PendingApplication';
                successAlertWithRedirect(result.msg, url);
                $('#submit_btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Please, Contact the Support for --- " + ex);
        }
    });
}

function viewIDImage(imageUrl) {
    var imageElement = document.getElementById('ImageId');
    imageElement.src = imageUrl;
}

function viewResume(base64Pdf) {
   
    const pdfElement = document.getElementById('resumePdfViewer');

    if (base64Pdf.startsWith("data:")) {
        pdfElement.src = base64Pdf;
    } else {
        pdfElement.src = "data:application/pdf;base64," + base64Pdf;
    }
}

$(document).ready(function () {
    $('#dataTable').DataTable();
});

function userToSuspend(id) {
    $('#user_id').val(id);
    $('#suspend_user').modal('show');
}
function SuspendUser() {

    var userId = $('#user_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/SuperAdmin/SuspendUser',
        data: {
            userId: userId
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/Index'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function userToDeactivate(id) {
    $('#user_id').val(id);
    $('#deactivate_user').modal('show');
}
function DeactivateUser() {

    var userId = $('#user_id').val();
    $.ajax({
        type: 'Post',
        dataType: 'Json',
        url: '/SuperAdmin/DeactivateUser',
        data: {
            userId: userId
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/SuperAdmin/Index'
                successAlertWithRedirect(result.msg, url)
                $('#submit_Btn').html(defaultBtnValue);
            }
            else {
                errorAlert(result.msg)
            }
        },
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function RemoveSuspension(id) {

    $.ajax({
        type: 'Post',
        dataType: 'Json',
        error: function (ex) {
            errorAlert("An error occured, please check and try again. Please contact admin if issue persists..");
        }
    })
}

function createTextbook() {
    var defaultBtnValue = $('#create_textbook_btn').html();
    $('#create_textbook_btn').html("Please wait...");
    $('#create_textbook_btn').attr("disabled", true);

    var name = $('#textbook_Name').val();
    var description = $('#textbook_Description').val();
    var textbookCode = $('#textbook_Code').val();
    var session = $('#textbook_Session').val();
    var pdfFile = $('#textbook_Pdf')[0].files[0];

    if (!name) {
        $('#create_textbook_btn').html(defaultBtnValue);
        $('#create_textbook_btn').attr("disabled", false);
        errorAlert("Please enter the textbook name");
        return;
    }
    if (!session || session == "0") {
        $('#create_textbook_btn').html(defaultBtnValue);
        $('#create_textbook_btn').attr("disabled", false);
        errorAlert("Please select a session");
        return;
    }
    if (!pdfFile) {
        $('#create_textbook_btn').html(defaultBtnValue);
        $('#create_textbook_btn').attr("disabled", false);
        errorAlert("Please upload a PDF file");
        return;
    }

    var formData = new FormData();
    formData.append("name", name);
    formData.append("description", description);
    formData.append("textbookCode", textbookCode);
    formData.append("textbookForEachSession", session);
    formData.append("pdfFile", pdfFile);

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/CreateTextbook',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/AcademicStaff/Textbooks');
            }
            else {
                $('#create_textbook_btn').html(defaultBtnValue);
                $('#create_textbook_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#create_textbook_btn').html(defaultBtnValue);
            $('#create_textbook_btn').attr("disabled", false);
            errorAlert("Unable to upload textbook. Please try again.");
        }
    });
}

function createLiveSession() {
    var defaultBtnValue = $('#create_live_session_btn').html();
    $('#create_live_session_btn').html("Please wait...");
    $('#create_live_session_btn').attr("disabled", true);

    var roomName = $('#liveSession_RoomName').val();
    var welcomeMessage = $('#liveSession_WelcomeMessage').val();
    var startDateTime = $('#liveSession_StartDateTime').val();
    var durationMinutes = $('#liveSession_Duration').val();

    if (!roomName) {
        $('#create_live_session_btn').html(defaultBtnValue);
        $('#create_live_session_btn').attr("disabled", false);
        errorAlert("Please enter a room name");
        return;
    }
    if (!startDateTime) {
        $('#create_live_session_btn').html(defaultBtnValue);
        $('#create_live_session_btn').attr("disabled", false);
        errorAlert("Please select a start date and time");
        return;
    }
    if (!durationMinutes || durationMinutes == "0") {
        $('#create_live_session_btn').html(defaultBtnValue);
        $('#create_live_session_btn').attr("disabled", false);
        errorAlert("Please select a duration");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/CreateLiveSession',
        dataType: 'json',
        data: {
            roomName: roomName,
            welcomeMessage: welcomeMessage,
            startDateTime: startDateTime,
            durationMinutes: durationMinutes
        },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/AcademicStaff/LiveSessions');
            }
            else {
                $('#create_live_session_btn').html(defaultBtnValue);
                $('#create_live_session_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#create_live_session_btn').html(defaultBtnValue);
            $('#create_live_session_btn').attr("disabled", false);
            errorAlert("Unable to schedule live session. Please try again.");
        }
    });
}

function approveTextbook(id) {
    $.ajax({
        type: 'POST',
        url: '/Librarian/ApproveTextbook',
        dataType: 'json',
        data: { id: id },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/Librarian/Textbooks');
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function () {
            errorAlert("Unable to approve textbook. Please try again.");
        }
    });
}

function declineTextbook(id) {
    $.ajax({
        type: 'POST',
        url: '/Librarian/DeclineTextbook',
        dataType: 'json',
        data: { id: id },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/Librarian/Textbooks');
            }
            else {
                errorAlert(result.msg);
            }
        },
        error: function () {
            errorAlert("Unable to decline textbook. Please try again.");
        }
    });
}

function saveCbtTest(isUpdate) {
    var btn = isUpdate ? $('#update_cbt_btn') : $('#create_cbt_btn');
    var defaultBtnValue = btn.html();
    btn.html('Please wait...').attr('disabled', true);

    var payload = {
        id: parseInt($('#cbt_Id').val(), 10) || 0,
        title: $('#cbt_Title').val(),
        description: $('#cbt_Description').val(),
        durationMinutes: $('#cbt_Duration').val(),
        startDateTime: $('#cbt_Start').val(),
        endDateTime: $('#cbt_End').val(),
        passMark: $('#cbt_PassMark').val(),
        markPerQuestion: $('#cbt_MarkPerQuestion').val(),
        maximumAttempts: $('#cbt_MaxAttempts').val(),
        shuffleQuestions: $('#cbt_ShuffleQuestions').is(':checked'),
        shuffleOptions: $('#cbt_ShuffleOptions').is(':checked'),
        isPublished: $('#cbt_Published').is(':checked'),
        browserCode: $('#cbt_BrowserCode').val(),
        instructions: $('#cbt_Instructions').val()
    };

    if (!payload.title) {
        btn.html(defaultBtnValue).attr('disabled', false);
        errorAlert('Please enter a test title');
        return;
    }

    $.ajax({
        type: 'POST',
        url: isUpdate ? '/AcademicStaff/UpdateCbtTest' : '/AcademicStaff/CreateCbtTest',
        dataType: 'json',
        data: payload,
        success: function (result) {
            if (!result.isError) {
                var redirect = isUpdate
                    ? '/AcademicStaff/EditCbtTest?id=' + payload.id
                    : '/AcademicStaff/ManageCbtQuestions?id=' + (result.testId || payload.id);
                successAlertWithRedirect(result.msg, redirect);
            } else {
                btn.html(defaultBtnValue).attr('disabled', false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            btn.html(defaultBtnValue).attr('disabled', false);
            errorAlert('Unable to save CBT test. Please try again.');
        }
    });
}

function deleteCbtTest(id) {
    Swal.fire({
        title: 'Delete test?',
        text: 'This action cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete'
    }).then(function (res) {
        if (!res.isConfirmed) return;
        $.ajax({
            type: 'POST',
            url: '/AcademicStaff/DeleteCbtTest',
            dataType: 'json',
            data: { id: id },
            success: function (result) {
                if (!result.isError) successAlertWithRedirect(result.msg, '/AcademicStaff/CbtTests');
                else errorAlert(result.msg);
            },
            error: function () { errorAlert('Unable to delete test.'); }
        });
    });
}

function toggleQuestionOptions() {
    var type = parseInt($('#q_Type').val(), 10);
    if (type === 3) {
        $('#mcqOptions').hide();
    } else {
        $('#mcqOptions').show();
    }
}

function resetQuestionForm() {
    $('#q_Id').val('0');
    $('#q_Text').val('');
    $('#q_OptionA, #q_OptionB, #q_OptionC, #q_OptionD').val('');
    $('#q_Correct').val('');
    $('#q_Explanation').val('');
    $('#q_Image').val('');
    $('#questionModalTitle').text('Add Question');
    toggleQuestionOptions();
}

$(document).on('click', '.btn-edit-question', function () {
    var q = JSON.parse($(this).attr('data-question'));
    $('#q_Id').val(q.Id);
    $('#q_Type').val(q.QuestionType);
    $('#q_Text').val(q.QuestionText);
    $('#q_OptionA').val(q.OptionA || '');
    $('#q_OptionB').val(q.OptionB || '');
    $('#q_OptionC').val(q.OptionC || '');
    $('#q_OptionD').val(q.OptionD || '');
    $('#q_Correct').val(q.CorrectAnswer);
    $('#q_Marks').val(q.Marks);
    $('#q_Explanation').val(q.Explanation || '');
    $('#questionModalTitle').text('Edit Question');
    toggleQuestionOptions();
    new bootstrap.Modal(document.getElementById('questionModal')).show();
});

function saveCbtQuestion() {
    var btn = $('#save_question_btn');
    var defaultBtnValue = btn.html();
    btn.html('Please wait...').attr('disabled', true);

    var formData = new FormData();
    formData.append('cbtTestId', $('#q_TestId').val());
    formData.append('questionId', $('#q_Id').val() || 0);
    formData.append('questionType', $('#q_Type').val());
    formData.append('questionText', $('#q_Text').val());
    formData.append('optionA', $('#q_OptionA').val());
    formData.append('optionB', $('#q_OptionB').val());
    formData.append('optionC', $('#q_OptionC').val());
    formData.append('optionD', $('#q_OptionD').val());
    formData.append('correctAnswer', $('#q_Correct').val());
    formData.append('marks', $('#q_Marks').val());
    formData.append('explanation', $('#q_Explanation').val());
    var imageFile = $('#q_Image')[0].files[0];
    if (imageFile) formData.append('imageFile', imageFile);

    if (!$('#q_Text').val()) {
        btn.html(defaultBtnValue).attr('disabled', false);
        errorAlert('Question text is required');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/SaveCbtQuestion',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, window.location.pathname + window.location.search);
            } else {
                btn.html(defaultBtnValue).attr('disabled', false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            btn.html(defaultBtnValue).attr('disabled', false);
            errorAlert('Unable to save question.');
        }
    });
}

function deleteCbtQuestion(id) {
    Swal.fire({
        title: 'Delete question?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete'
    }).then(function (res) {
        if (!res.isConfirmed) return;
        $.ajax({
            type: 'POST',
            url: '/AcademicStaff/DeleteCbtQuestion',
            dataType: 'json',
            data: { id: id },
            success: function (result) {
                if (!result.isError) successAlertWithRedirect(result.msg, window.location.pathname + window.location.search);
                else errorAlert(result.msg);
            }
        });
    });
}

function promptTakeCbtTest(testId, requiresCode) {
    if (requiresCode) {
        Swal.fire({
            title: 'Browser Code Required',
            input: 'text',
            inputPlaceholder: 'Enter browser code',
            showCancelButton: true,
            confirmButtonText: 'Start Test'
        }).then(function (res) {
            if (res.isConfirmed) startCbtTest(testId, res.value);
        });
    } else {
        startCbtTest(testId, null);
    }
}

function startCbtTest(testId, browserCode) {
    $.ajax({
        type: 'POST',
        url: '/Student/StartCbtTest',
        dataType: 'json',
        data: { testId: testId, browserCode: browserCode },
        success: function (result) {
            if (!result.isError && result.attemptId) {
                window.location.href = '/Student/TakeCbtTest?attemptId=' + result.attemptId;
            } else {
                errorAlert(result.msg || 'Unable to start test.');
            }
        },
        error: function () { errorAlert('Unable to start test.'); }
    });
}

function collectCbtAnswers() {
    var answers = [];
    $('.cbt-question-card').each(function () {
        var questionId = parseInt($(this).data('question-id'), 10);
        var questionType = parseInt($(this).data('question-type'), 10);
        var selected = '';
        if (questionType === 2) {
            var vals = [];
            $(this).find('.cbt-answer:checked').each(function () { vals.push($(this).val()); });
            selected = vals.sort().join(',');
        } else {
            selected = $(this).find('.cbt-answer:checked').val() || '';
        }
        answers.push({ questionId: questionId, selectedAnswer: selected });
    });
    return answers;
}

function submitCbtTest(autoSubmitted) {
    var btn = $('#submit_cbt_btn');
    if (btn.length) btn.prop('disabled', true);

    var attemptId = parseInt($('#cbtAttemptId').val(), 10);
    $.ajax({
        type: 'POST',
        url: '/Student/SubmitCbtTest?attemptId=' + attemptId,
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify({ answers: collectCbtAnswers(), autoSubmitted: autoSubmitted }),
        success: function (result) {
            if (!result.isError && result.attemptId) {
                window.onbeforeunload = null;
                window.location.href = '/Student/CbtTestResult?id=' + result.attemptId;
            } else {
                if (btn.length) btn.prop('disabled', false);
                errorAlert(result.msg || 'Unable to submit test.');
            }
        },
        error: function () {
            if (btn.length) btn.prop('disabled', false);
            errorAlert('Unable to submit test.');
        }
    });
}

function toggleAnnouncementDepartment() {
    var audience = $('#announcement_Audience').val();
    if (audience === '1') {
        $('#announcement_department_wrap').hide();
    } else {
        $('#announcement_department_wrap').show();
    }
}

function createAnnouncement() {
    var defaultBtnValue = $('#create_announcement_btn').html();
    $('#create_announcement_btn').html("Please wait...");
    $('#create_announcement_btn').attr("disabled", true);

    var payload = {
        title: $('#announcement_Title').val(),
        message: $('#announcement_Message').val(),
        audience: $('#announcement_Audience').val(),
        departmentId: $('#announcement_DepartmentId').val(),
        startDateTime: $('#announcement_Start').val(),
        endDateTime: $('#announcement_End').val()
    };

    if (!payload.title || !payload.message) {
        $('#create_announcement_btn').html(defaultBtnValue);
        $('#create_announcement_btn').attr("disabled", false);
        errorAlert("Please fill title and message");
        return;
    }
    if (!payload.startDateTime || !payload.endDateTime) {
        $('#create_announcement_btn').html(defaultBtnValue);
        $('#create_announcement_btn').attr("disabled", false);
        errorAlert("Please select start and end date/time");
        return;
    }
    if (payload.audience === "2" && !payload.departmentId) {
        $('#create_announcement_btn').html(defaultBtnValue);
        $('#create_announcement_btn').attr("disabled", false);
        errorAlert("Please select a department");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/CreateAnnouncement',
        dataType: 'json',
        data: payload,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/AcademicStaff/Announcements');
            } else {
                $('#create_announcement_btn').html(defaultBtnValue);
                $('#create_announcement_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#create_announcement_btn').html(defaultBtnValue);
            $('#create_announcement_btn').attr("disabled", false);
            errorAlert("Unable to create announcement. Please try again.");
        }
    });
}

function showStudyCenterDetails(card) {
    var name = card.getAttribute('data-name') || '';
    var address = card.getAttribute('data-address') || '';
    var country = card.getAttribute('data-country') || '';
    var contact = card.getAttribute('data-contact') || '';
    var image = card.getAttribute('data-image') || '/assets/img/about.jpg';

    document.getElementById('studyCenterModalName').textContent = name;
    document.getElementById('studyCenterModalAddress').textContent = address;
    document.getElementById('studyCenterModalCountry').textContent = country;
    document.getElementById('studyCenterModalContact').textContent = contact;
    document.getElementById('studyCenterModalImage').src = image;
    document.getElementById('studyCenterModalImage').alt = name;
}

function submitStudyCenter() {
    var defaultBtnValue = $('#submit_study_center_btn').html();
    $('#submit_study_center_btn').html("Please wait...");
    $('#submit_study_center_btn').attr("disabled", true);

    var name = $('#studyCenter_Name').val();
    var address = $('#studyCenter_Address').val();
    var country = $('#studyCenter_Country').val();
    var contactPerson = $('#studyCenter_ContactPerson').val();
    var imageFile = $('#studyCenter_Image')[0].files[0];

    if (!name || !address || !country || !contactPerson) {
        $('#submit_study_center_btn').html(defaultBtnValue);
        $('#submit_study_center_btn').attr("disabled", false);
        errorAlert("Please fill in all required fields.");
        return;
    }

    if (!imageFile) {
        $('#submit_study_center_btn').html(defaultBtnValue);
        $('#submit_study_center_btn').attr("disabled", false);
        errorAlert("Please upload an image for the study center.");
        return;
    }

    var formData = new FormData();
    formData.append("name", name);
    formData.append("address", address);
    formData.append("country", country);
    formData.append("contactPerson", contactPerson);
    formData.append("imageFile", imageFile);

    $.ajax({
        type: 'POST',
        url: '/Home/SubmitStudyCenter',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/Home/StudyCenters');
            } else {
                $('#submit_study_center_btn').html(defaultBtnValue);
                $('#submit_study_center_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#submit_study_center_btn').html(defaultBtnValue);
            $('#submit_study_center_btn').attr("disabled", false);
            errorAlert("Unable to submit study center. Please try again.");
        }
    });
}

function approveStudyCenter(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/ApproveStudyCenter',
        dataType: 'json',
        data: { id: id },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/SuperAdmin/PendingStudyCenters');
            } else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Something went wrong, contact support - " + ex);
        }
    });
}

function declineStudyCenter(id) {
    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/DeclineStudyCenter',
        dataType: 'json',
        data: { id: id },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/SuperAdmin/PendingStudyCenters');
            } else {
                errorAlert(result.msg);
            }
        },
        error: function (ex) {
            errorAlert("Please contact support - " + ex);
        }
    });
}

function submitContactMessage() {
    var defaultBtnValue = $('#submit_contact_btn').html();
    $('#submit_contact_btn').html("Please wait...");
    $('#submit_contact_btn').attr("disabled", true);

    var name = $('#contact_name').val();
    var email = $('#contact_email').val();
    var subject = $('#contact_subject').val();
    var message = $('#contact_message').val();

    if (!name || !email || !subject || !message) {
        $('#submit_contact_btn').html(defaultBtnValue);
        $('#submit_contact_btn').attr("disabled", false);
        errorAlert("Please fill in all fields.");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Home/SubmitContactMessage',
        dataType: 'json',
        data: {
            name: name,
            email: email,
            subject: subject,
            message: message
        },
        success: function (result) {
            $('#submit_contact_btn').html(defaultBtnValue);
            $('#submit_contact_btn').attr("disabled", false);

            if (!result.isError) {
                successAlert(result.msg);
                $('#contact_name').val('');
                $('#contact_email').val('');
                $('#contact_subject').val('');
                $('#contact_message').val('');
            } else {
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#submit_contact_btn').html(defaultBtnValue);
            $('#submit_contact_btn').attr("disabled", false);
            errorAlert("Unable to send your message. Please try again.");
        }
    });
}

var academicStaffPositionValue = 11;

$(document).on('click', '.reassign-role-btn', function () {
    openReassignStaffRoleModal(
        $(this).data('staff-id'),
        $(this).data('staff-position'),
        $(this).data('department-id'),
        $(this).data('staff-name')
    );
});

function openReassignStaffRoleModal(staffId, currentPosition, departmentId, staffName) {
    $('#reassign_staff_id').val(staffId);
    $('#reassign_staff_name').text(staffName || '');
    $('#reassign_staff_position').val(currentPosition);
    $('#reassign_department_id').val(departmentId > 0 ? departmentId : '');
    toggleReassignDepartmentField();
    $('#reassign_staff_role_modal').modal('show');
}

function toggleReassignDepartmentField() {
    var position = $('#reassign_staff_position').val();
    if (position == academicStaffPositionValue) {
        $('#reassign_department_group').show();
    } else {
        $('#reassign_department_group').hide();
        $('#reassign_department_id').val('');
    }
}

function reassignStaffRole() {
    var defaultBtnValue = $('#reassign_staff_role_btn').html();
    $('#reassign_staff_role_btn').html("Please wait...");
    $('#reassign_staff_role_btn').attr("disabled", true);

    var staffDocumentId = $('#reassign_staff_id').val();
    var newStaffPosition = $('#reassign_staff_position').val();
    var departmentId = $('#reassign_department_id').val();

    if (!staffDocumentId || !newStaffPosition) {
        $('#reassign_staff_role_btn').html(defaultBtnValue);
        $('#reassign_staff_role_btn').attr("disabled", false);
        errorAlert("Please select a role.");
        return;
    }

    if (newStaffPosition == academicStaffPositionValue && !departmentId) {
        $('#reassign_staff_role_btn').html(defaultBtnValue);
        $('#reassign_staff_role_btn').attr("disabled", false);
        errorAlert("Please select a department for Academic Staff.");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/ReassignStaffRole',
        dataType: 'json',
        data: {
            staffDocumentId: staffDocumentId,
            newStaffPosition: newStaffPosition,
            departmentId: departmentId || null
        },
        success: function (result) {
            $('#reassign_staff_role_btn').html(defaultBtnValue);
            $('#reassign_staff_role_btn').attr("disabled", false);

            if (!result.isError) {
                $('#reassign_staff_role_modal').modal('hide');
                successAlertWithRedirect(result.msg, '/SuperAdmin/ApprovedStaff');
            } else {
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#reassign_staff_role_btn').html(defaultBtnValue);
            $('#reassign_staff_role_btn').attr("disabled", false);
            errorAlert("Unable to reassign staff role. Please try again.");
        }
    });
}

function savePublicAssessment(isUpdate) {
    var btn = isUpdate ? $('#update_public_cbt_btn') : $('#create_public_cbt_btn');
    var defaultBtnValue = btn.html();
    btn.html('Please wait...').attr('disabled', true);

    var payload = {
        id: parseInt($('#cbt_Id').val(), 10) || 0,
        title: $('#cbt_Title').val(),
        description: $('#cbt_Description').val(),
        durationMinutes: $('#cbt_Duration').val(),
        startDateTime: $('#cbt_Start').val(),
        endDateTime: $('#cbt_End').val(),
        passMark: $('#cbt_PassMark').val(),
        markPerQuestion: $('#cbt_MarkPerQuestion').val(),
        shuffleQuestions: $('#cbt_ShuffleQuestions').is(':checked'),
        shuffleOptions: $('#cbt_ShuffleOptions').is(':checked'),
        isPublished: $('#cbt_Published').is(':checked'),
        instructions: $('#cbt_Instructions').val()
    };

    if (!payload.title) {
        btn.html(defaultBtnValue).attr('disabled', false);
        errorAlert('Please enter an assessment title');
        return;
    }

    $.ajax({
        type: 'POST',
        url: isUpdate ? '/SuperAdmin/UpdatePublicAssessment' : '/SuperAdmin/CreatePublicAssessment',
        dataType: 'json',
        data: payload,
        success: function (result) {
            if (!result.isError) {
                var redirect = isUpdate
                    ? '/SuperAdmin/EditPublicAssessment?id=' + payload.id
                    : '/SuperAdmin/ManagePublicAssessmentQuestions?id=' + (result.testId || payload.id);
                successAlertWithRedirect(result.msg, redirect);
            } else {
                btn.html(defaultBtnValue).attr('disabled', false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            btn.html(defaultBtnValue).attr('disabled', false);
            errorAlert('Unable to save assessment. Please try again.');
        }
    });
}

function deletePublicAssessment(id) {
    Swal.fire({
        title: 'Delete assessment?',
        text: 'This action cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete'
    }).then(function (res) {
        if (!res.isConfirmed) return;
        $.ajax({
            type: 'POST',
            url: '/SuperAdmin/DeletePublicAssessment',
            dataType: 'json',
            data: { id: id },
            success: function (result) {
                if (!result.isError) successAlertWithRedirect(result.msg, '/SuperAdmin/PublicAssessments');
                else errorAlert(result.msg);
            },
            error: function () { errorAlert('Unable to delete assessment.'); }
        });
    });
}

function savePublicCbtQuestion() {
    var btn = $('#save_question_btn');
    var defaultBtnValue = btn.html();
    btn.html('Please wait...').attr('disabled', true);

    var formData = new FormData();
    formData.append('cbtTestId', $('#q_TestId').val());
    formData.append('questionId', $('#q_Id').val() || 0);
    formData.append('questionType', $('#q_Type').val());
    formData.append('questionText', $('#q_Text').val());
    formData.append('optionA', $('#q_OptionA').val());
    formData.append('optionB', $('#q_OptionB').val());
    formData.append('optionC', $('#q_OptionC').val());
    formData.append('optionD', $('#q_OptionD').val());
    formData.append('correctAnswer', $('#q_Correct').val());
    formData.append('marks', $('#q_Marks').val());
    formData.append('explanation', $('#q_Explanation').val());
    var imageFile = $('#q_Image')[0].files[0];
    if (imageFile) formData.append('imageFile', imageFile);

    if (!$('#q_Text').val()) {
        btn.html(defaultBtnValue).attr('disabled', false);
        errorAlert('Question text is required');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/SuperAdmin/SavePublicCbtQuestion',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, window.location.pathname + window.location.search);
            } else {
                btn.html(defaultBtnValue).attr('disabled', false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            btn.html(defaultBtnValue).attr('disabled', false);
            errorAlert('Unable to save question.');
        }
    });
}

function deletePublicCbtQuestion(id) {
    Swal.fire({
        title: 'Delete question?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Delete'
    }).then(function (res) {
        if (!res.isConfirmed) return;
        $.ajax({
            type: 'POST',
            url: '/SuperAdmin/DeletePublicCbtQuestion',
            dataType: 'json',
            data: { id: id },
            success: function (result) {
                if (!result.isError) successAlertWithRedirect(result.msg, window.location.pathname + window.location.search);
                else errorAlert(result.msg);
            }
        });
    });
}

function submitAssessmentRegistration() {
    var defaultBtnValue = $('#submit_assessment_btn').html();
    $('#submit_assessment_btn').html('Please wait...').attr('disabled', true);

    var email = $('#assessment_email').val();
    var programType = $('#assessment_programType').val();
    var country = $('#assessment_country').val();
    var scholarshipType = $('#assessment_scholarshipType').val();

    if (!email || !programType || !country || !scholarshipType) {
        $('#submit_assessment_btn').html(defaultBtnValue).attr('disabled', false);
        errorAlert('Please fill in all fields.');
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/Home/SubmitAssessmentRegistration',
        dataType: 'json',
        data: {
            email: email,
            programType: programType,
            country: country,
            scholarshipType: scholarshipType
        },
        success: function (result) {
            if (!result.isError && result.data) {
                successAlertWithRedirect(result.msg, result.data);
            } else {
                $('#submit_assessment_btn').html(defaultBtnValue).attr('disabled', false);
                errorAlert(result.msg || 'Unable to start payment.');
            }
        },
        error: function () {
            $('#submit_assessment_btn').html(defaultBtnValue).attr('disabled', false);
            errorAlert('Unable to submit the form. Please try again.');
        }
    });
}

function submitPublicCbtTest(autoSubmitted) {
    if (window.__publicCbtSubmitted) return;
    window.__publicCbtSubmitted = true;
    var btn = $('#submit_cbt_btn');
    if (btn.length) btn.prop('disabled', true);

    var attemptId = parseInt($('#cbtAttemptId').val(), 10);
    var token = $('#cbtAccessToken').val();
    $.ajax({
        type: 'POST',
        url: '/Home/SubmitPublicAssessment?attemptId=' + attemptId + '&token=' + encodeURIComponent(token),
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify({ answers: collectCbtAnswers(), autoSubmitted: autoSubmitted }),
        success: function (result) {
            if (!result.isError && result.attemptId) {
                window.onbeforeunload = null;
                window.location.href = '/Home/AssessmentResult?id=' + result.attemptId + '&token=' + encodeURIComponent(token);
            } else {
                window.__publicCbtSubmitted = false;
                if (btn.length) btn.prop('disabled', false);
                errorAlert(result.msg || 'Unable to submit assessment.');
            }
        },
        error: function () {
            window.__publicCbtSubmitted = false;
            if (btn.length) btn.prop('disabled', false);
            errorAlert('Unable to submit assessment.');
        }
    });
}

function createAssignment() {
    var defaultBtnValue = $('#create_assignment_btn').html();
    $('#create_assignment_btn').html("Please wait...");
    $('#create_assignment_btn').attr("disabled", true);

    var name = $('#assignment_Name').val();
    var description = $('#assignment_Description').val();
    var validUntilDate = $('#assignment_ValidUntilDate').val();
    var totalMarks = $('#assignment_TotalMarks').val();
    var fileInput = $('#assignment_File')[0];
    var file = fileInput && fileInput.files.length ? fileInput.files[0] : null;

    if (!name) {
        $('#create_assignment_btn').html(defaultBtnValue);
        $('#create_assignment_btn').attr("disabled", false);
        errorAlert("Please enter an assignment title");
        return;
    }
    if (!description) {
        $('#create_assignment_btn').html(defaultBtnValue);
        $('#create_assignment_btn').attr("disabled", false);
        errorAlert("Please enter the assignment description");
        return;
    }
    if (!validUntilDate) {
        $('#create_assignment_btn').html(defaultBtnValue);
        $('#create_assignment_btn').attr("disabled", false);
        errorAlert("Please select a due date");
        return;
    }
    if (!totalMarks || parseFloat(totalMarks) <= 0) {
        $('#create_assignment_btn').html(defaultBtnValue);
        $('#create_assignment_btn').attr("disabled", false);
        errorAlert("Please enter total marks");
        return;
    }

    var formData = new FormData();
    formData.append("name", name);
    formData.append("description", description);
    formData.append("validUntilDate", validUntilDate);
    formData.append("totalMarks", totalMarks);
    if (file) {
        formData.append("file", file);
    }

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/CreateAssignment',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/AcademicStaff/Assignments');
            } else {
                $('#create_assignment_btn').html(defaultBtnValue);
                $('#create_assignment_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#create_assignment_btn').html(defaultBtnValue);
            $('#create_assignment_btn').attr("disabled", false);
            errorAlert("Unable to create assignment. Please try again.");
        }
    });
}

function submitAssignment() {
    var defaultBtnValue = $('#submit_assignment_btn').html();
    $('#submit_assignment_btn').html("Please wait...");
    $('#submit_assignment_btn').attr("disabled", true);

    var assignmentId = $('#assignment_Id').val();
    var comment = $('#assignment_Comment').val();
    var fileInput = $('#assignment_SubmissionFile')[0];
    var file = fileInput && fileInput.files.length ? fileInput.files[0] : null;

    var formData = new FormData();
    formData.append("assignmentId", assignmentId);
    formData.append("comment", comment || "");
    if (file) {
        formData.append("file", file);
    }

    $.ajax({
        type: 'POST',
        url: '/Student/SubmitAssignment',
        data: formData,
        processData: false,
        contentType: false,
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, '/Student/Assignments');
            } else {
                $('#submit_assignment_btn').html(defaultBtnValue);
                $('#submit_assignment_btn').attr("disabled", false);
                errorAlert(result.msg);
            }
        },
        error: function () {
            $('#submit_assignment_btn').html(defaultBtnValue);
            $('#submit_assignment_btn').attr("disabled", false);
            errorAlert("Unable to submit assignment. Please try again.");
        }
    });
}

function gradeAssignment(submissionId) {
    var score = $('#grade_score_' + submissionId).val();
    var feedback = $('#grade_feedback_' + submissionId).val();

    if (score === "" || score === null) {
        errorAlert("Please enter a score");
        return;
    }

    $.ajax({
        type: 'POST',
        url: '/AcademicStaff/GradeAssignment',
        dataType: 'json',
        data: {
            submissionId: submissionId,
            score: score,
            feedback: feedback
        },
        success: function (result) {
            if (!result.isError) {
                successAlertWithRedirect(result.msg, window.location.href);
            } else {
                errorAlert(result.msg);
            }
        },
        error: function () {
            errorAlert("Unable to save the grade. Please try again.");
        }
    });
}

