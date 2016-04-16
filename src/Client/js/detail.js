/*
 * detail
 * 詳細設定画面
 */
'use strict';
require('./modules/Prefs');
require('./modules/SiteForm');
require('./modules/Signaltemp');
require('./modules/Tabstrip');
require('./modules/Viewer');
require('./lib/jquery.countdown.js');
var Sender = require('./modules/Sender');
var Status = require('./modules/Status');


function onStatusChange(st) {
    if (st.Recording && !onStatusChange.redirected) {
        onStatusChange.redirected = true;
        alert('記録中のため、記録画面に移動します。');
        location.href = 'index.html';
    }
}

$(function() {
    $('#detail-pane').tabstrip({});
    $('#restart-button').click(function() {
        Sender.send('restart', {
            success: function() {
                $.countdown({
                    count: 10,
                    message: '再起動中です...',
                    hideCancel: true,
                    completed: function() {
                        location.reload();
                    }
                });
            }
        });
    });
    $('#shutdown-button').click(function() {
        $.countdown({
            count: 10,
            message: '融着機PCをシャットダウンしてよろしいですか？',
            dismissOnComplete: false,
            completed: function() {
                Sender.send('shutdown', {
                    success: function() {
                        alert('PCの電源が切れるまでお待ちください。');
                    }
                });
            }
        });
    });
    Status.get(onStatusChange);
    Status.onChange(onStatusChange);
});
