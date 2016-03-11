define(function (require, exports, module) {
    var doT = require("dot");
    var Global = require("global");
    var Easydialog = require("easydialog");
    require("pager");

    var ObjectJS = {};
    var CacheAttrValues = [];//订单品类属性缓存
    var Editor;

    ///taskid：任务id
    ///orderid:订单id
    ///stageid：订单阶段id
    ///mark:任务标记 1：材料 2 制版 3大货材料
    ///finishStatus：任务完成状态
    ///attrValues:订单制版属性
    ObjectJS.init = function (taskid, orderid, stageid, mark, finishStatus, attrValues, orderType, um, plateRemark) {
        ObjectJS.orderid = orderid;
        ObjectJS.stageid = stageid;
        ObjectJS.taskid = taskid;
        ObjectJS.orderType = orderType;
        if(attrValues!="")
            CacheAttrValues=JSON.parse(attrValues.replace(/&quot;/g, '"'));
        Editor = um;

        ObjectJS.mark = 1;
        if (mark == 2)
        {
            ObjectJS.mark = 2;
            $("#navProducts").hide();
            $("#platemakingContent").show();
            $(".tab-nav-ul li").removeClass("hover").eq(1).addClass("hover");
        }

        ObjectJS.bindEvent();
        ObjectJS.initTalkReply();

        //材料任务
        if ((mark == "1" || mark == "3") && finishStatus!=2) {
            ObjectJS.bindProduct();

            ObjectJS.bindRemoveRowBtn(); 
        }//制版任务
        else if (mark == "2" && finishStatus != 2) {
            ObjectJS.bindPlatemakingEvent();

            ObjectJS.getAmount2();
        }
        else {
            ObjectJS.getAmount2();
            ObjectJS.bindRemoveRowBtn();
        }

        if (Editor) {
            //制版工艺描述 富文本
            Editor.ready(function () {
                Editor.setContent(decodeURI(plateRemark));

                $(".edui-container").click(function () {
                    $(".edui-body-container").animate({ height: "600px" }, 500);
                });


                $(document).click(function (e) {
                    //隐藏制版列操作下拉框
                    if (!$(e.target).parents().hasClass("replyBox") && !$(e.target).hasClass("replyBox")) {

                        $(".replyBox").removeClass("autoHeight");
                    }


                    //隐藏制版列操作下拉框
                    if (!$(e.target).parents().hasClass("edui-container") && !$(e.target).hasClass("edui-container")) {
                        $(".edui-body-container").animate({ height: "100px" }, 500);
                    }
                });

            });
        }

        if ($("#PlateRemark").length == 1)
        {
            if (plateRemark != "") {
                $("#PlateRemark").html(decodeURI(plateRemark));
            }
            else {
                $(".edui-container").hide();
            }
        }


    };

    //任务基本信息操作事件
    //更新任务到期日期
    ObjectJS.bindEvent = function () {

        setTimeout(function () {
            if ($("#UpdateTaskEndTime").length == 1) {
                //更新任务到期日期
                var taskEndTime = {
                    elem: '#UpdateTaskEndTime',
                    format: 'YYYY-MM-DD',
                    min: laydate.now(),
                    max: '2099-06-16',
                    istime: false,
                    istoday: false,
                    choose: function () {
                        ObjectJS.UpdateTaskEndTime(ObjectJS.taskid);
                    }
                };
                laydate(taskEndTime);
            }
        }, 300);

        //标记任务完成
        $("#FinishTask").click(function () {
            ObjectJS.FinishTask();
        });

        //切换模块
        $(".tab-nav-ul li").click(function () {
            var _this = $(this);
            if (_this.hasClass("hover")) return;

            _this.siblings().removeClass("hover");
            _this.addClass("hover");
            $(".tab-nav").nextAll().hide();
            $("#" + _this.data("id")).show();

            ObjectJS.mark = _this.data("mark");

            if (_this.data("id") == "orderTaskLogs") {
                //任务讨论列表
                ObjectJS.getLogs(1);
            }
            else {
                $("#taskReplys").show();
                //任务讨论列表
                ObjectJS.initTalkReply();
            }
 
        });

        $(".replyBox").click(function () {

            $(this).addClass("autoHeight");
            $(this).find(".replyContent").focus();
        });

        $(document).click(function (e) {
            //隐藏制版列操作下拉框
            if (!$(e.target).parents().hasClass("replyBox") && !$(e.target).hasClass("replyBox")) {
                
                $(".replyBox").removeClass("autoHeight");
            }

            
            //隐藏制版列操作下拉框
            //if (!$(e.target).parents().hasClass("edui-container") && !$(e.target).hasClass("edui-container")) {
            //    $(".edui-body-container").animate({ height: "100px" }, 500);
            //}
        });

    }

    //更改任务到期时间
    ObjectJS.UpdateTaskEndTime = function () {
        Global.post("/Task/UpdateTaskEndTime", { taskID: ObjectJS.taskid, endTime: $("#UpdateTaskEndTime").val() }, function (data) {
            if (data.Result != 1) {
                alert("保存失败");
            }
        });
    }

    //标记任务完成
    ObjectJS.FinishTask = function () {
        confirm("标记完成的任务不可逆,确定完成?", function () {
            Global.post("/Task/FinishTask",
                {
                    taskID: ObjectJS.taskid,
                    orderID: ObjectJS.orderid,
                    orderType: ObjectJS.orderType
            }, function (data) {
                if (data.Result == 1) {
                    $("#FinishTask").addClass("btnccc").val("已完成").attr("disabled", "disabled");
                }
                else if (data.Result == 2) {
                    alert("前面阶段任务有未完成,不能标记完成");
                }
                else if (data.Result == 3) {
                    alert("无权限操作");
                }
                else if (data.Result == -1) {
                    alert("保存失败");
                }
            });
        });
    }

    //初始化任务讨论列表
    ObjectJS.initTalkReply = function () {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");

            if (txt.val().trim()) {
                var model = {
                    GUID: ObjectJS.orderid,
                    StageID: ObjectJS.stageid,
                    mark:ObjectJS.mark,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                ObjectJS.saveTaskReply(model);

                txt.val("");
            }

        });

        ObjectJS.getTaskReplys(1);

    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='2'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/Opportunitys/GetReplys", {
            guid: ObjectJS.orderid,
            stageid: ObjectJS.stageid,
            mark:ObjectJS.mark,
            pageSize: 10,
            pageIndex: page
        }, function (data) {
            $("#replyList").empty();
            if (data.items.length > 0) {
                doT.exec("template/customer/replys.html", function (template) {
                    var innerhtml = template(data.items);
                    innerhtml = $(innerhtml);

                    $("#replyList").html(innerhtml);

                    innerhtml.find(".btn-reply").click(function () {
                        var _this = $(this), reply = _this.nextAll(".reply-box");
                        reply.slideDown(500);
                        reply.find("textarea").focus();
                        reply.find("textarea").blur(function () {
                            if (!$(this).val().trim()) {
                                reply.slideUp(200);
                            }
                        });
                    });

                    innerhtml.find(".save-reply").click(function () {
                        var _this = $(this);
                        if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                            var entity = {
                                GUID: _this.data("id"),
                                StageID: _this.data("stageid"),
                                Mark: ObjectJS.mark,
                                Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                                FromReplyID: _this.data("replyid"),
                                FromReplyUserID: _this.data("createuserid"),
                                FromReplyAgentID: _this.data("agentid")
                            };

                            ObjectJS.saveTaskReply(entity);
                        }

                        $("#Msg_" + _this.data("replyid")).val('');
                        $(this).parent().slideUp(100);
                    });

                });
            }
            else {
                $("#replyList").html("<tr><td colspan='2'><div class='noDataTxt' >暂无评论!<div></td></tr>");
            }

            $("#pagerReply").paginate({
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
                    ObjectJS.getTaskReplys(page);
                }
            });
        });
    }

    //保存任务讨论
    ObjectJS.saveTaskReply = function (model) {
        var _self = this;

        Global.post("/Opportunitys/SavaReply", { entity: JSON.stringify(model) }, function (data) {
            doT.exec("template/customer/replys.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);

                $("#replyList .noDataTxt").parent().parent().remove();

                $("#replyList").prepend(innerhtml);

                innerhtml.find(".btn-reply").click(function () {
                    var _this = $(this), reply = _this.nextAll(".reply-box");
                    reply.slideDown(500);
                    reply.find("textarea").focus();
                    reply.find("textarea").blur(function () {
                        if (!$(this).val().trim()) {
                            reply.slideUp(200);
                        }
                    });
                });

                innerhtml.find(".save-reply").click(function () {
                    var _this = $(this);
                    if ($("#Msg_" + _this.data("replyid")).val().trim()) {
                        var entity = {
                            GUID: _this.data("id"),
                            Content: $("#Msg_" + _this.data("replyid")).val().trim(),
                            FromReplyID: _this.data("replyid"),
                            FromReplyUserID: _this.data("createuserid"),
                            FromReplyAgentID: _this.data("agentid")
                        };
                        ObjectJS.saveTaskReply(entity);

                    }
                    $("#Msg_" + _this.data("replyid")).val('');
                    $(this).parent().slideUp(100);
                });

            });
        });
    }

    //获取任务日志
    ObjectJS.getLogs = function (page) {
        var _self = this;
        $("#taskLogList").empty();

        Global.post("/Task/GetOrderTaskLogs", {
            taskID: _self.taskid,
            pageindex: page
        }, function (data) {

            doT.exec("template/common/logs.html", function (template) {
                var innerhtml = template(data.items);
                innerhtml = $(innerhtml);
                $("#taskLogList").append(innerhtml);
            });

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
        });
    }

    //任务材料操作事件
    ObjectJS.bindProduct = function () {
        ObjectJS.getAmount();

        //编辑数量
        $(".quantity").change(function () {
            if ($(this).val().isDouble() && $(this).val() > 0) {
                ObjectJS.editQuantity($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //编辑损耗
        $(".loss").change(function () {
            if ($(this).val().isDouble() && $(this).val() >= 0) {
                ObjectJS.editLoss($(this));
            } else {
                $(this).val($(this).data("value"));
            }
        });

        //删除产品
        $(".ico-del").click(function () {
            var _this = $(this);
            confirm("确认从清单中移除此材料吗？", function () {
                Global.post("/Orders/DeleteProduct", {
                    orderid: _self.orderid,
                    autoid: _this.data("id"),
                    name: _this.data("name")
                }, function (data) {
                    if (!data.status) {
                        alert("系统异常，请重新操作！");
                    } else {
                        _this.parents("tr.item").remove();
                        ObjectJS.getAmount();
                    }
                });
            });
        });

    };

    //更改材料单价
    ObjectJS.editPrice = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateOrderPrice", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            price: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.parent().nextAll(".amount").html((ele.parent().prevAll(".tr-quantity").find("label").text() * ele.val()).toFixed(2));
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //更改消耗量
    ObjectJS.editQuantity = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductQuantity", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //更改损耗量
    ObjectJS.editLoss = function (ele) {
        var _self = this;
        Global.post("/Orders/UpdateProductLoss", {
            orderid: _self.orderid,
            autoid: ele.data("id"),
            name: ele.data("name"),
            quantity: ele.val()
        }, function (data) {
            if (!data.status) {
                ele.val(ele.data("value"));
                alert("当前订单状态,不能进行修改");
            } else {
                ele.data("value", ele.val());
                _self.getAmount();
            }
        });
    }

    //计算总金额
    ObjectJS.getAmount = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            
            if (_this.prevAll(".tr-loss").find("input").length>0)
                _this.html(((_this.prevAll(".tr-quantity").html() * 1 + _this.prevAll(".tr-loss").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            else
                _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1 ) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
        $("#totalMoney").text((amount * $("#planQuantity").text()).toFixed(2));
    }

    ObjectJS.getAmount2 = function () {
        var amount = 0;
        $(".amount").each(function () {
            var _this = $(this);
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
    }




    //任务制版相关事件
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
        $(document).unbind().bind("click", function (e) {
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
            //$(".tbContentIpt:visible").each(function () {
            //    $(this).hide().prev().html($(this).val()).show();
            //});

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
            for (var i = 0;len = CacheAttrValues.length,i < len; i++)
            {
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

                        //setTimeout(function () {
                        //    $("#platemakingBody td[data-columnname='" + ObjectJS.columnnameid + "']:gt(0)").next().find(".tbContentIpt").show();
                        //}, 100);
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
            $newTR.find(".tbContentIpt").empty().show();
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
    ObjectJS.bindRemoveRowBtn = function () {
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

            if (CacheAttrValues.length==0) {
                noHaveLi = true;
                innerHtml = '<div style="width:300px;">制版属性没有配置,无选择</div>';
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
                        newColumnHeadr += '<td class="width100"></td>';

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

                        $("#platemakingBody").html(tableHtml).css({ "border-top": "1px solid #eee", "border-left": "1px solid #eee" });

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
        $(".tbContentIpt:visible").each(function () {
            $(this).attr("value", $(this).val() ).hide().prev().html($(this).val()).show();
        });

        var ValueIDs = '';
        $("#platemakingBody .tr-header td.columnHeadr").each(function () {
            ValueIDs += $(this).data("id")+'|';
        });

        Global.post("/Task/UpdateOrderPlateAttr", {
            orderID: ObjectJS.orderid,
            platehtml: encodeURI($("#platemakingBody").html()),
            taskID: ObjectJS.taskid,
            valueIDs: ValueIDs
        }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        });
    }

    //保存制版工艺说明
    ObjectJS.updateOrderPlateRemark = function () {
        Global.post("/Task/UpdateOrderPlateRemark", {
            orderID: ObjectJS.orderid,
            plateRemark: encodeURI(Editor.getContent())
        }, function (data) {
            if (data.Result == 1) {
                $(".edui-body-container").animate({ height: "100px" }, 500);
            }
        });
    }

    module.exports = ObjectJS;
});