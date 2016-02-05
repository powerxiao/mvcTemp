(function (window) {
    var globol = $("#txtUser");
    window.closeme = function () {
        var div = document.getElementById("ie6-warning");
        div.style.display = "none";
    };

    window.login = function () {
        // 清除 cooke ========================================
        $.cookie('$safeprojectname$UserMenuV', null, { path: '/' });

        $("#liMsg").hide().html("");
        var userId = $("#txtUser").val();
        if (userId == "" || userId == undefined) {
            layer.alert("请输入用户名！");
            $('#txtUser').focus();
            $('#txtUser').blur();
            globol = $('#txtUser');
            return;
        }

        var pwd = $("#txtPwd").val();
        if (pwd == "" || pwd == undefined) {
            layer.alert("请输入密码！");
            $("#txtPwd").focus();
            $("#txtPwd").blur();
            globol = $('#txtPwd');
            return;
        }

        var validateCode = $("#txtValidateCode").val();
        if (validateCode == "" || validateCode == undefined) {
            layer.alert("请输入验证码！");
            $("#txtValidateCode").focus();
            $("#txtValidateCode").blur();
            globol = $("#txtValidateCode");
            return;
        }
        $.ajax({
            type: "POST",
            url: "/Login/LoginIndex",
            cache: false,
            dataType: "json",
            data: { userID: userId, pwd: pwd, validateCode: validateCode },
            beforeSend: function () {
                $("#btnLogin").val("登录中...");
            },
            success: function (data, status) {
                var msg = decodeURIComponent(data);
                if (msg == "登陆成功") {
                    window.location.href = "/Home/Index";
                }
                else {
                    $("#btnLogin").val("登录")
                    $("#imgValidate").click();
                    $("#txtValidateCode").val("");
                    $("#liMsg").show().html(msg);
                }
            },
            error: function (e) {
                $("#btnLogin").val("登录")
                $("#liMsg").show().html(decodeURIComponent(e.responseText));
            }
        });
    };

    window.VisiCode = function () {
        $("#spValidateCode").show();
        $("#txtValidateCode").val("").css({ "color": "black" });
    };

    window.keyDown = function () {
        if (event.keyCode == 32) {
            layer.closeAll();
            globol.focus();
        }
        if (event.keyCode == 13) {
            if (event.srcElement.tagName.toLowerCase() === 'input') {
                login();
            }
            else {
                layer.closeAll();
                globol.focus();
            }
        }
    };
})(window);