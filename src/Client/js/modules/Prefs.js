/*
 * Prefx
 * 
 * 設定値
 */
'use strict';

// ローカルのみで開発するときはコメントはずす。製品版ではコメントアウト。
//require('../mockjax');

var Prefs = {
    // APIのURLをサーバーに向ける
    //API_ROOT: 'http://dev.xp/api/',
    // 製品版の設定
    API_ROOT: '/api/',
    
    // WebSocketのURLをサーバーに向ける
    //SOCKET_URL: 'ws://dev.xp:8002',
    // 製品版の設定
    SOCKET_URL: 'ws://' + location.hostname + ':8002',
    
    // カラーマップ画像のURLをNO COLORMAPにする
    //COLORMAP_URL: '/img/colormap-noimage.gif'
    // 製品版の設定
    COLORMAP_URL: 'colormap'
};
module.exports = Prefs;

