
define(function (require, exports, module) {
    require("plug/datapager/style.css");
    (function ($) {
        $.fn.paginate = function (options) {
            var opts = $.extend({}, $.fn.paginate.defaults, options);
            return this.each(function () {
                $this = $(this);
                var o = $.meta ? $.extend({}, opts, $this.data()) : opts;
                var selectedpage = o.start;
                $.fn.draw(o, $this, selectedpage);
            });
        };
        var outsidewidth_tmp = 0;
        var insidewidth = 0;
        var bName = navigator.appName;
        var bVer = navigator.appVersion;
        if (bVer.indexOf('MSIE 7.0') > 0)
            var ver = "ie7";
        $.fn.paginate.defaults = {
            total_count: 0,
            count: 5,
            start: 12,
            display: 5,
            border: true,
            border_color: '#4a9eee',
            text_color: '#333',
            background_color: '#fff',
            border_hover_color: '#4a9eee',
            text_hover_color: '#fff',
            background_hover_color: '#4a98e7',
            rotate: false,
            images: true,
            mouse: 'slide',
            float: "right",
            onChange: function () { return false; }
        };
        $.fn.draw = function (o, obj, selectedpage) {
            o.border_color = "#ccc";
            o.text_color = "#333";
            o.background_color = "#fff";
            o.border_hover_color = "#4a9eee";
            o.text_hover_color = "#fff";
            o.background_hover_color = "#4a98e7";

            if (o.display > o.count)
                o.display = o.count;
            if (o.count < 1) {
                $this.hide();
                return;
            }
            else
                $this.show();
            $this.css("width", o.display * 35 + 180 + ((o.total_count).toString().length * 8) + "px");
            $this.empty();
            if (o.images) {
                var spreviousclass = 'jPag-sprevious-img';
                var previousclass = 'jPag-previous-img';
                var snextclass = 'jPag-snext-img';
                var nextclass = 'jPag-next-img';
            }
            else {
                var spreviousclass = 'jPag-sprevious';
                var previousclass = 'jPag-previous';
                var snextclass = 'jPag-snext';
                var nextclass = 'jPag-next';
            }
            var _first = $(document.createElement('a')).addClass('jPag-first').html('上一页');

            var _divwrapleft = $(document.createElement('div')).addClass('jPag-control-back');
            _divwrapleft.append(_first);//.append(_rotleft)

            var _ulwrapdiv = $(document.createElement('div')).css('overflow', 'hidden');
            var _ul = $(document.createElement('ul')).addClass('jPag-pages')
            var c = (o.display - 1) / 2;
            var first = selectedpage - c;
            var selobj;
            for (var i = 0; i < o.count; i++) {
                var val = i + 1;
                if (val == selectedpage) {
                    var _obj = $(document.createElement('li')).html('<span class="jPag-current">' + val + '</span>');
                    selobj = _obj;
                    _ul.append(_obj);
                }
                else {
                    var _obj = $(document.createElement('li')).html('<a>' + val + '</a>');
                    _ul.append(_obj);
                }
            }
            _ulwrapdiv.append(_ul);

            var _last = $(document.createElement('a')).addClass('jPag-last').html('下一页');
            var _divwrapright = $(document.createElement('div')).addClass('jPag-control-front');
            _divwrapright.append(_last);//.append(_rotright)

            var _total_count = $(document.createElement('div')).addClass('jPag-count').html('第 <input value="' + o.start + '"></input>/' + o.count + ' 页 共 ' + o.total_count + ' 条');

            //append all:
            $this.addClass('jPaginate').append(_divwrapleft).append(_ulwrapdiv).append(_divwrapright).append(_total_count);

            if (o.float != "normal") {
                $this.css("float", o.float);
            }

            if (!o.border) {
                if (o.background_color == 'none') var a_css = { 'color': o.text_color };
                else var a_css = { 'color': o.text_color, 'background-color': o.background_color };
                if (o.background_hover_color == 'none') var hover_css = { 'color': o.text_hover_color };
                else var hover_css = { 'color': o.text_hover_color, 'background-color': o.background_hover_color };
            }
            else {
                if (o.background_color == 'none') var a_css = { 'color': o.text_color, 'border': '1px solid ' + o.border_color };
                else var a_css = { 'color': o.text_color, 'background-color': o.background_color, 'border': '1px solid ' + o.border_color };
                if (o.background_hover_color == 'none') var hover_css = { 'color': o.text_hover_color, 'border': '1px solid ' + o.border_hover_color };
                else var hover_css = { 'color': o.text_hover_color, 'background-color': o.background_hover_color, 'border': '1px solid ' + o.border_hover_color };
            }

            $.fn.applystyle(o, $this, a_css, hover_css, _first, _ul, _ulwrapdiv, _divwrapright);
            //calculate width of the ones displayed:
            var outsidewidth = outsidewidth_tmp - _first.parent().width() - 3;
            if (ver == 'ie7') {
                _ulwrapdiv.css('width', outsidewidth + 72 + 'px');
                _divwrapright.css('left', outsidewidth_tmp + 6 + 72 + 'px');
            }
            else {
                _ulwrapdiv.css('width', outsidewidth + 'px');
                _divwrapright.css('left', outsidewidth_tmp + 6 + 'px');
            }

            //first and last:
            _first.click(function (e) {
                if (o.start > 1) {
                    o.onChange(o.start - 1);
                    //_ulwrapdiv.find('li').eq(o.start - 2).click();
                }
            });
            _last.click(function (e) {
                if (o.start < o.count) {
                    o.onChange(o.start + 1);
                    //_ulwrapdiv.find('li').eq(o.start).click();
                }
            });

            _total_count.find("input").change(function () {
                var _this = $(this);
                if (_this.val().match(/^(0|([1-9]\d*))$/)) {
                    if (_this.val() <= 0) {
                        o.onChange(1);
                    } else if (_this.val() > o.count) {
                        o.onChange(o.count);
                    } else {
                        o.onChange(_this.val());
                    }
                } else {
                    _this.val(o.start);
                }
            });

            //click a page
            _ulwrapdiv.find('li').click(function (e) {
                selobj.html('<a>' + selobj.find('.jPag-current').html() + '</a>');
                var currval = $(this).find('a').html();
                $(this).html('<span class="jPag-current">' + currval + '</span>');
                selobj = $(this);
                $.fn.applystyle(o, $(this).parent().parent().parent(), a_css, hover_css, _first, _ul, _ulwrapdiv, _divwrapright);
                var left = (this.offsetLeft) / 2;
                var left2 = _ulwrapdiv.scrollLeft() + left;
                var tmp = left - (outsidewidth / 2);
                if (ver == 'ie7')
                    _ulwrapdiv.animate({ scrollLeft: left + tmp - _first.parent().width() + 52 + 'px' });
                else
                    _ulwrapdiv.animate({ scrollLeft: left + tmp - _first.parent().width() + 'px' });
                o.onChange(currval);
            });

            var last = _ulwrapdiv.find('li').eq(o.start - 1);
            last.attr('id', 'tmp');
            var left = document.getElementById('tmp').offsetLeft / 2;
            last.removeAttr('id');
            var tmp = left - (outsidewidth / 2);
            if (ver == 'ie7') _ulwrapdiv.animate({ scrollLeft: left + tmp - _first.parent().width() + 52 + 'px' });
            else _ulwrapdiv.animate({ scrollLeft: left + tmp - _first.parent().width() + 'px' }, 1);
        }

        $.fn.applystyle = function (o, obj, a_css, hover_css, _first, _ul, _ulwrapdiv, _divwrapright) {
            obj.find('a').css(a_css);
            obj.find('span.jPag-current').css(hover_css);
            obj.find('a').hover(
            function () {
                $(this).css(hover_css);
            },
            function () {
                $(this).css(a_css);
            }
            );
            obj.css('padding-left', _first.parent().width() + 5 + 'px');
            insidewidth = 0;

            obj.find('li').each(function (i, n) {
                if (i == (o.display - 1)) {
                    outsidewidth_tmp = this.offsetLeft + this.offsetWidth;
                }
                insidewidth += this.offsetWidth;
            })
            _ul.css('width', (insidewidth + 100) + 'px');
        }
    })(jQuery);
    module.exports = jQuery;
});