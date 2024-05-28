
//quét so sánh mã hàng và số lượng thực tế
var des = '';
var errorEPC = []
async function CompareReceipt(epc) {
    await CheckIdGoodsInSystem(epc)
        .then(function (data) {
            if (data?.status) {
                var more = $('.more').map(function () {
                    return this.id;
                }).get();
                var barcodes = epc
                if (more.includes("more" + barcodes + "")) {
                    $('#amountmore' + barcodes + '').text(Number($('#amountmore' + barcodes + '').text()) + 1)
                } else {
                    var tr = `<tr class="more" id="more${barcodes}">
                                      <td>${barcodes}</td>
                                      <td>Undeclared Data</td>
                                      <td id="amountmore${barcodes}">1</td>
                                       <td style="cursor: pointer;" name="location" id="location${barcodes}"><i class="nav-icon la la-search"></i></td>
                                      </tr>`
                    $('#des').append(tr)
                }
                errorEPC.push(
                    epc
                );
            } else {
                var statusEpc = data?.checkEpc?.Status
                if (statusEpc) {
                    var more = $('.more').map(function () {
                        return this.id;
                    }).get();
                    var barcodes = data?.checkEpc?.IdGoods
                    if (more.includes("more" + epc + "")) {
                        $('#amountmore' + epc + '').text(Number($('#amountmore' + epc + '').text()) + 1)
                    } else {
                        var tr = `<tr class="more" id="more${epc}">
                                      <td>${barcodes}</td>
                                      <td>${data?.checkEpc?.Name}</td>
                                      <td id="amountmore${epc}">1</td>
                                       <td style="cursor: pointer;" name="location" id="location${epc}"><i class="nav-icon la la-search"></i></td>
                                      </tr>`
                        $('#des').append(tr)
                    }
                    errorEPC.push(
                        epc
                    );
                } else {
                    var barcode = data?.checkEpc?.IdGoods;
                    var amountScan = $('.quantityscan' + barcode + '').eq(0).text();
                    var amountQuantity = $('#quantity' + barcode + '').text();
                    $('.quantityscan' + barcode + '').text(Number(amountScan) + 1);
                    if (Number(amountScan) + 1 < Number(amountQuantity)) {
                        $('.quantityscan' + barcode + '').css("background-color", 'red')
                    } else if (Number(amountScan) + 1 == Number(amountQuantity)) {
                        $('.quantityscan' + barcode + '').css("background-color", 'green')
                    } else {
                        $('.quantityscan' + barcode + '').css("background-color", 'yellow')
                    }
                }
            }
        })
        .catch(function (error) {
            // Xử lý lỗi
            toastr.error(error);
        })
    
}
$(document).on('click', 'td[name="location"]', function () {
    var epc = $(this).attr('id').substring(8)
    localStorage.setItem("locationEpc", epc)
    $.ajax({
        url: '/Epcs/LocationEpc',
        type: 'get',
        data: { epc }
    })
})
//kiểm tra mã hàng đã có trong hệ thống
function CheckIdGoodsInSystem(epc) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: '/Epcs/CheckIdGoodsInSystem',
            type: 'get',
            data: { epc },
            success: function (data) {
                if (data.code == 200) {
                    resolve(data);
                }
                else {
                    toastr.error(data.msg)
                    reject(data.msg);
                }
            },
            error: function (xhr, status, error) {
                reject(error);
            }
        });
    });
}
    //End

//-------------------add--------------------

