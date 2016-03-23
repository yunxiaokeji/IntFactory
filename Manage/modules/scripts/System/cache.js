define(function (require, exports, module) {
    require("jquery");
    var Verify = require("verify"),
        Global = require("global");

    var ObjectJS = {};
    
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    }

    ObjectJS.bindEvent = function () {
        $(".contentnew .btn").click(function () {
            ObjectJS.type = $(this).data("type");
            ObjectJS.clearCache();
        });
    }
    ObjectJS.clearCache = function () {

        Global.post("/System/ClearSystemCache", {type:ObjectJS.type}, function (data) {
            if (data.result == 1)
            {
                if(ObjectJS.type==1)
                    alert("分类缓存清理成功");
                else if(ObjectJS.type==2)
                    alert("属性缓存清理成功");
                else if (ObjectJS.type == 3)
                    alert("单位缓存清理成功");
            }
        });
    }

    module.exports = ObjectJS;
});