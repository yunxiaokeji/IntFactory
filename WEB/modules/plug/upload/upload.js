
define(function (require, exports, module) {

    require("plug/upload/jquery.form.js");
    var Defaults = {
        element: "#",		        //元素ID
        buttonText: "上传",         //按钮文本
        url: "/Plug/UploadFile",	//文件临时存储路径
        data: {},
        className: "ico-upload",
        multiple: false,//是否支持附件多选,
        fileType: 1,//附件类型 1:图片；2：文件；3图片和文件
        maxSize: 5 * 1024,//附件最大大小
        maxQuantity: 10,//最大上传文件个数
        beforeSubmit: function () { },
        error: function () { },
        success: function () { },		//上传成功
    };

    var UPLoad = function (options) {
        var _self = this;
        _self.setting = $.extend({}, Defaults, options);
        _self.init();
    };
    UPLoad.prototype.init = function () {
        var _self = this;
 
        if (_self.setting.element) {
            var form = $('<form id="' + _self.setting.element + '_postForm" enctype="multipart/form-data"></form>'),
                file = $('<input type="file" data-filequantity="0" accept="' + _self.setting.fileType + '" name="file" id="' + _self.setting.element + '_fileUpLoad" ' + (_self.setting.multiple ? 'multiple="multiple"' : '') + ' style="display:none;" />'),
                button = $('<input id="' + _self.setting.element + '_buttonSubmit" class="' + (_self.setting.className || "ico-upload") + '" type="button" value="' + _self.setting.buttonText + '" />')
            form.append(file).append(button);

            $(_self.setting.element).append(form);

            form.submit(function () {
                var options = {
                    target: _self.setting.target,
                    url: _self.setting.url,
                    type: "post",
                    data: _self.setting.data,
                    beforeSubmit: null,
                    error: _self.setting.error(),
                    success: _self.setting.success
                };
                $(this).ajaxSubmit(options);
                return false;
            })
            button.click(function () {
                file.click();
            });

            file.change(function () {
                var target = this;
                var _file = $(this);
                var files = target.files;
                if (files.length < 1) {
                    return false;
                }
                if ((files.length + parseInt(_file.data("filequantity"))) > _self.setting.maxQuantity) {
                    alert("上传文件最多10个");
                    return false;
                }
                var isIE = /msie/i.test(navigator.userAgent) && !window.opera;
                var pictypes = ["jpg", "png", "jpeg", "x-png", "x-tiff", "x-pjpeg"];
                var filetypes = ["rar", "txt", "zip", "doc", "ppt", "xls", "pdf", "docx", "xlsx"];
                var attachmenttypes=pictypes;
                var maxSize = _self.setting.maxSize;//2M 

                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    var filepath = file.name;
                    var fileSize = 0;
                    var isContinue = true;
                    var fileend = filepath.substring(filepath.lastIndexOf(".") + 1);

                    if (_self.setting.fileType == 2) {
                        attachmenttypes = filetypes;
                    }
                    else if (_self.setting.fileType == 3) {
                        attachmenttypes = attachmenttypes.concat(filetypes);
                    }
                    for (var k = 0; k < attachmenttypes.length; k++) {
                        if (attachmenttypes[i] == fileend) {
                            isContinue = true;
                            break;
                        }
                    }
                    if (!isContinue) {
                        alert("含有不支持的文件格式！");
                        target.value = "";
                        return false;
                    }

                    if (isIE && !files) {
                        var filePath = target.value;
                        var fileSystem = new ActiveXObject("Scripting.FileSystemObject");
                        if (!fileSystem.FileExists(filePath)) {
                            alert("附件不存在，请重新上传！");
                            target.value = "";
                            return false;
                        }
                        var file = fileSystem.GetFile(filePath);
                        fileSize = file.Size;
                    }
                    else
                    {
                        fileSize = file.size;
                    }

                    var fileSize = fileSize / 1024;
                    if (fileSize > maxSize) {
                        alert("附件大小不能大于" + maxSize / 1024 + "M！");
                        target.value = "";
                        return false;
                    }
                    if (fileSize <= 0) {
                        alert("附件大小不能为0M！");
                        target.value = "";
                        return false;
                    }
                }

                _file.data("filequantity", parseInt(_file.data("filequantity")) + files.length);
                form.submit();
            })
        }
    };

    exports.createUpload = function (options) {
        return new UPLoad(options);
    }
});
