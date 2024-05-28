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
                                      <td style="cursor: pointer;" name="location" id="location${epc}"><i class="nav-icon la la-search"></i></td>
                                      </tr>`
                    $('#des').append(tr)
                }
                errorEPC.push(
                    epc
                );
            } else {
                var statusEpc = data?.checkEpc?.Status
                if (statusEpc) {
                    var barcode = data?.checkEpc?.IdGoods;
                    var nameWareHouse = data?.checkEpc?.Name;
                    var nameWareHouseCurrent = $('#nameWarehouse').val()
                    if (nameWareHouse != nameWareHouseCurrent) {
                        var more = $('.more').map(function () {
                            return this.id;
                        }).get();
                        if (more.includes("more" + epc + "")) {
                            $('#amountmore' + epc + '').text(Number($('#amountmore' + epc + '').text()) + 1)
                        } else {
                            var tr = `<tr class="more" id="more${epc}">
                                      <td>${barcode}</td>
                                      <td>${nameWareHouse}</td>
                                      <td id="amountmore${epc}">1</td>
                                      <td style="cursor: pointer;" name="location" id="location${epc}"><i class="nav-icon la la-search"></i></td>
                                      </tr>`
                            $('#des').append(tr)
                        }
                    }
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
                } else {
                    var more = $('.more').map(function () {
                        return this.id;
                    }).get();
                    var barcodes = data?.checkEpc?.IdGoods
                    if (more.includes("more" + epc + "")) {
                        $('#amountmore' + epc + '').text(Number($('#amountmore' + epc + '').text()) + 1)
                    } else {
                        var tr = `<tr class="more" id="more${epc}">
                                      <td>${barcodes}</td>
                                      <td>Goods Not Imported</td>
                                      <td id="amountmore${epc}">1</td>
                                      <td style="cursor: pointer;" name="location" id="location${epc}"><i class="nav-icon la la-search"></i></td>
                                      </tr>`
                        $('#des').append(tr)
                    }
                    errorEPC.push(
                        epc
                    );
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
        data: {epc}
    })
})
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
//-------------------add--------------------
function Add(statusSave) {
    Stop();
    var ArraySales = [];
    var arrayRow = $('tbody').children(1).closest('tr.nhanhang');
    for (let i = 0; i < arrayRow.length/2; i++) {
        var idgood = arrayRow.eq(i).attr('id');
        var qtyScan = $('.quantityscan' + idgood + '').eq(0).text();
        var qty = $('#quantity' + idgood + '').text();
        ArraySales.push({
            "IdGoods": idgood,
            "Quantity" : qty,
            "QuantityScan": qtyScan
        })
    }
    var stringError = ""
    $.each(ArraySales, function (k, v) {
        var quantity = $('#quantity' + v.IdGoods + '').text();
        var check = Number(quantity) - Number(v.QuantityScan)
        if (check > 0) {
            stringError += resourcedelivery.mahh + " " + v.IdGoods + " " + resourcedelivery.thieu + " " + check + "\n";
        } else if (check < 0) {
            stringError += resourcedelivery.mahh + " " + v.IdGoods + " " + resourcedelivery.dư + " " + Math.abs(check) + "\n"
        }
    })
    if (stringError == "") {
        Swal.fire({
            title: "Do you want to save the changes?",
            showDenyButton: true,
            icon: "warning"
        }).then((result) => {
            if (result.isConfirmed) {
                checkScanSurplus(ArraySales, statusSave);
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
                checkScanSurplus(ArraySales, statusSave);
            } else if (result.isDenied) {

            }

        })
    }
}
function checkScanSurplus(ArraySales, statusSave) {
    var id = $('#id').val().trim();
    var idSaleOrder = $('input[name="idsalesorder"]').val();
    var Des = $('[name="GhiChu"]').val();
    $.each(errorEPC, function (k, v) {
        epcshow = epcshow.filter(function (element) {
            return element !== v;
        })
    });
    $.ajax({
        url: '/Delivery/Add',
        type: 'post',
        data: {
            statusSave,id, idSaleOrder, Des, ArraySales: JSON.stringify(ArraySales), ArrayEPC: JSON.stringify(epcshow)
        },
        success: function (data) {
            if (data.status == 200) {
                toastr.success(data.msg)
                $('#ghiChu').text(Des);
                $('#BILL').modal('show');
                Last();
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
//-------------Last----------
function Last() {
    var id = $('#id').val().trim();
    $.ajax({
        url: '/delivery/Last',
        type: 'get',
        data: {
            id
        },
        beforeSend: function () {
            $("#wait").css("display", "block");
        },
        success: function (data) {
            $('span[name="idpx"]').empty()
            $('span[name="datepx"]').empty()
            if (data.code == 200) {
                $.each(data.a, function (k, v) {
                    $('span[name="idpx"]').append(v.id)
                    $('span[name="datepx"]').append(v.datepx)
                })

            }
        },
        complete: function () {
            $("#wait").css("display", "none");
        }
    })
}

//-----------------------------
//$(document).scannerDetection({
//    timeBeforeScanTest: 200, // wait for the next character for upto 200ms
//    startChar: [120],
//    endChar: [13], // be sure the scan is complete if key 13 (enter) is detected
//    avgTimeByChar: 40, // it's not a barcode if a character takes longer than 40ms
//    ignoreIfFocusOn: false, // turn off scanner detection if an input has focus
//    minLength: 1,
//    onComplete: function (barcode, qty) {
//        CompareReceipt(barcode)

//    }, // main callback function
//    scanButtonKeyCode: 116, // the hardware scan button acts as key 116 (F5)
//    scanButtonLongPressThreshold: 5, // assume a long press if 5 or more events come in sequence
//    onError: function (string) { alert('Error ' + string); }
//});

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

