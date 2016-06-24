define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        moment = require("moment");
        require("daterangepicker");
        require("pager");
        require("mark");

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
    ObjectJS.isLoading = true;
  
    ObjectJS.init = function (isMy, nowDate) {
        Params.beginDate = nowDate;
        Params.endDate = nowDate;
        Params.pageSize = ($(".content-body").width() / 300).toFixed(0) * 3;
        var taskListType = Global.getCookie('TaskListType');
        if (taskListType) {
            Params.listType = taskListType;
        }
        $(".task-tabtype i[data-type=" + Params.listType + "]").addClass("checked").siblings().removeClass("checked");
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
        if (Params.isParticipate != 1) {
            ObjectJS.getList();
        }

    }

    ObjectJS.bindEvent = function () {

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

        if (Params.isParticipate == 1) {
            $(".search-stages li").eq(2).click();
        }

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
        
        //切换颜色标记
        $(".search-item-color li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                Params.pageIndex = 1;
                Params.colorMark = _this.data("id");
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

        //切换任务显示方式(列表或者卡片式)
        $(".search-sort .task-tabtype i").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }

            var _this = $(this);
            if (!_this.hasClass('checked')) {
                _this.addClass('checked').siblings().removeClass('checked');
                Params.listType = _this.data('type');
                Global.setCookie('TaskListType',Params.listType);
                ObjectJS.getList();
            }
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
        var showtype = Params.listType;
        $(".tr-header").nextAll().remove();

        if (showtype == "list") {
            $(".task-items").hide();
            $(".table-list").show();
            $(".tr-header").after("<tr><td colspan='11'><div class='data-loading'><div></td></tr>");
        }
        else {
            $(".table-list").hide();
            $(".task-items").show();
            $(".task-items").html("<div class='data-loading'><div>");
        }
        $(".content-body").find('.nodata-txt').remove();
        ObjectJS.isLoading = false;

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".tr-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/task/task-"+showtype+".html", function (template) {
                    var innerhtml = template(data.items);

                    innerhtml = $(innerhtml);
                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        onChange: function (obj, callback) {
                            ObjectJS.markTasks(obj.data("id"), obj.data("value"), callback);
                        }

                    });

                    if (showtype == "list") {
                        $(".table-list").append(innerhtml);
                    }
                    else {
                        $(".task-items").html(innerhtml);
                    }
                });
            }
            else {
                if (showtype == "list") {
                    $(".table-list").after("<div class='nodata-txt' >暂无数据!<div>");
                }
                else {
                    $(".task-items").html("<div class='nodata-txt' >暂无数据!<div>");
                }
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                   $(".tr-header").nextAll().remove();
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
                callback && callback(data.result);
            }
            ObjectJS.isLoading = true;
        });
    }

    module.exports = ObjectJS;
});