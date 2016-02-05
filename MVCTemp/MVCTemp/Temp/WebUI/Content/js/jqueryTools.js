(function ($) {
    // 数组对象转换成Json对象
    $.fn.serializeJson = function () {
        var serializeObj = {};
        var array = this.serializeArray();
        var str = this.serialize();
        $(array).each(function () {
            if (serializeObj[this.name]) {
                if ($.isArray(serializeObj[this.name])) {
                    serializeObj[this.name].push(this.value);
                } else {
                    serializeObj[this.name] = [serializeObj[this.name], this.value];
                }
            } else {
                serializeObj[this.name] = this.value;
            }
        });
        return serializeObj;
    };

    // Json对象转换成json字符串
    $.fn.jsonToString = function () {
        var parameters = $(this)[0];
        var arrResult = [];
        for (var key in parameters) {
            var parValue = parameters[key];

            if (parValue === null || parValue === undefined) {
                continue;
            }
            parValue = filterKeyWord(parValue); // 过滤"\"以免json序列化问题

            arrResult.push('"' + key + '":"' + parValue + '"');
        }

        var json = '{' + arrResult.join(',') + '}';
        return json;
    }

    // 转义"\"特殊字符
    function filterKeyWord(strKeyWord) {
        if (strKeyWord.indexOf("\\") === -1) {
            return strKeyWord;
        }

        var str = strKeyWord.split("\\");
        var len = str.length;
        var arr = [];
        for (var i = 0; i < len; i++) {
            var keyValue = str[i];
            arr.push(keyValue);
        }

        return arr.join("\\\\");
    }

    // 单选按钮列表选择
    $.fn.radioSelect = function () {
        var self = $(this);
        var result = "";
        $(self).each(function () {
            if ($(this).attr("checked")) {
                result = $(this).val();
                return false;
            }
        })
    }
 

})(jQuery);