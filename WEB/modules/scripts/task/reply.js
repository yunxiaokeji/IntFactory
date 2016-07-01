define(function (require, exports, module) {
    var Global = require("global");
    var doT = require("dot");
    var Tip = require("tip");
    var Qqface = require("qqface");
    Upload = require("upload");
    var ObjectJS = {};
    var Reply = {};
    var Controller = "Task";

    ///任务讨论
    //初始化任务讨论列表
    ObjectJS.initTalkReply = function (reply, moduleType,noGet) {
        Reply = reply;
        if (moduleType === "customer") {
            Controller = moduleType;
        }

        //任务讨论盒子点击
        $(".taskreply-box").click(function () {
            $(this).addClass("taskreply-box-hover").find(".reply-content").focus();
        });
        
        ObjectJS.replyAttachment("");

        //任务讨论盒子隐藏
        $(document).click(function (e) {
            if (!$(e.target).parents().hasClass("taskreply-box") && !$(e.target).hasClass("taskreply-box") &&
                !$(e.target).parents().hasClass("qqFace") && !$(e.target).hasClass("qqFace") &&
                !$(e.target).parents().hasClass("ico-delete") && !$(e.target).hasClass("ico-delete") &&
                !$(e.target).parents().hasClass("ico-delete-upload") && !$(e.target).hasClass("ico-delete-upload")&&
                !$(e.target).parents().hasClass("alert") && !$(e.target).hasClass("alert")) {
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
                $('.taskreply-box .task-file li').each(function () {
                    var _this = $(this);
                    attchments.push({
                        "Type": _this.data('isimg'),
                        "FilePath": _this.data('filepath'),
                        "FileName": _this.data('filename'),
                        "OriginalName": _this.data('originalname'),
                        "Size": _this.data("filesize"),
                        "ServerUrl": "",
                        "ThumbnailName": ""
                    });
                });
                ObjectJS.saveTaskReply(model, $(this), attchments);

                txt.val("");
                $('.taskreply-box .task-file').empty();
            }
            else {
                alert("请输入讨论内容");
            }

        });
       
        //讨论表情
        $(".taskreply-box .btn-emotion").each(function () {
            $(this).qqFace({
                assign: $(this).data("id"),
                path: '/modules/plug/qqface/arclist/'	//表情存放的路径
            });
        });
        
        //获取讨论
        if (!noGet) {
            ObjectJS.getTaskReplys(1);
        }

        //tip提示
        $("#reply-attachment").Tip({
            width: 100,
            msg: "上传附件最多10个"
        });

        var IsImage = 2;
        var ReplyId = "";
        var ImageCheck = ["image/x-png", "image/png", "image/gif", "image/jpeg", "image/tiff", "application/x-MS-bmp", "image/pjpeg"];
        var InnerHtml = [];
        var uploader = Qiniu.uploader({
            browse_button: 'reply-attachment',
            container: 'taskreply-box',
            drop_element: 'taskreply-box',
            file_path: "/Content/UploadFiles/Task/",
            maxQuantity: 5,
            maxSize: 5,
            fileType: 3,
            //auto_start: true,
            init: {                
                'FilesAdded': function (up, files) {                    
                    for (var i = 0; i < files.length; i++) {
                        var item = files[i];
                        for (var i = 0; i < ImageCheck.length; i++) {
                            if (ImageCheck[i] == item.type) {
                                IsImage = 1;
                                break;
                            }
                        }
                        var templateUrl = "/template/task/task-file-upload.html";
                        var fileBox = $("#reply-files" + ReplyId);
                        if (IsImage == 2) {
                            doT.exec(templateUrl, function (template) {
                                InnerHtml = template();
                                InnerHtml = $(InnerHtml);
                                fileBox.append(InnerHtml).fadeIn(300);

                                InnerHtml.find(".delete").click(function () {
                                    $(this).parent().remove();
                                    if (fileBox.find('li').length == 0) {
                                        fileBox.hide();
                                    }
                                });
                            });
                        } else {
                            doT.exec("/template/task/task-file-upload-img.html", function (template) {
                                InnerHtml = template();
                                InnerHtml = $(InnerHtml);
                                $("#reply-imgs" + ReplyId).append(InnerHtml).fadeIn(300);

                                InnerHtml.find(".delete").click(function () {
                                    $(this).parent().remove();
                                    if ($("#reply-imgs" + ReplyId).find('li').length == 0) {
                                        $("#reply-imgs" + ReplyId).hide();
                                    }
                                });
                            });
                        }
                    }
                },
                'UploadProgress': function (up, file) {
                    InnerHtml.find('.progress-number').html(file.percent + "%");
                    //var progress = new FileProgress(file, 'fsUploadProgress');
                    //var chunk_size = plupload.parseSize(this.getOption('chunk_size'));

                    //progress.setProgress(file.percent + "%", file.speed, chunk_size);
                },
                'FileUploaded': function (up, file, info) {                    
                    InnerHtml.find('.progress-number').remove();
                    var itemInfo = JSON.parse(info);
                    var itemFile = file;                  
                    
                    InnerHtml.data({
                        'isimg': IsImage,
                        'filepath': '',
                        'filename': itemInfo.key,
                        'filesize': itemFile.size,
                        'originalname': itemFile.name
                    });
                    var src = 'http://o9h6bx3r4.bkt.clouddn.com/' + InnerHtml.data('filepath') + InnerHtml.data('filename');
                    var imageObj = $('<img src="' + src + '" />');
                    InnerHtml.prepend(imageObj);
                },
                //若想在前端对每个文件的key进行个性化处理，可以配置该函数
                'Key': function (up, file) {
                    // 若想在前端对每个文件的key进行个性化处理，可以配置该函数
                    // 该配置必须要在 unique_names: false , save_key: false 时才生效
                    var filename = file.name;
                    var fileExtension = filename.substring(filename.lastIndexOf(".") + 1).toLowerCase();
                    var key = up.getOption("file_path") + (new Date()).valueOf() + "." + fileExtension;
                    // do something with key here
                    return key
                }
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
            else
            {
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
        var btnName = "";
        if (btnObject) {
            btnName = btnObject.html();
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
                btnObject.html(btnName).removeAttr("disabled");
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

    //上传附件
    ObjectJS.replyAttachment = function (replyid) {
        //Upload.createUpload({
        //    element: "reply-attachment" + replyid,
        //    buttonText: "&#xe65a;",
        //    className: "left iconfont",
        //    multiple: true,
        //    fileType: 3,//附件类型 1:图片；2：文件；3图片和文件
        //    maxSize: 5 * 1024,//附件最大大小
        //    maxQuantity: 10,//最大上传文件个数
        //    successItems: ".upload-files-" + replyid + " li",
        //    url: "/Plug/UploadFiles",
        //    data: { folder: '', action: 'add', oldPath: "" },
        //    success: function (data, status)
        //    {
        //        if (data.Stage == false)
        //        {
        //            alert("每次最多允许上传5个");
        //        }
        //        else {                   
        //            var len = data.Items.length;
        //            if (len > 0)
        //            {
        //                var templateUrl = "/template/task/task-file-upload.html";
        //                var fileBox = $("#reply-files" + replyid);
        //                if ($(".msg-" + replyid).val() == "") {
        //                    $(".msg-" + replyid).val(data.Items[0].originalName.split('.')[0]);
        //                }
                        
        //                var fileArr = new Array();
        //                var picArr = new Array();
        //                for (var i = 0; i < len ; i++) {
        //                    var item = data.Items[i];
                            
        //                    if (item.isImage == 1) {
        //                        picArr.push(item);
        //                    }
        //                    else {
        //                        fileArr.push(item);
        //                    }
        //                }
                        
        //                if (fileArr.length > 0) {
        //                    doT.exec(templateUrl, function (template) {
        //                        var innerhtml = template(fileArr);
        //                        innerhtml = $(innerhtml);
        //                        fileBox.append(innerhtml).fadeIn(300);

        //                        innerhtml.find(".delete").click(function () {
        //                            $(this).parent().remove();
        //                            if (fileBox.find('li').length == 0) {
        //                                fileBox.hide();
        //                            }
        //                        });


        //                    });
        //                }
        //                if (picArr.length > 0) {
        //                    doT.exec("/template/task/task-file-upload-img.html", function (template) {
        //                        var innerhtml = template(picArr);
        //                        innerhtml = $(innerhtml);
        //                        $("#reply-imgs" + replyid).append(innerhtml).fadeIn(300);

        //                        innerhtml.find(".delete").click(function () {
        //                            $(this).parent().remove();
        //                            if ($("#reply-imgs" + replyid).find('li').length == 0) {
        //                                $("#reply-imgs" + replyid).hide();
        //                            }
        //                        });
        //                    });
        //                }
        //            }
        //            else {
        //                alert("上传文件格式不正确或有文件超过10M");
        //            }

        //        }
        //    }
        //});
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
            
            $("#reply-attachment" + _this.data("replyid")).empty();

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

            ObjectJS.replyAttachment(_this.data("replyid"));                        
            
            //提示
            $("#reply-attachment" + _this.data("replyid")).Tip({
                width: 100,
                msg: "上传附件最多10个"
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

                $(".upload-files-" + _this.data("replyid") + " li").each(function () {
                    var _this = $(this);
                    attchments.push({
                        "Size": _this.data("filesize"),
                        "Type": _this.data('isimg'),
                        "ServerUrl": "",
                        "FilePath": _this.data('filepath'),
                        "FileName": _this.data('filename'),
                        "OriginalName": _this.data('originalname'),
                        "ThumbnailName": ""
                    });
                })
                ObjectJS.saveTaskReply(entity, _this, attchments);
                $(this).parent().slideUp(300);
                _this.parents('.reply-box').find(".task-file").empty();
            }
            else {
                alert("请输入讨论内容");
            }
            $("#Msg_" + _this.data("replyid")).val('');

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

                $(".close-enlarge-image").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty();
                });
                $(".enlarge-image-bgbox").unbind().click(function () {
                    $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
                    $(".enlarge-image-item").empty();
                });
                $(".zoom-botton").unbind().click(function (e) {
                    var scaleToAdd = 0.8;
                    if (e.target.id == 'zoomOutButton')
                        scaleToAdd = -scaleToAdd;
                    $('#enlargeImage').smartZoom('zoom', scaleToAdd);
                    return false;
                });

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
        
        //下载图标下滑切换
        replys.find(".upload-file li").hover(function () {
            $(this).find(".popup-download").stop(true).slideDown(300);
        },function () {
            $(this).find(".popup-download").stop(true).slideUp(300);
        });

        //下载附件
        $(".download").click(function () {
            window.open($(this).data('url') + "&isIE=" + (!!window.ActiveXObject || "ActiveXObject" in window ? "1" : ""));
        });
    }

    module.exports = ObjectJS;
});

