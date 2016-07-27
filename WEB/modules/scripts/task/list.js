define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Tip = require("tip"),
        moment = require("moment");
        require("daterangepicker");
        require("pager");
        require("colormark");

    var Params = {
        isMy: true,//是否获取我的任务
        userID: "",
        isParticipate:0,//是否获取我参与任务
        taskType: -1,
        colorMark: -1,
        status: 1,
        finishStatus:0,
        keyWords:"",
        beginDate: "",
        endDate: "",
        beginEndDate: "",
        endEndDate:"",
        orderType: -1,
        orderProcessID: "-1",
        orderStageID: "-1",
        invoiceStatus: -1,
        preFinishStatus: -1,
        taskOrderColumn: 0,//拍序列  0:创建时间；2：到期时间 
        isAsc:0,
        pageSize: 10,
        pageIndex: 1,
        listType: "list"
    };
    
    var ObjectJS = {};

    ObjectJS.init = function (isMy, nowDate, model) {
        var _self = this;
        ObjectJS.isLoading = true;
        ObjectJS.ColorList = JSON.parse(model.replace(/&quot;/g, '"'));
        Params.beginDate = nowDate;
        Params.endDate = nowDate;        

        if (isMy == 2) {
            Params.isParticipate = 1;
            document.title = "参与任务";
            $(".header-title").html("参与任务");
        }
        else if (isMy == 0)
        {
            Params.isMy = false;
            document.title = "所有任务";
            $(".header-title").html("所有任务");

            //人员筛选
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: false,
                    width: "140",
                    onChange: function (data) {
                        Params.pageIndex = 1;
                        Params.userID = data.userid;
                        ObjectJS.getList();
                    }
                });
            });
        }

        ObjectJS.bindEvent();

        //获取任务列表
        if (Params.isParticipate == 1) {
            Params.finishStatus = 1;
            $(".search-stages li").removeClass("hover");
            $(".search-stages li[data-id='1']").addClass("hover");
        }
        ObjectJS.getList();
    }

    ObjectJS.bindEvent = function () {
        var _self = this;

        //切换任务阶段
        $(".search-stages li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.finishStatus = _this.data("id");
                ObjectJS.getList();
            }
        });

        //关键字查询 任务编码、订单编码、任务标题
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (!ObjectJS.isLoading) {
                    return;
                }
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                ObjectJS.getList();
            });
        });

        /*标签过滤*/
        $("#filterMark").markColor({
            isAll: true,
            top: 30,
            left: 5,
            data: _self.ColorList,
            onChange: function (obj, callback) {
                callback && callback(true);
                Params.pageIndex = 1;
                Params.colorMark = obj.data("value");
                ObjectJS.getList();
            }
        });

        //切换订单类型
        $(".search-ordertype .item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                
                Params.orderType = _this.data("id");
                Params.orderProcessID = '-1';
                Params.orderStageID = '-1';
                ObjectJS.getList();
            }
        });

        //切换任务类型
        $(".search-process .item").on("click", function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.taskType = _this.data("id");
                ObjectJS.getList();        
            };
        });
       
        //切换任务上级任务进度
        $(".search-prefinishstatus .item").on("click", function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.preFinishStatus = _this.data("id");
                ObjectJS.getList();
            };
        });

        //预警切换
        $(".search-warning .item").on("click", function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                var status = _this.data("id");
                Params.invoiceStatus = status;
                ObjectJS.getList();
            };
        });

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
            Params.pageIndex = 1;
            Params.beginDate = start ? start.format("YYYY-MM-DD") : "";
            Params.endDate = end ? end.format("YYYY-MM-DD") : "";
            ObjectJS.getList();
        });

        //到期时间
        $("#iptExpireTime").daterangepicker({
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
            Params.pageIndex = 1;
            Params.beginEndDate = start ? start.format("YYYY-MM-DD") : "";
            Params.endEndDate = end ? end.format("YYYY-MM-DD") : "";            
            ObjectJS.getList();
        });

        //列表排序
        $(".sort-item").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _self = $(this);
            if (!_self.hasClass("hover")) {
                _self.addClass("hover").siblings().removeClass("hover");
            }
            var isasc = _self.data("isasc");
            var isactive = _self.attr("data-isactive");
            var orderbycloumn = _self.data("orderbycloumn");

            $(".search-sort li[data-isactive='1']").data("isactive", 0).find("span").removeClass("hover");

            if (isactive == 1) {
                if (isasc == 1) {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                }
                else {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                
                isasc = isasc == 1 ? 0 : 1;
            }
            else {
                if (isasc == 1) {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                else {
                    _self.find(".asc").removeClass("hover");
                    _self.find(".desc").addClass("hover");
                }

            }
            _self.data("isasc",isasc).attr("data-isactive",1);

            Params.isAsc = isasc;
            Params.taskOrderColumn = orderbycloumn;
            Params.pageIndex = 1;
            ObjectJS.getList();

        });
    }

    ObjectJS.getList = function () {

        ObjectJS.isLoading = false;

        $(".table-header").nextAll().remove();
        $(".table-header").after("<tr><td colspan='11'><div class='data-loading'><div></td></tr>");

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".table-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/task/task-list.html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);
                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        data: ObjectJS.ColorList,
                        onChange: function (obj, callback) {
                            ObjectJS.markTasks(obj.data("id"), obj.data("value"), callback);
                        }
                    });
                    innerhtml.find('.order-progress-item').each(function () {
                        var _this = $(this);
                        _this.css({ "width": _this.data('width') });
                    });
                    innerhtml.find('.progress-tip,.top-lump').each(function () {
                        var _this = $(this);
                        _this.css({ "left": (_this.parent().width() - _this.width()) / 2 });
                    });
                    innerhtml.find('.layer-line').css({ width: 0, left: "160px" });

                    $(".table-header").after(innerhtml);
                });
            }
            else {
                $(".table-header").after("<tr><td colspan='11'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                   Params.pageIndex = page;
                   ObjectJS.getList();
                }
            });

            ObjectJS.isLoading = true;
        });
    }

    //任务颜色标记
    ObjectJS.markTasks = function (ids, mark, callback) {
        if (mark < 0) {
            alert("不能标记此选项!");
            return false;
        }
        ObjectJS.isLoading = false;
        Global.post("/Task/UpdateTaskColorMark", {
            ids: ids,
            mark: mark
        }, function (data) {
            if (data.result == "10001") {
                alert("您没有标记任务的权限！");
                callback && callback(false);
            }
            else {
                callback && callback(data.status);
            }
            ObjectJS.isLoading = true;
        });
    }

    ObjectJS.setListPosition = function () {        
        var count = parseInt($(".task-items").width() / 269)
        var moreWidth = $(".task-items").width() - (269 * count);
        var marginRight = ((moreWidth + 15) / (count - 1)) + 15;
        
        for (var i = 0; i < $(".task-items .task-item").length; i++) {
            var _this = $(".task-items .task-item").eq(i);
            if ((i+1) % count == 0) {
                _this.css("margin-right", "0");
            }
            else {
                _this.css("margin-right", marginRight + "px");
            }            
        }
       
    }

    module.exports = ObjectJS;
});