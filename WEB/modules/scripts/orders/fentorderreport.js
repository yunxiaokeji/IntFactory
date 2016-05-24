define(function (require,exports,module) {
    var ObjectJS = {};
    ObjectJS.init = function (plate) {
        ObjectJS.bindEvent(plate);
        ObjectJS.removeTaskPlateOperate();
        ObjectJS.addTaskPlateCss();
    };

    ObjectJS.bindEvent = function (plate) {
        if (plate=="") {
            $("#Platemak").html('<tr><td class="no-border" style="width:500px;font-size:15px;">暂无！</td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        }
        
    };

    //删除行操作按钮
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#Platemak table").find("tr:first").find("td").css("border-top","0").css("border-bottom","1px solid");
        $("#Platemak table tr").each(function () {
            $(this).find("td:last").remove();
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");            
        });
        $("#Platemak table").css("border", "0");
    };

    ObjectJS.addTaskPlateCss = function () {
        $("#Processing").find("tr:last").find("td").css("border-bottom","0");
    }

    module.exports = ObjectJS;
});