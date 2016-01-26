
define(function (require, exports, module) {
    require("plug/sharemingdao/style.css");

    (function ($) {
        var Defaults = {
            share_post: true,
            share_task: true,
            share_schedule: true,
            post_pars: {
                content: '',
                groups: [],
                share_type:0
            },
            task_pars: {
                name: '',
                end_date: '',
                charger: {},
                members: [],
                url: "",
                des:''
                },
            schedule_pars: {
                name: '',
                start_date: '',
                end_date: '',
                members: [],
                address: '',
                url: "",
                des: ''
            },
            callback: function (type, url) { }
        };

        $.fn.sharemingdao = function (options) {
            Defaults = $.extend({}, Defaults, options);
            
            $(this).each(function () {
                shareMingDao($(this));
            }); 
        }

        
        //其他插件引用
        var doT = require("dot");
        var Easydialog = require("easydialog");
        var ChooseUser = require("chooseuser");
        //require("/modules/plug/laydate/need/laydate.css");
        //require("/modules/plug/laydate/skins/default/laydate.css");
        //require("/modules/plug/laydate/laydate.js");

        //分享明道
        var shareMingDao = function (obj) {

            $(obj).click(function () {
                
                if ($(".selectShareType").length > 0) {
                    $(".selectShareType").remove();
                    return false;
                }

                var left = $(this).offset().left;
                var width = $(this).width();
                var top = $(this).offset().top;

                drawShareType();

                var $selectShareType = $(".selectShareType");
                var targetTop = (top + 35) + "px";
                var targetLeft = (left - ($selectShareType.width() / 2 - width / 2) + 8) + "px"
                $selectShareType.css({ "left": targetLeft, "top": targetTop }).fadeIn();
            });

            $(document).click(function (e) {
                //隐藏下拉
                if (!$(e.target).parents().hasClass("selectShareType") && !$(e.target).parents().hasClass("btn_shareMD") && !$(e.target).hasClass("btn_shareMD")) {
                    $(".selectShareType").remove();
                }
            });
        };

        //画分享明道选择类型
        var drawShareType = function () {
            var html='  <div class="selectShareType hide">';
            html+='<div class="top-lump"></div>';
            html+='    <div class="top-lump2"></div>';
            html+='    <ul>';
            html+= '        <li id="li_shareMingdaoPost">';
            html += '            <div class="left"><img class="shareTypeIco" src="/modules/plug/sharemingdao/images/ico-post.png"/></div>';
            html+='            <div class="left mLeft5">分享动态</div>';
            html+='            <div class="clear"></div>';
            html+='        </li>';
            html += '        <li id="li_shareMingdaoTask">';
            html += '            <div class="left"><img class="shareTypeIco" src="/modules/plug/sharemingdao/images/ico-task.png"/></div>';
            html+='            <div class="left mLeft5">分享任务</div>';
            html+='            <div class="clear"></div>';
            html+='        </li>';
            html += '        <li id="li_shareMingdaoSchedule">';
            html += '            <div class="left"><img class="shareTypeIco" src="/modules/plug/sharemingdao/images/ico-schedule.png"/></div>';
            html+='            <div class="left mLeft5">分享日程</div>';
            html+='            <div class="clear"></div>';
            html+='        </li>';
            html+='    </ul>';
            html+= '</div>';
            html = $(html);

            $("body").append(html);


            $("#li_shareMingdaoPost").click(function () {
                $(".selectShareType").remove();
                shareMingdaoPost();
            });

            $("#li_shareMingdaoTask").click(function () {
                $(".selectShareType").remove();
                shareMingdaoTask();
            });

            $("#li_shareMingdaoSchedule").click(function () {
                $(".selectShareType").remove();
                shareMingdaoSchedule();
            });

            
        };

        //分享明道动态
        var shareMingdaoPost = function () {
            doT.exec("template/sharemingdao/share_mingdao_post.html", function (template) {
                var html = template([]);
                //显示分享明道动态弹出层
                Easydialog.open({
                    container: {
                        id: "share_mingdao_post",
                        header: "分享动态",
                        content: html
                    }
                });

                //填充默认值
                if (Defaults.share_post)
                {
                    $("#post-content").val(Defaults.post_pars.content);
                }

                //获取当前明道用户所属群组列表
                $.get("/MingDao/GetUserGroups", null, function (data) {
                    if (data.Result == 1)
                    {
                        $(".shareMDGroups ul").append($('<li><span class="ico-check pLeft5" data-id="everyone">所有同事</span></li>'));
                        for (var i = 0; len = data.Items.length, i < len; i++) {
                            var item=data.Items[i];
                            $(".shareMDGroups ul").append($('<li><span class="ico-check pLeft5" data-id="'+item.id+'">' + item.name + '</span></li>'));
                        }
                        $(".shareMDGroups ul").append($('<li><span class="myself">我自己</span></li>'));

                        //分享群组点击事件
                        $(".shareMDGroups ul li span").click(function () {
                            if ($(this).hasClass("ico-check"))
                                $(this).addClass("ico-checked").removeClass("ico-check");
                            else if ($(this).hasClass("ico-checked"))
                                $(this).addClass("ico-check").removeClass("ico-checked");
                            else {
                                $(".shareMDGroups ul li span.ico-checked").removeClass("ico-checked").addClass("ico-check");
                               
                            }

                            if ($(this).hasClass("myself"))
                                $(".btn_selectMDGroups").html("我自己");
                            else
                                $(".btn_selectMDGroups").html("选择 " + $(".shareMDGroups ul li span.ico-checked").length + " 项");
                        });


                        //隐藏下拉
                        $(document).click(function (e) {
                            if (!$(e.target).parents().hasClass("shareMDGroups") && !$(e.target).parents().hasClass("btn_selectMDGroups") && !$(e.target).hasClass("btn_selectMDGroups")) {
                                $(".shareMDGroups").slideUp(500);
                            }
                        });

                    }
                });

                //分享明道动态事件
                $("#btn-shareMingdaoPost").click(function () {
                    Easydialog.close();

                    var gIDs = '';
                    $(".shareMDGroups ul li span.ico-checked").each(function () {
                        gIDs += $(this).data("id")+",";
                    });


                    $.post("/MingDao/SharePost", {
                        msg: $("#post-content").val(),
                        gIDs: gIDs
                    }, function (data) {

                        if (data.Result == 1) {
                            alert("分享成功");
                        }
                        else {
                            alert("分享失败");
                        }
                    });
                });


            });
        };

        //分享明道任务
        var shareMingdaoTask = function () {
            
            doT.exec("template/sharemingdao/share_mingdao_task.html", function (template) {
                var html = template([]);
                //显示分享明道任务弹出层
                Easydialog.open({
                    container: {
                        id: "share_mingdao_task",
                        header: "分享任务",
                        content: html
                    }
                });

                //填充默认值
                if (Defaults.share_task) {
                    $("#task-name").val(Defaults.task_pars.name);
                    $("#task-enddate").val(Defaults.task_pars.end_date);
                    if (Defaults.task_pars.charger) {
                        if (Defaults.task_pars.charger.UserID) {
                            createMember2(Defaults.task_pars.charger, "task-charger", true);
                        }
                    }

                    if (Defaults.task_pars.members) {
                        if (Defaults.task_pars.members.length > 0) {
                            for (var i = 0; i < Defaults.task_pars.members.length; i++) {
                                createMember2(Defaults.task_pars.members[i], "task-members", false);
                            }
                        }
                    }
                    $("#task-des").val(Defaults.task_pars.des);
                }


                //加载任务截至时间
                var end = {
                    elem: '#task-enddate',
                    format: 'YYYY-MM-DD',
                    max: '2099-06-16',
                    istime: false,
                    istoday: false
                };
                laydate(end);

                //添加负责人
                $("#btn-task-charger").click(function () {
                    ChooseUser.create({
                        title: "添加负责人",
                        type: 1,
                        single: true,
                        callback: function (items) {
                            for (var i = 0; i < items.length; i++) {
                                createMember(items[i], "task-charger", true);
                            }
                        }
                    });
                });

                //添加成员
                $("#btn-task-members").click(function () {
                    ChooseUser.create({
                        title: "添加成员",
                        type: 1,
                        single: false,
                        callback: function (items) {
                            for (var i = 0; i < items.length; i++) {
                                createMember(items[i], "task-members", false);
                            }

                        }
                    });
                });

                //分享明道任务事件
                $("#btn-shareMingdaoTask").click(function () {
                    var OwnerID = '', MemberID = '';
                    $("#task-charger .member").each(function () {
                        OwnerID += $(this).data("id") + "|";
                    });
                    $("#task-members .member").each(function () {
                        MemberID += $(this).data("id") + "|";
                    });

                    if (OwnerID == '' || MemberID == '') {
                        alert("请选择负责人和成员");
                        return false;
                    }


                    $.post("/MingDao/ShareTask", {
                        name: $("#task-name").val(),
                        ownerUserID: OwnerID,
                        memberIDs: MemberID,
                        endDate: $("#task-enddate").val(),
                        des: $("#task-des").val(),
                        url: Defaults.task_pars.url
                    }, function (data) {
                        Easydialog.close();

                        if (data.Result == 1) {

                            Defaults.callback && Defaults.callback("Task", data.Url);

                            alert("分享成功");
                        }
                        else {
                            alert("分享失败");
                        }
                    });

                });


            });
        };

        //分享明道日程
        var shareMingdaoSchedule = function () {
            doT.exec("template/sharemingdao/share_mingdao_schedule.html", function (template) {
                var html = template([]);
                //显示分享明道日程弹出层
                Easydialog.open({
                    container: {
                        id: "share_mingdao_schedule",
                        header: "分享日程",
                        content: html
                    }
                });

                //填充默认值
                if (Defaults.share_schedule) {
                    $("#schedule-name").val(Defaults.schedule_pars.name);
                    $("#schedule-startdate").val(Defaults.schedule_pars.start_date);
                    $("#schedule-enddate").val(Defaults.schedule_pars.end_date);
                    if (Defaults.schedule_pars.members) {
                        if (Defaults.schedule_pars.members.length > 0) {
                            for (var i = 0; i < Defaults.schedule_pars.members.length; i++) {
                                createMember2(Defaults.schedule_pars.members[i], "schedule-members", false);
                            }
                        }
                    }
                    $("#schedule-address").val(Defaults.schedule_pars.address);
                    $("#schedule-des").val(Defaults.schedule_pars.des);
                }

                //加载日程起始时间
                var start = {
                    elem: '#schedule-startdate',
                    format: 'YYYY-MM-DD hh:mm',
                    max: '2099-06-16',
                    istime: true,
                    istoday: false
                };

                var end = {
                    elem: '#schedule-enddate',
                    format: 'YYYY-MM-DD  hh:mm',
                    max: '2099-06-16',
                    istime: true,
                    istoday: false
                };
                laydate(start);
                laydate(end);

                //添加成员
                $("#btn-schedule-members").click(function () {
                    ChooseUser.create({
                        title: "添加成员",
                        type: 1,
                        single: false,
                        callback: function (items) {
                            for (var i = 0; i < items.length; i++) {
                                createMember(items[i], "schedule-members", false);
                            }
                        }
                    });
                });

                //分享明道日程事件
                $("#btn-shareMingdaoSchedule").click(function () {
                    var MemberID = '';
                    $("#schedule-members .member").each(function () {
                        MemberID += $(this).data("id") + "|";
                    });

                    if (MemberID == '') {
                        alert("请选择成员");
                        return false;
                    }

                    $.post("/MingDao/ShareCalendar", {
                        name: $("#schedule-name").val(),
                        memberIDs: MemberID,
                        des: $("#schedule-des").val(),
                        address: $("#schedule-address").val(),
                        endDate: $("#schedule-enddate").val(),
                        startDate: $("#schedule-startdate").val(),
                        url: Defaults.schedule_pars.url
                    }, function (data) {
                        Easydialog.close();

                        if (data.Result == 1) {

                            Defaults.callback && Defaults.callback("Calendar", data.Url);
                            alert("分享成功");
                        }
                        else {
                            alert("分享失败");
                        }
                    });

                });

            });
        };

        var createMember = function (item, id, isSingle) {
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
        }

        var createMember2 = function (item, id, isSingle) {
            if ($("#" + id + " div[data-id='" + item.UserID + "']").html())
                return false;

            var html = '<div class="member left" data-id="' + item.UserID + '">';
            html += '    <div class="left pRight5">';
            html += '          <span>' + item.Name + '</span>';
            html += '     </div>';
            html += '      <div class="left mRight10 pLeft5"><a href="javascript:void(0);" onclick="$(this).parents(\'.member\').remove();">×</a></div>';
            html += '      <div class="clear"></div>';
            html += '   </div>';

            if (isSingle)
                $("#" + id).html(html);
            else
                $("#" + id).append(html);
        }

    })(jQuery)

    module.exports = jQuery;
});