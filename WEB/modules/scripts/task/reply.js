define(function (require, exports, module) {
    var ObjectJS = {};

    ///任务讨论
    //初始化任务讨论列表
    ObjectJS.init = function (orderid, stageid, mark) {
        debugger;
        ObjectJS.orderid = orderid;
        ObjectJS.stageid = stageid;
        ObjectJS.mark = mark;
        alert(1111);
        ObjectJS.bindEvent();

        ObjectJS.getTaskReplys(1);
    }

    ObjectJS.bindEvent = function () {
        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");

            if (txt.val().trim()) {
                var model = {
                    GUID: ObjectJS.orderid,
                    StageID: ObjectJS.stageid,
                    mark: ObjectJS.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                ObjectJS.saveTaskReply(model, $(this));

                txt.val("");
            }

        });
    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='2' style='border:none;'><div class='data-loading'><div></td></tr>");

        Global.post("/Opportunitys/GetReplys", {
            guid: ObjectJS.orderid,
            stageid: ObjectJS.stageid,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            if (data.items.length > 0) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").html(innerhtml);

                    ObjectJS.bindReplyOperate(innerhtml);

                });
            }
            else {
                $("#replyList").html("<tr><td colspan='2' style='border:none;'><div class='nodata-txt' >暂无评论!<div></td></tr>");
            }

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
                    ObjectJS.getTaskReplys(page);
                }
            });
        });
    }

    //保存任务讨论
    ObjectJS.saveTaskReply = function (model, btnObject) {
        var _self = this;
        var btnname = "";
        if (btnObject) {
            btnname = btnObject.html();
            btnObject.html("保存中...").attr("disabled", "disabled");
        }

        Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            if (btnObject) {
                btnObject.html(btnname).removeAttr("disabled");
            }

            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                innerhtml.hide();
                $("#replyList .nodata-txt").parent().parent().remove();
                $("#replyList").prepend(innerhtml);
                innerhtml.fadeIn(500);

                ObjectJS.bindReplyOperate(innerhtml);

            });
        });
    }

    //绑定讨论相关操作
    ObjectJS.bindReplyOperate = function (replys) {
        replys.find(".reply-content").each(function () {
            $(this).html(Global.replaceQqface($(this).html()));
        });

        replys.find(".btn-reply").click(function () {
            var _this = $(this), reply = _this.nextAll(".reply-box");

            $("#replyList .reply-box").each(function () {
                if ($(this).data("replyid") != reply.data("replyid")) {
                    $(this).hide();
                }
            });

            if (reply.is(":visible")) {
                reply.slideUp(300);
            }
            else {
                reply.slideDown(300);
            }

            reply.find("textarea").focus();

        });

        replys.find(".save-reply").click(function () {
            var _this = $(this);
            if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                var entity = {
                    GUID: _this.data("id"),
                    StageID: _this.data("stageid"),
                    Mark:ObjectJS.mark,
                    Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                    FromReplyID: _this.data("replyid"),
                    FromReplyUserID: _this.data("createuserid"),
                    FromReplyAgentID: _this.data("agentid")
                };
                ObjectJS.saveTaskReply(entity, _this);

            }
            $("#Msg_" + _this.data("replyid")).val('');
            $(this).parent().slideUp(300);
        });

        replys.find('.btn-emotion').each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });

    }

    module.exports = ObjectJS;
});

