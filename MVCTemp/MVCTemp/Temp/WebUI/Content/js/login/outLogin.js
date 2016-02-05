/**
 * @authors 万三
 */
define(function (require) {
    require("jquery.cookie");
    window.outLogin = function () {
        $.cookie('$safeprojectname$UserMenuV', { path: '/' });
        window.location.href = "/Login/LoginOut";
    }
})