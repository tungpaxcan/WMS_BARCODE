

//------------Register------------
$('#submit').click(function (e) {
   
        Register();
 
})


//------------Register------------
function Register() {
    var form = document.getElementById("form");
    var formData = new FormData(form);
    $.ajax({
        url: '/register/Register',
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
              $('.Loading').css("display","none");//Request is complete so hide spinner
        }
    })
}


//---------eye::pass---------
$('.icon-xl.far.fa-eye-slash').css("display", "none")
$('.icon-xl.far.fa-eye').click(function () {
    $('.icon-xl.far.fa-eye-slash').css("display", "block")
    $('.icon-xl.far.fa-eye').css("display", "none")
    $('input[name="Pass"]').attr("type", "text");
})
$('.icon-xl.far.fa-eye-slash').click(function () {
    $('.icon-xl.far.fa-eye-slash').css("display", "none")
    $('.icon-xl.far.fa-eye').css("display", "block")
    $('input[name="Pass"]').attr("type", "password");
})
