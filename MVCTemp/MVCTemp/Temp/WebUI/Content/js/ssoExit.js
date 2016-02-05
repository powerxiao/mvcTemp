$(document).ready(function() {
    var flg = $.query.get('type');
    if (flg != null && flg == "exit") {
        getRequest();
    }
});

function getRequest() {

    $.ajax({
        type: "Post",
        url: "../handler/SSOExitHandler.ashx?type=exit",
        dataType: "json",
        async: true,
        success: function(data) {

            if (data != null && data.status) {
                var url = data.url;

                if (url.indexOf("?") > -1) {
                    url = url + "&callback=?";
                }
                else {
                    url = url + "?callback=?";
                }
                //alert(url);
                $.getJSON(url, function(data) {

                    if (data != null) {

                        if (data && data.status) {

                            data.sso && $.each(data.sso,
                                    function() {
                                        //alert(this);
                                        $.getJSON(this);
                                    });
                        }
                    }
                });
            }
        },
        error: function(err) {

        }
    });
}