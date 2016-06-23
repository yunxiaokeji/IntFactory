define(function (require,exports,module) {
    var Global = require('global');
    var Upload = require('upload');
    var ObjectJS = {};
    
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    }

    ObjectJS.bindEvent = function () {

        ProductIco = Upload.createUpload({
            element: "upload",
            buttonText: "选择附件",
            className: "",
            multiple: false,
            data: { folder: '', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0) {
                    for (var i = 0; i < data.Items.length; i++) {
                        if ($(".feed-annex div:last-child li").length < 5) {
                            var img = $('<li><img src="' + data.Items[i] + '" /><i class="iconfont">&#xe620;</i></li>');
                            $(".feed-annex div:last-child ul").html(img);
                            img.find(".iconfont").click(function () {
                                $(this).parent().remove();
                            });
                        }
                    }
                } else {
                    alert("只能上传jpg/png/gif类型的图片，且大小不能超过5M！");
                }
            }
        });

        $("#btn-feedback").click(function () {
            var _this = $(this);
            if ($(".txt-title").val() == "") {
                alert("标题不能为空");
                return false;
            }
            if ($(".txt-description").val().length >= 1000)
            {
                alert("问题描述请在1000个字符以内");
                return false;
            }
            _this.val("提交中...");
            _this.attr("disabled", true);
            var entity = {
                Title: $(".txt-title").val(),
                ContactName: $(".txt-name").val(),
                MobilePhone: $(".txt-phone").val(),
                Type: $(".dropdown-list").val(),
                FilePath: $("#feed-images li img").attr("src"),
                Remark: $(".txt-description").val()
            };
            Global.post("/FeedBack/InsertFeedBack", { entity: JSON.stringify(entity) }, function (data) {
                _this.val("提交");
                _this.attr("disabled", false);
                if (data.Result == 1) {
                    alert("谢谢反馈");
                    setTimeout(function () { history.back(-1); }, 3000);
                }
                else {
                    alert("服务器繁忙");
                }
            });
      })

    }

    module.exports = ObjectJS;
  

})