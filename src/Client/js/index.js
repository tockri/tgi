'use strict';
require('./modules/Prefs');
var Viewer = require('./modules/Viewer');
require('./modules/Watchbox');
var Message = require('./modules/Message');
var Sender = require('./modules/Sender');
var Status = require('./modules/Status');

function initButtons() {
    // 開始ボタン
    $('#startbutton').click(function() {
        Message.initSound();
        Sender.send('modstatus', {
            data: {
                Recording: true
            }
        });
    });
    // 停止ボタン
    $('#stopbutton').click(function() {
        Sender.send('modstatus', {
            data: {
                Recording: false
            }
        });
    });
    
}


/**
 * ステータス変更時のイベントリスナ
 * @param {Object} st
 */
function onStatusChange(st) {
    var start = $('#startbutton');
    var stop = $('#stopbutton');
    if (st.AIRStatus === 'Initialized') {
        start.attr('disabled', false);
        stop.attr('disabled', false);
    } else {
        start.attr('disabled', true);
        stop.attr('disabled', true);
    }
    if (st.Recording) {
        Message.initSoundWithDialog('記録中です。OKをクリックしてください。');
        stop.show();
        start.hide();
    } else {
        stop.hide();
        start.show();
    }
}


$(function() {
    Status.get(onStatusChange);
    Status.onChange(onStatusChange);
    initButtons();
//    window.Message = Message;
});