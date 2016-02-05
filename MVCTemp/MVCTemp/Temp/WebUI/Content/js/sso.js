$(document).ready(function () {
        getRequest();
});

function getRequest() {
    $.ajax({
        type: "Post",
        url: "../ashx/SSOHandler.ashx?type=GetUrl",
        dataType: "json",
        async: true,
        success: function (data) {
            if (data != null && data != undefined && data.status) {
                var url = data.url;
                if (url.indexOf("?") > -1) {
                    url = url + "&callback=?";
                }
                else {
                    url = url + "?callback=?";
                }

                $.ajax({
                    url: url,
                    contentType: "application/json; charset=utf-8",
                    dataType: "jsonp",
                    callback: "?",
                    success: function (data) {
                        if (data != null) {
                            if (data != undefined && data.status) {
                                if (data.sso != null && data.sso != undefined) {
                                    data.sso && $.each(data.sso,
                                    function () {
                                        $.ajax({
                                            url: this,
                                            contentType: "application/json; charset=utf-8",
                                            dataType: "jsonp",
                                            callback: "?",
                                            success: function () {
                                            },
                                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                            }
                                        });

                                    });
                                }
                            }
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert('status:' + XMLHttpRequest.status);
                        //alert('readyState:' + XMLHttpRequest.readyState);
                        //alert('responseText:'+ XMLHttpRequest.responseText);  
                        //alert('textStatus:' + textStatus);
                    }
                });
            }
        },
        error: function (err) {
        }
    });
}

/**
*  func  getCookie()  获取单个cookie的值
*  pram  cookieName  cookie的名称
**/
function getCookie(cookieName) {
    var cookieObj = {},
            cookieSplit = [],
            cookieArr = document.cookie.split(";");
    for (var i = 0, len = cookieArr.length; i < len; i++)
        if (cookieArr[i]) {
            // 以等号（=）分组
            cookieSplit = cookieArr[i].split("=");

            cookieObj[$.trim(cookieSplit[0])] = $.trim(cookieSplit[1]);
        }

    return cookieObj[cookieName];
}

function setCookie(name, value) {
    document.cookie = name + "=" + decodeURIComponent(value);
}