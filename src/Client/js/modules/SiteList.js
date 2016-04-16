/*
 * SiteList
 * サイト一覧・選択・編集
 */
'use strict';

var Sender = require('./Sender');
var Logger = require('./Logger');
var Util = require('./Util');
var Status = require('./Status');

/**
 * サイト一覧テーブルの準備
 */
function reloadTable() {
    Sender.send('sites', {
        wait: true,
        success: function (data) {
            Logger.log(data, 'loadSites');
            if (!data.Sites) {
                return;
            }
            var e = Util.escape;
            var tbody = $('#site-table tbody');
            tbody.empty();
            $.each(data.Sites, function (i, row) {
                var edn = e(row.DirName);
                var tr = $('<tr id="tr-' + edn + '">'
                        + '<td class="select-mark center">'
                        + '<span class="okicon glyphicon glyphicon-ok"></span>'
                        + '</td>'
                        + '<td>' + edn + '</td>'
                        + '<td><a href="#siteEditModal" data-dirName="' + edn + '"'
                        + ' data-toggle="modal" data-target="#siteEditModal">'
                        + e(row.SiteName ? row.SiteName : '(未設定)') + '</a></td>'
                        + '<td class="control center">'
                        + '<button class="btn btn-sm btn-default select-button"'
                        + ' value="' + edn + '"'
                        + '>選択</button>'
                        + '</td></tr>');
                tbody.append(tr);
            });
            Status.get(function(st) {
                onStatusChanged(st);
            });
        }
    });
}

/**
 * ボタンイベントの準備
 */
function initButtons() {
    // サイト一覧の『選択」ボタン
    $('#site-table').on('click', '.select-button', function () {
        selectSite(this.value);
        this.blur();
    });
    // サイト編集画面の「選択」ボタン
    $('#select-button').click(function() {
        var form = $('#siteEditForm');
        var dirName = form.find('input[name=DirName]').val();
        selectSite(dirName);
        $('#siteEditModal').modal('hide');
    });
    // サイト編集画面の「複製」ボタン
    $('#duplicate-button').click(function() {
        if (!confirm('このサイト設定を複製して、新しいサイト設定を登録しますか？')) {
            return;
        }
        var form = $('#siteEditForm');
        var dirName = form.find('input[name=DirName]').val();
        Sender.send('duplicate', {
            data: {
                DirName: dirName
            },
            success: function(data) {
                setSiteEditFormValues(data.Values);
                reloadTable();
                alert('サイト設定を複製しました。');
            }
        });
    });
}

/**
 * サイトを選択する
 * @param {string} dirName
 */
function selectSite(dirName) {
    if (!confirm('使用するサイト設定を[' + dirName + ']に変更してよろしいですか？')) {
        return;
    }
    Sender.send('selectSite', {
        data: {
            DirName: dirName
        }
    });
}

/**
 * サイト編集画面表示次の読み込み
 */
function initSiteModal() {
    $('#siteEditModal').on('show.bs.modal', function (e) {
        var a = $(e.relatedTarget);
        var dirName = a.attr('data-dirName');
        Sender.send('config', {
            data: {
                DirName: dirName
            },
            success: function (data) {
                if (data.Values) {
                    setSiteEditFormValues(data.Values);
                }
            }
        });
    }).on('hide.bs.modal', function (e) {
        var form = $('#siteEditForm');
        form.find('>fieldset').attr('disabled', false);
        form.find('.message-pane').empty();
    });
}

/**
 * サイト編集画面のフォーム読み込み
 * @param {object} values
 */
function setSiteEditFormValues(values) {
    // 選択ボタン
    var selectButton = $('#select-button');
    Status.get(function(st) {
        if (st.SiteDirName === values.DirName) {
            selectButton.hide();
        } else {
            selectButton.show();
        }
    });
    var modal = $('#siteEditModal');
    modal.find('h4 .dirName').text(values.DirName);
    var form = $('#siteEditForm');
    form.find('>fieldset').attr('disabled', false);
    form.find('.message-pane').empty();
    $.each(values, function (key, value) {
        var radio = form.find('input:radio[name=' + key + ']');
        if (radio && radio.length) {
            radio.filter('[value=' + value + ']').attr('checked', true);
        } else {
            var control = form.find('input[name=' + key + ']'
                    + ',select[name=' + key + ']'
                    + ',textarea[name=' + key + ']');
            if (control.length) {
                control.val(value);
            }
        }
    });
}

/**
 * サイト編集画面の<form>を初期化する
 */
function initSiteEditForm() {
    var form = $('#siteEditForm');
    form.submit(function () {
        var values = form.serializeArray();
        if (!values) {
            return false;
        }
        form.find('>fieldset').attr('disabled', true);
        var pane = form.find('.message-pane');
        pane.empty();
        var params = {};
        $.each(values, function (i, nv) {
            params[nv.name] = nv.value;
        });
        Logger.log(params, 'Form SerializeArray');
        Sender.send('modconfig', {
            data: params,
            success: function (data) {
                pane.html('<div class="alert alert-success" role="alert">サイト設定を登録しました。</div>');
                reloadTable();
            },
            fail: function (data) {
                var e = Util.escape;
                form.find('>fieldset').attr('disabled', false);
                pane.html('<div class="alert alert-warning" role="alert">' + e(data.ErrorMessage) + '</div>');
            }
        });
    });
}

/**
 * サイト編集画面の<select>タグを初期化する
 */
function initSelects() {
    var e = Util.escape;
    Sender.send('configOptions', {
        wait: true,
        success: function (data) {
            $.each(['ThresholdSet', 'ColorMap', 'CofFile'], function (i, name) {
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
 * ステータス変更時、SiteDirNameが変更されたら表示を更新する
 * @param {object} status ステータスオブジェクト
 */
function onStatusChanged(status) {
    var dn = status.SiteDirName;
    var tr = $('#site-table #tr-' + dn);
    if (tr.is('.info')) {
        return;
    }
    $('#site-table tr.info').removeClass('info');
    tr.addClass('info');
}

$(function () {
    initSiteModal();
    initSiteEditForm();
    initButtons();
});
reloadTable();
initSelects();
Status.onChange(onStatusChanged);


