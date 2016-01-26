//活动模块
//MU
//2015-11-29

define(function (require, exports, module) {
    var Global = require("global"),
        doT = require("dot"),
        Verify = require("verify"), VerifyObject,
        Upload = require("upload"), PosterIco, editor,
        Easydialog = require("easydialog"),
        ChooseUser = require("chooseuser");
        require("pager");

    var Model = {};

    var ObjectJS = {};

    ObjectJS.Params = {
        PageSize: 10,
        PageIndex:1,
        KeyWords: "",
        IsAll: 0,//0：我的活动；1：所有活动
        Stage:2,
        BeginTime: "",
        EndTime: "",
        FilterType: 0,
        UserID:'',
        DisplayType:1//1：列表；2：卡片
    };

    ////初始化 列表
    ObjectJS.init = function (isAll) {
        var _self = this;
        _self.Params.IsAll = isAll;

        _self.bindEvent();
        
        _self.getList();

        if (isAll==1)
        {
            $(".header-title").html("所有活动");
            document.title = "所有活动";
        }
    }

    //绑定事件
    ObjectJS.bindEvent = function () {
        var _self = this;
        
        //时间段查询
        $("#SearchActivity").click(function () {
            if ($("#BeginTime").val() != "" || $("#EndTime").val() != "") {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.BeginTime = $("#BeginTime").val();
                ObjectJS.Params.EndTime = $("#EndTime").val();
                ObjectJS.getList();
            }
        });

        //关键字查询
        require.async("search", function () {
            $(".searth-module").searchKeys(function (keyWords) {
                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.KeyWords = keyWords;
                ObjectJS.getList();
            });
        });
 

        if (_self.Params.IsAll == 0) {
            //搜索
            require.async("dropdown", function () {
                var Types = [
                    {
                        ID: "1",
                        Name: "我负责的"
                    },
                    {
                        ID: "2",
                        Name: "我参与的"
                    }
                ];
                $("#ActivityType").dropdown({
                    prevText: "活动过滤-",
                    defaultText: "所有",
                    defaultValue: "-1",
                    data: Types,
                    dataValue: "ID",
                    dataText: "Name",
                    width: "120",
                    onChange: function (data) {
                        ObjectJS.Params.PageIndex = 1;
                        ObjectJS.Params.FilterType = data.value;
                        ObjectJS.getList();
                    }
                });

            });
        }
        else
        {
            require.async("choosebranch", function () {
                $("#chooseBranch").chooseBranch({
                    prevText: "人员-",
                    defaultText: "全部",
                    defaultValue: "",
                    userid: "-1",
                    isTeam: false,
                    width: "180",
                    onChange: function (data) {


                        ObjectJS.Params.PageIndex = 1;
                        ObjectJS.Params.UserID = data.userid;
                        ObjectJS.getList();

                        if (!$("#ActivityType").is(":visible")) {
                            //搜索
                            require.async("dropdown", function () {
                                var Types = [
                                    {
                                        ID: "1",
                                        Name: "我负责的"
                                    },
                                    {
                                        ID: "2",
                                        Name: "我参与的"
                                    }
                                ];
                                $("#ActivityType").dropdown({
                                    prevText: "活动过滤-",
                                    defaultText: "所有",
                                    defaultValue: "-1",
                                    data: Types,
                                    dataValue: "ID",
                                    dataText: "Name",
                                    width: "120",
                                    onChange: function (data) {
                                        ObjectJS.Params.PageIndex = 1;
                                        ObjectJS.Params.FilterType = data.value;
                                        ObjectJS.getList();
                                    }
                                });

                                $("#ActivityType").addClass("mLeft20");

                            });
                        }


                    }
                });
            });
        }

        //删除活动
        $("#deleteObject").click(function () {
            var _this = $(this);
            confirm("确认删除活动吗?", function () {
                Global.post("/Activity/DeleteActivity", { activityID: _this.data("id") }, function (data) {
                    if (data.Result == 1) {
                        if (ObjectJS.Params.IsAll == 0)
                            location.href = "/Activity/MyActivity";
                        else
                            location.href = "/Activity/Activitys";
                    }
                    else {
                        alert("删除失败");
                    }
                });
            });
        });

        //编辑活动
        $("#setObjectRole").click(function () {
            var _this = $(this);
            location.href = "/Activity/Operate/" + _this.data("id");
        });

        //显示模式切换
        $(".displayTab").click(function () {
            var type = parseInt($(this).data("type"));
            ObjectJS.Params.DisplayType = type;

            if (type == 1) 
            {
                $(this).find("img").attr("src", "/modules/images/ico-list-blue.png");
                $(this).next().find("img").attr("src", "/modules/images/ico-card-gray.png");
                $(".activityList").html('');

                $(".activityList").show();
                $(".activityCardList").hide();
            }
            else
            {
                $(this).find("img").attr("src", "/modules/images/ico-card-blue.png");
                $(this).prev().find("img").attr("src", "/modules/images/ico-list-gray.png");
                $(".activityCardList").html('');

                $(".activityList").hide();
                $(".activityCardList").show();
            }

            ObjectJS.getList();
        });

        //切换阶段
        $(".search-stages li").click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");

                ObjectJS.Params.PageIndex = 1;
                ObjectJS.Params.Stage = _this.data("id");
                ObjectJS.getList();
            }
        });

        //隐藏下拉
        $(document).click(function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("dropdown-ul") && !$(e.target).parents().hasClass("dropdown") && !$(e.target).hasClass("dropdown"))
            {
                $(".dropdown-ul").hide();
            }
        });
}

    //获取列表
    ObjectJS.getList = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();

        if (ObjectJS.Params.DisplayType == 1)
        {
            $(".activityList").html("<tr><td><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        }
        else {
            $(".activityCardList").html("<li ><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/></div></li>");
        }

        Global.post("/Activity/GetActivityList",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                keyWords: ObjectJS.Params.KeyWords,
                isAll: ObjectJS.Params.IsAll,
                beginTime: ObjectJS.Params.BeginTime,
                endTime: ObjectJS.Params.EndTime,
                stage: ObjectJS.Params.Stage,
                filterType: ObjectJS.Params.FilterType,
                userID: ObjectJS.Params.UserID
            },
            function (data) {
                _self.bindList(data.Items);

                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        _self.Params.PageIndex = page;
                        _self.getList();
                    }
                });
            }
        );
    }

    //加载列表
    ObjectJS.bindList = function (items) {
        if (items.length > 0)
        {
            if (ObjectJS.Params.DisplayType == 1) 
            {
                doT.exec("template/activity/activity_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 39 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });

                });
            }
            else
            {
                doT.exec("template/activity/activity_card_list.html", function (template) {
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);
                    var innerhtml = template(items);
                    innerhtml = $(innerhtml);

                    //操作
                    innerhtml.find(".dropdown").click(function () {
                        var _this = $(this);
                        var position = _this.find(".ico-dropdown-white").position();
                        $(".dropdown-ul li").data("id", _this.data("id"));

                        $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 30 }).show().mouseleave(function () {
                            $(this).hide();
                        });
                    });

                    $(".activityCardList").html(innerhtml);

                    require.async("businesscard", function () {
                        $(".activitymember").businessCard();
                    });
                });
            }
        }
        else
        {
            if (ObjectJS.Params.DisplayType == 1)
            {
                $(".activityList").html("<tr><td><div class='noDataTxt'>暂无数据!<div></td></tr>");
            }
            else 
            {
                $(".activityCardList").html("<li ><div class='noDataTxt'>暂无数据!</div></li>");
            }
        }
    }



    ////初始化操作 编辑、新增
    ObjectJS.initOperate = function (Editor, id) {
        var _self = this;
        editor = Editor;

        _self.bindOperateEvent();

        VerifyObject = Verify.createVerify({
            element: ".verify",
            emptyAttr: "data-empty",
            verifyType: "data-type",
            regText: "data-text"
        });

        if (id) {
            $(".header-title").html("编辑活动");
            _self.getDetail(1);
        }
        else
            ObjectJS.getUserDetail();

        setTimeout(function () { $(".edui-container").css("z-index", "600") }, 1000);
            
    }

    //绑定事件
    ObjectJS.bindOperateEvent = function () {
        var _self = this;

        //选择海报图片
        PosterIco = Upload.createUpload({
            element: "#Poster",
            buttonText: "选择活动海报图片",
            className: "",
            data: { folder: '/Content/tempfile/', action: 'add', oldPath: "" },
            success: function (data, status) {
                if (data.Items.length > 0)
                {
                    _self.IcoPath = data.Items[0];
                    $("#PosterDisImg").show().attr("src", data.Items[0]);
                    $("#PosterImg").val(data.Items[0]);
                }
            }
        });

        //添加负责人
        $("#addOwner").click(function () {
            ChooseUser.create({
                title: "添加负责人",
                type: 1,
                single: true,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "OwnerIDs",true);
                    }
                }
            });
        });

        //添加成员
        $("#addMember").click(function () {
            ChooseUser.create({
                title: "添加成员",
                type: 1,
                single: false,
                callback: function (items) {
                    for (var i = 0; i < items.length; i++) {
                        _self.createMember(items[i], "MemberIDs",false);
                    }

                }
            });
        });

        //保存活动
        $("#btnSaveActivity").click(function () {
            if (!VerifyObject.isPass()) {
                return;
            };

            var OwnerID='', MemberID='';
            $("#OwnerIDs .member").each(function () {
                OwnerID += $(this).data("id")+"|";
            });
            $("#MemberIDs .member").each(function () {
                MemberID += $(this).data("id") + "|";
            });
            if (OwnerID == '' || MemberID=='')
            {
                alert("请选择负责人和成员");
                return;
            }

            var BeginTime = '', EndTime = '';
            BeginTime=$("#BeginTime").val();
            EndTime=$("#EndTime").val();
            if (BeginTime == '' || EndTime == '')
            {
                alert("请选择开始时间和结束时间");
                return;
            }

            var model = {
                ActivityID: $("#ActivityID").val(),
                Name: $("#Name").val(),
                Poster: $("#PosterImg").val(),
                OwnerID: OwnerID,
                MemberID: MemberID,
                BeginTime: BeginTime,
                EndTime: EndTime,
                Address: $("#Address").val(),
                Remark: encodeURI(editor.getContent())
            };

            _self.saveModel(model);
        });
        
    }

    //获取详情
    ObjectJS.getDetail = function (option)//option=1 编辑页；option=2 详情页
    {
        var _self = this;
        Global.post("/Activity/GetActivityDetail",
            { activityID: $("#ActivityID").val() },
            function (data) {
                if (data.Item) {
                    var item = data.Item;

                    if (option == 1) {
                        $("#Name").val(item.Name);
                        $("#EndTime").val(item.EndTime.toDate("yyyy-MM-dd"));
                        $("#BeginTime").val(item.BeginTime.toDate("yyyy-MM-dd"));
                        $("#Address").val(item.Address);
                        $("#PosterImg").val(item.Poster);
                        if (item.Poster != '')
                            $("#PosterDisImg").attr("src", item.Poster).show();

                        ObjectJS.createMemberDetail(item.Owner, "OwnerIDs");
                        for (var i = 0; i < item.Members.length; i++)
                        {
                            ObjectJS.createMemberDetail(item.Members[i], "MemberIDs");
                        }

                        editor.ready(function () {
                            editor.setContent(decodeURI(item.Remark));
                        });
  
                    }
                    else
                    {
                        $("#Name").html(item.Name);
                        $("#Name").attr("src", "/Activity/Detail/" + item.ActivityID);
                        if (item.Owner) {
                            $("#OwnerName").html(item.Owner.Name);
                            $("#OwnerName").data("id", item.Owner.UserID);
                        }
                        $("#EndTime").html(item.EndTime.toDate("yyyy-MM-dd"));
                        $("#BeginTime").html(item.BeginTime.toDate("yyyy-MM-dd"));
                        $("#Address").html(item.Address);
                        $("#Remark").html(decodeURI(item.Remark));

                        if (item.Poster != '')
                            $("#Poster").attr("src", item.Poster);

                        if (item.Members)
                        {
                            for (var i = 0; i < item.Members.length; i++) {
                                var m = item.Members[i];
                                $("#MemberList").append("<li class='member' data-id='" + m.UserID + "'>" + m.Name + "</li>");
                            }
                        }

                        if ($("#MDUserID").val() != "")
                        {
                            $("#tr_shareMD").show();
                            require.async("sharemingdao", function () {
                                $("#btn_shareMD").sharemingdao({
                                    post_pars: {
                                        content: $("#Name").html(),
                                        groups: [],
                                        share_type: 0
                                    },
                                    task_pars: {
                                        name: $("#Name").html(),
                                        end_date: $("#EndTime").html(),
                                        charger: item.Owner,
                                        members: item.Members,
                                        des: '',
                                        url: "/Activity/Detail?id=" + $("#ActivityID").val() + "&source=md"
                                    },
                                    schedule_pars: {
                                        name: $("#Name").html(),
                                        start_date: $("#BeginTime").html(),
                                        end_date: $("#EndTime").html(),
                                        members: item.Members,
                                        address: $("#Address").html(),
                                        des: '',
                                        url: "/Activity/Detail?id=" + $("#ActivityID").val() + "&source=md"
                                    },
                                    callback: function (type, url) {
                                        if (type == "Calendar") {
                                            url = "<a href='" + url + "' target='_blank'>分享明道日程，点击查看详情</a>";
                                        } else if (type == "Task") {
                                            url = "<a href='" + url + "' target='_blank'>分享明道任务，点击查看详情</a>";
                                        }
                                        var entity = {
                                            ActivityID: $("#ActivityID").val(),
                                            Msg: encodeURI(url),
                                            FromReplyID: "",
                                            FromReplyUserID: "",
                                            FromReplyAgentID: ""
                                        };

                                        ObjectJS.SaveActivityReply(entity);
                                    }
                                });
                            });
                        }

                    }

                    $("#OwnerID").val(item.OwnerID);
                    $("#MemberID").val(item.MemberID);

                    require.async("businesscard", function () {
                        $(".member").businessCard();
                    });


                }
            });
    }

    //获取当前用户实体
    ObjectJS.getUserDetail = function () {
        Global.post("/Activity/GetUserDetail", null, function (data) {
            ObjectJS.createMemberDetail(data.Item, "OwnerIDs");
        });
    }

    //拼接一个用户成员
    ObjectJS.createMember = function (item, id, isSingle) {
        if ($("#" + id + " div[data-id='" + item.id + "']").html())
            return false;

        var html = '<div class="member left" data-id="' + item.id + '">';
        html += '    <div class="left pRight5">';
        html += '          <span>' + item.name + '</span>';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        if (isSingle)
            $("#" + id).html(html);
        else
            $("#" + id).append(html);

        require.async("businesscard", function () {
            $("div.member").businessCard();
        });
    }

    //拼接一个用户成员
    ObjectJS.createMemberDetail = function (item, id) {
        if (item.Avatar == '')
            item.Avatar = "/modules/images/defaultavatar.png";

        var html = '<div class="member left" data-id="' + item.UserID + '">';
        html += '    <div class="left pRight5">';
        html += '          <span>' + item.Name + '</span>';
        html += '     </div>';
        html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
        html += '      <div class="clear"></div>';
        html += '   </div>';

        $("#" + id).append(html); 
    }

    //保存实体
    ObjectJS.saveModel = function (model) {
        var _self = this;
        Global.post("/Activity/SavaActivity", { entity: JSON.stringify(model) }, function (data) {
            if (data.ID.length > 0) {
                location.href = "/Activity/MyActivity"
            }
        })
    }


    ////初始化 详情
    ObjectJS.initDetail = function () {
        var _self = this;
        _self.bindDetailEvent();

        _self.getDetail(2);

        ObjectJS.GetActivityReplys();
    }

    //绑定事件
    ObjectJS.bindDetailEvent = function () {
        var _self = this;
      
        $(".tab-nav .tab-nav-ul li").click(function () {
            $(".tab-nav .tab-nav-ul li").removeClass("hover");

            $(this).addClass("hover");
            $("div[name='activityDetail']").hide();
            $("#" + $(this).data("id")).show();

            if ($(this).data("id") == "activityCustoms")
            {
                ObjectJS.getCustomersByActivityID();
            }

        });

        $("#btnSaveActivityReply").click(function () {
            var entity =
           {
               ActivityID: $("#ActivityID").val(),
                Msg: $("#Msg").val(),
                FromReplyID: "",
                FromReplyUserID: "",
                FromReplyAgentID:""
            }

            ObjectJS.SaveActivityReply(entity);
            $("#Msg").val('');
        });
    }

    //保存活动评论
    ObjectJS.SaveActivityReply = function (entity) {
        if (!entity.Msg)
        {
            alert("内容不能为空");
            return false;
        }
        Global.post("/Activity/SavaActivityReply",
            {
                entity: JSON.stringify(entity)
            },
            function (data) {
                if (data.Items.length > 0) {
                    doT.exec("template/activity/activity_reply_list.html", function (template) {
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml);
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml).hide();

                        $("#activityReplyList").prepend(innerhtml);
                        innerhtml.fadeIn(1000);

                        innerhtml.find("div[name='btn_replyByReply']").unbind("click").click(function () {
                            var entity =
                           {
                               ActivityID: $("#ActivityID").val(),
                               Msg: $("#Msg_" + $(this).data("replyid")).val(),
                               FromReplyID: $(this).data("replyid"),
                               FromReplyUserID: $(this).data("createuserid"),
                               FromReplyAgentID: $(this).data("agentid")
                           }
                            ObjectJS.SaveActivityReply(entity);

                            $("#Msg_" + $(this).data("replyid")).val('');
                            $(this).parent().parent().slideUp(100);
                        });

                    });
                }
                else {
                    alert("评论失败");
                }
            });
    }

    //获取活动讨论列表
    ObjectJS.GetActivityReplys = function () {
        var _self = this;
        
        Global.post("/Activity/GetActivityReplys",
            {
                activityID:$("#ActivityID").val(),
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex
            },
            function (data) {
                if (data.Items.length > 0)
                    {
                        doT.exec("template/activity/activity_reply_list.html", function (template) {
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml);

                        $("#activityReplyList").html(innerhtml);
                        $("div[name='btn_replyByReply']").unbind("click").click(function () {
                            var entity =
                           {
                               ActivityID: $("#ActivityID").val(),
                               Msg: $("#Msg_" + $(this).data("replyid")).val(),
                               FromReplyID: $(this).data("replyid"),
                               FromReplyUserID: $(this).data("createuserid"),
                               FromReplyAgentID: $(this).data("agentid")
                           }

                            ObjectJS.SaveActivityReply(entity);
                            $("#Msg_" + $(this).data("replyid")).val('');
                            $(this).parent().parent().slideUp(100);
                        });


                        require.async("businesscard", function () {
                            $(".activitymember").businessCard();
                        });

                    });
                }

            }
        );
    }

    //获取活动对应客户列表
    ObjectJS.getCustomersByActivityID = function () {
        var _self = this;
        $(".tr-header").nextAll().remove();
        $(".tr-header").after("<tr><td colspan='7'><div class='dataLoading' ><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");

        Global.post("/Activity/GetCustomersByActivityID",
            {
                pageSize: ObjectJS.Params.PageSize,
                pageIndex: ObjectJS.Params.PageIndex,
                activityID: $("#ActivityID").val()
            },
            function (data) {
                $(".tr-header").nextAll().remove();

                if (data.Items.length > 0)
                {
                    doT.exec("template/activity/activity_customers.html", function (template) {
                        var innerhtml = template(data.Items);
                        innerhtml = $(innerhtml);

                        $(".tr-header").after(innerhtml);
                    });
                }
                else {
                    $(".tr-header").after("<tr><td colspan='7'><div class='noDataTxt' >暂无数据!<div></td></tr>");
                }
 
                $("#pager").paginate({
                    total_count: data.TotalCount,
                    count: data.PageCount,
                    start: _self.Params.PageIndex,
                    display: 5,
                    images: false,
                    mouse: 'slide',
                    onChange: function (page) {
                        $(".tr-header").nextAll().remove();
                        _self.Params.PageIndex = page;
                        _self.getCustomersByActivityID();
                    }
                });

            }
        );
    }

    module.exports = ObjectJS;
});