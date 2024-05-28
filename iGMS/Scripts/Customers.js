var pagenum = $("#pagenum option:selected").val();
var page = 1;
var seach = "";
Customers(pagenum, page, seach);

//phan trang
$('#page').on('click', 'li', function (e) {
    e.preventDefault();
    page = $(this).attr('id');
    Customers(pagenum, page, seach);


});


function Customers(pagenum, page, seach) {
    $.ajax({
        url: '/customer/Lists',
        type: 'get',
        data: { pagenum, page, seach },
        success: function (data) {
            var Stt = 1;
            $('#tbd').empty();
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                $.each(data.c, function (k, v) {
                    let table = '<tr id="' + v.id + '" role="row" class="odd">';
                    table += '<td>' + (Stt++) + '</td>'
                    table += '<td id="barcodekh' + v.id + '"><svg class="barcode' + v.id + '" ></svg></td>'
                    table += '<td>' + v.name + '</td>'
                    table += '<td>' + v.address + '</td>'
                    table += '<td >' + v.taxcode + '</td>'
                    table += '<td>' + v.phone + '</td>'
                    table += '<td>' + v.fax + '</td>'
                    table += '<td class="action" nowrap="nowrap">';
                    table += '<div class="dropdown dropdown-inline">';
                    table += '<a href="javascript:;" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                    table += '<i class="la la-cog"></i></a>';
                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                    table += '<ul class="nav nav-hoverable flex-column">';
                    table += ' <li class="nav-item">';
                    table += '<a class="nav-link" href="/Customer/Detailss/' + v.id + '">';
                    table += '<i class="nav-icon icon-xl la la-building"></i>';
                    table += '<span class="nav-text">Detail</span></a></li>';
                    table += ' <li class="nav-item">';
                    table += '<a class="nav-link" href="/Customer/Editss/' + v.id + '">';
                    table += '<i class="nav-icon la la-edit"></i>';
                    table += '<span class="nav-text">Edit</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" onclick="printDiv(\'barcodekh' + v.id + '\')">';
                    table += '<i class="nav-icon la la-print"></i><span class="nav-text">Print</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" name="delete">';
                    table += '<i class="nav-icon la la-trash"></i><span class="nav-text">Delete</span></a></li>';
                    table += '</ul></div></div>';
                    table += '</td>';
                    table += '</tr>';

                    $('#tbd').append(table);
                    JsBarcode(".barcode" + v.id + "", v.id);
                });

                //--------------------------------
                let kt_datatable_info = 'Showing 1 to ' + pagenum + ' of ' + data.count + ' entries'
                $('#kt_datatable_info').append(kt_datatable_info);
                //-----------------------------page---------------------------
                $('#page').empty();
                if (parseInt(page) >= 2) {
                    let pagemin = '<li id="' + 1 + '"><a class="a_1 a_2" >' + 1 + '...</a></li>';
                    $('#page').append(pagemin);
                    let pre = ' <li id="' + (parseInt(page) - 1) + '" class="paginate_button page-item previous disabled" >';
                    pre += '<a  aria-controls="kt_datatable" data-dt-idx="0" tabindex="0" class="page-link">';
                    pre += '<i class="ki ki-arrow-back"></i></a></li>';
                    $('#page').append(pre);
                }
                for (let i = parseInt(page); i <= (parseInt(page) + 4); i++) {
                    if (i == data.pages + 1) {
                        return;
                    }
                    let li = '<li id="' + i + '" class="paginate_button page-item ">';
                    li += '<a aria-controls="kt_datatable" data-dt-idx="1" tabindex="0" class="page-link">' + i + '</a></li>';
                    $('#page').append(li);


                }


                let next = '<li  id="' + (parseInt(page) + 1) + '" class="paginate_button page-item next" id="kt_datatable_next">';
                next += '<a href="#" aria-controls="kt_datatable" data-dt-idx="6" tabindex="0" class="page-link"><i class="ki ki-arrow-next"></i></a></li>';
                $('#page').append(next);

                let pagemax = '<li id="' + data.pages + '"><a class="a_1 a_2" >...' + data.pages + '</a></li>';
                $('#page').append(pagemax);

            } else (
                toastr.error(data.msg)
            )
        }
    })
}

$('#pagenum').on('change', function () {
    var pagenum = $("#pagenum option:selected").val();
    var page = 1;
    var seach = "";
    Customers(pagenum, page, seach)
})


//------------------------tim kiem------------------

$('#seach').on('keyup', function (e) {
    page = 1;
    seach = $('#seach').val();
    Customers(pagenum, page, seach);
});

