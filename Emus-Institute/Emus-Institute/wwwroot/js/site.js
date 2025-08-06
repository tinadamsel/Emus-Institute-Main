
function RegisterStudent() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

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

function Evaluate() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);


    var data = {};
    data.DepartmentId = $('#deptId').val();
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.OtherName = $('#othername').val();
    
    let evaluationDetails = JSON.stringify(data);
    $.ajax({
        type: 'Post',
        url: '/Account/EvaluateStudentDetails',
        dataType: 'json',
        data:
        {
            evaluationDetails: evaluationDetails,
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

function RegisterStaff() {
    var defaultBtnValue = $('#submit_btn').html();
    $('#submit_btn').html("Please wait...");
    $('#submit_btn').attr("disabled", true);

    var data = {};
    data.FirstName = $('#firstname').val();
    data.LastName = $('#lastname').val();
    data.UserName = $('#username').val();
    data.OtherName = $('#othername').val();
    data.Phonenumber = $('#phonenumber').val();
    data.Email = $('#email').val();
    data.Password = $('#password').val();
    data.ConfirmPassword = $('#confirmPassword').val();
    data.State = $('#state').val();
    data.Country = $('#country').val();
    data.Address = $('#address').val();
    data.DOB = $('#dateOfBirth').val();

    /* data.Identification = $('#validId').val();*/
    data.SubjectId = $('#subjectId').val();
    if (data.DOB == "") {
        data.DOB = "0001-01-01T00:00:00"
    };
    var appLetter = $('#appLetter').val();
    var StaffPosition;
    var validId = document.getElementById("validId").files;
    if (data.SubjectId > 0) {
        StaffPosition = "";
    } else {
        StaffPosition = $('.user-checkbox').data('user-id');
    }
    if (data.FirstName != "" && data.LastName != "" && data.UserName != "" && data.Phonenumber != ""
        && data.Email != "" && data.Password != "" && data.ConfirmPassword != "" && data.State != "" && data.Country != ""
        && data.Address != "" && validId[0] != null) {
        if (validId[0] != null) {
            const reader = new FileReader();
            reader.readAsDataURL(validId[0]);
            reader.onload = function () {
                validId = reader.result;
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
                        validId: validId
                    },
                    success: function (result) {
                        debugger;
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
        }

    } else {
        $('#submit_btn').html(defaultBtnValue);
        $('#submit_btn').attr("disabled", false);
        errorAlert("Please fill the form Correctly");
    };
}

function viewCoverLetter(id) {
    if (id != null) {
        $.ajax({
            type: 'get',
            dataType: 'json',
            url: '/HumanResource/GetCoverLetter',
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
    debugger;
    $.ajax({
        type: 'POST',
        url: '/HumanResource/ApproveApplication',
        dataType: 'json',
        data: {
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

$(document).ready(function () {
    $('#dataTable').DataTable();
});


