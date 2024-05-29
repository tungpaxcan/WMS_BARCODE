var pagenum = $("#pagenum option:selected").val();
var page = 1;
var seach = "";
Unit(pagenum, page, seach);

//phan trang
$('#page').on('click', 'li', function (e) {
    e.preventDefault();
    page = $(this).attr('id');
    Unit(pagenum, page, seach);


});


function Unit(pagenum, page, seach) {
    $.ajax({
        url: '/unit/List',
        type: 'get',
        data: { pagenum, page, seach },
        success: function (data) {
            $('#tbd').empty();
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                var STT = 1;
                $.each(data.c, function (k, v) {
                    var string = '/Unit/Edit';
                    let table = '<tr id="' + v.id + '" role="row" class="odd">';
                    table += '<td>' + (STT++) + '</td>'
                    table += '<td>' + v.id + '</td>'
                    table += '<td>' + v.name + '</td>'
                    if (v.description == null) {
                        table += '<td>'+'</td>';
                    } else {
                        table += '<td>' + v.description + '</td>';
                    }
                    table += '<td class="action" nowrap="nowrap">';
                    table += '<div class="dropdown dropdown-inline">';
                    table += '<a href="" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                    table += '<i class="la la-cog"></i></a>';
                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                    table += '<ul class="nav nav-hoverable flex-column">';
                    table += ' <li class="nav-item">';
                    table += `<a class="nav-link" onclick="check_Role('${ string } ','${ v.id }', 'Unit')">`;
                    table += '<i class="nav-icon la la-edit"></i>';
                    table += '<span class="nav-text">' + resources.edit + '</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" onclick="printDiv(\'' + v.id + '\')">';
                    table += '<i class="nav-icon la la-print"></i><span class="nav-text">' + resources.print +'</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" name="delete">';
                    table += '<i class="nav-icon la la-trash"></i><span class="nav-text">' + resources.delete +'</span></a></li>';
                    table += '</ul></div></div>';
                    table += '</td>';
                    table += '</tr>';
                    $('#tbd').append(table);
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
    Unit(pagenum, page, seach)
})


//------------------------tim kiem------------------

$('#seach').on('keyup', function (e) {
    page = 1;
    seach = $('#seach').val();
    Unit(pagenum, page, seach);
});

//----------------Add::Unit---------------------
function Add() {
    var name = $('#name').val().trim();
    var id = $('#id').val().trim();
    var description = $('#description').val().trim();
    $('.Loading').css("display", "block");
    $.ajax({
        url: '/unit/Add',
        type: 'post',
        data: {
            name, id, description
        },
        success: function (data) {
            if (data.code == 200) {
                toastr.success(data.msg);
                setTimeout(function () { window.location.href = "/Unit/Index" }, 1000);
            } else if (data.code == 300) {
                toastr.error(data.msg)
            }
            else {
                toastr.error(data.msg)
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");
           
        }
    })
}

//----------------Edit::Unit---------------------
function Edit() {
    var name = $('#name').val().trim();
    var id = $('#id').val().trim();
    var description = $('#description').val().trim();
    $('.Loading').css("display", "block");
    console.log(name,id)
    $.ajax({
        url: '/unit/Edit',
        type: 'post',
        data: {
            id, name, description
        },
        success: function (data) {
            if (data.code == 200) {
                toastr.success(data.msg);
                setTimeout(function () { window.location.href = "/Unit/Index" }, 1000);
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

//----------------Delete::Unit---------------------
$(document).on('click', 'a[name="delete"]', function () {
    var id = $(this).closest('tr').attr('id');
    var formData = new FormData();
    formData.append("href", '/GroupGoods/Delete');
    formData.append("function", 'Unit');

    $.ajax({
        type: "POST",
        url: "authorization/CheckRole",
        data: formData,
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.code === 200) {
                if (response.success) {
                    
                    Swal.fire({
                        title: resources.DeleteWarning,
                        text: resources.ConfirmDeleteWarning,
                        icon: 'warning',
                        showCancelButton: true,
                        confirmButtonColor: '#d33',
                        cancelButtonColor: '#3085d6',
                        confirmButtonText: resources.DeleteInfo,
                        cancelButtonText: resources.CancelInfo,
                    }).then((result) => {
                        if (result.isConfirmed) {
                            $.ajax({
                                url: '/unit/Delete',
                                type: 'post',
                                data: { id },
                                success: function (data) {
                                    if (data.code == 200) {
                                        toastr.success(data.msg);
                                        setTimeout(function () { window.location.href = "/Unit/Index" }, 1000);
                                    } else {
                                        toastr.error(data.msg);
                                    }
                                }
                            });
                        }
                    });
                } else {
                    toastr.error("Hiện tại không có quyền thực hiện tác vụ này");
                }
            } else if (response.code === 500) {
                toastr.error("Lỗi hệ thống");
            } else if (response.code === 400) {
                toastr.error("Không nhận được dữ liệu");
            }
        }
    })
})