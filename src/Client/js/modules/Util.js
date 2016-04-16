'use strict';

/**
 * 便利メソッド用クラス
 */
var Util = {
    /**
     * 実体参照の変換＋改行を<br>に変換する。波ダッシュ変換も行う。
     * @param {String} str
     * @returns {String}
     */
    escape: function (str) {
        return str ? ("" + str).replace(/[&<>"'\n\r 〜]/g, function (c) {
            return {
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                '\'': '&#x27;',
                '\n': '<br>',
                '\r': '',
                ' ': '&nbsp;',
                '〜': '&#65374;'
            }[c];
        }) : '';
    }

};
module.exports = Util;
