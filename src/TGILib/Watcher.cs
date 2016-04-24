using System;
using System.IO;
using System.Text;
using System.Drawing;
using SAT.Util;


namespace TGILib {

    /// <summary>
    /// WatchBoxからイベントを受け取るオブジェクト
    /// </summary>
    public class Watcher : IDisposable {
        // メイン監視枠
        public readonly WatchBox MainBox;
        // サブ監視枠
        public readonly WatchBox SubBox;

        private bool enabled;
        /// <summary>
        /// 有効・無効
        /// </summary>
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                if (enabled != value) {
                    enabled = value;
                    MainBox.Enabled = value;
                    SubBox.Enabled = value;
                    if (value) {
                        startTime = DateTime.Now;
                    } else {
                        CloseFile();
                    }
                    var app = TGIApp.Instance;
                    app.StatusChanged = true;
                }
            }
        }
        /// <summary>
        /// この数だけ連続したセルが正常な温度であればよい
        /// </summary>
        public int SafeCellCount = 3;

        private bool _outputEnabled = true;
        /// <summary>
        /// ファイル出力をするかどうか
        /// </summary>
        public bool OutputEnabled {
            get {
                return _outputEnabled;
            }
            set {
                _outputEnabled = value;
                if (!value) {
                    CloseFile();
                }
            }
        }


        /// <summary>
        /// 異常時に画像出力をするかどうか
        /// </summary>
        public bool ImageOutputEnabled {
            get;
            set;
        }

        private string _outputDir;
        /// <summary>
        /// ファイル出力ディレクトリ
        /// </summary>
        public string OutputDir {
            get {
                return _outputDir;
            }
            set {
                _outputDir = value;
                CloseFile();
            }
        }

        /// <summary>
        /// 画像ファイル出力ディレクトリ
        /// </summary>
        public string ImageOutputDir {
            get;
            set;
        }
        /// <summary>
        /// 閾値自動判定式の傾き
        /// </summary>
        public double ThresholdSlope = 0;
        /// <summary>
        /// 閾値自動判定式の切片
        /// </summary>
        public double ThresholdIntercept = 0;
        /// <summary>
        /// サブ監視枠の平均温度
        /// </summary>
        private double SubAverage = 0;
        /// <summary>
        /// 自動判定式から算出した安全な最低温度
        /// </summary>
        private double MinSafeTemp = 0;

        /// <summary>
        /// CSV出力先ポインタ
        /// </summary>
        private TextWriter csvout;
        /// <summary>
        /// 開始時刻
        /// </summary>
        private DateTime startTime;
        /// <summary>
        /// 前回ファイルに書き込んだ時刻
        /// </summary>
        private DateTime lastWrite = DateTime.MinValue;
        /// <summary>
        /// 前回画像を保存した時刻
        /// </summary>
        private DateTime lastWriteImage = DateTime.MinValue;
        /// <summary>
        /// 前回異常フラグをファイルに書き込んだ時刻
        /// </summary>
        private DateTime lastErrorWrite = DateTime.MinValue;
        /// <summary>
        /// 
        /// </summary>
        private string alertMessage;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainBox"></param>
        /// <param name="subBox"></param>
        public Watcher(WatchBox mainBox, WatchBox subBox) {
            MainBox = mainBox;
            SubBox = subBox;
            Enabled = false;
        }
        /// <summary>
        /// ファイルストリームを閉じる
        /// </summary>
        void CloseFile() {
            if (csvout != null) {
                csvout.Close();
                csvout.Dispose();
                csvout = null;
            }
        }
        /// <summary>
        /// ファイルストリームを開き直す
        /// </summary>
        private void ReopenFile() {
            if (Enabled && OutputEnabled) {
                try {
                    CloseFile();
                    csvout = Open("main");

                } catch (IOException ioe) {
                    OutputEnabled = false;
                    TGIApp.Instance.FireClientEvent(TGIApp.ClientEventType.ErrorMessage, 
                        "エラーが発生しました。\n" + ioe.Message + "\nファイル出力を中断しました。");
                }
            }
        }

        private TextWriter Open(string Name) {
            if (!Directory.Exists(_outputDir)) {
                Directory.CreateDirectory(_outputDir);
            }
            string fileBaseName = Path.Combine(_outputDir, Name + DateTime.Now.ToString("yyyyMMdd"));
            int suffix = 1;
            while (File.Exists(fileBaseName + "-" + suffix.ToString("d3") + ".csv")) {
                suffix++;
            }
            TextWriter fout = new StreamWriter(fileBaseName + "-" + suffix.ToString("d3") + ".csv", false, System.Text.Encoding.Default);
            WriteCurrentConfig(fout);
            StringBuilder sb = new StringBuilder();
            sb.Append("時刻,接合部温度,閾値温度,環境温度,エラー,ALL-MAX,ALL-MIN");
            for (int i = 0; i < MainBox.CellCount; i++) {
                sb.Append(",Cell" + i + "-MAX");
                sb.Append(",Cell" + i + "-MIN");
                sb.Append(",Cell" + i + "-AVG");
            }
            fout.WriteLine(sb.ToString());
            return fout;
        }

        /// <summary>
        /// 現在の設定値をCSVに書き出す
        /// </summary>
        private void WriteCurrentConfig(TextWriter fout) {
            var conf = TGIApp.Instance.CurrentConfig;
            var sb = new StringBuilder();
            WritePair(sb, "サイト名", conf.SiteName);
            WritePair(sb, "接合者名", conf.SitePerson);
            WritePair(sb, "cofファイル名", Path.GetFileName(conf.CofFilePath));
            WritePair(sb, "閾値判定変数", "(傾き=" + conf.ThresholdSlope + "/切片=" + conf.ThresholdIntercept + ")");
            WritePair(sb, "シートタイプ", conf.SheetType);
            WritePair(sb, "接合温度", conf.FusionTemp);
            WritePair(sb, "接合速度", conf.FusionSpeed);
            WritePair(sb, "圧力", conf.FusionPressure);
            WritePair(sb, "メモ", conf.Memo, false);
            fout.WriteLine(sb.ToString());
        }

        

        /// <summary>
        /// エスケープしてクオートで囲む
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void WritePair(StringBuilder sb, string name, string value, bool addComma = true) {
            sb.Append("\"").Append(name).Append(":\",\"").Append(value.Replace("\"", "\"\"")).Append("\"");
            if (addComma) {
                sb.Append(",");
            }
        }


        /// <summary>
        /// CSVを一行出力する
        /// </summary>
        /// <param name="cells">セル</param>
        /// <param name="now">現在時刻</param>
        /// <param name="safe">判定OK</param>
        private void Output(WatchCell[] cells, DateTime now, bool safe) {
            StringBuilder sb = new StringBuilder();
            TimeSpan ts = now - startTime;
            sb.Append("" + now.ToString("yyyy-MM-dd HH:mm:ss.f"));
            sb.Append("," + MainBox.AllAverage);
            sb.Append("," + MinSafeTemp);
            sb.Append("," + SubAverage);
            sb.Append("," + (safe ? "" : "1"));
            sb.Append("," + MainBox.AllMax);
            sb.Append("," + MainBox.AllMin);
            foreach (WatchCell cell in cells) {
                sb.Append("," + cell.Max);
                sb.Append("," + cell.Min);
                sb.Append("," + (int)Math.Round(cell.Average));
            }
            try {
                csvout.WriteLine(sb.ToString());
                lastWrite = now;
                if (!safe) {
                    lastErrorWrite = now;
                }
            } catch (IOException ioe) {
                OutputEnabled = false;
                TGIApp.Instance.FireClientEvent(TGIApp.ClientEventType.ErrorMessage, 
                    "エラーが発生しました。\n" + ioe.Message + "\nファイル出力を中断しました。");
            }
        }
        

        /// <summary>
        /// 温度測定を行った際のイベント
        /// </summary>
        /// <param name="cells"></param>
        public void Watch(ushort[] signals) {
            if (!Enabled) {
                return;
            }
            MainBox.Watch(signals);
            SubBox.Watch(signals);

            WatchCell[] cells = MainBox.WatchCells;
            DateTime now = DateTime.Now;
            bool safe = IsSafe(cells);
            if ((now - lastWrite).TotalMilliseconds >= 200
                || (!safe && (now - lastErrorWrite).TotalMilliseconds >= 200)) {
                // ファイル書きだしは0.2秒ごと
                if (OutputEnabled) {
                    if (csvout == null) {
                        ReopenFile();
                    }
                    Output(cells, now, safe);
                }
            }
            if (!safe) {
                var app = TGIApp.Instance;
                var evt = app.CurrentConfig.AlermSound ? TGIApp.ClientEventType.TempAlerm
                    : TGIApp.ClientEventType.TempWarn;
                app.FireClientEvent(evt, "検知：" + alertMessage);
                if (ImageOutputEnabled && now > lastWriteImage.AddSeconds(10)) {  // 10秒ごとに保存する
                    try {
                        string filePath = Path.Combine(ImageOutputDir, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");
                        app.SaveImageToFile(filePath);
                        lastWriteImage = now;
                    } catch (Exception e) {
                        ImageOutputEnabled = false;
                        Logger.warn(e, "画像保存時のエラー");
                        app.FireClientEvent(TGIApp.ClientEventType.ErrorMessage, 
                            "画像保存時にエラーが発生しました。\n" + e.Message + "\n画像保存を中断しました。");
                    }
                }
            }

        }

        /// <summary>
        /// すべてのセルの最低温度が異常値を示していたらfalseを返す
        /// （一つでも正常なセルがあればtrueを返す）
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool WatchAlgorithm1(WatchCell[] cells) {
            foreach (var cell in cells) {
                if (cell.Min > MinSafeTemp) {
                    return true;
                }
            }
            alertMessage = "枠全域の最低温度異常：閾値=" + MinSafeTemp.ToString("F02") + "℃";
            return false;

        }
        /// <summary>
        /// すべてのセルの最高温度が異常値を示していたらfalseを返す
        /// （一つでも正常なセルがあればtrueを返す）
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool WatchAlgorithm2(WatchCell[] cells) {
            return true;
/*            foreach (WatchCell cell in cells) {
                if (cell.Max <= SafeAreaMax) {
                    return true;
                }
            }
            alertMessage = "最高温度が異常値を示しています。";
            return false;
 */ 
        }

        /// <summary>
        /// 平均温度が範囲内におさまったセルが連続している
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool WatchAlgorithm3(WatchCell[] cells) {
            // 平均温度が範囲内におさまったセルが連続している
            int ir = 0;
            foreach (WatchCell cell in cells) {
                if (MinSafeTemp < cell.Average) {
                    ir++;
                    if (SafeCellCount <= ir) {
                        break;
                    }
                } else {
                    ir = 0;
                }
            }


            if (SafeCellCount <= ir) {
                return true;
            } else {
                alertMessage = "枠内一部の平均温度異常：閾値=" + MinSafeTemp.ToString("F02") + "℃";
                return false;
            }
        }

        /// <summary>
        /// サブ監視枠の平均温度から閾値を計算する
        /// </summary>
        private void CalcurateMinSafeTemp() {
            WatchCell[] cells = SubBox.WatchCells;
            float avg = 0;
            foreach (var cell in cells) {
                avg += cell.Average;
            }
            avg /= (float)cells.Length;
            SubAverage = avg;
            MinSafeTemp = avg * ThresholdSlope + ThresholdIntercept;
        }

        /// <summary>
        /// 温度データが基準値に収まっているかどうかの判定ルール
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private bool IsSafe(WatchCell[] cells) {
            if (ThresholdSlope != 0 && ThresholdIntercept != 0) {
                CalcurateMinSafeTemp();
                // すべてのセルで最低温度、最高温度が範囲内である
                return WatchAlgorithm1(cells)
                    && WatchAlgorithm2(cells)
                    && WatchAlgorithm3(cells);
            } else {
                return true;
            }
        }


        /// <summary>
        /// 破棄する
        /// </summary>
        public void Dispose() {
            CloseFile();
        }

    }
}
