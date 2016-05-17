define(function (require,exports,module) {

    require("pager");

    var doT = require("dot");
    
    var ObjectJS = {};

    var Params = {
        pageIndex: 1,
        beginTime: "",
        endTime: "",
        type: "-1",
        keyWords: "",
        isAsc:0
    };

    ObjectJS.init = function () {

        ObjectJS.getFeedBackList();
        ObjectJS.bindEvent();

    }

    //绑定事件
    ObjectJS.bindEvent = function () {

        $(".search-feedbacktype .item").click(function () {
            var _this = $(this);
            
            if (!_this.hasClass('hover'))
            {
                _this.siblings().removeClass("hover");
                _this.addClass('hover');
                Params.type = _this.data('type');
                ObjectJS.getFeedBackList();
            }
        })
        
        $("#btnSearch").click(function () {
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            ObjectJS.getFeedBackList();
        })

        $(".sort-item").click(function () {
            var _this=$(this);
            if (_this.data('isasc') == 0) {
                Params.isAsc = 1;
                _this.data('isasc', 1);
                _this.find(".desc").removeClass("hover");
                _this.find(".asc").addClass("hover");
            }
            else {
                Params.isAsc = 0;
                _this.data('isasc', 0);
                _this.find(".asc").removeClass("hover");
                _this.find(".desc").addClass("hover");
            }
            ObjectJS.getFeedBackList();
        })

        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                Params.pageIndex = 1;
                Params.keyWords = keyWords;
                ObjectJS.getFeedBackList();
            });
        });

        $("#pager").paginate({
            total_count: 12,
            count: 12,
            start: Params.pageIndex,
            display: 5,
            images: false,
            mouse: 'slide',
            onChange: function (page) {
                $(".tr-header").nextAll().remove();
                Params.pageIndex = page;
                ObjectJS.getFeedBackList();
            }
        });

    }

    //获取反馈列表
    ObjectJS.getFeedBackList = function () {

        $(".tr-header").after("<tr><td colspan='10'><div class='data-loading'><div></td></tr>");

        doT.exec("template/myaccount/feedbacklist.html", function (template) {

            var innerhtml = template();

            innerhtml = $(innerhtml);

            $(".table-list .tr-header").nextAll().remove();

            $(".table-list").append(innerhtml);

        });

    }

    module.exports = ObjectJS;

})