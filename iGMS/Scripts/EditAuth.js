
//-----Lấy thông tin---------
$.ajax({
    type: "GET",
    url: "http://bill.haphan.com/API/APIUsers",
    success: function (response) {
        if (response.status) {
            listUsers = response.data;
            for (var i = 0; i < listUsers.length; i++) {
                if (listUsers[i].user == $("#account").val()) {
                    console.log(listUsers[i]);
                    $("#name").val(listUsers[i].fullName);
                }
            }
        } else {
            console.log("Loi!!!");
        }

    },
    error: function (e) {
        console.log(e);
    }
})

//------------Bấm lưu------------
$('#submit').click(function (e) {
    Register();
})


//------------Chỉnh sửa quyền------------
function Register() {
    var form = document.getElementById("form");
    var formData = new FormData(form);
    $.ajax({
        url: '/register/EditAuth',
        type: 'post',
        data: formData,
        contentType: false, // Không thiết lập contentType để jQuery tự động xác định
        processData: false,
        success: function (data) {
            if (data.code == 200) {
                toastr.success(data.msg)
                setTimeout(function () { window.location.href = "/register/ListUser" }, 1000)
            } else {
                toastr.error(data.msg)
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}
