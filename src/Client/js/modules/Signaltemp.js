/*
 * Signaltemp.js
 * 色温度設定
 */
'use strict';
var Sender = require('./Sender');
var Logger = require('./Logger');
var Status = require('./Status');

/**
 * 現在のステップ
 * @type Number
 */
var step = 0;
/**
 * ステップ1のシグナル値
 * @type Number
 */
var step1Signal = 0;

/**
 * 色温度設定画面を初期化する
 */
function initSignaltemp() {
    $('.signaltemp-info').hide();
    step0();
    $('#color-abortbutton').hide().click(function () {
        step0();
    });
    $('#color-startbutton').click(function () {
        Sender.send('signaltemp-step0', {
            success: function (data) {
                step1();
            }
        });
    });
    $('#image-pane .viewer').click(function (e) {
        e.preventDefault();
        e.stopPropagation();
        switch (step) {
            case 1:
                step1Clicked(e.offsetX, e.offsetY);
                break;
            case 2:
                step2Clicked(e.offsetX, e.offsetY);
                break;
            default:
        }
    });
    $('#signaltemp-form').submit(function () {
        if (step !== 3) {
            return false;
        }
        step3Submit($(this));
    });
}
/**
 * 測定点1がクリックされた
 * @param {Number} x
 * @param {Number} y
 */
function step1Clicked(x, y) {
    $('#point1').show().css({
        top: y - 4,
        left: x - 4
    });
    Sender.send('signaltemp-step1', {
        data: {
            x: x,
            y: y
        },
        success: function (data) {
            var form = $('#signaltemp-form');
            $('#color-monitor1').css('background-color', data.Color);
            step1Signal = data.Signal;
            form.find('input[name=signal1]').val(data.Signal);
            form.find('input[name=temp1]').val(data.Temp);
            step2();
        }
    });
}
/**
 * 測定点2がクリックされた
 * @param {Number} x
 * @param {Number} y
 */
function step2Clicked(x, y) {
    $('#point2').show().css({
        top: y - 4,
        left: x - 4
    });
    Sender.send('signaltemp-step2', {
        data: {
            x: x,
            y: y
        },
        success: function (data) {
            if (Math.abs(step1Signal - data.Signal) < 1000) {
                alert('できるだけ温度の離れた2点を選択してください。');
                return;
            }
            var form = $('#signaltemp-form');
            $('#color-monitor2').css('background-color', data.Color);
            form.find('input[name=signal2]').val(data.Signal);
            form.find('input[name=temp2]').val(data.Temp);
            step3();
        }
    });
}
/**
 * 温度入力して「設定完了」ボタン
 * @param {jQuery} form
 */
function step3Submit(form) {
    var params = {};
    $.each(form.serializeArray(), function (i, nv) {
        params[nv.name] = nv.value;
    });
    if (Math.abs(params.temp1 - params.temp2) < 30) {
        alert('30℃以上の温度差を入力してください。');
        return;
    }
    Sender.send('signaltemp-submit', {
        data: params,
        success: function (data) {
            step0();
            $('#info4').slideDown();
            setTimeout(function () {
                $('#info4').slideUp();
            }, 3000);
        }
    });
    Logger.log(params, 'form values');
}
/**
 * 色温度設定を終了する
 */
function step0() {
    step = 0;
    step1Signal = 0;
    $('#color-monitor1').css('background-color', '#f0f0f0');
    $('#color-monitor2').css('background-color', '#f0f0f0');
    $('#color-submitbutton').attr('disabled', true);
    var form = $('#signaltemp-form');
    form.find('input').val('');
    $('.signaltemp-info:visible').slideUp();
    $('#color-startbutton').show();
    $('#color-abortbutton').hide();
    $('#signaltemp-pane .point').hide();
}

/**
 * 色温度設定を開始する
 */
function step1() {
    step = 1;
    $('#color-submitbutton').attr('disabled', true);
    var form = $('#signaltemp-form');
    form.find('input').val('');
    $('.signaltemp-info:not(#info1):visible').slideUp();
    $('#info1').slideDown();
    $('#color-startbutton').hide();
    $('#color-abortbutton').show();
    $('.color-monitor').css('background', 'transparent');
}
/**
 * 色温度設定の測定点２にうつる
 */
function step2() {
    step = 2;
    $('#color-submitbutton').attr('disabled', true);
    $('.signaltemp-info:not(#info2):visible').slideUp();
    $('#info2').slideDown();
}
/**
 * 温度入力にうつる
 */
function step3() {
    step = 3;
    $('#color-submitbutton').attr('disabled', false);
    $('.signaltemp-info:not(#info3):visible').slideUp();
    $('#info3').slideDown();
}

function onStatusChange(st) {
    var button = $('#color-startbutton');
    if (st.AIRStatus === 'Initialized') {
        button.attr('disabled', false);
    } else {
        button.attr('disabled', true);
    }
}

$(function () {
    initSignaltemp();
    Status.get(onStatusChange);
    Status.onChange(onStatusChange);
});
