define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Easydialog=null,
        moment = require("moment");
        require("daterangepicker");
        require("pager");
        require("colormark");

        var Params = {
        keyWords: "",
        filterType: 1,//1:我的任务；2：参与任务；-1：所有任务
        userID: "",
        orderType: -1,
        taskType: -1,
        colorMark: -1,
        finishStatus: 0,
        invoiceStatus: -1,
        preFinishStatus: -1,
        beginDate: "",//起始创建时间
        endDate: "",//截止创建时间
        beginEndDate: "",//起始到期时间
        endEndDate: "",//起始到期时间
        taskOrderColumn: 0,//排序列 1:创建时间；2：到期时间 
        isAsc:0,//排序方式 0：倒序；1：正序
        pageSize: 10,
        pageIndex: 1
    };
    
    var ObjectJS = {};
    ObjectJS.init = function (filterType, currentUserID, colorList) {
        ObjectJS.isLoading = true;
        ObjectJS.ColorList = JSON.parse(colorList.replace(/&quot;/g, '"'));
        ObjectJS.currentUserID = currentUserID;
        Params.filterType = filterType;
        if (filterType == 2) {
            document.title = "参与任务";
            $(".header-title").html("参与任务");
            Params.finishStatus = 1;
            $(".search-stages li").removeClass("hover");
            $(".search-stages li[data-id='1']").addClass("hover");
        }else if (filterType == -1){
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

                Params.invoiceStatus = _this.data("id");
                ObjectJS.getList();
            };
        });

        //创建时间
        $("#taskCreateTime").daterangepicker({
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
        $("#taskExpireTime").daterangepicker({
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
                }else {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }
                isasc = isasc == 1 ? 0 : 1;
            }else {
                if (isasc == 1) {
                    _self.find(".desc").removeClass("hover");
                    _self.find(".asc").addClass("hover");
                }else {
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

    //更改任务到期时间
    ObjectJS.updateTaskEndTime = function (taskid, maxhours, planTime) {
        if (maxhours == 0) {
            Easydialog = require("easydialog");
            doT.exec("/template/task/set-endtime.html", function (template) {
                var innerHtml = template();
                Easydialog.open({
                    container: {
                        id: "show-model-setRole",
                        header: "设置任务到期时间",
                        content: innerHtml,
                        yesFn: function () {
                            var  endTime=$("#UpdateTaskEndTime").val();
                            if (endTime == "") {
                                alert("任务到期时间不能为空", 2);
                                return
                            }

                            planTime = new Date(planTime).getTime();
                            var endTime2 = new Date(endTime).getTime();
                            if (planTime < endTime2) {
                                confirm("到期时间超过订单交货日期,确定设置?", function () { ObjectJS.updateTaskEndTimeAjax(taskid, endTime) });
                            } else {
                                ObjectJS.updateTaskEndTimeAjax(taskid, endTime);
                            }
                        }
                    }
                });

                var myDate = new Date();
                var minDate = myDate.toLocaleDateString();
                minDate = minDate + " 23:59:59"
                //更新任务到期日期
                var taskEndTime = {
                    elem: '#UpdateTaskEndTime',
                    format: 'YYYY/MM/DD hh:mm:ss',
                    min: minDate,
                    istime: true,
                    istoday: false
                };
                laydate(taskEndTime);
            });
        }
        else {
            ObjectJS.updateTaskEndTimeAjax(taskid,'');
        }
    }

    ObjectJS.updateTaskEndTimeAjax = function (taskid,endTime) {
        Global.post("/Task/UpdateTaskEndTime", {
            id: taskid,
            endTime: endTime
        }, function (data) {
            if (data.result == 0) {
                alert("操作无效");
            } else if (data.result == 2) {
                alert("任务已接受,不能操作");
            } else if (data.result == 3) {
                alert("没有权限操作");
            } else {
                alert("接受成功");
                if (endTime) {
                    $(".btn-accept[data-id=" + taskid + "]").next().html('结束日期：' + new Date($("#UpdateTaskEndTime").val()).toString('yyyy-MM-dd'));
                } else {
                    $(".btn-accept[data-id=" + taskid + "]").next(0).html('--');
                }
                $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-status').html('进行中').css({ "color": "#02C969" });
                $(".btn-accept[data-id=" + taskid + "]").parents('ul').find('.accept-date').html(new Date().toString('yyyy-MM-dd'));
                $(".btn-accept[data-id=" + taskid + "]").unbind().html("进行中").removeClass('btn').removeClass('btn-accept').css({ "color": "#02C969" });
            }
        });
    };

    ObjectJS.getList = function () {
        ObjectJS.isLoading = false;
        $(".table-header").nextAll().remove();
        $(".table-header").after("<tr><td style='padding:0;' colspan='10'><div class='data-loading'><div></td></tr>");

        Global.post("/Task/GetTasks", Params, function (data) {
            $(".table-header").nextAll().remove();

            if (data.items.length > 0) {
                doT.exec("template/task/task-list.html", function (template) {
                    data.items.currentUserID = ObjectJS.currentUserID;
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $(".table-header").after(innerhtml);
                    /*任务讨论浮层*/
                    require.async("showtaskdetail", function () {
                        innerhtml.find('.show-task-reply').showtaskdetail();
                    });

                    innerhtml.find('.btn-accept').click(function () {
                        ObjectJS.updateTaskEndTime($(this).data('id'), $(this).data('maxhours'), $(this).data('plantime'));
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

                    innerhtml.find(".mark").markColor({
                        isAll: false,
                        data: ObjectJS.ColorList,
                        onChange: function (obj, callback) {
                            ObjectJS.markTasks(obj.data("id"), obj.data("value"), callback);
                        }
                    });
                });
            }else {
                $(".table-header").after("<tr><td colspan='10' style='padding:0;'><div class='nodata-txt' >暂无数据!<div></td></tr>");
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
                alert("您没有标记任务的权限！", 2);
                callback && callback(false);
            }
            else {
                callback && callback(data.status);
            }
            ObjectJS.isLoading = true;
        });
    }

    module.exports = ObjectJS;
});