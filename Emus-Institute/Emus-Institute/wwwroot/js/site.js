
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

//function validateForm(id, message) {
//    debugger;
//    $("#" + id + "Error").text(message).css({ color: "red" });
//    $("#" + id).css({ border: "1px solid red" });
//}

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

function addDept() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var data = {};
    data.Name = $('#dept_Name').val();
    data.Description = $('#dept_Desc').val();

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
        url: '/HumanResource/DeclineApplication', // we are calling json method
        dataType: 'json',
        data:
        {
            id: id
        },
        success: function (result) {
            if (!result.isError) {
                var url = '/HumanResource/PendingApplication';
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


