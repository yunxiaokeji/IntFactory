

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        doT = require("dot");

    var FeedBack = {};
   
    Params = {
        pageIndex: 1,
        type: -1,
        status: -1,
        beginDate: '',
        endDate:'',
        keyWords: '',
        id:''
    };

    //列表初始化
    FeedBack.init = function () {
        FeedBack.bindEvent();
        FeedBack.getList();
    };

    //绑定事件
    FeedBack.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (Params.keyWords != keyWords) {
                    Params.pageIndex = 1;
                    Params.keyWords = keyWords;
                    FeedBack.getList();
                }
            });
        });

        //下拉状态、类型查询
        require.async("dropdown", function () {
            var Types = [
                {
                    ID: "1",
                    Name: "问题"
                },
                {
                    ID: "2",
                    Name: "建议"
                },
                {
                    ID: "3",
                    Name: "需求"
                }
            ];
            $("#FeedTypes").dropdown({
                prevText: "意见类型-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Types,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.type = parseInt(data.value);
                    FeedBack.getList();
                }
            });

            var Status = [
                {
                    ID: "1",
                    Name: "待解决"
                },
                {
                    ID: "2",
                    Name: "已解决"
                },
                {
                    ID: "3",
                    Name: "驳回"
                },
                {
                    ID: "4",
                    Name: "删除"
                }
            ];
            $("#FeedStatus").dropdown({
                prevText: "意见状态-",
                defaultText: "所有",
                defaultValue: "-1",
                data: Status,
                dataValue: "ID",
                dataText: "Name",
                width: "120",
                onChange: function (data) {
                    Params.pageIndex = 1;
                    Params.status = parseInt(data.value);
                    FeedBack.getList();
                }
            });



        });

        //时间段查询
        $("#SearchFeedBacks").click(function () {
            Params.pageIndex = 1;
            Params.beginDate = $("#BeginTime").val();
            Params.endDate = $("#EndTime").val();
            FeedBack.getList();
        });

    };

    //绑定数据列表
    FeedBack.getList = function () {
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='data-loading'><div></td></tr>");

        Global.post("/FeedBack/GetFeedBacks", Params, function (data) {
            $(".tr-header").nextAll().remove();

            doT.exec("template/feedback/FeedBack-list.html?3", function (templateFun) {
                var innerText = templateFun(data.items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

            if (data.items.length == 0) {
                $(".tr-header").after("<tr><td colspan='7'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }

            $("#pager").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    Params.pageIndex = page;
                    FeedBack.getList();
                }
            });

        });
    }

    FeedBack.detailInit = function (id) {
        Params.id = id;

        FeedBack.detailBindEvent();

        FeedBack.getDetail();
    }

    FeedBack.detailBindEvent = function () {
        $("#btn-finish").click(function () {
            FeedBack.updateFeedBackStatus(2);
        });

        $("#btn-cancel").click(function () {
            FeedBack.updateFeedBackStatus(3);
        });

        $("#btn-delete").click(function () {
            FeedBack.updateFeedBackStatus(9);
        });
    }

    //详情
    FeedBack.getDetail = function () {
        Global.post("/FeedBack/GetFeedBackDetail", { id: Params.id }, function (data) {
            if (data.item) {
                var item = data.item;

                $("#Title").html(item.Title);
                var typeName = "问题";
                if (item.Type == 2)
                    typeName = "建议";
                else if (item.Type == 3)
                    typeName = "需求";
                $("#Type").html(typeName);

                var statusName = "待解决";
                if (item.Status == 2) {
                    statusName = "已解决";
                }
                else if (item.Status == 3)
                    statusName = "驳回";
                else if (item.Status == 9)
                    statusName = "删除";
                $("#Status").html(statusName);

                $("#ContactName").html(item.ContactName);
                $("#MobilePhone").html(item.MobilePhone);
                $("#Remark").html(item.Remark);
                $("#CreateTime").html(item.CreateTime.toDate("yyyy-MM-dd hh:mm:ss"));
            } 
        });
    };

    //更改状态
    FeedBack.updateFeedBackStatus = function (status) {
        Global.post("/FeedBack/UpdateFeedBackStatus", { id: Params.id, status: status }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
                FeedBack.getFeedBackDetail();
            }
            else {
                alert("保存失败");
            }
        });
    };

    module.exports = FeedBack;
});