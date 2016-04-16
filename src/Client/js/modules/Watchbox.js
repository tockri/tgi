'use strict';
/*
 * Watchbox
 * 監視窓の操作系統
 */


var Sender = require('./Sender');

var listeners = [];
/**
 * メイン監視枠の表示を更新
 * @param {object} data
 */
function updateMainDimension(data) {
    Watchbox.Main = data;
    var $b1 = $('#watchbox1-1');
    var $b2 = $('#watchbox1-2');
    $b1.show();
    if (data.Mode.toLowerCase() === 'single') {
        $b2.hide();
        $b1.css({
            left: data.Left,
            top: data.Top,
            width: data.Width,
            height: data.Height
        });
    } else {
        var sw = (data.Width - data.Gap) / 2;
        $b1.css({
            left: data.Left,
            top: data.Top,
            width: sw,
            height: data.Height
        });
        $b2.css({
            left: data.Left + sw + data.Gap,
            top: data.Top,
            width: sw,
            height: data.Height
        });
        $b2.show();
    }
}
/**
 * サブ監視枠の表示を更新
 * @param {object} data
 */
function updateSubDimension(data) {
    Watchbox.Sub = data;
    var $b = $('#watchbox2');
    $b.css({
        left: data.Left,
        top: data.Top,
        width: data.Width,
        height: data.Height
    });
    $b.show();
}

function fireWatchboxChanged() {
    if (listeners) {
        $.each(listeners, function(i, l) {
            l();
        });
    }
}

/**
 * 監視枠クラスのインターフェイス
 */
var Watchbox = {
    /**
     * メイン監視枠の座標情報
     */
    Main: null,
    /**
     * サブ監視枠の座標情報
     */
    Sub: null,
    /**
     * Watchboxが変化した時に呼ばれるリスナを登録
     * @param {function} l
     */
    onChange: function(l) {
        if (typeof(l) === 'function') {
            listeners.push(l);
        }
    },
    updated: function(data) {
        updateMainDimension(data.Main);
        updateSubDimension(data.Sub);
        fireWatchboxChanged();
    }
};

Sender.onMessage('Watchbox', function (wbs) {
    $(function() {
        updateMainDimension(wbs.Main);
        updateSubDimension(wbs.Sub);
        fireWatchboxChanged();
    });
});

$(function() {
    Sender.send('watchbox', {
        success: function(data) {
            updateMainDimension(data.Main);
            updateSubDimension(data.Sub);
            fireWatchboxChanged();
        }
    });
});

module.exports = Watchbox;
