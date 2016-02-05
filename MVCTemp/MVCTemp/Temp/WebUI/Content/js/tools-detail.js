/**
 * 详细页面常用工具类
 * @authors guojia (chenxiaoyang)
 * @date    2015-05-12 20:23:01
 * @version $Id$
 */
define(function(require, exports, module) {
    require('html5Validate');

    /*============================ 内部变量 ==========================*/
    var validateMsg = function($ctrlName, msg) {
        var option = {
            css: {
                zIndex: 19891020
            }
        };
        $ctrlName.testRemind(msg, option).get(0).focus();
    }


    /**
     * 数据反射到对应页面DOM
     * @param  {Jobject} bindCtrl 对应DOM的JQuery对象
     * @param  {object} dataObj  数据对象
     */
    exports.dataReflection = function(bindCtrl, dataObj) {
        var $ctrl, $this, eleVal;
        if (bindCtrl && dataObj) {
            $ctrls = bindCtrl;
            $ctrls.each(function() {
                $this = $(this);
                if ($this.attr("name")) {
                    eleVal = dataObj[$this.attr("name")];
                    if (typeof eleVal !== "undefined")
                        switch ($this.attr("type")) {
                            case "checkbox":
                                if (typeof(eleVal) === "string" && eleVal.indexOf("/") >= 0 && (eleVal.indexOf($this.val() + "/") >= 0 || (eleVal.indexOf("/" + $this.val()) >= 0))) {
                                    $this.attr("checked", true);
                                } else {
                                    if ($this.val() == eleVal) {
                                        $this.attr("checked", true);
                                    }
                                }
                                break;
                            case "radio":
                                if ($this.val() == eleVal) {
                                    $this.attr("checked", true);
                                }
                                break;
                            default:
                                $this.val(eleVal);
                        }
                }
            })
        }
    };

    /**
     * 获取对应URL键的值
     * @param  {string} name 键名
     * @return {string}      对应的值
     */
    exports.getUrlParam = function(name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"),
            r = window.location.search.substr(1).match(reg);
        if (r != null) {
            return decodeURIComponent(r[2]);
        }
        return null;
    }

    /**
     * 正则校验数据格式
     * @param  {array} fieldArr 要校验的属性数组
     * @param  {regex} keyupReg keyup正则
     * @param  {regex} blurReg  blur正则
     */
    exports.checkData = function(fieldArr, keyupReg, blurReg) {
        // add validate on input
        var value;
        for (var i = fieldArr.length - 1; i >= 0; i--) {
            $("input[name=" + fieldArr[i] + "]").on("keyup", function() {
                value = $(this).val();
                value = value.replace(keyupReg, '').toUpperCase();
                $(this).val(value);
            });
            if (blurReg) {
                $("input[name=" + fieldArr[i] + "]").on("blur", function() {
                    value = $(this).val();
                    if (value !== "") {
                        if (!blurReg.test(value)) {
                            validateMsg($(this), "格式不正确");
                        } else {
                            $(this).val(parseFloat(value, 2).toFixed(2));
                        }
                    }
                })
            }
        };
    }

    /**
     * 验证非空
     * @param  {array}  fieldArr  验证字段数组
     * @param  {string}  dom4Check 验证的dom对应id
     * @param  {Boolean} isAlert   是否弹框提示
     * @return {Boolean}            是否验证通过
     */
    exports.checkRequired = function(fieldArr, dom4Check) {
        var $checkCtrl = $("#" + dom4Check + ""),
            $tmpCtrl, flag = true;
        if (Array.isArray(fieldArr)) {
            for (var i = 0; i < fieldArr.length; i++) {
                $tmpCtrl = $checkCtrl.find("[name=" + fieldArr[i] + "]");
                if ($tmpCtrl.length === 1) {
                    if ($.trim($tmpCtrl.val()) === "") {
                        flag = false;
                        break;
                    }
                }
            };
        } else if (typeof fieldArr === "string") {
            $tmpCtrl = $checkCtrl.find("[name=" + fieldArr + "]");
            if ($tmpCtrl.length === 1) {
                flag = false;
            }
        }
        if (flag === false) {
            validateMsg($tmpCtrl, "必填");
        }
        return flag;
    }

    /**
     * 获取次方数组的次方和
     * @param  {array} values 次方数组
     * @return {number}       次方和
     */
    exports.getPowValue = function(values) {
        var result = 0;
        if (Array.isArray(values)) {
            for (var i = 0; i < values.length; i++) {
                result += parseInt(values[i]);
            };
        } else {
            result = parseInt(values);
        }
        return result;
    }

    /**
     * 次方数组反绑到checkbox
     * @param {JObject} $ctrl checkbox数组
     * @param {number} sValue 属性值
     */
    exports.setPowValue = function($ctrl, sValue) {
        var pValue;
        for (var i = 0; i < $ctrl.length; i++) {
            pValue = $ctrl.eq(i).val();
            if ((sValue & pValue) == pValue) {
                $ctrl.eq(i).attr("checked", true);
            }
        };
    }
});
