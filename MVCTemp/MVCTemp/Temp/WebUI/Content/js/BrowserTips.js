/**
 * @authors 万三
 */
define(function (require, exports, module) {
    require("jquery.cookie");
    //依赖于jquery.cookie
    try {
        var cookieName = "BrowserTips";
        var tips = "<div id=\"browserTips\" style=\"background: #FFFF00;color: #FF0000;line-height: 25px;font-size: 13px;text-align:center;z-index: 9999;height: 25px;\">您的浏览器版本太低啦！这会造成您无法愉快的使用系统的功能，为了帮助您赚取更多利润，我们强烈建议您使用" +
"<a href=\"http://kegui.jptonghang.com/dm/Reminder/ieupdate.html\" target=\"_blank\">IE8</a>、" +
"<a href=\"http://se.360.cn/\" target=\"_blank\">360浏览器</a>&nbsp;&nbsp;<span style=\"cursor: pointer;border: 1px solid;padding: 0px 4px;cursor: pointer;\" onclick=\"window.CloseBrowserTips();\">X</span></div> ";

        function getBrowser() {
            var sys = {},
                    ua = navigator.userAgent.toLowerCase(),
                    s;
            (s = ua.match(/rv:([\d.]+)\) like gecko/)) ? sys.ie = s[1] :
                    (s = ua.match(/msie ([\d.]+)/)) ? sys.ie = s[1] :
                    (s = ua.match(/firefox\/([\d.]+)/)) ? sys.firefox = s[1] :
                    (s = ua.match(/chrome\/([\d.]+)/)) ? sys.chrome = s[1] :
                    (s = ua.match(/opera.([\d.]+)/)) ? sys.opera = s[1] :
                    (s = ua.match(/version\/([\d.]+).*safari/)) ? sys.safari = s[1] : 0;
            //先判断内核
            if (sys.ie) {
                var version = parseInt(sys.ie, 10);
                if (version < 8) {
                    return true;
                }
            }
            if (!sys.ie && !sys.chrome) {
                return true;
            }

            return false;
        }

        window.CloseBrowserTips = function () {
            $("#browserTips").hide();
            //加一个cookie。关闭提示
            $.cookie(cookieName, 1, { path: '/' });
        };

        var cookieValue = $.cookie(cookieName);
        //判断有没得cookie，如果没得就提示
        if (cookieValue === "" || cookieValue === null || typeof cookieValue === "undefined") {
            if (getBrowser()) {
                $("body").prepend(tips);
            }
        }
    }
    catch (e) {
        //不影响流程
    }
});