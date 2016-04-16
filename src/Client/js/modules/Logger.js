'use strict';

/**
 * Logger ログ出力する便利クラス
 */
var Logger = {
    /**
     * コンソールログを出力する。
     * @param {object} obj
     * @param {string} message
     */
    log: function (obj, message) {
        if (typeof (console) === 'object') {
            console.debug(message, obj);
        } else {
            alert({
                message: message,
                obj: obj
            });
        }

    },
    /**
     * コンソールログ出力してalertもする
     * @param {object} obj
     * @param {string} message
     */
    alert: function (obj, message) {
        Logger.log(obj, message);
        alert([obj, message]);
    }
};

module.exports = Logger;

