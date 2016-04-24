'use strict';

var Util = require('./Util');
var Sender = require('./Sender');
var Status = require('./Status');
var Logger = require('./Logger');

var alertTimer;
var playAlermTimer;
var alermPlaying;
var prevAIRStatus;

var soundInitialized = false;
var soundInitDialogShowing = false;

var Message = {
    /**
     * 静かに表示
     * @param {string} message メッセージ
     * @param {Number} duration メッセージを消す時間(msec)
     */
    show: function (message, duration) {
        $('#message-pane .alert.alert-info').remove();
        var div = $('<div class="alert alert-info">'
                + Util.escape(message) + '</div>');
        $('#message-pane').prepend(div);
        if (duration) {
            setTimeout(function () {
                div.slideUp(function () {
                    div.remove();
                });
            }, duration);
        }
    },
    /**
     * エラーメッセージ表示
     * @param {string} message メッセージ
     * @param {Number} duration メッセージを消す時間(msec)
     * @param {bool} withAlerm true指定すると音つき
     */
    alert: function (message, duration, withAlerm) {
        //$('#message-pane .alert.alert-danger').remove();
        if (alertTimer) {
            clearTimeout(alertTimer);
        }
        if (message) {
            var div = $('<div class="alert alert-danger">'
                    + Util.escape(message)
                    + '</div>');
            $('#message-pane').prepend(div);
            if (duration) {
                alertTimer = setTimeout(function () {
                    div.slideUp(function () {
                        div.remove();
                    });
                }, duration);
            }
        }
        if (withAlerm && soundInitialized) {
            var alerm = $('#alerm-sound');
            if (alerm.length) {
                if (!alermPlaying) {
                    alermPlaying = true;
                    alerm[0].play();
                    Logger.log(alerm[0], "alerm play");
                    setTimeout(function () {
                        alerm[0].pause();
                        alermPlaying = false;
                    }, 2000);
                }
            }
        }
    },
    stopAlerm: function() {
        if (alermPlaying) {
            var alerm = $('#alerm-sound');
            alerm[0].pause();
            alermPlaying = false;
        }
    },
    initSound: function() {
        if (!soundInitialized) {
            var alerm = $('#alerm-sound')[0];
            alerm.play();
            alerm.pause();
            soundInitialized = true;
        }
    },
    initSoundWithDialog: function(message) {
        if (soundInitialized) {
            return;
        }
        if (soundInitDialogShowing) {
            return;
        }
        soundInitDialogShowing = true;
        
        var mem = $('<div><div class="m">' + message + '</div><div class="b"></div></div>');
        mem.css({
            position: 'fixed',
            top: 0,
            left: 0,
            width: '100%',
            height: '100%',
            backgroundColor: 'rgba(0,0,0,0.75)'
        });
        mem.find('.m').css({
            color: 'white',
            fontSize: '200%',
            margin: '100px 20px',
            textAlign: 'center'
        });
        var btn = $('<button type="button">OK</button>');
        btn.css({
            padding: '10px 30px',
            fontSize: '200%',
            color: 'black',
            borderRadius: 4,
            background: '#ccc'
        });
        mem.find('.b').css({
            textAlign: 'center'
        }).append(btn);
        btn.click(function() {
            Message.initSound();
            setTimeout(function() {
                mem.remove();
            }, 100);
        });
        $('body').append(mem);
        
    }
};


Sender.onMessage('ErrorMessage', function(msg) {
    Message.alert(msg, 3000);
});
Sender.onMessage('InfoMessage', function(msg) {
    Message.show(msg, 3000);
});
Sender.onMessage('TempAlerm', function(msg) {
    Message.alert(msg, 5000, true);
});
Sender.onMessage('TempWarn', function(msg) {
    Message.alert(msg, 5000, false);
});
Sender.onMessage('ConnectionRefused', function(msg) {
    Message.alert(msg);
    $('body').addClass('refused');
    $('a,button').prop('disabled', true);
    $('a').attr('href', '#');
});
Sender.onMessage('ErrorClose', function(msg) {
    Message.alert(msg);
});

function onStatusChange(st) {
    if (st.AIRStatus === 'NotConnected') {
        Message.show('赤外線カメラが接続されていません。');
    } else if (st.AIRStatus === 'NotInitialized') {
        Message.show('赤外線カメラが初期化されていません。');
    } else if (st.AIRStatus === 'Initializing') {
        Message.show('赤外線カメラの初期化中です。');
    } else if ((prevAIRStatus === null || prevAIRStatus === 'Initializing') && st.AIRStatus === 'Initialized') {
        Message.show('赤外線カメラの初期化完了しました。', 1000);
    }
    if (!st.Recording) {
        Message.stopAlerm();
    }
    prevAIRStatus = st.AIRStatus;
}

Status.onChange(onStatusChange);
$(function() {
    Status.get(onStatusChange);
    var alerm = $('#alerm-sound');
    if (alerm.length) {
        var al = alerm[0];
        //al.play();
        //al.pause();
    }
});

module.exports = Message;
