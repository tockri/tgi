(function() {
    'use strict';
    require('./lib/jquery.mockjax');
    
    var API_ROOT = '/api/';
    // watchboxのスタブ
    $.mockjax({
        url: API_ROOT + 'watchbox',
        responseTime: 700,
        responseText: {
            Success: true,
            Main: {
                Mode: 'double',
                Left: 70,
                Top: 20,
                Width: 180,
                Height: 20,
                Gap: 80
            },
            Sub: {
                Mode: 'single',
                Left: 290,
                Top: 60,
                Width: 20,
                Height: 20,
                Gap: 0
            }
        }
    });
    // modwatchboxのスタブ
    $.mockjax({
        url: API_ROOT + "modwatchbox",
        responseTime: 700,
        responseText: {
            Success: true,
            Watchboxes: {
                Main: {
                    Mode: 'single',
                    Left: 72,
                    Top: 20,
                    Width: 180,
                    Height: 20,
                    Gap: 80
                },
                Sub: {
                    Mode: 'single',
                    Left: 292,
                    Top: 60,
                    Width: 20,
                    Height: 20,
                    Gap: 0
                }
            }
        }
    });
    $.mockjax({
        url: API_ROOT + "configOptions",
        responseTime: 700,
        responseText: {
            Success: true,
            // 閾値判定用変数セット
            ThresholdSet: {
                1: '変数セット(1)',
                2: '変数セット(2)',
                3: '変数セット(3)'
            },
            // 色温度
            ColorMap: {
                1: '20℃〜70℃を強調',
                2: '50℃〜100℃を強調',
                3: '80℃〜130℃を強調',
                4: '110℃〜160℃を強調'
            },
            // cofファイル
            CofFile: {
                1: '2001101.cof',
                2: '2001102.cof',
                3: '2001103.cof',
                4: '2001104.cof'
            }
        }
    });

    // configのスタブ
    $.mockjax({
        url: API_ROOT + "config",
        responseTime: 700,
        response: function(setting) {
            if (setting.data.DirName) {
                this.responseText.Values.DirName = setting.data.DirName;
            }
        },
        responseText: {
            Success: true,
            // その他
            Values: {
                DirName: '20140912001',
                SiteName: 'サイト名',
                SitePerson: '接合者名',
                SheetType: 'シートタイプ',
                FusionTemp: '接合温度',
                FusionSpeed: '速度',
                FusionPressure: '圧力',
                Memo: 'メモ',
                CsvOutput: 1,
                ImageOutput: 0,
                AlermSound: 0,
                ThresholdSet: 2,
                ColorMap: 2,
                CofFile: 2
            }
        }
    });
    // modconfigのスタブ
    $.mockjax({
        url: API_ROOT + "modconfig",
        responseTime: 700,
        responseText: {
            Success: true
        }
    });
    // duplicateのスタブ
    $.mockjax({
        url: API_ROOT + "duplicate",
        responseTime: 700,
        responseText: {
            Success: true,
            // その他
            Values: {
                DirName: '20140912004',
                SiteName: 'コピー：サイト名',
                SitePerson: '接合者名',
                SheetType: 'シートタイプ',
                FusionTemp: '接合温度',
                FusionSpeed: '速度',
                FusionPressure: '圧力',
                Memo: 'メモ',
                FileOutput: 1,
                ImageOutput: 0,
                AlermSound: 0,
                ThresholdSet: 2,
                ColorMap: 3,
                CofFile: 2
            }
        }
    });
    
    // statusのスタブ
    var Recording = false;
    var count = 2;
    var selectedDirName = '201409191001';
    $.mockjax({
        url: API_ROOT + "status",
        responseTime: 700,
        response: function(setting) {
            //count--;
            //this.responseText.Initialized = (count === 0);
            this.responseText.SiteDirName = selectedDirName;
        },
        responseText: {
            Success: true,
            Recording: false,
            AIRStatus: 'NotConnected',
            SiteDirName: '201409191001'
        }
    });
    // modstatusのスタブ
    $.mockjax({
        url: API_ROOT + "modstatus",
        responseTime: 700,
        response: function(setting) {
            Recording = this.responseText.Recording = setting.data.record;
        },
        responseText: {
            Success: true,
            Recording: Recording,
            Initialized: true
        }
    });
    // Sitesのスタブ
    $.mockjax({
        url: API_ROOT + "sites",
        responseTime: 700,
        responseText: {
            Success: true,
            Sites: [
                {
                    DirName: 'default',
                    SiteName: 'サイト1'
                },
                {
                    DirName: '201409191001',
                    SiteName: 'サイト2'
                },
                {
                    DirName: '201409190909',
                    SiteName: 'サイト9'
                },
                {
                    DirName: '201409191008',
                    SiteName: 'サイト8'
                },
                {
                    DirName: '201409190907',
                    SiteName: 'サイト7'
                },
                {
                    DirName: '201409191006',
                    SiteName: 'サイト6'
                },
                {
                    DirName: '201409190905',
                    SiteName: 'サイト5'
                },
                {
                    DirName: '201409191004',
                    SiteName: 'サイト4'
                },
                {
                    DirName: '201409191002',
                    SiteName: 'サイト3'
                }
            ]
        }
    });
    // SiteEditのスタブ
    $.mockjax({
        url: API_ROOT + "siteEdit",
        responseTime: 700,
        responseText: {
            Success: true
        }
    });
    // selectSite
    $.mockjax({
        url: API_ROOT + 'selectSite',
        responseTime: 700,
        response: function(setting) {
            selectedDirName = setting.data.DirName;
        },
        responseText: {
            Success: true
        }
    });
    // Colortemp
    // カメラを停止する。Signal配列のスナップショットを覚える。
    $.mockjax({
        url: API_ROOT + 'signaltemp-step0',
        responseTime: 700,
        responseText: {
            Success: true
        }
    });
    $.mockjax({
        url: API_ROOT + 'signaltemp-step1',
        responseTime: 700,
        responseText: {
            Success: true,
            Signal: 10212,
            Color: '#7099ff'
        }
    });
    $.mockjax({
        url: API_ROOT + 'signaltemp-step2',
        responseTime: 700,
        responseText: {
            Success: true,
            Signal: 21029,
            Color: '#70ffcc'
        }
    });
    $.mockjax({
        url: API_ROOT + 'signaltemp-submit',
        responseTime: 700,
        responseText: {
            Success: true
        }
    });
    
    // shutdownのスタブ
    $.mockjax({
        url: API_ROOT + 'shutdown',
        responseTime: 100,
        responseText: {
            Success: true
        }
    });
    
    
})();

