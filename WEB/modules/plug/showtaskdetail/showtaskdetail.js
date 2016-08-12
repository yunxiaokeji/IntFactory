
define(function (require, exports, module) {
    require("plug/showtaskdetail/style.css");
    
    var Global = require("global"),
        ChooseUser = require("chooseuser");
    var doT = require("dot");
    var TalkReply = require("replys");

    (function ($) {
        //默认参数
        var defaultParas = {
            Name: "tb-task-list",
            mark: 0,
            UpdateTaskEndTimeCallBack: function (endtime, taskid) { },
            FinishTaskCallBack: function () { }
        };
        var isClickEventFinish = true;

        $.fn.showtaskdetail = function (options) {
            defaultParas = $.extend([], defaultParas, options);

            $(this).click(function () {
                if (isClickEventFinish) {
                    isClickEventFinish = false;
                    $(this).addClass("taskDetailContent");

                    defaultParas.taskid = $(this).data("taskid");
                    defaultParas.guid = $(this).data("orderid");
                    defaultParas.orderid = $(this).data("orderid");
                    defaultParas.stageid = $(this).data("stageid");
                    defaultParas.mark = $(this).data("mark");
                    defaultParas.self = $(this).data("self");
                    defaultParas.lock = $(this).data("lock");
                    

                    var $taskDetailContent = $("#taskDetailContent");

                    //没有任务详情对象
                    if ($taskDetailContent.length == 0) {
                        drawTaskDetail();
                    }
                    else {
                        //查询新的任务详情
                        if ($taskDetailContent.data("taskid") != defaultParas.taskid) {
                            drawTaskDetail();
                        }
                        else//隐藏显示的任务详情
                        {
                            if ($taskDetailContent.css("width") == "500px") {
                                $taskDetailContent.animate({ width: '0px' }, 200);
                            }
                            else {
                                $taskDetailContent.show().animate({ width: '500px' }, 200);
                            }
                        }
                    }
                    isClickEventFinish = true;
                }

            });
        };

        //获取任务详情
        var drawTaskDetail = function () {

            if ($("#taskDetailContent").length > 0) {
                $("#taskDetailContent").data("taskid", defaultParas.taskid);
                $("#taskDetailContent").empty();
            } else {
                $("body").append('<div class="task-layer-box" id="taskDetailContent" data-taskid="' + defaultParas.taskid + '"></div>');
            }
            $("#taskDetailContent").append("<div class='data-loading' ><div>");
            $("#taskDetailContent").animate({ width: '500px' }, 200);

            doT.exec("plug/showtaskdetail/task-detail.html", function (template) {
                Global.post("/Task/GetTaskDetail", { id: defaultParas.taskid }, function (data) {
                    //获取任务详情内容
                    var item = data.item;
                    var items = [item];
                    var innerhtml = template(items);

                    $("#taskDetailContent").empty();
                    $("#taskDetailContent").append(innerhtml);

                    $("#btnLockTask").hide();

                    if (item.FinishStatus == 0) {
                        $("#taskDetailContent").css("box-shadow", "0 0 5px #ccc");
                    } else if (item.FinishStatus == 1) {
                        $("#taskDetailContent").css("box-shadow", "0 0 5px #45BF67");
                    } else if (item.FinishStatus == 2) {
                        $("#taskDetailContent").css("box-shadow", "0 0 5px #3A7DE5");
                        $("#btnLockTask").show();
                        //是否有权限
                        if (!defaultParas.lock) {
                            if (item.LockStatus == 1) {
                                $("#btnLockTask").html("重启任务").data("lock", item.LockStatus);
                            } else {
                                $("#btnLockTask").html("锁定任务").data("lock", item.LockStatus);
                            }
                            $("#btnLockTask").click(function () {
                                var _this = $(this);
                                if (_this.data("lock") == 1) {
                                    confirm("任务重启后可以继续操作任务，确认重启吗？", function () {
                                        Global.post("/Orders/UpdateTaskLockStatus", {
                                            taskid: defaultParas.taskid,
                                            status: 2
                                        }, function (result) {
                                            if (result.status) {
                                                window.location = window.location;
                                                _this.data("lock", 2).html("锁定任务");
                                            } else {
                                                alert("任务重启失败");
                                            }
                                            
                                        });
                                    });
                                } else {
                                    confirm("任务锁定后不可继续操作，确认锁定吗？", function () {
                                        Global.post("/Orders/UpdateTaskLockStatus", {
                                            taskid: defaultParas.taskid,
                                            status: 1
                                        }, function (result) {
                                            if (result.status) {
                                                window.location = window.location;
                                                _this.data("lock", 1).html("重启任务");
                                            } else {
                                                alert("任务锁定失败");
                                            }
                                        });
                                    });
                                }
                            });
                        } else {
                            $("#btnLockTask").remove();
                        }
                    }

                    //隐藏下拉
                    $(document).click(function (e) {
                        if (!$(e.target).parents().hasClass("task-layer-box") && !$(e.target).hasClass("task-layer-box")
                            && !$(e.target).parents().hasClass("easyDialog_wrapper") && !$(e.target).hasClass("easyDialog_wrapper")
                            && !$(e.target).parents().hasClass("alert") && !$(e.target).hasClass("alert")
                            && !$(e.target).parents().hasClass("stage-items") && !$(e.target).hasClass("stage-items")
                            && !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace")
                            && !$(e.target).parents().hasClass("ico-delete-upload") && !$(e.target).hasClass("ico-delete-upload")
                            && !$(e.target).hasClass("qn-delete")
                            && !$(e.target).parents().hasClass("ico-delete") && !$(e.target).hasClass("ico-delete")
                            ) {
                            $("#taskDetailContent").animate({ width: '0px' }, 100);
                        }
                    });
                    //转移
                    if (defaultParas.self == 1) {
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
                                                taskid: defaultParas.taskid
                                            }, function (data) {
                                                if (data.result) {
                                                    $("#taskOwnerID").text(items[0].name);
                                                    $("#changeTaskOwner").data("userid", items[0].id);
                                                }
                                            });
                                        } else {
                                            alert("请选择不同人员进行更换!");
                                        }
                                    }
                                }
                            });
                        });
                    }
                    else {
                        $("#changeTaskOwner").hide();
                    }

                    TalkReply.initTalkReply({
                        element: ".taskContent .talk-body",
                        guid: defaultParas.taskid,
                        type: 10, /*1 客户 2订单 10任务 */
                        pageSize: 10
                    });
                });
            });
        }

    })(jQuery)

    module.exports = jQuery;
});