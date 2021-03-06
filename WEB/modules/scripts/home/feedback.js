﻿define(function (require,exports,module) {
    var Global = require('global');
    var Upload = require('upload');
    var ObjectJS = {};
    
    ObjectJS.init = function () {
        ObjectJS.bindEvent();
    }

    ObjectJS.bindEvent = function () {
        var upload = Upload.uploader({
            browse_button: 'upload',        
            file_path: "/Content/UploadFiles/FeedBack/",
            successItems: '#feed-images li',
            picture_container: 'feed-images',
            maxSize: 5,
            fileType: 1,
            multi_selection: false,
            init: {}
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

            var imgs = '';
            $("#feed-images li").each(function(){
                imgs += $(this).data("server") + $(this).data("filename") + ",";
            });

            var entity = {
                Title: $(".txt-title").val(),
                ContactName: $(".txt-name").val(),
                MobilePhone: $(".txt-phone").val(),
                Type: $(".dropdown-list").val(),
                FilePath: imgs,
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