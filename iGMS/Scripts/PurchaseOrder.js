var d = new Date();
var seach = '';
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------

//Tìm kiếm mã hàng
$('#seachidgood').on('change',function (e) {
        seach = $('#seachidgood').val().trim();
        ListGoods(seach)
})
function setArrayChecked() {
    var checkboxesChecked = [];
    var checkboxes = document.getElementsByName('change');
    // loop over them all
    for (var i = 0; i < checkboxes.length; i++) {
        // And stick the checked ones onto an array...
        if (checkboxes[i].checked) {
            if (checkboxesChecked.includes(checkboxes[i].value) == false) {
                checkboxesChecked.push({
                    "IdGoods": checkboxes[i].value,
                    "Quantity": $('#amount' + checkboxes[i].value + '').val().trim()
                });
            }
        } else {
            checkboxesChecked.filter(function (item) {
                return item !== checkboxes[i].value;
            });
        }
    }
    return checkboxesChecked;
}
//End

//-------------------------------------------------------------------------------------------------------------------------------------------------------------------

//hiển thị danh sách mã hàng

function ListGoods(id) {
    $.ajax({
        url: '/Goods/Detail',
        type: 'get',
        data: { id },
        success: function (data) {
            var Stt = $('#tbd').children('tr').length+1;
            $('#kt_datatable_info').empty();
            if (data.code == 200) {
                var ids = $('.nhanhang').map(function () {
                    return this.id;
                }).get();
                if (ids.includes(data.goods.Id)) {
                    var amount = $('#amount' + data.goods.Id + '').val().trim()
                    $('#amount' + data.goods.Id + '').val(Number(amount)+1)
                    $('#' + data.goods.Id + 'abc').attr('data-amount' + data.goods.Id + '', Number(amount) + 1)
                }
                else {
                    let table = '<tr id="' + data.goods.Id + '" role="row" class="odd nhanhang">';
                    table += '<td class="datatable-cell-sorted datatable-cell-center datatable-cell datatable-cell-check" data-field="RecordID" aria-label="2"><span style="width: 30px;"><label class="checkbox checkbox-single kt-checkbox--solid"><input id="' + data.goods.Id + 'abc" type="checkbox" checked="true" data-amount' + data.goods.Id + '="1" value="' + data.goods.Id + '" name="change" />&nbsp;<span></span></label></span></td>'
                    table += '<td>' + (Stt) + '</td>'
                    table += '<td>' + data.goods.Id + '</td>'
                    table += '<td>' + data.goods.Name + '</td>'
                    table += '<td><input type="number" value="1" name="sum" id="amount' + data.goods.Id + '" /></td>'
                    table += '<td>' + data.goods.nameGroupGoods + '</td>'
                    table += '<td>' + data.goods.nameUnit + '</td>'
                    table += '<td name="delete"><i class="icon-2x text-dark-50 flaticon-delete-1"></i></td>'
                    table += '</tr>';
                    $('#tbd').append(table);
                }
                $('#seachidgood').val('')
            }
            else {
                toastr.error(data.msg)
            }
        }
    })
}
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------

//Chọn tích tất cả các sản phẩm
$('#allchange').click(function () {
    var allchange = $('#allchange').is(":checked");
    if (allchange == false) {
        $('input[name="change"]').map(function () {
            $(this).prop('checked', false);
        })
    }
    else {
        $('input[name="change"]').map(function () {
            $(this).prop('checked', true);
        })
    }
})
//End
function Detail(id) {
    $.ajax({
        url: '/purchaseOrder/Detail',
        type: 'post',
        data: {id},
        beforeSend: function () {
            $("#wait").css("display", "block");
        },
        success: function (data) {
            $('span[name="iddh"]').empty()
            $('span[name="datedh"]').empty()
            $('span[name="customer"]').empty()
            $('span[name="address"]').empty()
            $('span[name="nameware"]').empty()
            $('#tbdmodal').empty()
            if (data.code == 200) {
                $('span[name="iddh"]').append(data.po.id)
                $('span[name="datedh"]').append(formatDate(data.po.createDate))
                $('span[name="customer"]').append(data.po.nameCustomer)
                $('span[name="address"]').append(data.po.addressCustomer)
                $('span[name="nameware"]').append(data.po.nameWarehouse)
                $.each(data.detailPo, function (k, v) {
                    let table = '<tr>';
                    table += '<td>' + (k + 1) + '</td>'
                    table += '<td>' + v.idGoods + '</td>'
                    table += '<td>' + v.nameGoods + '</td>'
                    table += '<td>' + v.quantity + '</td>'
                    table += '<td>' + v.nameGroupGoods + '</td>'
                    table += '<td>' + v.nameUnit + '</td>'
                    table += '</tr>';
                    $('#tbdmodal').append(table)
                })
            }
            else {
                toastr.error(resourcepurchase.TaoDonThatBai)
            }
        },
        complete: function () {
            $("#wait").css("display", "none");
        }
    })

}
//-------------------------------------------------------------------------------------------------------------------------------------------------------------------

//Thêm đơn mua hàng vào cơ sở dữ liệu
$(document).on('click', '#add', function (e){
    e.preventDefault();
    var form = document.getElementById("form");
    var formData = new FormData(form);
    for (var i = 0; i < setArrayChecked().length; i++) {
        formData.append('data[' + i + '].IdGoods', setArrayChecked()[i].IdGoods);
        formData.append('data[' + i + '].Quantity', setArrayChecked()[i].Quantity);
    }
    var tt = 0;
    $('input[name="sum"]').each(function () {
        tt += parseInt($(this).val()) || 0;
    });
    formData.append("tt", tt);
    $.ajax({
        url: '/purchaseorder/Add',
        type: 'post',
        data: formData,
        contentType: false, // Không thiết lập contentType để jQuery tự động xác định
        processData: false, // Không xử lý dữ liệu trước khi gửi
        success: function (data) {
            if (data.code == 200) {
                if ($('#tbd').children('tr').length === 0) {
                    toastr.warning(resourcepurchase.TimMaHHDeLapPhieu);
                } else {
                    toastr.success(data.msg)
                    Detail(data.id);
                    $('#BILL').modal('show')
                }
            }
            else {
                toastr.error(data.msg)
            }
        }
    })
})
$(document).on('click', 'td[name="delete"]', function (e) {
    $(this).closest('tr').remove()
})


