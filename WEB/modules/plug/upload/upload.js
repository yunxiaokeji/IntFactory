
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
        successItems:'',
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
                file = $('<input type="file" data-filequantity="0" class="'+_self.setting.element + '_fileUpLoad" accept="' + _self.setting.fileType + '" name="file" id="' + _self.setting.element + '_fileUpLoad" ' + (_self.setting.multiple ? 'multiple="multiple"' : '') + ' style="display:none;" />'),
                button = $('<input id="' + _self.setting.element + '_buttonSubmit" class="' + (_self.setting.className || "ico-upload") + '" type="button" value="' + _self.setting.buttonText + '" />')
            form.append(file).append(button);
            $("#"+_self.setting.element).append(form);

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
                var files = target.files;
                if (files.length < 1) {
                    return false;
                }
                if (_self.setting.successItems != '') {
                    if ($(_self.setting.successItems).length + files.length > _self.setting.maxQuantity) {
                        alert("上传文件最多" + _self.setting.maxQuantity + "个", 2);
                        return false;
                    }
                }
                if (files.length > _self.setting.maxQuantity) {
                    alert("上传文件最多" + _self.setting.maxQuantity + "个", 2);
                    return false;
                }

                var isIE = /msie/i.test(navigator.userAgent) && !window.opera;
                var pictypes = ["jpg", "png", "jpeg", "x-png", "x-tiff", "x-pjpeg"];
                var filetypes = ["rar", "txt", "zip", "doc", "ppt", "xls", "pdf", "docx", "xlsx", "prj", "emf"];
                var attachmenttypes=pictypes;
                var maxSize = _self.setting.maxSize;//2M 
                var isContinue = false;

                for (var i = 0; i < files.length; i++) {
                    var file = files[i];
                    var filepath = file.name;
                    var fileSize = 0;
                    isContinue = false;
                    var fileExtension = filepath.substring(filepath.lastIndexOf(".") + 1).toLowerCase();

                    if (_self.setting.fileType == 2) {
                        attachmenttypes = filetypes;
                    }
                    else if (_self.setting.fileType == 3) {
                        attachmenttypes = attachmenttypes.concat(filetypes);
                    }
                    for (var k = 0; k < attachmenttypes.length; k++) {
                        if (attachmenttypes[k] == fileExtension) {
                            isContinue = true;
                            break;
                        }
                    }
                    if (!isContinue) {
                        alert("含有不支持的文件格式！", 2);
                        target.value = "";
                        return false;
                    }

                    if (isIE && !files) {
                        var filePath = target.value;
                        var fileSystem = new ActiveXObject("Scripting.FileSystemObject");
                        if (!fileSystem.FileExists(filePath)) {
                            alert("附件不存在，请重新上传！", 2);
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
                        alert("附件大小不能大于" + maxSize / 1024 + "M！", 2);
                        target.value = "";
                        return false;
                    }
                    if (fileSize <= 0) {
                        alert("附件大小不能为0M！", 2);
                        target.value = "";
                        return false;
                    }
                }

                form.submit();
            })
        }
    };

    exports.createUpload = function (options) {
        return new UPLoad(options);
    }
});