function Add(statusSave) {
    Stop();
    var arrayGoods = [];
    var arrayRow = $('tbody').children(1).closest('tr.nhanhang');
    for (let i = 0; i < arrayRow.length / 2; i++) {
        var idGoods = arrayRow.eq(i).attr('id');
        var qtyScan = $('.quantityscan' + idGoods + '').eq(0).text();
        arrayGoods.push({
            "IdGoods": idGoods,
            "QuantityScan": qtyScan
        })
    }
    var stringError = ""
    $.each(arrayGoods, function (k, v) {
        var quantity = $('#quantity' + v.IdGoods + '').text();
        var check = Number(quantity) - Number(v.QuantityScan)
        if (check > 0) {
            stringError += resourcereceipt.mahh + " " + v.IdGoods + " " + resourcereceipt.thieu + " " + check + "\n"
        } else if (check < 0) {
            stringError += resourcereceipt.mahh + " " + v.IdGoods + " " + resourcereceipt.dư + " " +  Math.abs(check) + "\n"
        }
    })
    if (stringError == "") {
        Swal.fire({
            title: "Do you want to save the changes?",
            showDenyButton: true,
            icon: "warning"
        }).then((result) => {
            if (result.isConfirmed) {
                checkScan(arrayGoods, statusSave)
            } else if (result.isDenied) {

            }

        })
      
    } else {
        Swal.fire({
            title: stringError,
            showDenyButton: true,
            icon: "warning"
        }).then((result) => {
            if (result.isConfirmed) {
                checkScan(arrayGoods, statusSave)
            } else if (result.isDenied) {
               
            }
          
        })
    }
}
function checkScan(arrayGoods, statusSave) {
    var id = $('#id').val().trim();
    var purchaseorder = $('input[name="purchaseorder"]').val();
    var user1 = $('#user1').val();
    var user2 = $('#user2').val();
    var Des = $('[name="GhiChu"]').val();
    $.each(errorEPC, function (k, v) {
        epcshow = epcshow.filter(function (element) {
            return element !== v;
        })
    });
    $.ajax({
        url: '/receipt/Add',
        type: 'post',
        data: {
            statusSave,id, purchaseorder, user1, user2, Des, arrayGoods: JSON.stringify(arrayGoods), arrayepc: JSON.stringify(epcshow)
        },
        success: function (data) {
            if (data.status == 200) {
                toastr.success(data.msg);
                LastReceipt();
                $('#ghiChu').text(Des);
                $('#BILL').modal('show');
            } else if (data.status == 300) {
                Swal.fire({
                    title: resourcereceipt.canhbao,
                    text: data.msg,
                    icon: "warning",
                    showCancelButton: true,
                    confirmButtonText: resourcereceipt.quetlaitatca,
                    cancelButtonText: resourcereceipt.huy,
                    cancelButtonColor: "#d33",
                    showCloseButton: true,
                    showDenyButton: true,
                    denyButtonText: resourcereceipt.quetlaimatrung,
                }).then((result) => {
                    if (result.isConfirmed) {
                        //Quét lại tất cả.
                        epcshow = [];
                        $('td[class*="quantityscan"]').text("0").css('background', "red")
                        $('#des').empty();
                    } else if (result.dismiss === Swal.DismissReason.Deny) {
                        // Tìm kiếm lấy mã trùng gửi...
                        Swal.fire({
                            title: resourcereceipt.matrung,
                            html: data.epc,
                            icon: "info",
                            showCancelButton: true,
                            confirmButtonText: "Ok",
                            cancelButtonText: resourcereceipt.gui,
                            showCloseButton: true,
                        }).then((result) => {
                            if (result.isConfirmed) {
                                //Ok
                                var amountS = $('.quantityscan' + data.c).eq(0);
                                var amount = $('#quantity' + data.c);
                                var bill = $('#BILL .quantityscan' + data.c);
                                epcshow = epcshow.filter(function (element) {
                                    return element !== data.epc;
                                });
                                amountS.text(Number(amountS.text()) - 1);
                                if (Number(amountS.text()) < Number(amount.text())) {
                                    amountS.css("background-color", 'red')
                                    bill.css("background-color", 'red');
                                } else if (Number(amountS.text()) == Number(amount.text())) {
                                    amountS.css("background-color", 'green')
                                    bill.css("background-color", 'green');
                                } else {
                                    amountS.css("background-color", 'yellow')
                                    bill.css("background-color", 'yellow');
                                }
                                bill.text(amountS.text());
                            } else {
                                $.ajax({
                                    url: "receipt/sendepc",
                                    type: "get",
                                    data: { epc: data.epc },
                                    success: function (data) {

                                    }
                                })
                            }
                        });

                    } else {
                        //Hủy
                        Swal.close();
                    }
                });
            }
            else {
                toastr.error(data.msg);
            }
        },
        complete: function () {
            $("#wait").css("display", "none");
        }
    })
}
//-------------LastReceip----------
function LastReceipt() {
    var id = $('#id').val().trim();
    $.ajax({
        url: '/receipt/ReceipLast',
        type: 'get',
        data: {
            id
        },
        beforeSend: function () {
            $("#wait").css("display", "block");
        },
        success: function (data) {
            $('span[name="idpn"]').empty()
            $('span[name="datepn"]').empty()
            if (data.code == 200) {
                $.each(data.a, function (k, v) {
                    $('span[name="idpn"]').append(v.id)
                    $('span[name="datepn"]').append(v.datepn)
                })
            }
        },
        complete: function () {
            $("#wait").css("display", "none");
        }
    })
}
var start;
function RFID() {
    DeleteEpc()
    $('.on').css('display', 'none')
    $('.off').css('display', 'block')
    start = setInterval(function () { AllShowEPC() }, 200);
}

//stop scanner
function Stop() {
    $('.on').css('display', 'block')
    $('.off').css('display', 'none')
    clearInterval(start);
}
function StopHH() {
    $('.on').css('display', 'block')
    $('.off').css('display', 'none')
    rfid.disconnect();
}
var epcshow = [];
function AllShowEPC() {
    $.ajax({
        url: '/rfid/AllShowEPC',
        type: 'get',
        success: function (data) {
            if (data.code == 200) {
                $.each(data.a, function (k, v) {
                    $.ajax({
                        url: '/rfid/falseEPC',
                        type: 'post',
                        data: { epc: v.id },
                        success: function (data) {
                            if (data.code == 200) {
                                if (!epcshow.includes(v.id)) {
                                    epcshow.push(v.id)
                                    CompareReceipt(v.id)
                                }

                            }

                        }
                    })

                })
            }
        }
    })
}
function DeleteEpc() {
    $.ajax({
        url: '/rfid/DeleteEPC',
        type: 'post',
    })
}

function RFIDHandHeld() {
    rfid.statusEvent = "statusEvent(%json)";
    rfid.tagEvent = "TagHandler(%json)";
    RFIDEnumrate();
}
//RFID
function TagHandler(tagarray) {
    for (i = 0; i < tagarray.TagData.length; i++) {
        rfid.beepOnRead = true;
        var epc = tagarray.TagData[i].tagID
        if (!epcshow.includes(epc)) {
            epcshow.push(epc)
            CompareReceipt(epc)
        }
    }

}
function RFIDEnumrate() {
    rfid.transport = 'serial'
    rfid.readerID = 'RFID1';
    rfid.enumerate();
    setTimeout(function () {
        rfid.connect();
        rfid.startTriggerType = "triggerPress";
        rfid.performInventory();
        rfid.beepOnRead = 1;
    }, 1000);
}
function StopHH() {
    rfid.stop();
    rfid.disconnect();
}




