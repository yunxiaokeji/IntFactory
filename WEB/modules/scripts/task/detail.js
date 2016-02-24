define(function (require, exports, module) {
    var doT = require("dot");
    var Global = require("global");
    require("pager");

    var ObjectJS = {};

    ObjectJS.init = function (taskid, stageid, orderid, mark) {
        ObjectJS.orderid = orderid;
        ObjectJS.taskid = taskid;

        ObjectJS.bindEvent(taskid, stageid, orderid);
        if (mark == "1") {
            ObjectJS.bindProduct();
        }
        else if (mark == "2") {
            ObjectJS.bindPlatemakingEvent();
        }
    };

    //任务制版相关事件
    ObjectJS.bindPlatemakingEvent = function () {
        ObjectJS.bindDocumentClick();
        ObjectJS.binddropdown();

        ObjectJS.bindContentClick();
        ObjectJS.bindInputBlur();

        ObjectJS.bindAddColumn();
        ObjectJS.bindRemoveColumn();

        ObjectJS.bindAddRow();
        ObjectJS.bindRemoveRow();
    };

    ObjectJS.bindDocumentClick = function () {
        $(document).unbind().bind("click", function (e) {
            //隐藏下拉
            if (!$(e.target).parents().hasClass("ico-dropdown") && !$(e.target).hasClass("ico-dropdown")) {
                $(".dropdown-ul").hide();
            }

            if (!$(e.target).parents().hasClass("tbContentIpt") && !$(e.target).hasClass("tbContentIpt") && !$(e.target).hasClass("tbContent") && !$(e.target).parents().hasClass("tbContent")) {
                
                $(".tbContentIpt:visible").each(function () {
                    $(this).hide().prev().html($(this).val()).show();
                });
            }
        });


    }
    
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

    ObjectJS.bindContentClick = function () {
        $(".table-list span.tbContent").unbind().bind("click", function () {
            $(this).hide().next().show();
        });
    }

    ObjectJS.bindInputBlur = function () {
        $(".table-list .tbContentIpt").unbind().bind("blur", function () {
            $(this).hide().prev().html($(this).val()).show();
        });
    }

    ObjectJS.bindAddColumn = function () {
        $("#btn-addColumn").unbind().bind("click", function () {
            var date = new Date();
            var columnnameid = date.toLocaleString()+date.getMilliseconds();
            var newColumnHeadr = '<td class="width50 tLeft" data-columnname="columnname' + columnnameid + '"><span class="tbContent">新列名</span><input class="hide tbContentIpt" value="新列名" type="text"/><span class="ico-dropdown mLeft10" data-columnname="columnname' + columnnameid + '"></span></td>';
            $("td[data-columnname='" + $(this).data("columnname") + "']").eq(0).after(newColumnHeadr);

            var newColumn = '<td class="tLeft width50" data-columnname="columnname' + columnnameid + '"><span class="tbContent">无内容</span><input class="hide tbContentIpt" value="无内容" type="text"/></td>';
            $("td[data-columnname='" + $(this).data("columnname") + "']:gt(0)").after(newColumn);

            ObjectJS.binddropdown();
            ObjectJS.bindContentClick();
            ObjectJS.bindInputBlur();
        });
    }
    
    ObjectJS.bindRemoveColumn = function () {
        $("#btn-removeColumn").unbind().bind("click", function () {
            if ($(".tr-header td").length == 2) {
                alert("只剩最后一列,不能删除");
                return;
            }

            $("td[data-columnname='" + $(this).data("columnname") + "']").remove();
        });
    }

    ObjectJS.bindAddRow = function () {
        $("span.btn-addRow").unbind().bind('click',function () {
            var $newTR = $("<tr>" + $(this).parent().parent().html() + "</tr>");
            $(this).parent().parent().after($newTR);

            ObjectJS.bindContentClick();
            ObjectJS.bindInputBlur();

            ObjectJS.bindAddRow();
            ObjectJS.bindRemoveRow();
        });
    }

    ObjectJS.bindRemoveRow = function () {
        $("span.btn-removeRow").unbind().bind('click', function () {
            if ($("span.btn-removeRow").length == 1)
            {
                alert("只剩最后一行,不能删除");
                return;
            }

            $(this).parent().parent().remove();
        });
    }


    //任务基本信息操作事件
    //更新任务到期日期
    ObjectJS.bindEvent = function (taskid, stageid, orderid) {
        setTimeout(function () {
            //更新任务到期日期
            var taskEndTime = {
                elem: '#UpdateTaskEndTime',
                format: 'YYYY-MM-DD',
                max: '2099-06-16',
                istime: false,
                istoday: false,
                choose: function () {
                    ObjectJS.UpdateTaskEndTime(ObjectJS.taskid);
                }
            };
            laydate(taskEndTime);
        }, 300);

        //标记任务完成
        $("#FinishTask").click(function () {
            ObjectJS.FinishTask(taskid);
        });

        //任务讨论列表
        ObjectJS.initTalkReply(orderid, stageid);
    }

    ObjectJS.UpdateTaskEndTime = function (taskID) {
        Global.post("/Task/UpdateTaskEndTime", { taskID: taskID, endTime: $("#UpdateTaskEndTime").val() }, function (data) {
            if (data.Result == 1) {
                alert("保存成功");
            }
        });
    }

    //标记任务完成
    ObjectJS.FinishTask = function (taskID) {
        confirm("标记完成的任务不可逆,确定完成?", function () {
            Global.post("/Task/FinishTask", { taskID: taskID }, function (data) {
                if (data.Result == 1) {
                    alert("标记任务完成");
                    $("#FinishTask").addClass("btnccc").val("已完成").attr("disabled", "disabled");
                }
                else if (data.Result == 2) {
                    alert("前面阶段任务有未完成,不能标记完成");
                }
                else if (data.Result == 3) {
                    alert("无权限操作");
                }
            });
        });
    }


    //初始化任务讨论列表
    ObjectJS.initTalkReply = function (orderid, stageid) {
        var _self = this;

        $("#btnSaveTalk").click(function () {
            var txt = $("#txtContent");

            if (txt.val().trim()) {
                var model = {
                    GUID: orderid,
                    StageID: stageid,
                    Content: txt.val().trim(),
                    FromReplyID: "",
                    FromReplyUserID: "",
                    FromReplyAgentID: ""
                };
                ObjectJS.saveTaskReply(model);

                txt.val("");
            }

        });

        ObjectJS.getTaskReplys(orderid, stageid, 1);

    }

    //获取任务讨论列表
    ObjectJS.getTaskReplys = function (orderid, stageid, page) {
        var _self = this;
        $("#replyList").empty();
        $("#replyList").html("<tr><td colspan='8'><div class='dataLoading'><img src='/modules/images/ico-loading.jpg'/><div></td></tr>");
        Global.post("/Opportunitys/GetReplys", {
            guid: orderid,
            stageid: stageid,
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
                $("#replyList").html("<tr><td colspan='8'><div class='noDataTxt' >暂无评论!<div></td></tr>");
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
                    ObjectJS.getTaskReplys(orderid, stageid, page);
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
                alert("价格修改失败，可能因为订单状态已改变，请刷新页面后重试！");
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
                alert("系统异常，请重新操作！");
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
                alert("系统异常，请重新操作！");
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
            _this.html(((_this.prevAll(".tr-quantity").find("input").val() * 1 + _this.prevAll(".tr-loss").find("input").val() * 1) * _this.prevAll(".tr-price").find("label").text()).toFixed(2));
            amount += _this.html() * 1;
        });
        $("#amount").text(amount.toFixed(2));
        $("#totalMoney").text((amount * $("#planQuantity").text()).toFixed(2));
    }

    module.exports = ObjectJS;
});