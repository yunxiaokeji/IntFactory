define(function (require, exports, module) {
    var Global = require("global");
    var doT = require("dot");
    var Qqface = require("qqface");
    Upload = require("upload");
    var ObjectJS = {};
    var Reply = {};
    var Controller = "Task";

    ///任务讨论
    //初始化任务讨论列表
    ObjectJS.initTalkReply = function (reply, moduleType) {

        Reply = reply;
        if (moduleType === "customer") {
            Controller = moduleType;
        }

        //任务讨论盒子点击
        $(".taskreply-box").click(function () {
            $(this).addClass("taskreply-box-hover").find(".reply-content").focus();
        });

        //任务讨论盒子隐藏
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("taskreply-box") && !$(e.target).hasClass("taskreply-box") &&
                !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace") &&
                !$(e.target).parents().hasClass("ico-delete") && !$(e.target).hasClass("ico-delete") &&
                !$(e.target).parents().hasClass("ico-delete-upload") && !$(e.target).hasClass("ico-delete-upload")) {
                $(".taskreply-box").removeClass("taskreply-box-hover");

            }
        });

        //保存讨论
        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");
            if (txt.val().trim()) {
                var model = {
                    GUID: Reply.guid,
                    StageID: Reply.stageid,
                    mark: Reply.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                var attchments = [];

                $("#btnSaveTalk").parents('.taskreply-box').find('.task-file li').each(function () {
                    var _this = $(this);
                    attchments.push({
                        "Type": _this.data('isimg'),
                        "ServerUrl": "",
                        "FilePath": _this.data('filepath'),
                        "FileName": _this.data('filename'),
                        "OriginalName": _this.data('originalname'),
                        "ThumbnailName": ""
                    });
                })
                ObjectJS.saveTaskReply(model, $(this), attchments);

                $("#btnSaveTalk").parents('.taskreply-box').find(".task-file").empty();
                txt.val("");
            }

        });
       
        //讨论表情
        $(".btn-emotion").each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });
        
        //获取讨论
        //ObjectJS.getTaskReplys(1);

       
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

        });
    }

    //保存任务讨论
    ObjectJS.saveTaskReply = function (model, btnObject,attchments) {
        var _self = this;
        var btnname = "";
        if (btnObject) {
            btnname = btnObject.html();
            btnObject.html("保存中...").attr("disabled", "disabled");
        }
       
        var params = { entity: JSON.stringify(model), attchmentEntity: JSON.stringify(attchments) };
            
        if (Controller == "customer") {
            params.customerID = Reply.guid;
        }
        else {
            params.taskID = Reply.taskid;
        }

        Global.post("/" + Controller + "/SavaReply", params, function (data) {
            if (btnObject) {
                btnObject.html(btnname).removeAttr("disabled");
            }

            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                innerhtml.hide();
               
                innerhtml.fadeIn(500);
                $("#replyList .nodata-txt").parent().parent().remove();
                $("#replyList").prepend(innerhtml);
                ObjectJS.bindReplyOperate(innerhtml);

            });
        });
    }

    //绑定任务讨论操作
    ObjectJS.bindReplyOperate = function (replys) {
        //替换表情内容
        replys.find(".reply-content").each(function () {
            $(this).html(Global.replaceQqface($(this).html()));
        });

        //打开讨论盒
        replys.find(".btn-reply").click(function () {
            var _this = $(this);
            var reply = _this.nextAll(".reply-box");
            $("#btn-update-reply" + _this.data("replyid")).empty();

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

            //讨论附件
            Upload.createUpload({
                element: "#btn-update-reply" + _this.data("replyid"),
                buttonText: "&#xe65a;",
                className: "left iconfont",
                multiple: false,
                url: "/Plug/UploadFiles",
                data: { folder: '', action: 'add', oldPath: "" },
                success: function (data, status) {
                    if (data.Items.length > 0) {
                        for (var i = 0; i < data.Items.length; i++) {
                            if ($(".task-file li").length <= 9) {
                                var templateUrl = "/template/task/task-file-upload.html";
                                var appendHtml = $("#file_" + _this.data("replyid"));
                                if (data.Items[i].isImage == 1) {
                                    templateUrl = "/template/task/task-file-upload-img.html";
                                    appendHtml = $("#images_" + _this.data("replyid"));
                                }
                                doT.exec(templateUrl, function (template) {
                                    var file = template(data.Items);
                                    file = $(file);
                                    appendHtml.append(file).fadeIn(300);
                                    file.find(".delete").click(function () {
                                        $(this).parent().remove();
                                        if (appendHtml.find('li').length == 0) {
                                            appendHtml.hide();
                                        }
                                    });
                                });
                            }
                            return;
                        }
                    } else {
                        alert("上传失败");
                    }
                }
            });
        });

        //保存讨论
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
                var attchments = [];

                _this.parents('.reply-box').find('.task-file li').each(function () {
                    var _this = $(this);
                    attchments.push({
                        "Type": _this.data('isimg'),
                        "ServerUrl": "",
                        "FilePath": _this.data('filepath'),
                        "FileName": _this.data('filename'),
                        "OriginalName": _this.data('originalname'),
                        "ThumbnailName": ""
                    });
                })

                ObjectJS.saveTaskReply(entity, _this,attchments);
                _this.parents('.reply-box').find(".task-file").empty();
            }
            $("#Msg_" + _this.data("replyid")).val('');
            $(this).parent().slideUp(300);
        });

        //讨论表情
        replys.find('.btn-emotion').each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });

        //绑定图片放大功能
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        replys.find(".orderImage-repay").click(function () {
            if ($(this).attr("src")) {
                $("#Images-reply .hoverimg").removeClass("hoverimg");
                $(this).parent().addClass("hoverimg");
                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })
                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).data("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                $(".left-enlarge-image").unbind().click(function () {
                    var ele = $("#Images-reply .hoverimg").prev();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $("#Images-reply .hoverimg").removeClass("hoverimg");
                        ele.addClass("hoverimg");
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });

                $(".right-enlarge-image").unbind().click(function () {
                    var ele = $("#Images-reply .hoverimg").next();
                    if (ele && ele.find("img").attr("src")) {
                        var _img = ele.find("img");
                        $("#Images-reply .hoverimg").removeClass("hoverimg");
                        ele.addClass("hoverimg");
                        $(".enlarge-image-item").empty();
                        $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                        $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                    }
                });
            }
        });
        
        //
        replys.find(".no-img li").hover(function () {
            $(this).find(".popup-download").stop(true).slideDown(300);
        },function () {
            $(this).find(".popup-download").stop(true).slideUp(300);
        });

        //
        $(".download").click(function () {
            window.open($(this).data('url'), '_target');
        });
    }

    module.exports = ObjectJS;
});

