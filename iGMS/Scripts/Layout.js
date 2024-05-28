//--------------------Session::Name--------------
/*chat()*/
$(function UserSession() {
    $.ajax({
        url: '/home/UserSession',
        type: 'get',
        success: function (data) {
            if (data.code == 200) {
                $('span[name="userSession"]').append(data.user.FullName);
                $('span[name="firstUserSession"]').append(data.user.UserName);
                $('span[name="nameServer"]').append(data.server);
                $('span[name="nameData"]').append(data.database);
/*                $('span[name="firstUserSession"]').append(data.user.substring(0,1));*/
                //$('span[name="emailSession"]').append(data.email);
                //$('span[name="loginTimeSession"]').append(data.loginTime);
            } else {
                Swal.fire({
                    title: "Lỗi?",
                    text: data.msg,
                    icon: "error"
                });
            }
        }
    })
})


function chat() {
    $.ajax({
        url: '/home/Chat',
        type: 'get',
        success: function (data) {
            $('span[name="numberNotifi"]').attr('hidden', false)
            $('div[name="listChat"]').empty()
            if (data.code == 200) {               
                var numberNotifi = $('span[name="numberNotifi"]').text()
                $('span[name="numberNotifi"]').text(data.numNotYetNotice)
                if (numberNotifi == '0') {
                    $('span[name="numberNotifi"]').attr('hidden',true)
                }
                $.each(data.listChat, function (k, v) {
                    var listChat = `<div class="navi navi-icon-circle navi-spacer-x-0">
                                    <div class="d-flex align-items-center justify-content-between mb-5"data-toggle="modal" data-target=".kt_chat_modal" name="detailChat" id="id_token_${v.LatestMessage.IdUserSend}">
                                        <div class="d-flex align-items-center">
                                            <div class="symbol symbol-circle symbol-50 mr-3">
                                                <img alt="Pic" src="/assets/media/users/blank.png">
                                            </div>
                                            <div class="d-flex flex-column">
                                                <a class="text-dark-75 text-hover-primary font-weight-bold font-size-lg">${v.UserSend}</a>
                                                <span class="font-weight-bold font-size-sm" style="color:${(v.LatestMessage.Status == false ? "black" : "gray")}">${v.LatestMessage.Text.substring(0,50)}</span>
                                            </div>
                                        </div>
                                        <div class="d-flex flex-column align-items-end">
                                            <span class="text-muted font-weight-bold font-size-sm">${getTimeDifference(formatDateTime(v.LatestMessage.CreateDate))}</span>
                                            <span class="label label-sm label-danger">${v.Count}</span>
                                        </div>
                                    </div>
                                </div>`
                    $('div[name="listChat"]').append(listChat)
                })
               
            } else {
                
            }
        }
    })
}

$(document).on('click', 'div[name="detailChat"]', function (e) {
    var id = $(this).attr('id').substring(9)
    document.getElementById('scroll').scrollTop = document.getElementById('scroll').scrollHeight;
    detailChat(1,id)
})
var page = 1;
function detailChat(pageNumber,id) {
    $.ajax({
        url: '/home/DetailChat',
        type: 'get',
        data: { pageNumber, id },
        success: function (data) {
            if (data.code == 200) {
                if (id != $('#scroll').attr('data-idSend')) {
                    $('.messages').empty()
                    page = 1
                }
                $.each(data.chat, function (k, v) {
                    var chat = $('.chat').map(function () {
                        return this.id
                    }).get()
                    if (!chat.includes("chat"+v.Id)) {
                        $('div[name="nameSend"]').text(v.IdUserSend == id ? v.UserSend : v.UserReceipt)
                        $('span[name="statusSend"]').text(v.statusSend)
                        var div = `<div id="chat${v.Id}" class="chat d-flex flex-column mb-5 align-items-${v.IdUserSend == id ? "start" : "end"}">
                                    <div class="d-flex align-items-center">
                                        <div class="symbol symbol-circle symbol-40 mr-3">
                                            <img alt="Pic" src="assets/media/users/blank.png">
                                        </div>
                                        <div>
                                            <a href="#" class="text-dark-75 text-hover-primary font-weight-bold font-size-h6">${v.UserSend}</a>
                                            <span class="text-muted font-size-sm">${getTimeDifference(formatDateTime(v.CreateDate))}</span>
                                        </div>
                                    </div>
                                    <div class="mt-2 rounded p-5 bg-light-success text-dark-50 font-weight-bold font-size-lg text-left max-w-400px">${v.Text}</div>
                                </div>`
                        $('.messages').append(div)
                        $('#scroll').attr('data-idSend', id)
                    }
                 
                })
                if (data.pages < page) {

                } else {
                    page++;
                }
               
            } else {

            }
        }
    })
}

