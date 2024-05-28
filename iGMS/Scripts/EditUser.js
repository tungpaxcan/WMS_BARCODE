

//------------Register------------
$('#submit').click(function (e) {
    Register();
})


//------------Register------------
function Register() {
    var form = document.getElementById("form");
    var formData = new FormData(form);
    $.ajax({
        url: '/register/EditV2',
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