//----------------Add::KHCN---------------------
function Adds() {
    var id = "KHCN" + $('#id').val().trim();
    var name = $("#name").val().trim();
    var province = $("#province option:selected").val() == -1 ? "n" : $("#province option:selected").val();
    var district = $("#district option:selected").val() == -1 ? "n" : $("#district option:selected").val();
    var town = $("#town option:selected").val() == -1 ? "n" : $("#town option:selected").val();
    var addresss = $("#address").val().trim().length == 0 ? "Không Có" : $("#address").val().trim();
    var address = province + " ," + district + " ," + town + " ," + addresss;
    var nametransaction = $("#nametransaction").val().trim().length == 0 ? "Không Có" : $("#nametransaction").val().trim();
    var taxcode = $("#taxcode").val().trim().length == 0 ? 0 : $("#taxcode").val().trim();
    var fax = $("#fax").val().trim().length == 0 ? 0 : $("#fax").val().trim();
    var phone = $("#phone").val().trim();
    var email = $("#email").val().trim();
    var represent = $("#represent").val().trim().length == 0 ? "Không Có" : $("#represent").val().trim();
    var position = $("#position").val().trim().length == 0 ? "Không Có" : $("#position").val().trim();
    var website = $("#website").val().trim().length == 0 ? "Không Có" : $("#website").val().trim();
    var stk = $("#stk").val().trim().length == 0 ? 0 : $("#stk").val().trim();
    var bank = $("#bank").val().trim().length == 0 ? "Không Có" : $("#bank").val().trim();
    var industry = $("#industry option:selected").val();
    var groupgoods = $("#groupgoods option:selected").val() == 4 || $("#groupgoods option:selected").val() == null ? 4 : $("#groupgoods option:selected").val();
    var debtfrom = $("#debtfrom").val().trim().substring(1).replace(/,$/, '.').replace(/,/g, "");
    var debtto = $("#debtto").val().trim().substring(1).replace(/,$/, '.').replace(/,/g, "");
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error("Nhập Tên Khách hàng")
        return;
    } if (phone.length < 9 || phone.length > 11) {
        toastr.error("Nhập Đúng Số Điện Thoại")
        return;
    }
    if (id.length <= 0) {
        toastr.error("Nhập Mã KH")
        return;
    } if (validateEmail(email)) {
        toastr.error("Nhập Đúng Email")
        return;
    }
    $.ajax({
        url: '/customer/Add',
        type: 'post',
        data: {
            id, name, address, nametransaction, taxcode, fax, phone, email,
            represent, position, website, stk, bank, groupgoods, debtfrom, debtto

        },
        success: function (data) {
            if (data.code == 200) {
                Swal.fire({
                    title: "Thêm Khách Hàng Thành Công",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Confirm me!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
                window.location.href = "/Customer/Indexs";
            } else if (data.code == 300) {
                toastr.error(data.msg)
            }
            else {
                toastr.error(data.msg)
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}
//----------------Edit::KHCN---------------------
function Edit() {
    var id = $('#id').val().trim();
    var name = $("#name").val().trim();
    var address = $("#address").val().trim();
    var nametransaction = $("#nametransaction").val().trim().length == 0 ? "Không Có" : $("#nametransaction").val().trim();
    var taxcode = $("#taxcode").val().trim().length == 0 ? 0 : $("#taxcode").val().trim();
    var fax = $("#fax").val().trim().length == 0 ? 0 : $("#fax").val().trim();
    var phone = $("#phone").val().trim();
    var email = $("#email").val().trim();
    var represent = $("#represent").val().trim().length == 0 ? "Không Có" : $("#represent").val().trim();
    var position = $("#position").val().trim().length == 0 ? "Không Có" : $("#position").val().trim();
    var website = $("#website").val().trim().length == 0 ? "Không Có" : $("#website").val().trim();
    var stk = $("#stk").val().trim().length == 0 ? 0 : $("#stk").val().trim();
    var bank = $("#bank").val().trim().length == 0 ? "Không Có" : $("#bank").val().trim();
    var industry = $("#industry option:selected").val();
    var groupgoods = $("#groupgoods option:selected").val() == 4 || $("#groupgoods option:selected").val() == null ? 4 : $("#groupgoods option:selected").val();
    var debtfrom = $("#debtfrom").val().trim().substring(1).replace(/,$/, '.').replace(/,/g, "");
    var debtto = $("#debtto").val().trim().substring(1).replace(/,$/, '.').replace(/,/g, "");
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error("Nhập Tên Khách hàng")
        return;
    } if (phone.length < 9 || phone.length > 11) {
        toa.error("Nhập Đúng Số Điện Thoại")
        return;
    }
    if (id.length <= 0) {
        toastr.error("Nhập Mã KH")
        return;
    } if (validateEmail(email)) {
        toastr.error("Nhập Đúng Email")
        return;
    }
    $.ajax({
        url: '/customer/Edit',
        type: 'post',
        data: {
            id, name, address, nametransaction, taxcode, fax, phone, email,
            represent, position, website, stk, bank, groupgoods, debtfrom, debtto

        },
        success: function (data) {
            if (data.code == 200) {
                Swal.fire({
                    title: "Sửa Khách Hàng Thành Công",
                    icon: "success",
                    buttonsStyling: false,
                    confirmButtonText: "Confirm me!",
                    customClass: {
                        confirmButton: "btn btn-primary"
                    }
                });
                window.location.href = "/Customer/Indexs";
            } else if (data.code == 300) {
                toastr.error(data.msg)
            }
            else {
               toastr.error(data.msg)
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}
//----------------Delete::KHDN---------------------
$(document).on('click', "a[name='delete']", function () {
    var id = $(this).closest('tr').attr('id');
    if (confirm("Bạn Muốn Xóa Khách Hàng Này ???")) {
        $.ajax({
            url: '/customer/Delete',
            type: 'post',
            data: {
                id
            },
            success: function (data) {
                if (data.code == 200) {
                    Swal.fire({
                        title: "Xóa Khách hàng Thành Công",
                        icon: "success",
                        buttonsStyling: false,
                        confirmButtonText: "Confirm me!",
                        customClass: {
                            confirmButton: "btn btn-primary"
                        }
                    });
                    window.location.href = "/Customer/Indexs";
                }
                else {
                    alert(data.msg)
                }
            }
        })
    }
})
//-------------barcode-------------
$('#id').keyup(function () {
    var id = $('#id').val().trim();
    JsBarcode("#barcode", "KHDN" + id);
})