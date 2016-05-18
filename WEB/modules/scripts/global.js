define(function (require, exports, module) {
    var Global = {},
        jQuery = require("jquery");

    Global.againSubmitText = "数据加载中，请勿重复操作";

    Global.post = function (url, params, callback, anync) {
        jQuery.ajax({
            type: "POST",
            url: url,
            data: params,
            dataType: "json",
            async: !anync,
            cache: false,
            success: function (data) {
                if (data.error) {
                    return;
                } else {
                    !!callback && callback(data);
                }
            }
        });
    }
    //格式化日期
    Date.prototype.toString = function (format) {
        var o = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "h+": this.getHours(),
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            "S": this.getMilliseconds()
        }

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    };
    //日期字符串转换日期格式
    String.prototype.toDate = function (format) {
        var d = new Date();
        d.setTime(this.match(/\d+/)[0]);
        return (!!format) ? d.toString(format) : d;
    }
    //截取字符串
    String.prototype.subString = function (len) {
        if (this.length > len) {
            return this.substr(0, len-1) + "...";
        }
        return this;
    }
    //判断字符串是否整数
    String.prototype.isInt = function () {
        return this.match(/^(0|([1-9]\d*))$/);
    }
    //判断字符串是否数字
    String.prototype.isDouble = function () {
        return this.match(/^\d+(.\d+)?$/);
    }

    /*重写alert*/
    window.alert = function (msg, url) {
        $("#window_alert").remove();

        var _alter = $("<div id='window_alert' class='alert'></div>");
        var _header = $("<div class='alert-header'>提示</div>");
        var _wrap = $("<div class='alert-wrap'></div>").html(msg);
        var _bottom = $("<div class='alert-bottom'></div>"),
            _close = $("<div class='confirm right'>立即关闭</div>");
        _bottom.append(_close);
        _alter.append(_header).append(_wrap).append(_bottom);
        _alter.appendTo("body");

        var left = $(window).width() / 2 - (_alter.width() / 2);
        _alter.offset({ left: left });
        _close.click(function () {
            _alter.remove();
            if (url) {
                location.href = url;
            }
        });
        setTimeout(function () {
            _alter.remove();
            if (url) {
                location.href = url;
            }
        }, 5000);
    }

    /*重写confirm*/
    window.confirm = function (msg, confirm, cancel) {
        $("#window_confirm").remove();
        var _layer = $("<div class='alert-layer'><div>")
        var window_confirm = $("<div id='window_confirm' class='alert'></div>");
        var _header = $("<div class='alert-header'>提示</div>");
        var _wrap = $("<div class='alert-wrap'></div>").html(msg);
        var _bottom = $("<div class='alert-bottom'></div>"),
            _close = $("<div class='close mLeft10'>取消</div>"),
            _confirm = $("<div class='confirm mRight10'>确认</div>");

        _bottom.append(_confirm).append(_close);
        window_confirm.append(_header).append(_wrap).append(_bottom);

        _layer.appendTo("body");
        window_confirm.appendTo("body");

        var left = $(window).width() / 2 - (window_confirm.width() / 2);
        window_confirm.offset({ left: left });

        _close.click(function () {
            _layer.remove();
            window_confirm.remove();
            cancel && cancel();
        });
        _confirm.click(function () {
            _layer.remove();
            window_confirm.remove();
            confirm && confirm();
        });
    }

    /*生成GUID*/
    Global.guid = function () {
        var S4 = function () {
            return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
        };
        return (guid = S4() + S4() + "-" + S4() + "-" + S4() + "-" + S4() + "-" + S4() + S4() + S4());
    }

    Global.validateMobilephone = function (mobilephone) {
        var reg = /^0?1[3|4|5|8][0-9]\d{8}$/;

        return reg.test(mobilephone);
    }

    //数字转化为千位
    Number.prototype.toMoney = function () {
        var _value, _arr, _int, _decimal, _re;
        _value = this.toString();
        _arr = _value.split(".");
        _int = _arr[0], _decimal = "00";
        if (_arr.length > 1) {
            _decimal = _arr[1];
        }
        _int = _int.replace(/^(-?\d*)$/, "$1,");

        _re = /(\d)(\d{3},)/;
        while (_re.test(_int)) {
            _int = _int.replace(_re, "$1,$2");
        }
        _value = _int + _decimal;
        _value = _value.replace(/,(\d*)$/, ".$1");
        return _value;
    };

    //字符格式数字转化为千位
    String.prototype.toMoney = function () {
        var _value, _arr, _int, _decimal, _re;
        _value = this;
        _arr = _value.split(".");
        _int = _arr[0], _decimal = "00";
        if (_arr.length > 1) {
            _decimal = _arr[1];
        }
        _int = _int.replace(/^(-?\d*)$/, "$1,");

        _re = /(\d)(\d{3},)/;
        while (_re.test(_int)) {
            _int = _int.replace(_re, "$1,$2");
        }
        _value = _int + _decimal;
        _value = _value.replace(/,(\d*)$/, ".$1");
        return _value;
    };

    //获取密码复杂等级
    Global.passwordLevel=function(password) {
        var Modes = 0;
        for (i = 0; i < password.length; i++) {
            Modes |= Global.CharMode(password.charCodeAt(i));
        }
        return Global.bitTotal(Modes);
       
    }

    //CharMode函数
    Global.CharMode=function (iN) {
        if (iN >= 48 && iN <= 57)//数字
            return 1;
        if (iN >= 65 && iN <= 90) //大写字母
            return 2;
        if ((iN >= 97 && iN <= 122) || (iN >= 65 && iN <= 90))
            //大小写
            return 4;
        else
            return 8; //特殊字符
    }

    //bitTotal函数
    Global.bitTotal=function (num) {
        modes = 0;
        for (i = 0; i < 4; i++) {
            if (num & 1) modes++;
            num >>>= 1;
        }
        return modes;
    }

    //查看结果
    Global.replaceQqface= function (str) {
        str = str.replace(/\</g, '&lt;');
        str = str.replace(/\>/g, '&gt;');
        str = str.replace(/\n/g, '<br/>');
        str = str.replace(/\[em_([0-9]*)\]/g, '<img align="absbottom" src="/modules/plug/qqface//arclist/$1.gif" border="0" />');
        return str;
    }

    module.exports = Global;
});