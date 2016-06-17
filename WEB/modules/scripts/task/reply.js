define(function (require, exports, module) {
    var Global = require("global");
    var doT = require("dot");
    var Qqface = require("qqface");
    Upload = require("upload");
    var ObjectJS = {};
    var Reply = {};
    var Controller = "Opportunitys";
    
    var IsCallBack = false;

    ///任务讨论
    //初始化任务讨论列表
    ObjectJS.initTalkReply = function (reply,moduleType,callback) {
        Reply = reply;
        if (moduleType === "customer") {
            Controller = moduleType;
        }
        else if(moduleType==="task") {
            ObjectJS.callBack = callback;
            IsCallBack = true;
        }
       
        //任务讨论盒子点击
        $(".taskreply-box").click(function () {
            $(this).addClass("taskreply-box-hover").find(".reply-content").focus();
        });

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");
            var attchments = [{}];
            if (txt.val().trim()) {
                var model = {
                    GUID: Reply.guid,
                    StageID: Reply.stageid,
                    mark: Reply.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: "",
                    Attachments:attchments
                };
                ObjectJS.saveTaskReply(model, $(this));

                txt.val("");
            }

        });
       

        $(".btn-emotion").each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });
        
        ObjectJS.getTaskReplys(1);

        //任务讨论盒子隐藏
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("taskreply-box") && !$(e.target).hasClass("taskreply-box")&&
                !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace"))
            {
                $(".taskreply-box").removeClass("taskreply-box-hover");
                
            }
        });
    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='2' style='border:none;'><div class='data-loading'><div></td></tr>");
        Global.post("/" + Controller + "/GetReplys", {
            guid: Reply.guid,
            stageid: Reply.stageid,
            mark: Reply.mark,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();

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

            if (IsCallBack) {
                ObjectJS.callBack();
                IsCallBack = false;
            }

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

        Global.post("/" + Controller + "/SavaReply", { entity: JSON.stringify(model) }, function (data) {
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

    //绑定任务讨论操作
    ObjectJS.bindReplyOperate = function (replys) {
        replys.find(".reply-content").each(function () {
            $(this).html(Global.replaceQqface($(this).html()));
        });

        //回复点击
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

        //回复
        replys.find(".save-reply").click(function () {
            var _this = $(this);
            if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                var entity = {
                    GUID: _this.data("id"),
                    StageID: _this.data("stageid"),
                    Mark: Reply.mark,
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

        replys.find(".no-img li").hover(function () {
            $(this).find(".popup-download").stop(true).slideDown(300);
            $(".popup-download").click(function () {
                alert("ok");
            });
        },function () {
            $(this).find(".popup-download").stop(true).slideUp(300);
        });

        Upload.createUpload({
            element: "#btn-update-reply",
            buttonText: "&#xe618;",
            className: "left iconfont",
            multiple: false,
            data: { folder: '', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    for (var i = 0; i < data.Items.length; i++) {
                        if ($("#orderImages-reply li").length < 5) {
                            var img = $('<li><img src="' + data.Items[i] + '" /><span class="ico-delete"></span> </li>');  
                            $("#orderImages-reply").append(img).fadeIn(300);
                            img.find(".ico-delete").click(function () {
                                $(this).parent().remove();
                                if ($("#orderImages-reply li").length == 0) {                                    
                                    $("#orderImages-reply").hide();
                                }
                            });
                        }
                    }
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！");
                }
            }
        });
        
    }

    module.exports = ObjectJS;
});

