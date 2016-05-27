define(function (require,exports,module) {
    var ObjectJS = {};
    ObjectJS.init = function (plate,price,costprice) {
        ObjectJS.bindEvent(plate, price, costprice);
        ObjectJS.removeTaskPlateOperate();
        ObjectJS.addTaskPlateCss();
    };

    ObjectJS.bindEvent = function (plate, price, costprice) {
        if (plate == "") {
            $("#Platemak").html('<tr><td class="no-border" style="width:500px;font-size:15px;">暂无！</td></tr>')
        } else {
            $("#Platemak").html(decodeURI(plate));
        };

        $("#btnWord").click(function () {            
            ObjectJS.tableWord();
        });
        var conclusion = parseInt( price) +parseInt( costprice);
        
        $(".conclusion").html(conclusion);
        
    };

    //删除行操作按钮
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#Platemak table").find("tr:first").find("td").css({ "border-top": "0", "border-bottom": "1px solid", "font-size": "16px" });
        $("#Platemak table").find("tr").find("td").find("span").css("margin-left","40%");        
        $("#Platemak table tr").each(function () {            
            $(this).find("td:last").remove();            
            $(this).find("td:last").css("border-right", "0");
            $(this).find("td:first").css("border-left", "0");            
        });
        $("#Platemak table").css("border", "0").css("height","100%");
    };

    ObjectJS.addTaskPlateCss = function () {
        $(".Processing").find("tr:last").find("td").addClass("no-border-bottom");
    }

    ObjectJS.tableWord = function () {
        var elTable = document.getElementById("tabletoexcel");
        
        if (!document.body.createTextRange) {
            //非IE情况下
            
            //var oWD = new ActiveXObject("Word.Application");
            //var oDC = oWD.Documents.Add("", 0, 1);
            //var orange = oDC.Range(0, 1);            
            //orange.Paste();
            //oWD.Application.Visible = true;
            //oWD = true;
        } else {
            //IE情况下
            var sel = document.body.createTextRange();
            sel.moveToElementText(elTable);
            sel.execCommand("Copy");
            var oWD = new ActiveXObject("Word.Application");
            var oDC = oWD.Documents.Add("", 0, 1);
            var orange = oDC.Range(0, 1);            
            orange.Paste();
            oWD.Application.Visible = true;
            oWD = true;
        }
        ///table转换execl表格《不可用【只有在IE下可用】》

        //var selection; //申明range 对象
        //if (window.getSelection) {
        //    //主流的浏览器，包括mozilla，chrome，safari
        //    selection = window.getSelection();
           
        //} else if (document.selection) {
        //    selection = document.selection.createRange();//IE浏览器下的处理，如果要获取内容，需要在selection 对象上加上text 属性
            
        //}

        //var elTable = document.getElementById("tabletoexcel"); //table1改成你的tableID
        
        //var oRangeRef = document.body.createTextRange();

        //oRangeRef.moveToElementText(elTable);

        //oRangeRef.execCommand("Copy");

        //try {
        //    var appExcel = new ActiveXObject("Excel.Application");
        //} catch (e) {
        //    alert("无法调用Office对象，请确保您的机器已安装了Office并已将本系统的站点名加入到IE的信任站点列表中！");
        //    return;
        //}

        //appExcel.Visible = true;

        //appExcel.Workbooks.Add().Worksheets.Item(1).Paste();

        //appExcel = true;
    };
    module.exports = ObjectJS;
});