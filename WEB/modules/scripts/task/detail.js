define(function (require, exports, module) {
    var doT = require("dot");
    var Global = require("global");
    var TalkReply = require("scripts/task/reply");
    var Easydialog = null;
    var ChooseUser = null;
    require("pager");

    var ObjectJS = {};
    var CacheAttrValues = [];//订单品类属性缓存
    var Editor;

    ///taskid：任务id
    ///orderid:订单id
    ///stageid：订单阶段id
    ///mark:任务标记 1：材料 2 制版 3大货材料
    ///finishStatus：任务完成状态
    ///attrValues:订单品类属性
    ///orderType:订单类型
    ///um:富文本编辑器
    ///plateRemark:制版工艺描述
    ObjectJS.init = function (attrValues, um, plateRemark, orderimages, task, isWarn,order) {
        var task = JSON.parse(task.replace(/&quot;/g, '"'));

        ObjectJS.orderid = task.OrderID;
        ObjectJS.guid = task.OrderID;
        ObjectJS.ownerid = task.OwnerID;
        ObjectJS.endTime = task.EndTime.toDate("yyyy/MM/dd hh:mm:ss");
        ObjectJS.finishStatus = task.FinishStatus;
        ObjectJS.status = task.Status;
        ObjectJS.stageid = task.StageID;
        ObjectJS.taskid = task.TaskID;
        ObjectJS.orderType = task.OrderType;
        ObjectJS.isWarn = isWarn;
        ObjectJS.isPlate = true;//任务是否制版
        ObjectJS.model = JSON.parse(order.replace(/&quot;/g, '"'));
        if (attrValues != "")
            CacheAttrValues = JSON.parse(attrValues.replace(/&quot;/g, '"'));//制版属性缓存
        Editor = um;
        ObjectJS.mark = task.Mark;//任务标记 用于做标记任务完成的限制条件
        ObjectJS.materialMark = 0;//任务材料标记 用于算材料列表的金额统计
        ObjectJS.orderimages = orderimages;
        $(".part-btn").hide();
        //事件绑定
        ObjectJS.bindEvent();

        //材料任务
        if ($("#btn-addMaterial").length == 1) {
            ObjectJS.materialMark = 1;
            if (ObjectJS.mark == 21) {
                ObjectJS.materialMark = 2;
            }

            ObjectJS.bindProduct();

            ObjectJS.removeTaskPlateOperate();
        }
            //制版任务
        else if ($("#btn-updateTaskRemark").length == 1) {
            if (Easydialog == null) {
                Easydialog = require("easydialog");
            }
            ObjectJS.bindPlatemakingEvent();
        }
        else {
            ObjectJS.removeTaskPlateOperate();
        }


        //统计材料总金额
        ObjectJS.getProductAmount();

        //制版工艺描述
        if (Editor) {
            Editor.ready(function () {
                Editor.setContent(decodeURI(plateRemark));
            });
        }

        //制版工艺说明
        if ($("#PlateRemark").length == 1) {
            if (plateRemark != "") {
                $("#PlateRemark").html(decodeURI(plateRemark));
            }
            else {
                $("#PlateRemark").html(decodeURI("<div class='pAll10'>暂无工艺说明</div>"));
            }
        }

        //判断制版任务是否执行了
        if (ObjectJS.mark == 12) {
            if ($("#platemakingBody .table-list").length == 0) {
                ObjectJS.isPlate = false;
            }
        }

        if (ObjectJS.mark === 15 || ObjectJS.mark === 25) {
            ObjectJS.getExpress();
        }

        ObjectJS.isLoading = true;
    };

    //#region任务基本信息操作
    //绑定事件
    ObjectJS.bindEvent = function () {

        //裁剪录入
        $("#btnCutoutOrder").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            ObjectJS.cutOutGoods();
        });

        //车缝录入
        $("#btnSewnOrder").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            ObjectJS.sewnGoods();
        });

        //发货录入
        $("#btnSendOrder").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            ObjectJS.sendGoods();
        });

        //切换模块
        $(".module-tab li").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            if (_this.hasClass("hover")) return;
            $(".part-btn").hide();
            if (_this.data("btn")) {
                $("#" + _this.data("btn")).show();
            }
            _this.siblings().removeClass("hover");
            _this.addClass("hover");

            $("#navTask").children().hide();
            $("#" + _this.data("id")).show();



            if (_this.data("id") == "orderTaskLogs") {
                if (!_this.data("isget")) {
                    //任务日志列表
                    ObjectJS.getLogs(1);
                    _this.data("isget", "1");
                }
            }
            else if (_this.data("id") == "navCutoutDoc") {
                ObjectJS.getCutoutDoc();
            }
            else if (_this.data("id") == "navSewnDoc") {
                ObjectJS.getSewnDoc();
            }
            else if (_this.data("id") == "navSendDoc") {
                ObjectJS.getSendDoc();
            }

        });

        //标记任务完成
        if ($("#FinishTask").length == 1) {
            $("#FinishTask").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                ObjectJS.finishTask();
            });
        }

        //接受任务
        if ($("#AcceptTask").length == 1) {
            $("#AcceptTask").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                ObjectJS.updateTaskEndTime();
            });
        }

        //添加任务成员
        if ($("#addTaskMembers").length == 1) {
            ChooseUser = require("chooseuser");

            $("#addTaskMembers").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                ChooseUser.create({
                    title: "添加任务成员",
                    type: 1,
                    single: false,
                    callback: function (items) {
                        var memberIDs = '';
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            console.log(item.id);
                            if (ObjectJS.ownerid == item.id) {
                                continue;
                            }

                            if ($(".memberlist" + " tr[data-id='" + item.id + "']").html()) {
                                continue;
                            }

                            ObjectJS.createTaskMember(item);
                            console.log(item);
                            memberIDs += item.id + ",";
                        }

                        if (memberIDs != '') {
                            ObjectJS.addTaskMembers(memberIDs);
                        }


                    }
                });

            });

            //任务负责人更改成员权限
            $('.check-lump').click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var _this = $(this);
                var confirmMsg = "确定将" + _this.parents('li').find('.membername').text() + "的权限设置为<span style='font-size:14px;color:red;'>" + (_this.data('type') == 1 ? "查看" : "编辑") + "</span>?";
               
                if (!_this.hasClass('hover')) {
                    confirm(confirmMsg, function () {
                        Global.post("/Task/UpdateMemberPermission", {
                            taskID: _this.data('taskid'),
                            memberID: _this.data('memberid'),
                            type: _this.data('type')
                        }, function (data) {
                            if (data.result == 1) {
                                _this.parents('tr').find('.check-lump').removeClass('hover');
                                _this.addClass('hover');
                            } else {
                                alert('授权失败');
                            }
                        })
                    })
                }
            })

            //列表删除任务成员
            $(".memberlist td.removeTaskMember").unbind().click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var memberID = $(this).data("id");
                var confirmMsg = "确定删除成员<span style='color:red;font-size:14px;'>" + $(this).parents('tr').find('.membername').text() + "</span>?";

                confirm(confirmMsg, function () {
                    ObjectJS.removeTaskMember(memberID);
                });
            });

            ////删除任务成员
            //$("#taskMemberIDs a.removeTaskMember").unbind().click(function () {
            //    var memberID = $(this).data("id");
            //    confirm("确定删除任务成员?", function () {
            //        ObjectJS.removeTaskMember(memberID);
            //    });
            //});

        }

        
        //显示剩余时间
        ObjectJS.showTime();

        //绑定任务样式图
        ObjectJS.bindOrderImages();

        //初始化任务讨论列表
        TalkReply.initTalkReply(ObjectJS);
        
    }

    //更改任务到期时间
    ObjectJS.updateTaskEndTime = function () {
        Easydialog = require("easydialog");
        var innerHtml = '<div class="pTop10 pBottom5"><span class="width80" style="display:inline-block;">到期时间:</span><input style="width:180px;" type="text" class="taskEndTime" id="UpdateTaskEndTime" placeholder="设置到期时间"/></div>';
        Easydialog.open({
            container: {
                id: "show-model-setRole",
                header: "设置任务到期时间",
                content: innerHtml,
                yesFn: function () {
                    if ($("#UpdateTaskEndTime").val() == "") {
                        alert("任务到期时间不能为空");
                        return;
                    }

                    confirm("任务到期时间不可逆，确定设置?", function () {
                        ObjectJS.isLoading = false;
                        Global.post("/Task/UpdateTaskEndTime", {
                            id: ObjectJS.taskid,
                            endTime: $("#UpdateTaskEndTime").val()
                        }, function (data) {
                            if (data.result == 0) {
                                alert("操作无效");
                            }
                            else if (data.result == 2) {
                                alert("任务已接受,不能操作");
                            }
                            else if (data.result == 3) {
                                alert("没有权限操作");
                            }
                            else {
                                location.href = location.href;
                            }
                            ObjectJS.isLoading = true;
                        });
                    });

                }
            }
        });

        var myDate = new Date();
        var minDate = myDate.toLocaleDateString();
        minDate = minDate + " 23:59:59"
        //更新任务到期日期
        var taskEndTime = {
            elem: '#UpdateTaskEndTime',
            format: 'YYYY-MM-DD hh:mm:ss',
            min: minDate,
            max: '2099-06-16',
            istime: true,
            istoday: false
        };
        laydate(taskEndTime);



    }

    //标记任务完成
    ObjectJS.finishTask = function () {
        if (ObjectJS.mark == 11) {
            if ($("#navProducts .table-list tr").length == 2) {
                alert("材料没有添加,不能标记任务完成");
                return;
            }

        }
        else if (ObjectJS.mark == 12) {
            if ($("#platemakingBody .table-list").length == 0) {
                alert("制版没有设置,不能标记任务完成");
                return;
            }
            else if (!ObjectJS.isPlate) {
                alert("制版没有设置,不能标记任务完成");
                return;
            }
        }

        confirm("标记完成的任务不可逆,确定完成?", function () {
            $("#FinishTask").val("完成中...").attr("disabled", "disabled");
            ObjectJS.isLoading = false;
            Global.post("/Task/FinishTask",
               {
                   id: ObjectJS.taskid
               }, function (data) {
                   $("#FinishTask").val("标记完成").removeAttr("disabled");
                   if (data.result == 1) {
                       location.href = location.href;
                   }
                   else if (data.result == 2) {
                       alert("前面阶段任务有未完成,不能标记完成");
                   }
                   else if (data.result == 3) {
                       alert("无权限操作");
                   }
                   else if (data.result == 4) {
                       alert("任务没有接受，不能设置完成");
                   }
                   else if (data.result == -1) {
                       alert("保存失败");
                   }
                   ObjectJS.isLoading = true;
               });
        });
    }

    //添加任务成员
    ObjectJS.addTaskMembers = function (memberIDs) {
        ObjectJS.isLoading = false;
        Global.post("/Task/AddTaskMembers", {
            id: ObjectJS.taskid,
            memberIDs: memberIDs
        }, function (data) {
            if (data.result == 0) {
                alert("添加失败");
            }
            else {
                //$("#taskMemberIDs a.removeTaskMember").unbind().click(function () {
                //    var memberID = $(this).data("id");
                //    confirm("确定删除任务成员?", function () {
                //        ObjectJS.removeTaskMember(memberID);
                //    });
                //});

                //任务负责人更改成员权限
                $('.check-lump').unbind().click(function () {
                    var _this = $(this);
                    var confirmMsg = "确定将" + _this.parents('li').find('.membername').text() + "的权限设置为<span style='font-size:14px;color:red;'>" + (_this.data('type') == 1 ? "查看" : "编辑") + "</span>?";

                    if (!ObjectJS.isLoading) {
                        return;
                    }
                    var _this = $(this);
                    var confirmMsg = "确定将" + _this.parents('li').find('.membername').text() + "的权限设置为<span style='font-size:14px;color:red;'>" + (_this.data('type') == 1 ? "查看" : "编辑") + "</span>?";

                    if (!_this.hasClass('hover')) {
                        confirm(confirmMsg, function () {
                            Global.post("/Task/UpdateMemberPermission", {
                                taskID: _this.data('taskid'),
                                memberID: _this.data('memberid'),
                                type: _this.data('type')
                            }, function (data) {
                                if (data.result == 1) {
                                    _this.parents('tr').find('.check-lump').removeClass('hover');
                                    _this.addClass('hover');
                                } else {
                                    alert('授权失败');
                                }
                            })
                        })
                    }
                })

                //列表删除任务成员
                $(".memberlist td.removeTaskMember").unbind().click(function () {
                    var memberID = $(this).data("id");
                    var confirmMsg = "确定删除成员<span style='color:red;font-size:14px;'>" + $(this).parents('tr').find('.membername').text() + "</span>?";

                    confirm(confirmMsg, function () {
                        ObjectJS.removeTaskMember(memberID);
                    });
                });


            }
            ObjectJS.isLoading = true;
        });
    }

    //删除任务成员
    ObjectJS.removeTaskMember = function (memberID) {
        ObjectJS.isLoading = false;
        Global.post("/Task/RemoveTaskMember", {
            id: ObjectJS.taskid,
            memberID: memberID
        }, function (data) {
            if (data.result == 0) {
                alert(memberIDs);
            }
            else {
                //$("#taskMemberIDs" + " div[data-id='" + memberID + "']").remove();
                $(".memberlist tr[data-id='" + memberID + "']").remove();
            }
            ObjectJS.isLoading = true;
        });
    }

    //拼接任务成员html
    ObjectJS.createTaskMember = function (item) {
        var html = '';
        html += '<div class="task-member left" data-id="' + item.id + '">';
        html += '<div class="left pRight5"><span>' + item.name + '</span></div>';
        html += '<div class="left mRight10 pLeft5"><a class="removeTaskMember" href="javascript:void(0);" data-id="' + item.id + '" >×</a></div>';
        html += '<div class="clear"></div>';
        html += '</div>';

        var memberListHtml = '';
        memberListHtml += '<tr data-id="' + item.id + '">';
        memberListHtml += '<td class="tLeft pLeft10"><i><img onerror="$(this).attr("src","/modules/images/defaultavatar.png"); src="' + (item.Avatar == null ? "/modules/images/defaultavatar.png" : item.Avatar) + '" /></i><i class="membername">' + item.name + '</i></td>';
        memberListHtml += '<td><i class="hand ico-radiobox check-lump hover" data-taskid="' + ObjectJS.taskid + '" data-memberid="' + item.id + '" data-type=1 ><span></span></i></td>';
        memberListHtml += '<td><i class="hand ico-radiobox check-lump" data-taskid="' + ObjectJS.taskid + '" data-memberid="' + item.id + '" data-type=2 ><span></span></i></td>';
        memberListHtml += '<td class="removeTaskMember iconfont hand" data-id="' + item.id + '">&#xe651;</td>';

        $('.memberlist .member-items tbody').append(memberListHtml);
    }

    //任务到期时间倒计时
    ObjectJS.showTime = function () {
        if (ObjectJS.status == 8) {
            return;
        }
        if (ObjectJS.finishStatus!=1) {
            return;
        }

        var time_end = (new Date(ObjectJS.endTime)).getTime();
        var time_start = new Date().getTime(); //设定当前时间
        
        // 计算时间差 
        var time_distance = time_end - time_start;
        var overplusTime = false;
        if (time_distance < 0) {
            if (!overplusTime) {
                $("#overplusTime").html("超期时间");
                $(".taskBaseInfo .li-plustime .task-time").css("background-color", "red");
            }
            overplusTime = true;
            time_distance = time_start - time_end;
        }
        else {
            if (ObjectJS.isWarn == 1) {
                if (!overplusTime) {
                    $(".taskBaseInfo .li-plustime .task-time").css({ "background-color": "orange", "color": "#fff" });
                }
                overplusTime = true;
            }
        }

        // 天
        var int_day = Math.floor(time_distance / 86400000)
        time_distance -= int_day * 86400000;
        // 时
        var int_hour = Math.floor(time_distance / 3600000)
        time_distance -= int_hour * 3600000;
        // 分
        var int_minute = Math.floor(time_distance / 60000)
        time_distance -= int_minute * 60000;
        // 秒 
        var int_second = Math.floor(time_distance / 1000)
        // 时分秒为单数时、前面加零 
        if (int_day < 10) {
            int_day = "0" + int_day;
        }
        if (int_hour < 10) {
            int_hour = "0" + int_hour;
        }
        if (int_minute < 10) {
            int_minute = "0" + int_minute;
        }
        if (int_second < 10) {
            int_second = "0" + int_second;
        }
        // 显示时间 
        $("#time-d").html(int_day);
        $("#time-h").html(int_hour);
        $("#time-m").html(int_minute);
        $("#time-s").html(int_second);

        // 设置定时器
        setTimeout(function () { ObjectJS.showTime() }, 1000);
    }

    //绑定任务样式图
    ObjectJS.bindOrderImages = function () {
        var orderimages = ObjectJS.orderimages;
        var _self = this;
        var images = orderimages.split(",");
        _self.images = images;
        for (var i = 0; i < images.length; i++) {
            if (images[i]) {
                if (i == 0) {
                    $("#orderImage").attr("src", images[i]);
                }
                var img = $('<li class="' + (i == 0 ? 'hover' : "") + '"><img src="' + images[i] + '" /></li>');
                $(".order-imgs-list").append(img);
            }
        }

        $(".order-imgs-list img").parent().click(function () {
            var _this = $(this);
            if (!_this.hasClass("hover")) {
                _this.siblings().removeClass("hover");
                _this.addClass("hover");
                $("#orderImage").attr("src", _this.find("img").attr("src"));
            }
        });

        //图片放大功能
        //图片放大功能
        var width = document.documentElement.clientWidth, height = document.documentElement.clientHeight;
        $("#orderImage").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            if ($(this).attr("src")) {

                $(".enlarge-image-bgbox,.enlarge-image-box").fadeIn();
                $(".right-enlarge-image,.left-enlarge-image").css({ "top": height / 2 - 80 })

                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + $(this).attr("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
            }
        });
        $(".close-enlarge-image").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
            $(".enlarge-image-item").empty();
        });
        $(".enlarge-image-bgbox").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            $(".enlarge-image-bgbox,.enlarge-image-box").fadeOut();
            $(".enlarge-image-item").empty();
        });
        $(".zoom-botton").click(function (e) {
            if (!ObjectJS.isLoading) {
                return;
            }
            var scaleToAdd = 0.8;
            if (e.target.id == 'zoomOutButton')
                scaleToAdd = -scaleToAdd;
            $('#enlargeImage').smartZoom('zoom', scaleToAdd);
            return false;
        });

        $(".left-enlarge-image").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var ele = $(".order-imgs-list .hover").prev();
            if (ele && ele.find("img").attr("src")) {
                var _img = ele.find("img");
                $(".order-imgs-list .hover").removeClass("hover");
                ele.addClass("hover");
                $("#orderImage").attr("src", _img.attr("src"));
                $(".enlarge-image-item").empty();
                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
            }
        });

        $(".right-enlarge-image").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var ele = $(".order-imgs-list .hover").next();
            if (ele && ele.find("img").attr("src")) {
                var _img = ele.find("img");
                $(".order-imgs-list .hover").removeClass("hover");
                ele.addClass("hover");
                $("#orderImage").attr("src", _img.attr("src"));
                $(".enlarge-image-item").empty();
                $(".enlarge-image-item").append('<img id="enlargeImage" src="' + _img.attr("src") + '"/>');
                $('#enlargeImage').smartZoom({ 'containerClass': 'zoomableContainer' });
            }
        });
    }

    //获取任务日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#taskLogList").empty();
        ObjectJS.isLoading=false
        Global.post("/Task/GetOrderTaskLogs", {
            id: _self.taskid,
            pageindex: page
        }, function (data) {

            if (data.items.length > 0) {
                doT.exec("template/common/logs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    $("#taskLogList").html(innerhtml);
                });
            }
            else {
                $("#taskLogList").html("<div class='nodata-txt'>暂无数据!</div>");
            }

            $("#pagerLogs").paginate({
                total_count: data.totalCount,
                count: data.pageCount,
                start: page,
                display: 5,
                border: true,
                rotate: true,
                images: false,
                mouse: 'slide',
                float: "left",
                onChange: function (page) {
                    _self.getLogs(page);
                }

            });
            ObjectJS.isLoading = true;
        });
    }
    //#endregion

    // #region任务材料基本操作
    //绑定事件
    ObjectJS.bindProduct = function () {

        //编辑价位
        $(".price").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            if ($(this).val().isDouble() && $(this).val() >= 0) {
                ObjectJS.editPrice($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑数量
        $(".quantity").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            if ($(this).val().isDouble() && $(this).val() > 0) {
                ObjectJS.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗率
        $(".loss-rate").change(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var lossRate = $(this).val();

            if (lossRate > -1) {
                ObjectJS.editLossRate($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗量
        $(".loss").change(function () {
            //if (!ObjectJS.isLoading) {
            //    return;
            //}
            //var loss = parseFloat($(this).val());
            //if (!isNaN(loss)) {
            //    if (loss < 0) {
            //        if (-loss >= parseFloat($(this).parent().prev().html())) {
            //            $(this).val($(this).data("value"));
            //            return;
            //        }
            //    }

            //    ObjectJS.editLoss($(this));
            //} else {
            //    $(this).val($(this).data("value"));
            //}
        });

        //删除产品
        $(".ico-del").click(function () {
            if (!ObjectJS.isLoading) {
                return;
            }
            var _this = $(this);
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: ObjectJS.guid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        ObjectJS.getProductAmount();
                    }
                });
            });
        });

        //生成采购单
        if ($("#btnEffectiveOrderProduct").length == 1) {
            $("#btnEffectiveOrderProduct").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                ObjectJS.effectiveOrderProduct();
            });
        }

    };

    //更改材料单价
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;
        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });
    }

    //更改损耗量
    ObjectJS.editLoss = function (ele) {
        //var _self = this;
        //ObjectJS.isLoading = false;
        //Global.post("/Orders/UpdateProductLoss", {
        //    orderid: _self.guid,
        //    autoid: ele.data("id"),
        //    name: ele.data("name"),
        //    quantity: ele.val()
        //}, function (data) {
        //    if (!data.status) {
        //        ele.val(ele.data("value"));
        //        alert("当前订单状态,不能进行修改");
        //    } else {
        //        ele.data("value", ele.val());
        //        _self.getProductAmount();
        //    }
        //    ObjectJS.isLoading = true;
        //});
    }

    //更改损耗率
    ObjectJS.editLossRate = function (ele) {
        var _self = this;
        ObjectJS.isLoading = false;

        ele.data('value', ele.val());

        var loss = ((ele.val() * 1) * (ele.parents('tr').find('.tr-quantity').html() * 1)).toFixed(3);

        ele.parents('tr').find('.tr-loss').html(loss);

        Global.post("/Orders/UpdateProductLoss", {
            orderid: _self.guid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getProductAmount();
            }
            ObjectJS.isLoading = true;
        });

      


    }
    
    //生成采购单
    ObjectJS.effectiveOrderProduct = function () {
        ObjectJS.isLoading = false;
        Global.post("/Orders/EffectiveOrderProduct", {
            orderID: ObjectJS.guid
        }, function (data) {
            if (data.result == 1) {
                location.href = location.href;
            }
            ObjectJS.isLoading = true;
        });
    }

    //计算总金额
    ObjectJS.getProductAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);

            if (ObjectJS.materialMark == 0) {
                amount += _this.html() * 1;
            }
            else if (ObjectJS.materialMark == 1) {
                _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(3));
                amount += _this.html() * 1;
            }
            else if (ObjectJS.materialMark == 2) {
                _this.html(((_this.prevAll(".tr-quantity").html() * 1 + _this.prevAll(".tr-loss").html() * 1) * _this.prevAll(".tr-price").find(".price").val()).toFixed(3));
                amount += _this.html() * 1;
            }
        });

        $("#amount").text(amount.toFixed(3));
    }
    //#endregion

    //#region任务制版相关事件
    //绑定
    ObjectJS.bindPlatemakingEvent = function () {
        ObjectJS.bindDocumentClick();
        ObjectJS.binddropdown();

        ObjectJS.bindContentClick();

        ObjectJS.bindAddColumn();
        ObjectJS.bindRemoveColumn();

        ObjectJS.bindAddRow();
        ObjectJS.bindRemoveRow();

        $("#btn-updateTaskRemark").click(function () {
            ObjectJS.updateOrderPlatemaking();
        });

        $("#btn-updatePlateRemark").click(function () {
            ObjectJS.updateOrderPlateRemark();
        });

        ObjectJS.bindAddTaskPlate();
    };

    //文档点击的隐藏事件
    ObjectJS.bindDocumentClick = function () {
        $(document).bind("click", function (e) {
            //隐藏制版列操作下拉框
            if (!$(e.target).parents().hasClass("ico-dropdown") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }
        });
    }

    //显示制版列操作下拉框
    ObjectJS.binddropdown = function () {
        $(".ico-dropdown").unbind().bind("click", function () {
            var _this = $(this);
            var position = _this.position();
            $(".dropdown-ul li").data("columnname", _this.data("columnname"));
            $(".dropdown-ul").css({ "top": position.top + 20, "left": position.left - 70 }).show().mouseleave(function () {
                $(this).hide();
            });

            return false;
        });

    }

    //制版的内容点击
    ObjectJS.bindContentClick = function () {
        $("#platemakingBody .tr-content td").unbind().bind("click", function () {
            $(this).find('.tbContent').hide();
            $(this).find('.tbContentIpt').show().focus();
        });
    }

    //添加新列
    ObjectJS.bindAddColumn = function () {
        $("#btn-addColumn").unbind().bind("click", function () {
            ObjectJS.columnnameid = $(this).data("columnname");
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            var noHaveLi = true;
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];
                if ($(".table-list td[data-columnname='columnname_" + item.ValueID + "']").length == 0) {
                    innerHtml += '<li class="role-item" data-id="' + item.ValueID + '">' + item.ValueName + '</li>';
                    noHaveLi = false;
                }
            }
            innerHtml += '</ul>';

            if (noHaveLi) {
                innerHtml = '<div style="width:300px;">制版属性列已全部添加设置了</div>';
            }

            Easydialog.open({
                container: {
                    id: "show-model-setRole",
                    header: "新增制版属性列",
                    content: innerHtml,
                    yesFn: function () {
                        var $hovers = $("#setTaskPlateAttrBox li.hover");
                        if ($hovers.length == 0) return;

                        var newColumnHeadr = '';
                        var newColumn = '';
                        $hovers.each(function () {
                            var columnnameid = $(this).data("id");
                            var columnnamename = $(this).html();

                            newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '">';
                            newColumnHeadr += '<span>' + columnnamename + '</span>';
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';
                        });


                        $("#platemakingBody td[data-columnname='" + ObjectJS.columnnameid + "']").eq(0).after(newColumnHeadr);
                        $("#platemakingBody td[data-columnname='" + ObjectJS.columnnameid + "']:gt(0)").after(newColumn).find("tbContentIpt").show();

                        ObjectJS.binddropdown();
                        ObjectJS.bindContentClick();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();
                    }
                }

            });

            $("#setTaskPlateAttrBox .role-item").click(function () {
                if (!$(this).hasClass("hover"))
                    $(this).addClass("hover");
                else
                    $(this).removeClass("hover");
            });

        });
    }

    //删除列
    ObjectJS.bindRemoveColumn = function () {
        $("#btn-removeColumn").unbind().bind("click", function () {

            if ($("#platemakingBody .tr-header td").length == 3) {
                alert("只剩最后一列,不能删除");
                return;
            }

            $("#platemakingBody .table-list td[data-columnname='" + $(this).data("columnname") + "']").remove();
        });
    }

    //添加行
    ObjectJS.bindAddRow = function () {
        $("div.btn-addRow").unbind().bind('click', function () {
            var $newTR = $("<tr class='tr-content'>" + $(this).parent().parent().parent().html() + "</tr>");
            $newTR.find(".tbContentIpt").empty().attr("value", "").show();
            $newTR.find(".tbContent").empty();
            $(this).parent().parent().parent().after($newTR);

            ObjectJS.bindContentClick();
            ObjectJS.bindAddRow();
            ObjectJS.bindRemoveRow();
        });
    }

    //删除行
    ObjectJS.bindRemoveRow = function () {
        $("div.btn-removeRow").unbind().bind('click', function () {
            if ($("div.btn-removeRow").length == 1) {
                alert("只剩最后一行,不能删除");
                return;
            }

            $(this).parent().parent().parent().remove();
        });
    }

    //删除行操作按钮
    ObjectJS.removeTaskPlateOperate = function () {
        $("span.ico-dropdown").remove();
        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").remove();
        });
    }

    //新增制版属性列
    ObjectJS.bindAddTaskPlate = function () {
        if ($("#btn-addTaskPlate").length == 0) return;

        $("#btn-addTaskPlate").unbind().bind("click", function () {

            var noHaveLi = false;
            var innerHtml = '<ul id="setTaskPlateAttrBox" class="role-items">';
            for (var i = 0; len = CacheAttrValues.length, i < len; i++) {
                var item = CacheAttrValues[i];
                innerHtml += '<li class="role-item" data-id="' + item.ValueID + '">' + item.ValueName + '</li>';
            }
            innerHtml += '</ul>';

            if (CacheAttrValues.length == 0) {
                noHaveLi = true;
                innerHtml = '<div style="width:300px;">制版属性没有配置,请联系后台管理员配置</div>';
            }


            Easydialog.open({
                container: {
                    id: "show-model-setRole",
                    header: "新增制版属性列",
                    content: innerHtml,
                    yesFn: function () {

                        var $hovers = $("#setTaskPlateAttrBox li.hover");
                        if ($hovers.length == 0) return;

                        var tableHtml = '<table class="table-list">';
                        var newColumnHeadr = '<tr class="tr-header">';
                        newColumnHeadr += '<td class="width100 tLeft">部位/尺码</td>';

                        var newColumn = '<tr class="tr-content">';
                        newColumn += '<td class="tLeft width100">';
                        newColumn += '<span class="tbContent"></span>';
                        newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                        newColumn += '</td>';

                        $hovers.each(function () {
                            var columnnameid = $(this).data("id");
                            var columnnamename = $(this).html();

                            newColumnHeadr += '<td class="width100 tLeft columnHeadr" data-columnname="columnname_' + columnnameid + '" data-id="' + columnnameid + '">';
                            newColumnHeadr += '<span>' + columnnamename + '</span>';
                            newColumnHeadr += '<span class="ico-dropdown mRight10 right" data-columnname="columnname_' + columnnameid + '"></span>';
                            newColumnHeadr += '</td>';

                            newColumn += '<td class="tLeft width100" data-columnname="columnname_' + columnnameid + '">';
                            newColumn += '<span class="tbContent"></span>';
                            newColumn += '<input class="tbContentIpt" value="" type="text"/>';
                            newColumn += '</td>';
                        });

                        newColumnHeadr += '<td class="width150 center">操作</td>';
                        newColumnHeadr += '</tr>';

                        newColumn += '<td class="width150 center">';
                        newColumn += '    <div class="platemakingOperate">';
                        newColumn += '        <div class="btn-addRow btn-create left" title="添加新行"></div>';
                        newColumn += '        <div class="btn-removeRow btn-remove mLeft10 left" title="删除此行"></div>';
                        newColumn += '        <div class="clear"></div>';
                        newColumn += '   </div>';
                        newColumn += '</td>';
                        newColumn += '</tr>';

                        tableHtml += newColumnHeadr + newColumn;
                        tableHtml += '</table>';

                        $("#platemakingBody").html(tableHtml).css({ "border-top": "1px solid #eee", "border-left": "1px solid #eee" }).show();

                        ObjectJS.binddropdown();
                        ObjectJS.bindContentClick();
                        ObjectJS.bindAddRow();
                        ObjectJS.bindRemoveRow();

                        $("#btn-updateTaskRemark").show();
                        $("#btn-addTaskPlate").hide();

                    }
                }

            });


            $("#setTaskPlateAttrBox .role-item").click(function () {
                if (!$(this).hasClass("hover"))
                    $(this).addClass("hover");
                else
                    $(this).removeClass("hover");
            });
        });
    }

    //保存制版信息
    ObjectJS.updateOrderPlatemaking = function () {
        if ($("#platemakingBody").html() == "") { return; }

        if ($(".tbContentIpt:visible").length == 0) { return; }

        $(".tbContentIpt:visible").each(function () {
            $(this).attr("value", $(this).val()).hide().prev().html($(this).val()).show();
        });

        var valueIDs = '';
        $("#platemakingBody .tr-header td.columnHeadr").each(function () {
            valueIDs += $(this).data("id") + '|';
        });
        ObjectJS.isLoading = false;
        Global.post("/Task/UpdateOrderPlateAttr", {
            orderID: ObjectJS.guid,
            taskID: ObjectJS.taskid,
            platehtml: encodeURI($("#platemakingBody").html()),
            valueIDs: valueIDs
        }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
                ObjectJS.isPlate = true;
            }
            else {
                alert("aa");
            }
            ObjectJS.isLoading = true;
        });
    }

    //保存制版工艺说明
    ObjectJS.updateOrderPlateRemark = function () {
        ObjectJS.isLoading = false;
        Global.post("/Task/UpdateOrderPlateRemark", {
            orderID: ObjectJS.guid,
            plateRemark: encodeURI(Editor.getContent())
        }, function (data) {
            if (data.result == 1) {
                alert("保存成功");
            }
            ObjectJS.isLoading = true;
        });
    }
    //#endregion

    ObjectJS.platePrint = function () {
        $("span.ico-dropdown").hide();

        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").hide();
        });

        var obj = $("#platemakingBody");
        //打开一个新窗口newWindow
        var newWindow = window.open("打印窗口", "_blank");
        //要打印的div的内容
        var docStr = "<style type='text/css'>.platemakingBody td {border-bottom:1px solid #eee;border-right:1px solid #eee;width:100px;}</style>";
        docStr += obj.html();

        $("span.ico-dropdown").show();
        $("#platemakingContent table tr").each(function () {
            $(this).find("td:last").show();
        });

        //打印内容写入newWindow文档
        newWindow.document.write(docStr);
        //关闭文档
        newWindow.document.close();
        //调用打印机
        newWindow.print();
        //关闭newWindow页面
        newWindow.close();


    }

    //发货记录
    ObjectJS.getSendDoc = function () {
        var _self = this;
        $("#navSendDoc .tr-header").nextAll().remove();
        $("#navSendDoc .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            type: 2
        }, function (data) {
            $("#navSendDoc .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/senddocs.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#navSendDoc .tr-header").after(innerhtml);
                });
            } else {
                $("#navSendDoc .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            ObjectJS.isLoading = true;
        });

    }

    //裁剪记录
    ObjectJS.getCutoutDoc = function () {
        var _self = this;
        $("#navCutoutDoc .tr-header").nextAll().remove();
        $("#navCutoutDoc .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            type: 1
        }, function (data) {
            $("#navCutoutDoc .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/cutoutdoc.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);
                    console.log(innerhtml);
                    $("#navCutoutDoc .tr-header").after(innerhtml);
                });
            } else {
                $("#navCutoutDoc .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            ObjectJS.isLoading = true;
        });

    }

    //缝制记录
    ObjectJS.getSewnDoc = function () {
        var _self = this;
        $("#navSewnDoc .tr-header").nextAll().remove();
        $("#navSewnDoc .tr-header").after("<tr><td colspan='10'><div class='data-loading' ><div></td></tr>");
        ObjectJS.isLoading = false;
        Global.post("/Orders/GetGoodsDocByOrderID", {
            orderid: _self.orderid,
            type: 11
        }, function (data) {
            $("#navSewnDoc .tr-header").nextAll().remove();
            if (data.items.length > 0) {
                doT.exec("template/orders/cutoutdoc.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#navSewnDoc .tr-header").after(innerhtml);
                });
            } else {
                $("#navSewnDoc .tr-header").after("<tr><td colspan='10'><div class='nodata-txt' >暂无数据!<div></td></tr>");
            }
            ObjectJS.isLoading = true;
        });

    }

    //裁剪录入
    ObjectJS.cutOutGoods = function () {
        var _self = this;
        doT.exec("template/orders/cutoutgoods.html", function (template) {
            console.log(_self.model.OrderGoods);
            var innerText = template(_self.model.OrderGoods);
            Easydialog = require("easydialog");
            Easydialog.open({
                container: {
                    id: "showCutoutGoods",
                    header: "大货单裁片登记",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showCutoutGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (details.length > 0 || $("#showCutoutGoods .check").hasClass("ico-checked")) {
                            ObjectJS.isLoading = false;
                            Global.post("/Orders/CreateOrderCutOutDoc", {
                                orderid: _self.orderid,
                                doctype: 1,
                                isover: $("#showCutoutGoods .check").hasClass("ico-checked") ? 1 : 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.id) {
                                    alert("裁片登记成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("裁片登记失败！");
                                }
                                ObjectJS.isLoading = true;
                            });
                        } else {
                            alert("请输入裁剪数量");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });
            $("#showCutoutGoods .check").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            $("#showCutoutGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                }
            });
        });
    };

    //车缝录入
    ObjectJS.sewnGoods = function () {
        var _self = this;
        doT.exec("template/orders/sewn-goods.html", function (template) {
            var innerText = template(_self.model.OrderGoods);
            Easydialog = require("easydialog");
            Easydialog.open({
                container: {
                    id: "showSewnGoods",
                    header: "大货单缝制登记",
                    content: innerText,
                    yesFn: function () {
                        var details = ""
                        $("#showSewnGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (details.length > 0) {
                            ObjectJS.isLoading = false;
                            Global.post("/Orders/CreateOrderSewnDoc", {
                                orderid: _self.orderid,
                                doctype: 11,
                                isover: $("#showSewnGoods .check").hasClass("ico-checked") ? 1 : 0,
                                expressid: "",
                                expresscode: "",
                                details: details,
                                remark: $("#expressRemark").val().trim()
                            }, function (data) {
                                if (data.id) {
                                    alert("缝制登记成功!", location.href);
                                } else if (data.result == "10001") {
                                    alert("您没有操作权限!")
                                } else {
                                    alert("缝制登记失败！");
                                };
                                ObjectJS.isLoading = true;
                            });
                        } else {
                            alert("请输入车缝数量");
                            return false;
                        }
                    },
                    callback: function () {

                    }
                }
            });
            $("#showSewnGoods .check").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            $("#showSewnGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                } else if (_this.val() > _this.data("max")) {
                    _this.val(_this.data("max"));
                }
            });
        });
    };

    //发货
    ObjectJS.sendGoods = function () {
        var _self = this;
        doT.exec("template/orders/sendordergoods.html", function (template) {
            var innerText = template(_self.model.OrderGoods);
            Easydialog = require("easydialog");
            Easydialog.open({
                container: {
                    id: "showSendOrderGoods",
                    header: "大货单发货",
                    content: innerText,
                    yesFn: function () {

                        var details = ""
                        $("#showSendOrderGoods .list-item").each(function () {
                            var _this = $(this);
                            var quantity = _this.find(".quantity").val();
                            if (quantity > 0) {
                                details += _this.data("id") + "-" + quantity + ",";
                            }
                        });
                        if (details.length > 0) {
                            if (!$("#expressid").data("id") || !$("#expressCode").val()) {
                                alert("请完善快递信息!");
                                return false;
                            }
                        } else if (!$("#showSendOrderGoods .check").hasClass("ico-checked")) {
                            alert("请输入发货数量");
                            return false;
                        }
                        ObjectJS.isLoading = false;
                        Global.post("/Orders/CreateOrderSendDoc", {
                            orderid: _self.orderid,
                            doctype: 2,
                            isover: $("#showSendOrderGoods .check").hasClass("ico-checked") ? 1 : 0,
                            expressid: $("#expressid").data("id"),
                            expresscode: $("#expressCode").val(),
                            details: details,
                            remark: $("#expressRemark").val().trim()
                        }, function (data) {
                            if (data.id) {
                                alert("发货成功!", location.href);
                            } else if (data.result == "10001") {
                                alert("您没有操作权限!")
                            } else {
                                alert("发货失败！");
                            };
                            ObjectJS.isLoading = true;
                        });

                    },
                    callback: function () {

                    }
                }
            });
            //快递公司
            require.async("dropdown", function () {
                var dropdown = $("#expressid").dropdown({
                    prevText: "",
                    defaultText: "请选择",
                    defaultValue: "",
                    data: _self.express,
                    dataValue: "ExpressID",
                    dataText: "Name",
                    width: "180",
                    isposition: true,
                    onChange: function (data) {

                    }
                });
            });
            $("#showSendOrderGoods .check").click(function () {
                if (!ObjectJS.isLoading) {
                    return;
                }
                var _this = $(this);
                if (!_this.hasClass("ico-checked")) {
                    _this.addClass("ico-checked").removeClass("ico-check");
                } else {
                    _this.addClass("ico-check").removeClass("ico-checked");
                }
            });
            $("#showSendOrderGoods").find(".quantity").change(function () {
                var _this = $(this);
                if (!_this.val().isInt() || _this.val() <= 0) {
                    _this.val("0");
                } else if (_this.val() > _this.data("max")) {
                    _this.val(_this.data("max"));
                }
            });
        });
    };



    //加载快递公司列表
    ObjectJS.getExpress = function () {
        Global.post("/Plug/GetExpress", {}, function (data) {
            ObjectJS.express = data.items;
        });
    }
    module.exports = ObjectJS;
});