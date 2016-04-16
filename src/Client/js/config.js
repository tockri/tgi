'use strict';
// mockjax
require('./modules/Prefs');
var Viewer = require('./modules/Viewer');
var Sender = require('./modules/Sender');
var Util = require('./modules/Util');
var Watchbox = require('./modules/Watchbox');
var Status = require('./modules/Status');
var Message = require('./modules/Message');

/**
 * タブの登録
 */
function initTabs() {
    require('./modules/Tabstrip');
    $('#config-form').tabstrip({
        selected: function (tab) {
            switch (tab.attr('id')) {
                case 'tab-main':
                    // メイン
                    $('#watchbox-main').removeClass('hidden');
                    $('#watchbox-sub').addClass('hidden');
                    break;
                case 'tab-sub':
                    // サブ
                    $('#watchbox-main').addClass('hidden');
                    $('#watchbox-sub').removeClass('hidden');
                    break;
                case 'tab-other':
                    // その他
                    $('#watchbox-main').addClass('hidden');
                    $('#watchbox-sub').addClass('hidden');
                    break;
            }
        }
    });
}

/**
 * 加速する
 * @param {string} prx
 * @param {bool} doFast
 */
function setFast(prx, doFast) {
    var keys = [
        'up', 'left', 'right', 'down', 'narrow', 'wide', 'gapnarrow', 'gapwide'
    ];
    $.each(keys, function (i, key) {
        $('#' + prx + '-' + key).attr('value', doFast ? '10' : '1');
    });
}
/**
 * ボタンイベント
 */
function initButtons() {
    $('#config-form button.btn').click(function () {
        var id = this.id;
        var $b = $(this);
        var tab = $b.closest('.tab-pane').attr('id');
        $b.blur();
        if (tab === 'config-main' || tab === 'config-sub') {
            // メイン監視枠、サブ監視枠
            if (id === 'main-fast') {
                // メイン監視枠の加速ボタン
                $b.toggleClass('btn-info');
                setFast('main', $b.is('.btn-info'));
            } else if (id === 'sub-fast') {
                // サブ監視枠の加速ボタン
                $b.toggleClass('btn-info');
                setFast('sub', $b.is('.btn-info'));
            } else {
                var params = {
                    Key: id,
                    Value: $b.val()
                };
                Sender.send('modwatchbox', {
                    data: params,
                    success: function(data) {
                        Watchbox.updated(data);
                    }
                });
            }
        }
    });
}

/**
 * selectの選択肢を読み込む
 */
function initConfigOptions() {
    var e = Util.escape;
    Sender.send('configOptions', {
        wait: true,
        success: function (data) {
            $.each(['ThresholdSet', 'ColorMap'], function (i, name) {
                var sel = $('#select-' + name);
                sel.html('<option value=""></option>');
                $.each(data[name], function (key, value) {
                    sel.append('<option value="' + key + '">' + e(value) + '</option>');
                });
            });
        }
    });
}

/**
 * しきい値判定用変数と色温度マップのselectのイベントを設定する
 */
function initSelects() {
    $('#config-form select').change(function() {
        var name = this.name;
        var $b = $(this);
        if ($b.val()) {
            var params = {};
            params[name] = $b.val();
            Sender.send('modconfig', {
                data: params
            });
        } else {
            alert('未選択にすることはできません。');
        }
    });
    
}


/**
 * その他タブの内容を読み込む
 * @returns {undefined}
 */
function initConfigValues() {
    var e = Util.escape;
    Sender.send('config', {
        wait: true,
        success: function (data) {
            if (data.Values) {
                $.each(data.Values, function (key, value) {
                    var span = $('#span-' + key);
                    if (span.length) {
                        span.html(e(value ? value : '(未設定)'));
                    } else {
                        var sel = $('#select-' + key);
                        if (sel.length) {
                            sel.val(value);
                        }
                    }
                });
            }
        }
    });
}

/**
 * 監視枠変更イベント
 * @returns {undefined}
 */
function updateForm() {
    if (!Watchbox.Main) {
        return;
    }
    if (Watchbox.Main.Mode === 'Double') {
        $('#main-double').addClass('active');
        $('#main-single').removeClass('active');
        $('#main-gapnarrow').attr('disabled', false);
        $('#main-gapwide').attr('disabled', false);
    } else {
        $('#main-single').addClass('active');
        $('#main-double').removeClass('active');
        $('#main-gapnarrow').attr('disabled', true);
        $('#main-gapwide').attr('disabled', true);
    }
}

function onStatusChange(st) {
    if (st.Recording && !onStatusChange.redirected) {
        onStatusChange.redirected = true;
        alert('記録中のため、記録画面に移動します。');
        location.href = 'index.html';
    }
}

$(function () {
    initTabs();
    initButtons();
    initConfigOptions();
    initConfigValues();
    initSelects();
    Status.get(onStatusChange);
    Status.onChange(onStatusChange);
});
Watchbox.onChange(updateForm);

