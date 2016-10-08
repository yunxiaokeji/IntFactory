define(function (require, exports, module) {
    var Global = require("global"),
        Dot = require("dot"),
        moment = require("moment"),
        Verify = require("verify"), VerifyObject,
        Easydialog = require("easydialog");
    require("pager");
    require("daterangepicker");
    var Upload = require("upload");

    var ObjectJS = {};
    var moduleType = 0;
    var createtype = 0;
    ObjectJS.isLoading = true;

    var Params = {
        Types:"-1",
        Keywords: "",
        BeginTime:"",
        EndTime: "",
        PageIndex: 1,
        PageSize: 10,
        OrderBy: "c.CreateTime desc",
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
                alert("数据正在加载中");
                return;
            }
            var _this = $(this), type = _this.data("idsource");
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");               
                createtype = type;
                Params.Types = type;
                Params.PageIndex = 1;
                ObjectJS.getTypeList();
            }
        });

        //关键字搜索
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (!ObjectJS.isLoading) {
                    alert("数据正在加载中");
                    return;
                }
                Params.PageIndex = 1;
                Params.Keywords = keyWords;
                ObjectJS.getTypeList();                
            });
        });

        //排序
        $(".sort-item").click(function () {
            if (!ObjectJS.isLoading) {
                alert("数据正在加载中");
                return;
            }
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

        $("#createTypes").click(function () {
            Dot.exec("template/helpcenter/type/create-type.html", function (template) {
                var innerText = template([]);
                Easydialog.open({
                    container: {
                        id: "show-model-detail",
                        header: "新建分类",
                        content: innerText,
                        yesFn: function () {
                            if (!VerifyObject.isPass()) {
                                return false;
                            }
                            ObjectJS.createType($("#contactType"));
                        },
                        callback: function () {

                        }
                    }
                });

                ObjectJS.bindSelect();
                ObjectJS.bindUpload();

                $("#select .item .check-lump").eq(createtype-1).click();

                VerifyObject = Verify.createVerify({
                    element: ".verify",
                    emptyAttr: "data-empty",
                    verifyType: "data-type",
                    regText: "data-text"
                });
            });
        });

        $(".add-category").click(function () {
            ObjectJS.createType($(".type"));
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
                                        var sort = $(".sort").val();
                                        var desc = $("#desc").val();
                                        var img = $("#cateGoryImages li img").data("src");
                                        if (img==undefined) {
                                            img = "";
                                        }
                                        Global.post("/HelpCenter/UpdateType", {
                                            TypeID: typeID,
                                            Name: type,
                                            desc: desc,
                                            icon: img,
                                            moduleType: moduleType,
                                            sort: sort
                                        }, function (e) {
                                            if (e.status) {
                                                Params.PageIndex = 1;
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
                            $(".sort").val(item.Sort);
                            $("#desc").val(item.Remark);
                            $("#select .item .check-lump").removeClass("hover");
                            $("#select .item .check-lump[data-id=" + item.ModuleType + "]").addClass("hover");
                            moduleType = item.ModuleType;
                            if (item.Icon) {
                                $("#cateGoryImages").html("<li><img src='" + item.Icon + "?imageView2/1/w/60/h/60' data-src=" + item.Icon + "></li>");
                            }
    

                            ObjectJS.bindUpload();
                        });
                    });

                    innerHtml.find(".delete").click(function () {                        
                        var _this = $(this);
                        var typeID = _this.data("id");
                        var functionArray = [
                            { id: "1fba4255-8eaa-4823-baae-134add3dc05b"},
                            { id: "42c0bb53-07f1-43e7-857b-6ed589ec093f"},
                            { id: "cfa08906-0b09-44e0-978d-b0abb48c6735"},
                            { id: "95088962-ec5d-4a3f-ae96-85827bee02e9"},
                            { id: "08673e32-d738-4730-8580-d17d49855f8e"},
                            { id: "e83b8979-4244-4f8a-bdfd-14bbe168b175"},
                            { id: "e7a834a0-405f-4c30-8a5e-ab600af1c07c"},
                            { id: "45daf2fc-3ffa-4fdf-8649-2128aa4ba333"}
                        ];

                        for (var i = 0; i < functionArray.length; i++) {
                            if (typeID == functionArray[i].id) {
                                alert("该分类为初始化分类，不可删除1");
                                return;
                            }
                        }

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
                $(".category").append("<tr class='lists'><td colspan='5'><div class='nodata-txt'>暂无数据</div><td></tr>");
                $("#pager").hide();
            }
        })
    }

    ObjectJS.createType = function (obj) {
        var moduleType = $("#select .item .hover").data("id");
        var txt = obj.val();
        var desc = $(".desc").val();
        var img = $("#cateGoryImages li img").data("src");
        var sort = $(".sort").val();
        if (sort == "") {
            sort = 0;
        }
        Global.post("/HelpCenter/InsertType", { Name: txt, desc: desc, moduleType: moduleType, img: img, sort: sort }, function (data) {
            if (data.status == 1) {
                alert("添加成功");
                ObjectJS.getTypeList();
            } else if (data.status == 0) {
                alert("添加失败");
            } else {
                alert("分类名称已存在");
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