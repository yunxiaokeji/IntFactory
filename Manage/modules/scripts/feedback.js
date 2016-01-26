

define(function (require, exports, module) {

    require("jquery");
    require("pager");
    var Global = require("global"),
        doT = require("dot");

    var FeedBack = {};
   
    FeedBack.Params = {
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
        FeedBack.bindData();
    };

    //绑定事件
    FeedBack.bindEvent = function () {
        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                if (FeedBack.Params.keyWords != keyWords) {
                    FeedBack.Params.pageIndex = 1;
                    FeedBack.Params.keyWords = keyWords;
                    FeedBack.bindData();
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
                    FeedBack.Params.pageIndex = 1;
                    FeedBack.Params.type = parseInt(data.value);
                    FeedBack.bindData();
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
                    FeedBack.Params.pageIndex = 1;
                    FeedBack.Params.status = parseInt(data.value);
                    FeedBack.bindData();
                }
            });



        });

        //时间段查询
        $("#SearchFeedBacks").click(function () {
            if ($("#BeginTime").val() != '' || $("#EndTime").val() != '') {
                FeedBack.Params.pageIndex = 1;
                FeedBack.Params.beginDate = $("#BeginTime").val();
                FeedBack.Params.endDate = $("#EndTime").val();
                FeedBack.bindData();
            }
        });

    };

    //绑定数据列表
    FeedBack.bindData = function () {
        $(".tr-header").nextAll().remove();

        Global.post("/FeedBack/GetFeedBacks", FeedBack.Params, function (data) {
            doT.exec("template/FeedBack-list.html?3", function (templateFun) {
                var innerText = templateFun(data.Items);
                innerText = $(innerText);
                $(".tr-header").after(innerText);
            });

            $("#pager").paginate({
                total_count: data.TotalCount,
                count: data.PageCount,
                start: FeedBack.Params.pageIndex,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                onChange: function (page) {
                    FeedBack.Params.pageIndex = page;
                    FeedBack.bindData();
                }
            });

        });
    }

    FeedBack.detailInit = function (id) {
        FeedBack.Params.id = id;

        FeedBack.detailBindEvent();
        FeedBack.getFeedBackDetail();
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
    FeedBack.getFeedBackDetail = function () {
        Global.post("/FeedBack/GetFeedBackDetail", { id: FeedBack.Params.id }, function (data) {
            if (data.Item) {
                var item = data.Item;

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
        Global.post("/FeedBack/UpdateFeedBackStatus", { id: FeedBack.Params.id, status: status }, function (data) {
            if (data.Result == 1) {
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