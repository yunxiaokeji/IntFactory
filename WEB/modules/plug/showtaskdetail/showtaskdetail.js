
define(function (require, exports, module) {
    require("plug/showtaskdetail/style.css");
    var doT = require("dot");
    var Global = require("global");

    (function ($) {
        //默认参数
        var defaultParas = {
            name: "tb-task-list"
        };

        $.fn.showtaskdetail = function (options) {
            defaultParas = $.extend([], defaultParas, options);

            $(this).click(function () {
                var taskid = $(this).data("taskid");
                var orderid = $(this).data("orderid");
                var stageid = $(this).data("stageid");

                var $taskDetailContent = $("#taskDetailContent");
  
                //没有任务详情对象
                if ($taskDetailContent.length == 0){
                    drawTaskDetail(taskid, orderid, stageid);
                }
                else{
                    //查询新的任务详情
                    if ($taskDetailContent.data("taskid") != taskid) {
                        drawTaskDetail(taskid, orderid, stageid);
                    }
                    else//隐藏显示的任务详情
                    {
                        if ($taskDetailContent.css("right") == "0px") {
                            $taskDetailContent.animate({ right: '-490px' }, 500, function () { $("#taskDetailContent") .hide()});
                        }
                        else
                            $taskDetailContent.show().animate({ right: '0px' }, 500);
                    }
                    
                }

            });
        };

        //获取任务详情
        var drawTaskDetail = function (taskid, orderid, stageid) {

            doT.exec("template/task/task-detail.html", function (template) {
                Global.post("/task/GetTaskDetail", { taskID: taskid }, function (data) {
                    //获取任务详情内容
                    var arr = [];
                    arr.push(data.Item);
                    var html = template(arr);

                    $("#taskDetailContent").remove();
                    $("body").append(html);


                    $("#taskDetailContent").css("height",($(document).height()-70)+"px").animate({ right: '0px' }, 500);

                    //隐藏下拉
                    $(document).click(function (e) {
                        if (!$(e.target).parents().hasClass("taskContent") && !$(e.target).hasClass("taskContent") && !$(e.target).parents().hasClass(defaultParas.name) && !$(e.target).hasClass(defaultParas.name) && !$(e.target).parents().hasClass("jPag-pages") && !$(e.target).hasClass("jPag-pages")) {
                            $("#taskDetailContent").animate({ right: '-490px' }, 500, function () { $("#taskDetailContent").hide()});
                        }
                    });

                    //更新任务到期日期
                    var taskEndTime = {
                        elem: '#UpdateTaskEndTime',
                        format: 'YYYY-MM-DD',
                        max: '2099-06-16',
                        istime: false,
                        istoday: false,
                        choose: function () {
                            UpdateTaskEndTime(taskid);
                        }
                    };
                    laydate(taskEndTime);

                    //标记任务完成
                    $("#FinishTask").click(function () {
                        FinishTask(taskid);
                    });

                    //任务讨论列表
                    initTalkReply(orderid, stageid);
                });

            });
            
        }

        //更新任务到期日期
        var UpdateTaskEndTime = function (taskID) {
            Global.post("/Task/UpdateTaskEndTime", { taskID: taskID, endTime: $("#UpdateTaskEndTime").val() }, function (data) {
                if (data.Result == 1)
                {
                    //alert("保存成功");
                }
            });
        }

        //标记任务完成
        var FinishTask = function (taskID) {
            confirm("标记完成的任务不可逆,确定完成?", function () {
                Global.post("/Task/FinishTask", { taskID: taskID }, function (data) {
                    if (data.Result == 1) {
                        //alert("标记任务完成");
                        $("#FinishTask").addClass("btnccc").val("已完成").attr("disabled", "disabled");

                    }
                    else if (data.Result == 2) {
                        alert("前面阶段任务有未完成,不能标记完成");
                    }
                });
            });
        }

        //初始化任务讨论列表
        var initTalkReply = function (orderid, stageid) {
            var _self = this;

            $("#btnSaveTalk").click(function () {
                var txt = $("#txtContent");

                if (txt.val().trim()) {
                    var model = {
                        GUID: orderid,
                        StageID: stageid,
                        Content: txt.val().trim(),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    saveTaskReply(model);

                    txt.val("");
                }

            });

            getTaskReplys(orderid,stageid, 1);

        }

        //获取任务讨论列表
        var getTaskReplys = function (orderid,stageid, page) {
            var _self = this;
            $("#replyList").empty();

            Global.post("/Opportunitys/GetReplys", {
                guid: orderid,
                stageid:stageid,
                pageSize: 10,
                pageIndex: page
            }, function (data) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").append(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });

                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                StageID: _this.data("stageid"),
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            saveTaskReply(entity);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                    //require.async("businesscard", function () {
                    //    innerhtml.find("img").businessCard();
                    //});
                });

                $("#pagerReply").paginate({
                    total_count: data.totalCount,
                    count: data.pageCount,
                    start: page,
                    display: 5,
                    border: true,
                    rotate: true,
                    images: false,
                    mouse: 'slide',
                    float: "left",
                    onChange: function (page) {
                        getTaskReplys(orderid, stageid, page);
                    }
                });
            });
        }

        //保存任务讨论
        var saveTaskReply = function (model) {
            var _self = this;

            Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").prepend(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });

                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };
                            saveTaskReply(entity);

                        }
                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                    //require.async("businesscard", function () {
                    //    innerhtml.find("img").businessCard();
                    //});
                });
            });
        }


    })(jQuery)

    module.exports = jQuery;
});