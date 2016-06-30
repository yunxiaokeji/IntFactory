define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");

    require("switch");
    var $ = require('jquery');
    require("color")($);

    
    var ColorModel = {};
    var ObjectJS = {};
    var tableID = 1;

    //初始化
    ObjectJS.init = function () {
        $('#createColor').hide();
        var _self = this;
        _self.bindEvent();
        _self.bindColorList();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        $(document).click(function (e) {
            //隐藏下拉  
            if ((!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown"))
                && (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("color-item") && !$(e.target).hasClass("color-item"))) {
                $(".dropdown-ul").hide();
            }
        });

        /*添加标记颜色*/
        $(".addmark li div").click(function () {
            ColorModel.ColorID = 0;
            ColorModel.ColorName = "";
            ColorModel.ColorValue = "";
            _self.createColor();            
        });

        /*删除标记颜色*/
        $("#deleteColor").click(function () {
            var _this = $(this);
            confirm("标签删除后不可恢复,确认删除吗？", function () {
                _self.deleteColor(_this.data("id"), function (result) {
                    if (result == 1) {
                        alert("标签删除成功");
                        ObjectJS.bindColorList();
                    } else if (result == 10002) {
                        alert("标签已关联客户，删除失败");
                    } else if (result == -100) {
                        alert("标签不能全部删除,操作失败");
                    } else if (result == -200) {
                        alert("标签已经被删除,请刷新查看");
                    } else {
                        alert("删除失败");
                    }
                });
            });
        });

        /*编辑颜色*/
        $("#updateColor").click(function () {
            var _this = $(this);
            Global.post("/System/GetLableColorByColorID", { colorid: _this.data("id"),lableType:tableID }, function (data) {
                var model = data.model;
                ColorModel.ColorID = model.ColorID;
                ColorModel.ColorName = model.ColorName;
                ColorModel.ColorValue = model.ColorValue;
                _self.createColor();
            });
        });

        //切换模块
        $(".module-tab li").click(function () {
            var _this = $(this), id = _this.data("id"), labelid = _this.data("labelid");
            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $("#" + id).show().siblings().hide();            
            tableID = labelid;
            _self.bindColorList();
        });        
    }

    //添加/编辑弹出层
    ObjectJS.createColor = function () {
        var _self = this;
        doT.exec("template/system/sources-color.html", function (template) {
            var html = template([]);
            Easydialog.open({
                container: {
                    id: "show-model-detail",
                    header: !ColorModel.ColorID ? "新建客户标签" : "编辑客户标签",
                    content: html,
                    yesFn: function () {
                        if (!VerifyObject.isPass()) {
                            return false;
                        }
                        ColorModel.ColorName = $("#colorName").val();
                        ColorModel.ColorValue = $("#colorName").data('value');
                        _self.saveColorModel(ColorModel);
                    },callback: function () {
                    }
                }
            });

            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });

            ColorModel.ColorValue = ColorModel.ColorValue == "" ? "#d9d9d9" : ColorModel.ColorValue
            $("#colorName").data('value', ColorModel.ColorValue);
            $("#colorName").val(ColorModel.ColorName);
            $("#colorValue").spectrum({
                color: ColorModel.ColorValue,
                showInput: true,
                className: "full-spectrum",
                showPalette: true,
                showSelectionPalette: true,
                maxPaletteSize: 10,
                preferredFormat: "hex",
                cancelText: '取消',
                chooseText: '确认',
                togglePaletteOnly: true,
                hide: function () {
                    $('#colorName').data('value', $("#colorValue").val());
                },
                palette: [
                    ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", "rgb(204, 204, 204)", "rgb(217, 217, 217)", "rgb(255, 255, 255)",
                    "rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                    "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)", "rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                    "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                    "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                    "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                    "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                    "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                    "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                    "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                    "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                    "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
                ]
            });
            //if (ColorModel.ColorID == '') {
            //    $(".full-spectrum").click();
            //}
        });
    };

    ObjectJS.saveColorModel = function (model) {
        var _self = this;
        Global.post("/System/SaveLableColor", { lablecolor: JSON.stringify(model), lableType: tableID }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            if (data.ID > 0) {
                ObjectJS.bindColorList();
            } else {
                alert("系统已存在相同标签颜色！");
                return;
            }
        });
    }

    //加载列表
    ObjectJS.bindColorList = function () {
        var _self = this;
        $("#customermark").html('');
        var urlItem = "";        
        var _this = "";

        if (tableID == 1) {
            _this = $("#customermark");                    
        } else if (tableID == 2) {
            _this = $("#ordermark");
        } else {
            _this = $("#taskmark");
        }

        Global.post("/System/GetLableColor", { lableType: tableID }, function (data) {
            if (data.items.length > 0) {
                for (var i = 0; i < data.items.length; i++) {
                    var item = data.items[i];
                    urlItem += '<li data-id="' + item.ColorID + '" data-value="' + item.ColorValue + '" data-name="' + item.ColorName + '"  class="color-item"><div class="left color-leftzuoyou" style=" border-right:18px solid ' + item.ColorValue + '; "></div><div class="left colordiv" style="background-color:' + item.ColorValue + '";>' + item.ColorName + '</div></li>';
                }
            }
            _this.html(urlItem);

            _this.find(".color-item").click(function () {
                var _this = $(this);
                var position = _this.position();
                $(".colordrop li").data("id", _this.data("id"));
                $(".colordrop").css({ "top": position.top + 45, "left": position.left + 30 }).show().mouseleave(function () {
                    $('.colordrop').hide();
                });
            });
        });
    }


    //删除color
    ObjectJS.deleteColor = function (colorid, callback) {
        Global.post("/System/DeleteColor", { colorid: colorid, lableType: tableID }, function (data) {
            !!callback && callback(data.result);
        });
    }

    module.exports = ObjectJS;
});