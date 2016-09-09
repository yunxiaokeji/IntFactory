define(function (require, exports, module) {
    var Global = require("global"),
        Dot = require("dot"),
        moment = require("moment"),
        Easydialog = require("easydialog");
    require("pager");
    require("daterangepicker");
    var Upload = require("upload");

    var ObjectJS = {};
    var moduleType = 0;
    ObjectJS.isLoading = true;

    var Params = {
        Types:"-1",
        Keywords: "",
        BeginTime:"",
        EndTime: "",
        PageIndex: 1,
        PageSize: 10,
        OrderBy: "m_helpType.CreateTime desc",
    }
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
        ObjectJS.getTypeList();
    };

    ObjectJS.bindEvent = function () {
        //日期插件
        $("#iptCreateTime").daterangepicker({
            showDropdowns: true,
            empty: true,
            opens: "right",
            ranges: {
                '今天': [moment(), moment()],
                '昨天': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                '上周': [moment().subtract(6, 'days'), moment()],
                '本月': [moment().startOf('month'), moment().endOf('month')]
            }
        }, function (start, end, label) {
            Params.PageIndex = 1;
            Params.BeginTime = start ? start.format("YYYY-MM-DD") : "";
            Params.EndTime = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.getTypeList();
        });    

        //选择模块
        $(".module-source .item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this), type = _this.data("idsource");
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");                
               
                Params.Types = type;
                ObjectJS.getTypeList();
            }
        });

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                ObjectJS.getTypeList();                
            });
        });

        //排序
        $(".sort-item").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) {
                if (_this.find(".asc").hasClass("hover")) {
                    _this.find(".asc").removeClass("hover");
                    _this.find(".desc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " desc ";
                } else {
                    _this.find(".desc").removeClass("hover");
                    _this.find(".asc").addClass("hover");
                    Params.OrderBy = _this.data("column") + " asc ";
                }
            } else {
                _this.addClass("hover").siblings().removeClass("hover");
                _this.siblings().find(".hover").removeClass("hover");
                _this.find(".desc").addClass("hover");
                Params.OrderBy = _this.data("column") + " desc ";
            }
            Params.PageIndex = 1;
            ObjectJS.getTypeList();
        });

        $(".add-category").click(function () {
            var moduleType = $("#select .item .hover").data("id");
            var txt = $(".type").val();
            var desc = $(".desc").val();
            if (txt == "") {
                alert("分类不能为空");
                return;
            }
            var img = $("#cateGoryImages li img").data("src");            
            Global.post("/HelpCenter/InsertType", { Name: txt, desc: desc, moduleType: moduleType, img: img }, function (data) {
                if (data.status == 1) {
                    alert("添加成功");
                    window.location = "/HelpCenter/Types";
                } else if (data.status == 0) {
                    alert("添加失败");
                } else {
                    alert("分类名称已存在");
                }
            })
        });

        ObjectJS.bindSelect("select");

        ObjectJS.bindUpload();
    };

    ObjectJS.getTypeList = function () {
        ObjectJS.isLoading = false;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='15'><div class='data-loading'><div></td></tr>");
        Global.post("/HelpCenter/GetTypes", { filter: JSON.stringify(Params) }, function (data) {
            ObjectJS.isLoading = true;
            $(".tr-header").nextAll().remove();
            if (data.items.length > 0) {                
                Dot.exec("/template/helpcenter/type/type-list.html", function (template) {
                    var innerHtml = template(data.items);
                    innerHtml = $(innerHtml);
                    $(".category").append(innerHtml);
                    
                    innerHtml.find(".update").click(function () {
                        var _this = $(this), typeID = _this.data("id"), index = _this.data("index");
                        Dot.exec("/template/helpcenter/type/updata-type.html", function (template) {
                            var innerText = template(data.items);                            
                            Easydialog.open({
                                container: {
                                    id: "show-model-detail",
                                    header: "编辑分类",
                                    content: innerText,
                                    yesFn: function () {
                                        var type = $(".type").val();                                        
                                        var img = $("#cateGoryImages li img").data("src");                                        
                                        Global.post("/HelpCenter/UpdateType", { TypeID: typeID, Name: type, icon: img, moduleType: moduleType }, function (e) {
                                            if (e.status) {
                                                ObjectJS.getTypeList();
                                            } else {
                                                alert("修改失败");
                                            }
                                        });
                                    },
                                    callback: function () {

                                    }
                                }
                            });

                            ObjectJS.bindSelect("update");                           

                            var item = data.items[index];
                            $(".type").val(item.Name);
                            $("#select .item .check-lump").removeClass("hover");
                            $("#select .item .check-lump[data-id=" + item.ModuleType + "]").addClass("hover");
                            if (item.Icon) {
                                $("#cateGoryImages").html("<li><img src='" + item.Icon + "?imageView2/1/w/60/h/60' data-src=" + item.Icon + "></li>");
                            }
    

                            ObjectJS.bindUpload();
                        });
                    });

                    innerHtml.find(".delete").click(function () {
                        var _this = $(this);
                        var typeID = _this.data("id");
                        var confirmMsg = "确定删除此分类?";
                        confirm(confirmMsg, function () {                            
                            Global.post("/HelpCenter/DeleteType", { TypeID: typeID }, function (data) {
                                if (data.status == 1) {
                                    _this.parent().parent().fadeOut(400, function () {
                                        _this.remove();
                                    });
                                } else if (data.status == 0) {
                                    alert("删除失败");
                                } else {
                                    alert("请先删除该分类下数据");
                                }
                            })
                        });
                    });
                })

                $("#pager").paginate({
                    total_count: data.totalCount,
                    count: data.pageCount,
                    start: Params.PageIndex,
                    display: 5,
                    border: true,
                    border_color: '#fff',
                    text_color: '#333',
                    background_color: '#fff',
                    border_hover_color: '#ccc',
                    text_hover_color: '#000',
                    background_hover_color: '#efefef',
                    rotate: true,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        Params.PageIndex = page;
                        ObjectJS.getTypeList();
                    }
                });
            } else {
                $(".category").append("<tr class='lists' style='height:100px;'><td class='center font16' colspan='4'>暂无数据<td></tr>");
            }
        })
    }

    ObjectJS.bindUpload = function () {
        var uploader = Upload.uploader({
            browse_button: 'uploadImg',
            picture_container: "cateGoryImages",
            successItems: "#cateGoryImages li",
            //image_view: "?imageView2/1/w/60/h/60",
            file_path: "/Content/UploadFiles/HelpCenter/",
            maxSize: 5,
            fileType: 1,
            multi_selection: false,
            init: {}
        });
    }

    ObjectJS.bindSelect = function (obj) {
        $("#select .item .check-lump").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this), id = _this.data("id");
            if (obj=="update") {
                moduleType = id;
            } else {
                Params.Types=id;
            }
            
            if (!_this.hasClass("hover")) {
                $("#select .item .check-lump").removeClass("hover");
                _this.addClass("hover");
            };
        });
    }

    module.exports = ObjectJS;
})