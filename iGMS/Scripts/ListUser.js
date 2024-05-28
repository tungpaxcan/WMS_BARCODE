
var pagenum = $("#pagenum option:selected").val();
var page = 1;
var seach = "";
var listUsers = {};
CheckAPIFunction("/Register/ListUser");
//ListUser(pagenum, page, seach);

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
                        var type = getFunction.Method;
                        var values = JSON.parse(getFunction.ReturnValue);
                        ListUserApi(url, name, type, param, values);
                    }
                } else {
                    ListUser(pagenum, page, seach);
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

function ListUserApi(url, name, type, params, values) {
    $.ajax({
        type: type,
        url: url + name,
        data: { params },
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.status) {
                var checkValues = response.data[0];
                console.log(values["data"][0]);
                if (!(Object.keys(checkValues).length === Object.keys(values["data"][0]).length)) {
                    toastr.error(`Đối tượng trả về không khớp với hệ thống. Hãy điều chỉnh lại để khớp với hệ thống`);
                } else {
                    listUsers = response.data;
                    $('#tbd').empty();
                    $('#tbdmodal').empty();
                    $('#kt_datatable_info').empty();
                    //var data = JSON.stringify(response.data);
                    var listUser = JSON.stringify(response.data);
                    var form = new FormData();
                    form.append("listUser", listUser);
                    form.append("listCount", listUser.length);
                    form.append("pageSize", pagenum);
                    form.append("page", page);
                    //var listUser = data;
                    //var form = new FormData();
                    //form.append("users", listUser);
                    $.ajax({
                        type: "POST",
                        url: "/register/ListApi",
                        data: form,
                        contentType: false,
                        processData: false,
                        success: function (response) {
                            if (response.code === 200) {
                                var data = JSON.parse(JSON.parse(response.data));
                                var role = response.role;
                                var roleAdmin = response.roleAdmin;
                                for (var i = 0; i < data.length; i++) {
                                    var string1 = "/Register/EditUser";
                                    var string2 = "/Register/EditAuth";
                                    let table = '<tr id="' + data[i].user + '" role="row" class="odd">';
                                    table += '<td>' + data[i].fullName + '</td>';
                                    table += '<td>' + data[i].user + '</td>';
                                    table += '<td></td>';
                                    table += '<td>Admin</td>';
                                    table += '<td></td>';
                                    table += '<td>Admin</td>';
                                    table += '<td ">';
                                    for (var j = 0; j < roleAdmin.length; j++) {
                                        if (data[i].user === roleAdmin[j].username) {
                                            table += roleAdmin[j].ManageMainCategories + roleAdmin[j].PurchaseManager + roleAdmin[j].SalesManager
                                                + roleAdmin[j].WarehouseManagement + roleAdmin[j].SystemManagement + roleAdmin[j].AccountManager;
                                            break;
                                        }
                                    }
                                    for (var k = 0; k < role.length; k++) {
                                        if (data[i].user === role[k].username) {
                                            table += role[k].AddWarehouse + role[k].EditWarehouse + role[k].DeleteWarehouse
                                                + role[k].AddGoods + role[k].EditGoods + role[k].DeleteGoods
                                                + role[k].AddCustomer + role[k].EditCustomer + role[k].DeleteCustomer
                                                + role[k].AddTypeCustomer + role[k].EditTypeCustomer + role[k].DeleteTypeCustomer
                                                + role[k].AddUser + role[k].EditUser + role[k].EditPassWord + role[k].DeleteUser
                                                + role[k].BackupSql + role[k].AddGroupGoods + role[k].EditGroupGoods + role[k].DeleteGroupGoods;
                                            break;
                                        }
                                    }
                                    table += '</td>';
                                    table += '<td class="action" nowrap="nowrap">';
                                    table += '<div class="dropdown dropdown-inline">';
                                    table += '<a href="javascript:;" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                                    table += '<i class="la la-cog"></i></a>';
                                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                                    table += '<ul class="nav nav-hoverable flex-column">';
                                    table += ' <li class="nav-item">';
                                    table += `<a class="nav-link" onclick="check_Role('${string1} ','${data[i]} ', 'EditUser')">`;
                                    table += '<i class="nav-icon la la-edit"></i>';
                                    table += '<span class="nav-text">Sửa</span></a></li>';
                                    table += ' <li class="nav-item">';
                                    table += '<a class="nav-link" href="/Register/EditPass/' + data[i].user + '">';
                                    table += '<i class="nav-icon la la-edit"></i>';
                                    table += '<span class="nav-text">Sửa Mật Khẩu</span></a></li>';
                                    table += ' <li class="nav-item">';
                                    table += `<a class="nav-link" onclick="EditUserAuthorization('${data[i].user}')">`;
                                    table += '<i class="nav-icon la la-edit"></i>';
                                    table += '<span class="nav-text">Sửa quyền User</span></a></li>';
                                    table += '<li class="nav-item"><a class="nav-link" onclick="printDiv(\'barcodeuser' + data[i].user + '\')">';
                                    table += '<i class="nav-icon la la-print"></i><span class="nav-text">In</span></a></li>';
                                    table += '<li class="nav-item"><a class="nav-link" name="delete">';
                                    table += '<i class="nav-icon la la-trash"></i><span class="nav-text">Xóa</span></a></li>';
                                    table += '</ul></div></div>';
                                    table += '</td>';
                                    table += '</tr>';
                                    $('#tbd').append(table);
                                    $('#tbdmodal').append(table);
                                }
                            }
                        },
                        error: function (e) {
                            console.log(e);
                        }
                    })
                }
            } else {
                console.log("Loi!!!");
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
    //ListUser(pagenum, page,seach); 
});

function EditUserAuthorization(username) {

    var newUser = "";
    var splitUser = "";
    if (username.includes(".")) {
        splitUser = username.split(".");
        for (var i = 0; i < splitUser.length; i++) {
            if (i == splitUser.length - 1) {
                newUser += splitUser[i];
                break;
            }
            newUser += splitUser[i];
            newUser += "-";
        }
    } else {
        newUser = username;
    }
    
    window.location.href = "/Register/EditAuth/" + newUser;
}
function ListUser(pagenum, page, seach) {
    $.ajax({
        url: '/register/List',
        type: 'get',
        data: { pagenum, page, seach },
        success: function (data) {
            $('#tbd').empty();
            $('#tbdmodal').empty();
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                $.each(data.c, function (k, v) {
                    let table = '<tr id="'+v.id+'" role="row" class="odd">';
                    table +='<td>'+v.name+'</td>'
                    table += '<td>'+v.user+'</td>'
                    table += '<td>' + formatDate(v.createDate)+'</td>'
                    table += '<td>' + v.createBy+'</td>'
                    table += '<td>' + formatDate(v.modifyDate)+'</td>'
                    table += '<td>' + v.modifyBy+'</td>'
                    table += '<td ">' + v.ManageMainCategories + v.PurchaseManager + v.SalesManager
                                      + v.WarehouseManagement + v.SystemManagement + v.AccountManager
                                    + v.AddWarehouse + v.EditWarehouse + v.DeleteWarehouse
                                    + v.AddGoods + v.EditGoods + v.DeleteGoods
                                    + v.AddCustomer + v.EditCustomer + v.DeleteCustomer
                                    + v.AddTypeCustomer + v.EditTypeCustomer + v.DeleteTypeCustomer
                                    + v.AddUser + v.EditUser + v.EditPassWord + v.DeleteUser
                                    + v.BackupSql + v.AddGroupGoods + v.EditGroupGoods + v.DeleteGroupGoods
        
                          + '</td>'
                    table += '<td class="action" nowrap="nowrap">';
                    table += '<div class="dropdown dropdown-inline">';
                    table += '<a href="javascript:;" class="btn btn-sm btn-clean btn-icon" data-toggle="dropdown">';
                    table += '<i class="la la-cog"></i></a>';
                    table += '<div class="dropdown-menu dropdown-menu-sm dropdown-menu-right">';
                    table += '<ul class="nav nav-hoverable flex-column">';
                    table += ' <li class="nav-item">';
                    table += '<a class="nav-link" href="/Register/EditUser/' + v.id + '">';
                    table += '<i class="nav-icon la la-edit"></i>';
                    table += '<span class="nav-text">Sửa</span></a></li>';
                    table += ' <li class="nav-item">';
                    table += '<a class="nav-link" href="/Register/EditPass/' + v.id + '">';
                    table += '<i class="nav-icon la la-edit"></i>';
                    table += '<span class="nav-text">Sửa Mật Khẩu</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" onclick="printDiv(\'barcodeuser'+v.id+'\')">';
                    table += '<i class="nav-icon la la-print"></i><span class="nav-text">In</span></a></li>';
                    table += '<li class="nav-item"><a class="nav-link" name="delete">';
                    table += '<i class="nav-icon la la-trash"></i><span class="nav-text">Xóa</span></a></li>';
                    table += '</ul></div></div>';
                    table += '</td>';
                    table += '</tr>';
                    $('#tbd').append(table);
                    $('#tbdmodal').append(table);
                });

                //--------------------------------
                let kt_datatable_info = 'Showing 1 to ' + pagenum + ' of ' + data.count + ' entries'
                $('#kt_datatable_info').append(kt_datatable_info);
                //-----------------------------page---------------------------
                $('#page').empty();
                if (parseInt(page) >= 2) {
                    let pagemin = '<li id="' + 1 + '"><a class="a_1 a_2" >' + 1 + '...</a></li>';
                    $('#page').append(pagemin);
                    let pre = ' <li id="' + (parseInt(page) - 1) +'" class="paginate_button page-item previous disabled" >';
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

$('#pagenum').on('change',function () {
    var pagenum = $("#pagenum option:selected").val();
    var page = 1;
    var seach = "";
    ListUser(pagenum, page,seach)
})



//------------------------tim kiem------------------

$('#seach').on('keyup', function (e) {
        page = 1;
        seach = $('#seach').val();
        ListUser(pagenum, page, seach);
});
$('#print').click(function () {
    $('.modal-xl').css("max-width","100%")
    $('.action').css("display","none")
})
$('#close').click(function () {
    $('.print').css("display", "none");
    $('.action').css("display", "block")
})


//------------------------Delete--------------------
$(document).on('click', 'a[name="delete"]', function () {
    var id = $(this).closest('tr').attr('id');
    Swal.fire({
        title: 'Bạn Muốn Xóa User Này ?',
        text: "Hành động này sẽ không được phục hồi !!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/register/Delete',
                type: 'post',
                data: { id },
                success: function (data) {
                    if (data.code == 200) {
                        toastr.success(data.msg);
                        setTimeout(function () { window.location.href = "/Register/ListUser" }, 500);
                    } else {
                        toastr.error(data.msg);
                    }
                }
            });
        }
    });
})





