'use strict';

var Sender = require('./Sender');

var loadedStatus = null;
var changeListeners = [];

/**
 * ステータスのインターフェイス
 * 
 * ステータスオブジェクトの仕様は
 * AIRStatus: NotConnected|NotInitialized|Initializing|Initialized
 * Recording: true|false
 * SiteDirName: 現在のサイトディレクトリ名
 */
var Status = {
    /**
     * 現在のステータスを返す
     * @param {Function} callback(status)
     */
    get: function(callback) {
        if (!loadedStatus) {
            Sender.send('status', {
                success: function(data) {
                    loadedStatus = data;
                    callback && callback(loadedStatus);
                    fireStatusChanged();
                }
            });
        } else {
            callback && callback(loadedStatus);
        }
    },
    /**
     * ステータス変更時のリスナを登録する
     * @param {function} func
     */
    onChange: function(func) {
        changeListeners.push(func);
    }
};

function fireStatusChanged() {
    $.each(changeListeners, function(i, listener) {
        listener(loadedStatus);
    });
}

Sender.onMessage('Status', function(data) {
    loadedStatus = data;
    fireStatusChanged();
});

module.exports = Status;
