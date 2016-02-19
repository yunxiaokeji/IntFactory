
define(function (require, exports, module) {
    require("plug/showtaskdetail/style.css");
    require("plug/showtaskdetail/customer.css");
    var doT = require("dot");
    var Global = require("global");

    (function ($) {

        $.fn.showtaskdetail=function (options) {
            $(this).click(function () {
                var taskid = $(this).data("taskid");
                var orderid = $(this).data("orderid");
                var $taskDetailContent = $("#taskDetailContent");

                //没有任务详情对象
                if ($taskDetailContent.length == 0){
                    drawTaskDetail(taskid, orderid);
                }
                else{
                    //查询新的任务详情
                    if ($taskDetailContent.data("taskid") != taskid) {
                        drawTaskDetail(taskid, orderid);
                    }
                    else//隐藏显示的任务详情
                    {
                        if ($taskDetailContent.offset().left < $(document).width()-10)
                            $taskDetailContent.animate({ right: '-380px' }, 500);
                        else
                            $taskDetailContent.animate({ right: '0px' }, 500);
                    }
                    
                }

            });
        };

        //获取任务详情
        var drawTaskDetail = function (taskid,orderid) {

            doT.exec("template/task/task-detail.html", function (template) {
                var arr = [];
                arr.push({ TaskID: taskid });
                var html = template(arr);

                $("#taskDetailContent").remove();
                $("body").append(html);

                $("#taskDetailContent").animate({ right: '0px' }, 500);

                var taskEndTime = {
                    elem: '#UpdateTaskEndTime',
                    format: 'YYYY-MM-DD',
                    max: '2099-06-16',
                    istime: false,
                    istoday: false
                };
                laydate(taskEndTime);

                initTalk(orderid);
            });
            
        }

        var initTalk = function (orderid) {
            var _self = this;

            $("#btnSaveTalk").click(function () {
                var txt = $("#txtContent");

                if (txt.val().trim()) {
                    var model = {
                        GUID: orderid,
                        Content: txt.val().trim(),
                        FromReplyID: "",
                        FromReplyUserID: "",
                        FromReplyAgentID: ""
                    };
                    saveTaskReply(model);

                    txt.val("");
                }

            });

            getTaskReplys(orderid, 1);

        }

        //
        var getTaskReplys = function (orderid, page) {
            var _self = this;
            $("#replyList").empty();

            Global.post("/Opportunitys/GetReplys", {
                guid: orderid,
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
                    border_color: '#fff',
                    text_color: '#333',
                    background_color: '#fff',
                    border_hover_color: '#ccc',
                    text_hover_color: '#000',
                    background_hover_color: '#efefef',
                    rotate: true,
                    images: false,
                    mouse: 'slide',
                    float: "left",
                    onChange: function (page) {
                        getReplys(orderid, page);
                    }
                });
            });
        }

        //
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