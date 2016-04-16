/**
 * タブ切り替えライブラリ
 */
'use strict';


$.fn.tabstrip = function (opt) {
    return this.each(function () {
        var elem = $(this);
        opt = $.extend(true, {
        }, opt);
        var ul = elem.children('ul');
        var tabs = ul.find('li');
        var panes = elem.children('div');
        ul.wrap('<div class="g-tabstrip-box"></div>');
        var wrapper = ul.closest('.g-tabstrip-box');

        function select(tab) {
            if (!tab.is('.selected') && !tab.is('.disabled')) {
                var pane = tab.data('tab-pane');
                if (pane) {
                    tabs.removeClass('selected');
                    tab.addClass('selected');
                    panes.hide();
                    pane.show();
                    if (opt.selected) {
                        opt.selected(tab);
                    }
                }
            }
        }


        ul.addClass('g-tabstrip-row');
        panes.addClass('g-tabstrip-pane');
        tabs.addClass('g-tabstrip-tab');
        wrapper.after('<div class="g-tabstrip-line"></div>');

        tabs.each(function (i, li) {
            var tab = $(li);
            var pane = panes.eq(i);
            tab.data('tab-pane', pane);
            tab.css('width', (100 / tabs.length) + '%');
        });
        tabs.click(function () {
            select($(this));
        });
        select(tabs.eq(opt.initial || 0));
    });
};
