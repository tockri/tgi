'use strict';

var Sender = require('./Sender.js');
var Prefs = require('./Prefs');
/**
 * カラーマップを更新する
 */
function updateColormap() {
    $('#colormap-image').attr('src', Prefs.COLORMAP_URL + '?r=' + Math.random());
}
/**
 * 画像ビューアのインターフェイス
 */
var Viewer = {
    img: null
};

$(function() {
    Viewer.img = $('#thermo-image');
    Sender.onMessage('blob', function (data) {
        var blob = new Blob([data], {
            type: 'image/png'
        });
        Viewer.img.attr('src', URL.createObjectURL(blob));
    });
    Sender.onMessage('ColorMap', function(data) {
        updateColormap();
    });
    updateColormap();
});


module.exports = Viewer;