function detailChatNew(id) {
    var pageNumber = 1
    $.ajax({
        url: '/home/DetailChat',
        type: 'get',
        data: { pageNumber, id },
        success: function (data) {
            if (data.code == 200) {
                var chat = $('.chat').map(function () {
                    return this.id
                }).get()
                if (!chat.includes("chat" +data.chatNew.Id)) {
                    var div = `<div id="chat${data.chatNew.Id}" class="chat d-flex flex-column mb-5 align-items-${data.chatNew.IdUserSend == id ? "start" : "end"}">
                                    <div class="d-flex align-items-center">
                                        <div class="symbol symbol-circle symbol-40 mr-3">
                                            <img alt="Pic" src="assets/media/users/blank.png">
                                        </div>
                                        <div>
                                            <a href="#" class="text-dark-75 text-hover-primary font-weight-bold font-size-h6">${data.chatNew.UserSend}</a>
                                            <span class="text-muted font-size-sm">${getTimeDifference(formatDateTime(data.chatNew.CreateDate))}</span>
                                        </div>
                                    </div>
                                    <div class="mt-2 rounded p-5 bg-light-success text-dark-50 font-weight-bold font-size-lg text-left max-w-400px">${data.chatNew.Text}</div>
                                </div>`
                    $('.messages').prepend(div)
                    document.getElementById('scroll').scrollTop = document.getElementById('scroll').scrollHeight;
                }
            } else {

            }
        }
    })
}
var lastScrollTop = 0;
$('#scroll').scroll(function () {
    var id = $(this).attr('data-idSend')
    var element = $(this)[0];
    var scrollTop = element.scrollTop;
    if (scrollTop > lastScrollTop) {
        
    } else {
        // Đang trượt lên
        if (scrollTop <= 1) {
            detailChat(page, id)
        }
    }
    lastScrollTop = scrollTop;
});

$(document).on('keypress', '#valueMes', function (e) {
    if (e.which == 13) {
        Send()
    }
})

function Send() {
    var text = $('#valueMes').val()
    var id = $('#scroll').attr('data-idSend')
    $.ajax({
        url: '/home/Send',
        type: 'post',
        data: { text, id },
        success: function (data) {
            if (data.code == 200) {
                $('#valueMes').val('')
               
            } else {

            }
        }
    })
}
function getTimeDifference(sentTime) {
    var targetDate = new Date(sentTime);
    // Lấy thời gian hiện tại
    var now = new Date();
    // Chuyển đổi thời gian từ miligiây sang giây, phút, giờ và ngày
    var minutes = now.getMinutes() - targetDate.getMinutes();
    var hours = now.getHours() - targetDate.getHours();
    var days = now.getDate() - targetDate.getDate();
    // Xác định định dạng kết quả
    var result = '';

    if (days > 0) {
        result = days + ' ngày trước';
    } else if (hours > 0) {
        result = hours + ' giờ trước';
    } else if (minutes > 0) {
        result = minutes + ' phút trước';
    } else {
        result = 'vừa gửi';
    }

    return result;
}


function ChangeSession() {
    $.ajax({
        url: '/home/ChangeSession',
        type: 'post',
        success: function (data) {
            if (data.code == 200) {
           
            }
        }
    })
}
