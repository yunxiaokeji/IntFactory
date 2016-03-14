
define(function (require, exports, module) {
    require("plug/showtaskdetail/style.css");
    var doT = require("dot");
    var Global = require("global"),
        ChooseUser = require("chooseuser");

    (function ($) {
        //默认参数
        var defaultParas = {
            Name: "tb-task-list",
            UpdateTaskEndTimeCallBack: function (endtime, taskid) { },
            FinishTaskCallBack: function () { }
        };

        var IsClickEventFinish = true;
        $.fn.showtaskdetail = function (options) {
            defaultParas = $.extend([], defaultParas, options);

            $(this).click(function () {
                if (IsClickEventFinish) {
                    IsClickEventFinish = false;
                    $(this).addClass("taskDetailContent");

                    var taskid = $(this).data("taskid");
                    var orderid = $(this).data("orderid");
                    var stageid = $(this).data("stageid");
                    var mark = $(this).data("mark");

                    var $taskDetailContent = $("#taskDetailContent");

                    //没有任务详情对象
                    if ($taskDetailContent.length == 0) {
                        drawTaskDetail(taskid, orderid, stageid, mark);
                    }
                    else {
                        //查询新的任务详情
                        if ($taskDetailContent.data("taskid") != taskid) {
                            drawTaskDetail(taskid, orderid, stageid, mark);
                        }
                        else//隐藏显示的任务详情
                        {
                            if ($taskDetailContent.css("width") == "480px") {
                                $taskDetailContent.animate({ width: '0px' }, 200);
                            }
                            else
                                $taskDetailContent.show().animate({ width: '480px' }, 200);
                        }
                    }
                    IsClickEventFinish = true;

                }

            });
        };

        //获取任务详情
        var drawTaskDetail = function (taskid, orderid, stageid, mark) {

            doT.exec("plug/showtaskdetail/task-detail.html", function (template) {
                Global.post("/task/GetTaskDetail", { taskID: taskid }, function (data) {
                    //获取任务详情内容
                    var arr = [];
                    arr.push(data.Item);
                    var html = template(arr);

                    $("#taskDetailContent").remove();
                    $("body").append(html);


                    $("#taskDetailContent").animate({ width: '480px' }, 200);

                    //隐藏下拉
                    $(document).click(function (e) {
                        if (!$(e.target).parents().hasClass("taskContent") && !$(e.target).hasClass("taskContent") && !$(e.target).parents().hasClass("taskDetailContent") && !$(e.target).hasClass("taskDetailContent")
                            && !$(e.target).parents().hasClass("easyDialog_wrapper") && !$(e.target).hasClass("easyDialog_wrapper") && !$(e.target).parents().hasClass("alert") && !$(e.target).hasClass("alert")) {
                            $("#taskDetailContent").animate({ width: '0px' }, 100);
                        }
                    });

                    $("#changeTaskOwner").click(function () {
                        var _this = $(this);
                        ChooseUser.create({
                            title: "更换负责人",
                            type: 1,
                            single: true,
                            callback: function (items) {
                                if (items.length > 0) {
                                    if (_this.data("userid") != items[0].id) {
                                        Global.post("/Task/UpdateTaskOwner", {
                                            userid: items[0].id,
                                            taskid: taskid
                                        }, function (data) {
                                            if (data.status) {
                                                $("#taskOwnerID").text(items[0].name);
                                            }
                                        });
                                    } else {
                                        alert("请选择不同人员进行更换!");
                                    }
                                }
                            }
                        });
                    });

                    initTalkReply(orderid, stageid, mark);

                });

            });
            
        }

        //更新任务到期日期
        var UpdateTaskEndTime = function (taskID) {
            Global.post("/Task/UpdateTaskEndTime", { taskID: taskID, endTime: $("#UpdateTaskEndTime").val() }, function (data) {
                if (data.Result == 1)
                {
                    defaultParas.UpdateTaskEndTimeCallBack($("#UpdateTaskEndTime").val(), taskID);
                    //alert("保存成功");
                }
            });
        }

        //标记任务完成
        var FinishTask = function (taskID) {
            confirm("标记完成的任务不可逆,确定完成?", function () {
                Global.post("/Task/FinishTask", { taskID: taskID }, function (data) {
                    if (data.Result == 1) {
                        alert("标记任务完成");
                        $("#FinishTask").addClass("btnccc").val("已完成").attr("disabled", "disabled");

                        defaultParas.FinishTaskCallBack();
                    }
                    else if (data.Result == 2) {
                        alert("前面阶段任务有未完成,不能标记完成");
                    }
                    else if (data.Result == 3) {
                        alert("无权限操作");
                    }
                });
            });
        }

        //初始化任务讨论列表
        var initTalkReply = function (orderid, stageid, mark) {
            var _self = this;

            $("#btnSaveTalk").click(function () {
                var txt = $("#txtContent");

                if (txt.val().trim()) {
                    var model = {
                        GUID: orderid,
                        StageID: stageid,
                        Mark: mark,
                        Content: txt.val().trim(),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    saveTaskReply(model);

                    txt.val("");
                }

            });

            getTaskReplys(orderid, stageid, mark, 1);

        }

        //获取任务讨论列表
        var getTaskReplys = function (orderid, stageid, mark, page) {
            var _self = this;
            $("#replyList").empty();

            Global.post("/Opportunitys/GetReplys", {
                guid: orderid,
                stageid: stageid,
                mark: mark,
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
                                Mark: mark,
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
                        getTaskReplys(orderid, stageid, mark, page);
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