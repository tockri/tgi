'use strict';

var Prefs = require('./Prefs');
// 定数
var API_ROOT = Prefs.API_ROOT;
//var SOCKET_URL = 'ws://' + location.hostname + ':8002';
var SOCKET_URL = Prefs.SOCKET_URL;
// 
var Logger = require('./Logger');
/**
 * WebSocketのイベントリスナ
 * @type Array
 */
var listeners = {};
/**
 * WebSocket
 * @type WebSocket
 */
var socket = null;
/**
 * 接続エラー回数。5を超えたらリトライを中断する。
 * @type Number
 */
var connectErrorCount = 0;

/**
 * WebSocketメッセージ受信イベント
 * @param {object} evt
 */
function onSocketMessage(evt) {
    connectErrorCount = 0;
    var data = evt.data;
    if (data.constructor === String) {
        var m = data.match(/^([^\t]+)\t([\s\S]+)/);
        if (m) {
            //Logger.log(m, 'Socket Message');
            var comm = m[1];
            if (listeners[comm]) {
                var data = $.parseJSON(m[2]);
                //Logger.log(data, 'parsed object');
                $.each(listeners[comm], function (i, l) {
                    l(data);
                });
            }
        } else {
            Logger.log('unknown :' + data);
        }
    } else if (data.constructor === Blob) {
        if (listeners['blob']) {
            $.each(listeners.blob, function (i, l) {
                l(data);
            });
        }
    } else {
        Logger.alert(data, 'unknown data');
    }
}

/**
 * WebSocketを開始する
 */
function startWebSocket() {
    if (!socket) {
        try {
            socket = new WebSocket(SOCKET_URL);
            socket.addEventListener('message', onSocketMessage);
            socket.addEventListener('close', function() {
                socket = null;
                Logger.log(socket, "socket closed!");
                if (++connectErrorCount < 5) {
                    setTimeout(startWebSocket, 500);
                } else {
                    onSocketMessage({
                        data: 'ErrorClose\t"通信切断しました。"'
                    });
                }
            });
        } catch (ex) {
            Logger.alert(ex, 'websocket failed.');
        }
    }
}



/**
 * 通信ヘルパー
 */
var Sender = {
    /**
     * WebSocket通信のリスナーを登録する
     * @param {string} command コマンド種別
     * @param {type} listener function(message)
     * @returns {undefined}
     */
    onMessage: function (command, listener) {
        if (!listeners[command]) {
            listeners[command] = [];
        }
        listeners[command].push(listener);
        // socketが開始していなかったら開始する
        startWebSocket();
    },
    /**
     * Ajaxでリクエストを送る
     * @param {string} command
     * @param {object} opts {
     *    async: 非同期
     *    data: パラメータ
     *    success(data): trueレスポンス時
     *    fail(data): falseレスポンス時
     *    error(status, exception): エラー時
     *    wait: trueにするとリスナメソッドをdocument.loadを待ってから実行する
     * }
     */
    send: function (command, opts) {
        opts = opts || {};
        $.ajax({
            url: API_ROOT + command,
            async: typeof (opts.async) === 'undefined' ? true : opts.async,
            dataType: 'json',
            data: $.extend(true, {
                r: Math.random()
            }, opts.data),
            success: function (data) {
                Logger.log(data, 'send(' + command + '):success');
                if (data.Success) {
                    if (opts.success) {
                        if (opts.wait) {
                            $(function() {
                                opts.success(data);
                            });
                        } else {
                            opts.success(data);
                        }
                    }
                } else {
                    Logger.log(data, command + ' failed.');
                    if (opts.fail) {
                        if (opts.wait) {
                            $(function() {
                                opts.fail(data);
                            });
                        } else {
                            opts.fail(data);
                        }
                    } else {
                        alert(command + ' failed.');
                    }
                }
            },
            error: function (xhr, status, error) {
                Logger.log([xhr, status, error], 'send error.');
                if (opts.error) {
                    if (opts.wait) {
                        $(function() {
                            opts.error(status, error);
                        });
                    } else {
                        opts.error(status, error);
                    }
                }
            }
        });
    }
};

module.exports = Sender;

