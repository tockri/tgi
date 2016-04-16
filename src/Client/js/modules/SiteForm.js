/*
 * サイト編集画面
 */
var Sender = require('./Sender');
var Util = require('./Util');
var Status = require('./Status');
var Message = require('./Message');

function reloadSiteSelect() {
    var sel = $('#site-select');
    Sender.send('sites', {
        wait: true,
        success: function (data) {
            var e = Util.escape;
            sel.empty();
            $.each(data.Sites, function (i, row) {
                var edn = e(row.DirName);
                var label = e(row.SiteName);
                sel.append('<option value="' + edn + '">' + label + ' (' + edn + ')' + '</option>');
            });
            Status.get(function(st) {
                sel.val(st.SiteDirName);
            });
        }
    });
}
/**
 * サイト切り替えselectの初期化
 */
function initSiteSelect() {
    var sel = $('#site-select');
    reloadSiteSelect();
    sel.change(function() {
        var option = sel.find('option:selected');
        var label = option.text();
        var dirName = option.val();
        if (!confirm('使用するサイト設定を ' + label + ' に変更してよろしいですか？')) {
            return;
        }
        Sender.send('selectSite', {
            data: {
                DirName: dirName
            },
            success: function(data) {
                SiteForm.fetchValues(dirName);
            }
        });
    });
}
/**
 * サイト複製ボタンのイベント
 */
function initSiteDuplicateButton() {
    $('#site-duplicate-button').click(function() {
        if (!confirm('このサイト設定を複製して、新しいサイト設定を登録しますか？')) {
            return;
        }
        var sel = $('#site-select');
        var dirName = sel.val();
        Sender.send('duplicate', {
            data: {
                DirName: dirName
            },
            success: function(data) {
                reloadSiteSelect();
                SiteForm.setFormValues(data.Values);
                var dirName = data.Values.DirName;
                Sender.send('selectSite', {
                    data: {
                        DirName: dirName
                    },
                    success: function(data) {
                        sel.val(dirName);
                    }
                });
                alert('サイト設定を複製しました。');
            }
        });
    });
}


/**
 * サイト編集画面の<select>タグを初期化する
 */
function initSelectOptions() {
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
 * 書き込んだらすぐにsubmitするイベントを登録する
 */
function initFormEvent() {
    var form = $('#form-siteform');
    // ラジオボタンクリックでsubmit
    form.find(':radio').click(function() {
        submitForm();
    });
    // textの変更でbuttonの色変える
    form.find(':text,textarea').change(function() {
        var t = $(this);
        var btn = t.closest('.input-group').find('.btn');
        btn.removeClass(t, 'btn-default').addClass('btn-primary');
    });
    // buttonクリックでsubmit
    form.find('.input-group .btn').click(function() {
        submitForm();
    });
    // select変更でsubmit
    form.find('select').change(function() {
        submitForm();
    });
}

function submitForm() {
    var form = $('#form-siteform');
    var data = {};
    $.each(form.serializeArray(), function(i, v) {
        data[v.name] = v.value;
    });
    Sender.send('modconfig', {
        data: data,
        success: function() {
            form.find('.btn.btn-primary')
                    .removeClass('btn-primary')
                    .addClass('btn-default');
            reloadSiteSelect();
            alert('サイト情報を更新しました。');
        }
    });
}

/**
 * サイト編集画面のインターフェイス
 */
var SiteForm = {
    /**
     * サイト属性値をformに読み込む
     * @param {string} dirName
     */
    fetchValues: function(dirName) {
        Sender.send('config', {
            data: {
                DirName: dirName
            },
            success: function (data) {
                SiteForm.setFormValues(data.Values);
            }
        });
    },
    /**
     * サイト属性編集フォームの入力値を設定する
     * @param {object} values
     */
    setFormValues: function(values) {
        var form = $('#form-siteform');
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
};

initSelectOptions();
$(function() {
    initSiteSelect();
    initSiteDuplicateButton();
    Status.get(function(st) {
        SiteForm.fetchValues(st.SiteDirName);
    });
    initFormEvent();
});

module.exports = SiteForm;

