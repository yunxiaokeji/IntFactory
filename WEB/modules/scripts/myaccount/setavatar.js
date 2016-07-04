define(function (require, exports, module) {
    var Global = require("global"),
        Upload = require("upload"),
        Verify = require("verify"), VerifyObject;

    var ObjectJS = {};

    //初始化
    ObjectJS.init = function () {
        var _self = this;
        _self.bindEvent();
    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        //tab切换
        $(".search-stages li").click(function () {
            var _this = $(this);
            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $(".content-body div[name='accountInfo']").hide().eq( parseInt(_this.data("id")) ).show();
        });


        //头像设置
        var options =
	    {
	        thumbBox: '.thumbBox',
	        spinner: '.spinner',
	        imgSrc: ''
	    }

        var cropper = $('.imageBox').cropbox(options);

        $('#upload-file').on('change', function () {
            var reader = new FileReader();
            reader.onload = function (e) {
                options.imgSrc = e.target.result;
                cropper = $('.imageBox').cropbox(options);
            }
            reader.readAsDataURL(this.files[0]);
            this.files = [];
        })

        $('#btnZoomIn').on('click', function () {
            cropper.zoomIn();
        })

        $('#btnZoomOut').on('click', function () {
            cropper.zoomOut();
        })
        
        $('#btnCrop').on('click', function () {
            if ($(".imageBox").css("background-image") == "none") return;

            var img = cropper.getDataURL();
            $('.cropped').html('').show();
            $('.cropped').append('<img src="' + img + '" align="absmiddle" style="width:64px;margin-top:4px;border-radius:64px;box-shadow:0px 0px 12px #7E7E7E;" ><p>64px*64px</p>');
            $('.cropped').append('<img src="' + img + '" align="absmiddle" style="width:128px;margin-top:4px;border-radius:128px;box-shadow:0px 0px 12px #7E7E7E;"><p>128px*128px</p>');
            $('.cropped').append('<img src="' + img + '" align="absmiddle" style="width:160px;margin-top:4px;border-radius:160px;box-shadow:0px 0px 12px #7E7E7E;"><p>160px*160px</p>');



            Global.post("/MyAccount/SaveAccountAvatar", { avatar: img }, function (data) {

                if (data.result == 1) {
                    $("#currentUser .avatar").attr("src", data.avatar + "?t=" + new Date().toLocaleString());
                }
                else {
                    alert("保存失败");
                }
            });
        });
    }

    module.exports = ObjectJS;
});