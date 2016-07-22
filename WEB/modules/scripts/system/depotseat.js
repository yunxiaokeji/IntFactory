
define(function (require, exports, module) {
    var Global = require("global"),
        Verify = require("verify"), VerifyObject,
        doT = require("dot"),
        Easydialog = require("easydialog");
    require("pager");
    require("switch");
    var Params = {
        keyWords: "",
        wareid: "-1",
        pageSize: 20,
        pageIndex: 1,
        totalCount: 0
    }, EntityModel = {
        DepotID: "",
        WareID: ""
    };

    var ObjectJS = {};
    //列表页初始化
    ObjectJS.init = function (wareid) {
        var _self = this;

        Params.wareid = wareid;
        EntityModel.WareID = wareid;
        _self.getList();
        _self.bindEvent();
    }
    //弹出层
    ObjectJS.showCreate = function (callback) {
        var _self = this;
        doT.exec("template/system/depotseat_add.html", function (templateFun) {

            var html = html = templateFun([]);

            Easydialog.open({
                container: {
                    id: "depotseat-add-div",
                    header: EntityModel.DepotID == "" ? "新建货位" : "编辑货位",
                    content: html,
                    yesFn: function () {

                        if (!VerifyObject.isPass("#depotseat-add-div")) {
                            return false;
                        }
                        var entity = {
                            DepotID: EntityModel.DepotID,
                            Name: $("#name").val().trim(),
                            DepotCode: $("#depotcode").val().trim(),
                            WareID: EntityModel.WareID,
                            Status: $("#status").prop("checked") ? 1 : 0,
                            Description: $("#description").val()
                        };

                        _self.savaEntity(entity, callback);
                    },
                    callback: function () {

                    }
                }
            });

            $("#depotcode").focus();

            //编辑填充数据
            if (EntityModel.DepotID) {
                $("#name").val(EntityModel.Name);
                $("#depotcode").val(EntityModel.DepotCode);
                $("#status").prop("checked", EntityModel.Status == 1);
                $("#description").val(EntityModel.Description);
               
            }

            VerifyObject = Verify.createVerify({
                element: ".verify",
                emptyAttr: "data-empty",
                verifyType: "data-type",
                regText: "data-text"
            });
        });
    }

    //保存
    ObjectJS.savaEntity = function (entity) {
        var _self = this;
        Global.post("/System/SaveDepotSeat", { obj: JSON.stringify(entity) }, function (data) {
            if (data.ID.length > 0) {
                _self.getList();
            }
        })
    }

    //绑定列表页事件
    ObjectJS.bindEvent = function () {
        var _self = this;

        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown")) {
                $(".dropdown-ul").hide();
            }
        });

        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.keyWords = keyWords;
                Params.pageIndex = 1;
                _self.getList();
            });
        });

        //添加
        $(".btn-add").on("click", function () {
            EntityModel.DepotID = "";
            _self.showCreate();
        });

        //删除
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("货位删除后不可恢复,确认删除吗？", function () {
                Global.post("/System/DeleteDepotSeat", {
                    id: _this.data("id"),
                    wareid: Params.wareid
                }, function (data) {
                    if (data.Status) {
                        _self.getList();
                    } else {
                        alert("货位已存在产品，不能删除！");
                    }
                });
            });
        });

        //编辑
        $("#updateObject").click(function () {
            var _this = $(this);
            Global.post("/System/GetDepotByID", {
                id: _this.data("id"),
                wareid: Params.wareid
            }, function (data) {
                EntityModel = data.Item;
                _self.showCreate();
            });
        });
    }

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $("#warehouse-items").nextAll().remove();
        Global.post("/System/GetDepotSeats", Params, function (data) {
            doT.exec("template/system/depotseats.html", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $("#warehouse-items").after(innerText);

                //下拉事件
                innerText.find(".dropdown").click(function () {
                    var _this = $(this);

                    var position = _this.find(".ico-dropdown").position();
                    $(".dropdown-ul li").data("id", _this.data("id"));
                    $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 55 }).show().mouseleave(function () {
                        $(this).hide();
                    });
                });

                //绑定启用插件
                innerText.find(".status").switch({
                    open_title: "点击启用",
                    close_title: "点击禁用",
                    value_key: "value",
                    change: function (data,callback) {
                        _self.editStatus(data, data.data("id"), data.data("value"), callback);
                    }
                });
                innerText.find(".sort-up,.sort-down").click(function () {
                    var _this = $(this);
                    Global.post("/System/UpdateDepotSeatSort", {
                        depotid: _this.data("id"),
                        wareid: Params.wareid,
                        type: _this.data("type")
                    }, function (data) {
                        if (data.status) {
                            var parent = _this.parents(".list-item");
                            if (_this.data("type") == 0) {
                                parent.insertBefore(parent.prev());
                            } else {
                                parent.insertAfter(parent.next());
                            }
                            _self.hideUpDown();
                        } else {
                            alert("优先级调整失败", function () {
                                location.href = location.href;
                            });
                        }
                    })
                });

                _self.hideUpDown();
            });

            

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: Params.pageIndex,
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
                    Params.pageIndex = page;
                    Brand.getList();
                }
            });
        });
    }

    //隐藏排序按钮
    ObjectJS.hideUpDown = function () {

        $(".table-list .list-item").find(".sort-up,.sort-down").show();

        $(".table-list .list-item").first().find(".sort-up").hide();
        $(".table-list .list-item").last().find(".sort-down").hide();
    }

    //更改状态
    ObjectJS.editStatus = function (obj, id, status, callback) {
        var _self = this;
        Global.post("/System/UpdateDepotSeatStatus", {
            id: id,
            status: status ? 0 : 1
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有此操作权限，请联系管理员帮您添加权限！");
                return;
            }
            !!callback && callback(data.Status);
        });
    }

    module.exports = ObjectJS;
})