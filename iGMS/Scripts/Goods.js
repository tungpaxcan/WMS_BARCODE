var pagenum = $("#pagenum option:selected").val();
var page = 1;
var seach = "";

CheckAPIFunction("/Goods/Index");
//Goods(pagenum, page, seach);

function CheckAPIFunction(name) {
    var form = new FormData();
    form.append("name", name);

    $.ajax({
        type: "POST",
        url: "/authorization/CheckAPIFunction",
        data: form,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.code === 200) {
                if (response.status) {
                    var getFunction = response.apiFunction;
                    var param = response.paramsF;
                    var url = response.apiHost;
                    console.log(getFunction);
                    console.log(param);
                    if (getFunction.NameFunctionAPI === null || getFunction.NameFunctionAPI === "") {
                        toastr.error("Không có tên function");
                        setTimeout(() => {
                            window.location.href = "/";
                        }, 1000);
                    }
                    else if (getFunction.Method === null || getFunction.Method === "") {
                        toastr.error("Không có phương thức HTTP tương ứng");
                        setTimeout(() => {
                            window.location.href = "/";
                        }, 1000);
                    } else {
                        var name = getFunction.NameFunctionAPI;
                        var type = getFunction.Method
                        GoodsAPI(url, name, type, param);
                    }
                } else {
                    Goods(pagenum, page, seach);
                }
            } else if (response.code === 400) {
                toastr.error("Không có dữ liệu");
            } else {
                toastr.error(response.msg);
            }
        },
        error: function (e) {
            console.log(e);
        }
    })
}

//phan trang
$('#page').on('click', 'li', function (e) {
    e.preventDefault();
    page = $(this).attr('id');
    Goods(pagenum, page, seach);
});


function GoodsAPI(url, name, type, params) {

}

function Goods(pagenum, page, seach) {
    $.ajax({
        url: '/goods/List',
        type: 'get',
        data: { pagenum, page, seach },
        success: function (data) {
            var Stt = 1;
            $('#tbd').empty();
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                $.each(data.c, function (k, v) {
                    var string = '/Goods/Edit';
                    let table = '<tr id="' + v.id + '" role="row" class="odd">';
                    table += '<td class="bg-white">' + (Stt++) + '</td>'
                    table += '<td class="bg-white">' + v.id + '</td>'
                    table += '<td class="bg-white">' + v.name + '</td>'
                    table += '<td class="bg-white">' + v.unit + '</td>'
                    table += '<td class="bg-white">' + v.gd + '</td>'
                    table += '<td class="bg-white">' + v.date + '</td>'
                    table += '<td class="bg-white">' + v.inventory + '</td>'
                    //--------Action----------
                    table += '<td class="action bg-white" nowrap="nowrap">';
                    table += '<div class="dropdown dropdown-inline">';
                    table += '<a href="" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                    table += '<i class="la la-cog"></i></a>';
                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                    table += '<ul class="nav nav-hoverable flex-column">';
                    table += ' <li class="nav-item">';
                    table += ' <li class="nav-item">';
                    table += `<a class="nav-link" onclick="check_Role('${string} ','${v.id}', 'EditGoods')">`;
                    table += '<i class="nav-icon la la-edit"></i>';
                    table += '<span class="nav-text">' + resources.edit + ' </span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" onclick="printDiv(\'barcodehh' + v.id + '\')">';
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
                alert(data.msg)
            )
        }
    })
}

$('#pagenum').on('change', function () {
    var pagenum = $("#pagenum option:selected").val();
    var page = 1;
    var seach = "";
    Goods(pagenum, page, seach)
})


//------------------------tim kiem------------------

$('#seach').on('keyup', function (e) {
    page = 1;
    seach = $('#seach').val();
    Goods(pagenum, page, seach);
});
//----------------Delete::Goods---------------------
$(document).on('click', "a[name='delete']", function () {
    var formData = new FormData();
    formData.append("href", '/Goods/Delete');
    formData.append("function", 'DeleteGoods');

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
                    var id = $(this).closest('tr').attr('id');
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
                                url: '/goods/Delete',
                                type: 'post',
                                data: {
                                    id
                                },
                                success: function (data) {
                                    if (data.code == 200) {
                                        toastr.success(data.msg);
                                        setTimeout(function () { window.location.href = "/Goods/Index" }, 1000)
                                    }
                                    else {
                                        toastr.error(data.msg);
                                    }
                                }
                            })
                        } else if (data.code == 300) {
                            alert(data.msg)
                        }
                    })
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

function Delete(id) {

}
