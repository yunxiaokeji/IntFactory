define(function (require, exports, module) {
    var Global = require("global"),
        Dot = require("dot"), editor;
    var Upload = require("upload");

    var ObjectJS = {};

    ObjectJS.TypeID = "";

    ObjectJS.init = function (Editor, model,list) {
        var _self = this;
        editor = Editor;
        model = JSON.parse(model.replace(/&quot;/g, '"'));
        list = JSON.parse(list.replace(/&quot;/g, '"'));

        ObjectJS.bindEvent(model,list);
    };

    ObjectJS.bindEvent = function (model, list) {        
        $("#selector .item .check-lump[data-id=" + model.ModuleType + "]").addClass("hover");
        
        $(".title").val(model.Title);
        $(".sort").val(model.Sort);
        $(".keywords").val(model.KeyWords);
        var img = model.MainImg == "" ? "/modules/images/img-noimg.png" : model.MainImg;
        $("#cateGoryImages").html("<li><img style='width:60px;height:60px;' src='" + img + "?imageView2/1/w/60/h/60' data-src=" + img + "></li>");
        editor.ready(function () {
            editor.setContent(decodeURI(model.Detail));
        });
                
        $(".update-details").click(function () {
            ObjectJS.updateContent(model.ContentID);
        });

        $("#selector .item .check-lump").click(function () {            
            var _this = $(this), id = _this.data("id");
            if (!_this.hasClass("hover")) {
                $("#selector .item .check-lump").removeClass("hover");
                _this.addClass("hover");

                Global.post("/HelpCenter/GetTypesByModuleType", { type: id }, function (data) {
                    if (data.items.length > 0) {
                        ObjectJS.cateGoryDropDown(data.items, model, 1);
                    } else {
                        alert("网络延迟，请重试");
                    }
                });
            };
        });

        var uploader = Upload.uploader({
            browse_button: 'uploadImg',
            picture_container: "cateGoryImages",
            successItems: "#cateGoryImages li",
            //image_view: "?imageView2/1/w/60/h/60",
            file_path: "/Content/UploadFiles/HelpCenter/",
            maxSize: 5,
            fileType: 1,
            multi_selection: false,
            init: {}
        });
        
        ObjectJS.cateGoryDropDown(list, model, 2);
        
    };

    ObjectJS.updateContent = function (id) {   
        var title = $(".title").val();
        var sort = $(".sort").val();
        var keywords = $(".keywords").val();
        var mainImg=$("#cateGoryImages li img").data("src");        
        var content = encodeURI(editor.getContent());
        
        Global.post("/HelpCenter/UpdateContent", {
            id: id,
            title: title,
            sort: sort,
            keyWords: keywords,
            mainImg:mainImg,
            content: content,
            typeID: ObjectJS.TypeID
        }, function (e) {
            if (e.status) {                                            
                alert("修改成功");
                window.location = "/HelpCenter/Contents";
            } else {
                alert("修改失败");
            }
        });
    }
    
    ObjectJS.cateGoryDropDown = function (item,model,bl) {
        $(".dropdown").empty();
        $(".dropdown").append('<div id="category_Down" style="margin-left:83px;"></div>');
        require.async("dropdown", function () {
            var types = [];
            for (var i = 0; i < item.length; i++) {
                types.push({
                    ID: item[i].TypeID,
                    Name: item[i].Name
                })
            }            
            if (bl == 1) {
                ObjectJS.TypeID = types[0].ID;
            };
            $(".dropdown #category_Down").dropdown({
                prevText: "分类-",
                defaultText: types[0].Name,
                defaultValue: types[0].ID,
                data: types,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    if (ObjectJS.TypeID != data.value) {
                        ObjectJS.TypeID = data.value;
                    }
                }
            });
            if (bl == 2) {
                ObjectJS.TypeID = model.TypeID;
                $("#category_Down .dropdown-text").html("分类-" + model.TypeName);
            }
        });
    }
        
    module.exports = ObjectJS;
});