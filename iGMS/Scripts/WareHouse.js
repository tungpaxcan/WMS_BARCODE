var checkApi = false;
var pagenum = $("#pagenum option:selected").val();
var page = 1;
var seach = "";
WareHouse(pagenum, page, seach);

//phan trang
$('#page').on('click', 'li', function (e) {
    e.preventDefault();
    page = $(this).attr('id');
    WareHouse(pagenum, page, seach);


});

function WareHouse(pagenum, page, seach) {
    $.ajax({
        url: '/warehouse/List',
        type: 'get',
        data: { pagenum, page, seach },
        success: function (data) {
            var Stt = 1;
            $('#tbd').empty();
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                $.each(data.c, function (k, v) {
                    var string = '/WareHouse/Edit';
                    let table = '<tr id="' + v.id + '" role="row" class="odd">';
                    table += '<td>' + (Stt++) + '</td>'
                    table += '<td>' + v.id + '</td>'
                    table += '<td>' + v.name + '</td>'
                    table += '<td>' + v.min + '</td>'
                    table += '<td>' + v.max + '</td>'
                    table += '<td class="action" nowrap="nowrap">';
                    table += '<div class="dropdown dropdown-inline">';
                    table += '<a href="" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                    table += '<i class="la la-cog"></i></a>';
                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                    table += '<ul class="nav nav-hoverable flex-column">';
                    table += ' <li class="nav-item">';
                    table += `<a class="nav-link" onclick="check_Role('${string} ','${v.id }', 'EditWareHouse')">`;
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
                alert(data.msg)
            )
        }
    })
}

$('#pagenum').on('change', function () {
    var pagenum = $("#pagenum option:selected").val();
    var page = 1;
    var seach = "";
    WareHouse(pagenum, page, seach)
})


//------------------------tim kiem------------------

$('#seach').on('keyup', function (e) {
    page = 1;
    seach = $('#seach').val();
    WareHouse(pagenum, page, seach);
});
//----------------Add::WareHouses---------------------

//----Add Api: In Progress----------------
/*function CheckAddApi() {
    var name = $('#name').val().trim();
    var min = $('#min').val().trim();
    var max = $('#max').val().trim();
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error(resourceAddWH.nhaptenkho)
        return;
    }

    if (min.length <= 0) {
        toastr.error(resourceAddWH.nhapslmin)
        return;
    }
    if (max.length <= 0) {
        toastr.error(resourceAddWH.nhapslmax)
        return;
    }

    var form = new FormData();
    form.append("name", "/WareHouse/Add");

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
                        var type = getFunction.Method;
                        var values = JSON.parse(getFunction.ReturnValue);
                        var checkStatusResponse = getFunction.ReturnJson;
                        var getStatusResponse = getFunction.StatusReturnJson;
                        var message = getFunction.NameReturnJson;
                        AddApi(url, name, type, param, values, checkStatusResponse, getStatusResponse, message);
                    }
                } else {
                    Add(name, min, max);
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

function AddApi(url, name, type, params, values, checkStatus, status, message) {
    console.log(params);
    var formData = [];
    formData.push($('#name').val().trim());
    formData.push($('#min').val().trim());
    formData.push($('#max').val().trim());

    if (params.length < formData.length) {
        toastr.error("Thiếu tham số đầu trong phần cài đặt chức năng vào so với hệ thống. Sửa lại để phù hợp với hệ thống");
        return;
    }

    if (type.toLowerCase() === "get") {
        var input = "";
        for (var i = 0; i < params.length; i++) {
            if (i != 0) {
                input += "&";
            }
            input += params[i].name + "=" + formData[i];
        }
    } else if (type.toLowerCase() === "post") {
        var input = {};
        for (var i = 0; i < params.length; i++) {
            input[params[i].name] = formData[i];
        }
    }

    $.ajax({
        url: url + name,
        type: type,
        data: input,
        success: function (data) {
            console.log(data[checkStatus])
            if (data[checkStatus] == status) {
                toastr.success(data[message]);
                setTimeout(function () { window.location.href = "/WareHouse/Index" }, 1000)
            } else {
                toastr.error(data[message]);
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}*/
//----Add Api: In Progress----------------
function Add() {
    var name = $('#name').val().trim();
    var min = $('#min').val().trim();
    var max = $('#max').val().trim();
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error(resourceAddWH.nhaptenkho)
        return;
    }

    if (min.length <= 0) {
        toastr.error(resourceAddWH.nhapslmin)
        return;
    }
    if (max.length <= 0) {
        toastr.error(resourceAddWH.nhapslmax)
        return;
    }

    $.ajax({
        url: '/WareHouse/Add',
        type: 'post',
        data: {
            name, min, max
        },
        success: function (data) {
            if (data.code == 200) {
                toastr.success(data.msg);
                setTimeout(function () { window.location.href = "/WareHouse/Index" }, 1000)
            } else if (data.code == 300) {
                toastr.error(data.msg)
            }
            else {
                toastr.error(data.msg);
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}

//----------------Edit::WareHouses---------------------

//-------------Edit Api: In Progress------------
/*function CheckEditApi() {
    $('.Loading').css("display", "block");
    var name = $('#name').val().trim();
    var id = $('#id').val().trim();
    var min = $('#min').val().trim();
    var max = $('#max').val().trim();
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error(resourceEditWH.nhaptenkho)
        return;
    }
    if (min.length <= 0) {
        toastr.error(resourceEditWH.nhapslmin)
        return;
    }
    if (max.length <= 0) {
        toastr.error(resourceEditWH.nhapslmax)
        return;
    }

    var form = new FormData();
    form.append("name", "/WareHouse/Edit");

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
                        var type = getFunction.Method;
                        var values = JSON.parse(getFunction.ReturnValue);
                        var checkStatusResponse = getFunction.ReturnJson;
                        var getStatusResponse = getFunction.StatusReturnJson;
                        var message = getFunction.NameReturnJson;
                        EditApi(url, name, type, param, values, checkStatusResponse, getStatusResponse, message);
                    }
                } else {
                    Edit(id, name, min, max);
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

function EditApi(url, name, type, params, values, checkStatus, status, message) {
    console.log(params);
    var formData = [];
    formData.push($('#id').val().trim());
    formData.push($('#name').val().trim());
    formData.push($('#min').val().trim());
    formData.push($('#max').val().trim());

    if (params.length < formData.length) {
        toastr.error("Thiếu tham số đầu trong phần cài đặt chức năng vào so với hệ thống. Sửa lại để phù hợp với hệ thống");
        return;
    }

    if (type.toLowerCase() === "get") {
        var input = "";
        for (var i = 0; i < params.length; i++) {
            if (i != 0) {
                input += "&";
            }
            input += params[i].name + "=" + formData[i];
        }
    } else if (type.toLowerCase() === "post") {
        var input = {};
        for (var i = 0; i < params.length; i++) {
            input[params[i].name] = formData[i];
        }
    }

    $.ajax({
        url: url + name,
        type: type,
        data: input,
        success: function (data) {
            if (data[checkStatus] == status) {
                toastr.success(data[message]);
                setTimeout(function () { window.location.href = "/WareHouse/Index" }, 1000)
            } else {
                toastr.error(data[message]);
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}*/
//-------------Edit Api: In Progress------------

function Edit() {
    var name = $('#name').val().trim();
    var id = $('#id').val().trim();
    var min = $('#min').val().trim();
    var max = $('#max').val().trim();
    $('.Loading').css("display", "block");
    if (name.length <= 0) {
        toastr.error(resourceEditWH.nhaptenkho)
        return;
    }
    if (min.length <= 0) {
        toastr.error(resourceEditWH.nhapslmin)
        return;
    }
    if (max.length <= 0) {
        toastr.error(resourceEditWH.nhapslmax)
        return;
    }

    $.ajax({
        url: '/warehouse/Edit',
        type: 'post',
        data: {
            id, name, min, max
        },
        success: function (data) {
            if (data.code == 200) {
                toastr.success(data.msg)
                setTimeout(function () { window.location.href = "/WareHouse/Index" }, 1000)
            }
            else
            {
                toastr.error(data.msg)
            }
        },
        complete: function () {
            $('.Loading').css("display", "none");//Request is complete so hide spinner
        }
    })
}
//----------------Delete::WareHouses---------------------
$(document).on('click', 'a[name="delete"]', function () {
    var id = $(this).closest('tr').attr('id');
    var formData = new FormData();
    formData.append("href", '/Warehouse/Delete');
    formData.append("function", 'DeleteWareHouse');
    $.ajax({
        type: "POST",
        url: "authorization/CheckRole",
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
                            console.log(id);
                            $.ajax({
                                url: '/warehouse/Delete',
                                type: 'post',
                                data: { id },
                                success: function (data) {
                                    if (data.code == 200) {
                                        toastr.success(data.msg);
                                        setTimeout(function () { window.location.href = "/WareHouse/Index" }, 1000);
                                    }
                                    else {
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

function CheckDeleteApi() {
    
}
