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
                ObjectJS.saveTaskReply(model, $(this),attchments);
                $("#btnSaveTalk").parents('.taskreply-box').find(".task-file").empty();
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
                !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace") &&
                !$(e.target).parents().hasClass("ico-delete") && !$(e.target).hasClass("ico-delete") &&
                !$(e.target).parents().hasClass("ico-delete-upload") && !$(e.target).hasClass("ico-delete-upload"))
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
                    //图片放大功能
                    var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;

                    innerhtml.find("#orderImage").click(function () {
                        if ($(this).attr("src")) {
                            $(this).parent().addClass("hoverimg");
                            $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                            $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })
                            $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).data("src") + '"/>');
                            $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                        }
                    });

                    innerhtml.find(".close-enlarge-image").click(function () {
                        $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                        $(".enlarge-image-item").empty();                       
                    });
                    
                    innerhtml.find(".enlarge-image-bgbox").click(function () {
                        $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                        $(".enlarge-image-item").empty();
                    });
                    
                    innerhtml.find(".zoom-botton").click(function (e) {
                        var scaleToAdd = 0.8;
                        if (e.target.id == 'zoomOutButton')
                            scaleToAdd = -scaleToAdd;
                        $('#enlargeImage').smartZoom('zoom', scaleToAdd);
                        return false;
                    });
                    
                    $(".left-enlarge-image").click(function () {
                        var ele = $("#orderImages .hoverimg").prev();
                        if (ele && ele.find("img").attr("src")) {
                            var _img = ele.find("img");
                            $("#orderImages .hoverimg").removeClass("hoverimg");
                            ele.addClass("hoverimg");
                            $("#orderImage").attr("src", _img.data("src"));
                            $(".enlarge-image-item").empty();
                            $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                            $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                        }
                    });
                    
                    $(".right-enlarge-image").click(function () {
                        var ele = $("#orderImages .hoverimg").next();
                        if (ele && ele.find("img").attr("src")) {
                            var _img = ele.find("img");
                            $("#orderImages .hoverimg").removeClass("hoverimg");
                            ele.addClass("hoverimg");
                            $("#orderImage").attr("src", _img.data("src"));
                            $(".enlarge-image-item").empty();
                            $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.data("src") + '"/>');
                            $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
                        }
                    });

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
    ObjectJS.saveTaskReply = function (model, btnObject,attchments) {
        var _self = this;
        var btnname = "";
        if (btnObject) {
            btnname = btnObject.html();
            btnObject.html("保存中...").attr("disabled", "disabled");
        }
        
        Global.post("/" + Controller + "/SavaReply", { entity: JSON.stringify(model), taskID: Reply.taskid, attchmentEntity: JSON.stringify(attchments) }, function (data) {
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
                            if (data.Items[i].isImage == 2) {
                                doT.exec("/template/task/task-file-upload.html", function (template) {
                                    var file = template(data.Items);
                                    $("#file_" + _this.data("replyid")).append(file).fadeIn(300);
                                    $(".ico-delete-upload").click(function () {
                                        $(this).parent().remove();
                                        if ($("#file_" + _this.data("replyid") + " li").length == 0) {
                                            $("#file_" + _this.data("replyid")).hide();
                                        }
                                    });
                                });
                            } else {
                                doT.exec("/template/task/task-file-upload-img.html", function (template) {
                                    var file = template(data.Items);
                                    $("#images_" + _this.data("replyid")).append(file).fadeIn(300);
                                    $(".ico-delete").click(function () {
                                        $(this).parent().remove();
                                        if ($("#images_" + _this.data("replyid") + " li").length == 0) {
                                            $("#images_" + _this.data("replyid")).hide();
                                        }
                                    });
                                });
                            };
                            return;
                        }
                    } else {
                        alert("上传失败");
                    }
                }
            });
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

        replys.find('.btn-emotion').each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });

        replys.find(".no-img li").hover(function () {
            $(this).find(".popup-download").stop(true).slideDown(300);
        },function () {
            $(this).find(".popup-download").stop(true).slideUp(300);
        });

        $(".download").click(function () {
            window.open($(this).data('url'), '_target');
        });
        
    }

    module.exports = ObjectJS;
});

