using System;
using System.Collections.Generic;
using SAT.Util;
using TGILib.Server;
using System.Drawing;
using System.IO;
using System.Threading;
using TGILib.AIR;
using System.Text;
using Microsoft.Win32;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Windows.Forms;

namespace TGILib {
    /// <summary>
    /// 閾値判定用変数のセット
    /// </summary>
    public class ThresholdSet {
        /// <summary>
        /// セット名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 傾き
        /// </summary>
        public double Slope;
        /// <summary>
        /// 切片
        /// </summary>
        public double Intercept;
    }
    /// <summary>
    /// サイト名一覧用の構造体
    /// </summary>
    public class SiteEntry {
        /// <summary>
        /// ディレクトリ名
        /// </summary>
        public string DirName;
        /// <summary>
        /// サイト名
        /// </summary>
        public string SiteName;
    }

    /// <summary>
    /// TGIアプリケーションの中枢
    /// </summary>
    public class TGIApp : IDisposable {

        // ------------------------- static ------------------------------
        private static TGIApp singleton = new TGIApp();
        /// <summary>
        /// クライアントにWebSocketで通知するイベント種別
        /// </summary>
        public enum ClientEventType {
            /// <summary>
            /// 未定義のエラーメッセージ
            /// </summary>
            ErrorMessage,
            /// <summary>
            /// 単に表示させるメッセージ
            /// </summary>
            InfoMessage,
            /// <summary>
            /// 監視枠が変更されたイベント
            /// </summary>
            Watchbox,
            /// <summary>
            /// ステータスが変更されたイベント
            /// </summary>
            Status,
            /// <summary>
            /// 温度検知イベント
            /// </summary>
            TempAlerm,
            TempWarn,
            /// <summary>
            /// カラーマップ変更イベント
            /// </summary>
            ColorMap
        }


        // ------------------------- instance ----------------------------
        #region インスタンス変数
        private WebServer WebServer;
        private HttpRouter router;
        private WSServer WSServer;
        private Config currentConfig;
        private System.Threading.Timer statusTimer;

        public readonly WatchBox MainWatchBox = new WatchBox();
        public readonly WatchBox SubWatchBox = new WatchBox();
        public readonly Watcher Watcher;
        /// <summary>
        /// 画像データ
        /// </summary>
        private Bitmap bmp = new Bitmap(320, 240);
        /// <summary>
        /// png画像
        /// </summary>
        private byte[] pngImage;
        /// <summary>
        /// ステータスが変更された
        /// </summary>
        public bool StatusChanged {
            get;
            set;
        }
        /// <summary>
        /// 監視枠が変更された
        /// </summary>
        public bool WatchboxChanged {
            get;
            set;
        }
        private DateTime lastSessionReset = DateTime.Now;


        #endregion

        /// <summary>
        /// シングルトンインスタンスを返す
        /// </summary>
        public static TGIApp Instance {
            get {
                return singleton;
            }
        }
        /// <summary>
        /// シングルトンパターン
        /// </summary>
        private TGIApp() {
            SubWatchBox.Config.Gap = 0;
            SubWatchBox.Config.Mode = "single";
            Watcher = new Watcher(MainWatchBox, SubWatchBox);
        }
        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void Init() {
            WebServer = new WebServer(AppConfig.HttpPort);
            WebServer.DocumentRoot = AppConfig.HttpDocRoot;
            router = new HttpRouter(WebServer);
            WSServer = new WSServer(AppConfig.WebSocketPort);
            statusTimer = new System.Threading.Timer(new TimerCallback((o) => {
                TimerEvent();
            }), null, 3000, 1000);
            // サイト設定を読み込む
            SetCurrentConfigName(AppConfig.SiteDirName);
            WebServer.Start();
            WSServer.Start();
            AIRProxy air = AIRProxy.Instance;
            air.OnStateChanged += new AIRProxy.StateChangeHandler(air_OnStateChanged);
            air.OnGetSignals += new AIRProxy.GetSignalsHandler(air_OnGetSignals);
            air.OnError += new AIRProxy.ErrorHandler(air_OnError);
            air.SetCofFilePath(CurrentConfig.CofFilePath);
            air.Initialize();
        }
        /// <summary>
        /// エラーイベント
        /// </summary>
        /// <param name="e"></param>
        void air_OnError(AIRException e) {
            FireClientEvent(ClientEventType.ErrorMessage, e.Message);
        }
        /// <summary>
        /// 画像取得イベント
        /// </summary>
        void air_OnGetSignals(ushort[] signals) {
            var s = AIRSupport.Instance;
            s.WriteBitmap(signals, bmp);
            using (MemoryStream buf = new MemoryStream()) {
                bmp.Save(buf, System.Drawing.Imaging.ImageFormat.Png);
                pngImage = buf.ToArray();
            }
            Watcher.Watch(signals);
            WSServer.SendToAllClients(pngImage);
        }
        /// <summary>
        /// ステータス変化イベント
        /// </summary>
        /// <param name="state"></param>
        void air_OnStateChanged(AIRProxy.ProxyState state) {
            var air = AIRProxy.Instance;
            if (state == AIRProxy.ProxyState.Initialized) {
                // 初期化完了したらすぐ画像取得スレッドを開始
                air.StartImaging();
            } else {

            }
            FireStatusChange();
        }
        /// <summary>
        /// 現在の画像をファイル出力する
        /// </summary>
        /// <param name="filePath">出力先(拡張子png)</param>
        public void SaveImageToFile(string filePath) {
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            using (FileStream fs = new FileStream(filePath, FileMode.Create)) {
                fs.Write(pngImage, 0, pngImage.Length);
            }
        }

        /// <summary>
        /// 終了メソッド
        /// </summary>
        public void Dispose() {
            WebServer.Stop();
            WSServer.Dispose();
            AIRProxy.Instance.Release();
            statusTimer.Dispose();
        }

        /// <summary>
        /// 再起動する
        /// </summary>
        public void Restart() {
            var t = new Thread(new ThreadStart(() => {
                Thread.Sleep(1000);
                Dispose();
                Application.Restart();
            }));
            t.Start();
        }

        /// <summary>
        /// 終了してPCの電源を切る
        /// </summary>
        public void ShutdownPC() {
            var t = new Thread(new ThreadStart(() => {
                Thread.Sleep(1000);
                Dispose();
                // shutdown.exeを実行する
                var psi = new ProcessStartInfo();
                psi.FileName = "shutdown.exe";
                psi.Arguments = "/s /f /t 0";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                var p = Process.Start(psi);
            }));
            t.Start();
        }
        /// <summary>
        /// タイマーでイベント通知する
        /// </summary>
        private void TimerEvent() {
            if (StatusChanged) {
                FireStatusChange();
            }
            if (WatchboxChanged) {
                FireWatchboxChange();
            }
            if ((DateTime.Now - lastSessionReset).TotalSeconds > 30) {
                WSServer.CloseAllSessions();
                lastSessionReset = DateTime.Now;
            }
        }

        /// <summary>
        /// イベントを通知する
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void FireClientEvent(ClientEventType type, object data) {
            JsonWriter json = new JsonWriter();
            json.ReadableFormat = false;
            WSServer.SendToAllClients(type.ToString() + "\t" + json.Encode(data));
        }

        /// <summary>
        /// Status変更イベントを通知する
        /// </summary>
        private void FireStatusChange() {
            var mess = new Dictionary<string, object>();
            mess["Recording"] = Watcher.Enabled;
            mess["AIRStatus"] = AIR.AIRProxy.Instance.State;
            mess["SiteDirName"] = AppConfig.SiteDirName;
            FireClientEvent(TGIApp.ClientEventType.Status, mess);
            StatusChanged = false;
        }
        
        /// <summary>
        /// Watchbox変更イベントを通知する
        /// </summary>
        private void FireWatchboxChange() {
            var wb = new Dictionary<string, object>();
            wb["Main"] = MainWatchBox.Config;
            wb["Sub"] = SubWatchBox.Config;
            FireClientEvent(TGIApp.ClientEventType.Watchbox, wb);
            WatchboxChanged = false;
        }


        #region コマンド関連
        /// <summary>
        /// 現在のサイト設定オブジェクト
        /// </summary>
        public Config CurrentConfig {
            get {
                if (currentConfig == null) {
                    var dirName = AppConfig.SiteDirName;
                    currentConfig = GetConfig(dirName);
                }
                return currentConfig;
            }
        }

        /// <summary>
        /// 現在のサイト設定を変更する
        /// </summary>
        /// <param name="dirName"></param>
        public void SetCurrentConfigName(string dirName) {
            var dirPath = Path.Combine(AppConfig.SiteConfigRoot, dirName);
            if (!Directory.Exists(dirPath)) {
                FireClientEvent(ClientEventType.ErrorMessage,  "ディレクトリが存在しません。: " + dirName);
            } else {
                AppConfig.SiteDirName = dirName;
                AppConfig.Save();
                FireStatusChange();
                currentConfig = null;
                LoadSiteConfig();
                StatusChanged = true;
                WatchboxChanged = true;
            }
        }

        /// <summary>
        /// サイトディレクトリ名の一覧を返す
        /// </summary>
        /// <returns></returns>
        public List<SiteEntry> GetSites() {
            var dirs = Directory.GetDirectories(AppConfig.SiteConfigRoot);
            var ret = new List<SiteEntry>();
            foreach (var dir in dirs) {
                ret.Add(new SiteEntry() {
                    DirName = Path.GetFileName(dir),
                    SiteName = Config.SiteNameFor(dir)
                });
            }
            return ret;
        }

        /// <summary>
        /// サイト設定オブジェクトを返す
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public Config GetConfig(string dirName) {
            if (AppConfig.SiteDirName == dirName && currentConfig != null) {
                return currentConfig;
            }
            string dirPath = Path.Combine(AppConfig.SiteConfigRoot, dirName);
            if (!Directory.Exists(dirPath)) {
                dirPath = Path.Combine(AppConfig.SiteConfigRoot, "default");
                if (!Directory.Exists(dirPath)) {
                    Directory.CreateDirectory(dirPath);
                }
            }
            Config ret = new Config(dirPath);
            if (ret.CofFilePath == null) {
                ret.CofFilePath = GetDefaultCofFilePath();
            }
            return ret;
        }

        /// <summary>
        /// 指定したサイト設定を複製して新しいサイトを作成する
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public Config Duplicate(string dirName) {
            var dirPath = Path.Combine(AppConfig.SiteConfigRoot, dirName);
            string newDirPath;
            string newDirName;
            while (true) {
                newDirName = DateTime.Now.ToString("yyyyMMddHHmmss");
                newDirPath = Path.Combine(AppConfig.SiteConfigRoot, newDirName);
                if (Directory.Exists(newDirPath)) {
                    Thread.Sleep(1000);
                } else {
                    break;
                }
            }
            Config.Copy(dirPath, newDirPath);
            return GetConfig(newDirName);
        }

        /// <summary>
        /// デフォルトのcofファイルパスを返す
        /// </summary>
        /// <returns></returns>
        private string GetDefaultCofFilePath() {
            string[] files = Directory.GetFiles(AppConfig.CofRoot);
            foreach (var file in files) {
                return file;
            }
            return null;
        }

        /// <summary>
        /// 設置されているcofファイルのファイル名一覧を返す
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetCofFileNames() {
            string[] files = Directory.GetFiles(AppConfig.CofRoot);
            Dictionary<string, string> ret = new Dictionary<string, string>();
            int counter = 1;
            foreach (var file in files) {
                if (file.EndsWith(".cof")) {
                    var fn = Path.GetFileName(file);
                    ret[counter.ToString()] = fn;
                    counter++;
                }
            }
            return ret;
        }
        /// <summary>
        /// Configに設定されている値に対応するキーを返す
        /// </summary>
        /// <param name="cofFilePath"></param>
        /// <returns></returns>
        public string GetCofFileKey(string cofFilePath) {
            var fileName = Path.GetFileName(cofFilePath);
            foreach (var kv in GetCofFileNames()) {
                if (kv.Value == fileName) {
                    return kv.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// 登録されている閾値判定用変数の名前一覧を返す
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetThresholdNames() {
            var ret = new Dictionary<string, string>();
            foreach (var kv in GetThresholdSets()) {
                ret[kv.Key] = kv.Value.Name + "(傾き=" + kv.Value.Slope + "/切片=" + kv.Value.Intercept + ")";
            }
            return ret;
        }

        /// <summary>
        /// 傾きと切片に対応する閾値判定用キーを返す
        /// </summary>
        /// <param name="slope"></param>
        /// <param name="intercept"></param>
        /// <returns></returns>
        public string GetThresholdSetKey(double slope, double intercept) {
            foreach (var kv in GetThresholdSets()) {
                var t = kv.Value;
                if (t.Slope == slope && t.Intercept == intercept) {
                    return kv.Key;
                }
            }
            return "";
        }

        /// <summary>
        /// 登録されている閾値判定用変数セットの一覧を返す
        /// CSVファイルがいつ変更されてもいいように、毎回読み込んで返す
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ThresholdSet> GetThresholdSets() {
            string filePath = AppConfig.ThresholdSetFile;
            var ret = new Dictionary<string, ThresholdSet>();
            if (File.Exists(filePath)) {
                using (TextFieldParser parser = new TextFieldParser(filePath, Encoding.GetEncoding("Shift_JIS"))) {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    int counter = 1;
                    while (!parser.EndOfData) {
                        var fields = parser.ReadFields();
                        if (fields.Length == 3) {
                            try {
                                var ts = new ThresholdSet() {
                                    Name = fields[0],
                                    Slope = Double.Parse(fields[1]),
                                    Intercept = Double.Parse(fields[2])
                                };
                                ret[counter.ToString()] = ts;
                                counter++;
                            } catch (Exception) {
                                FireClientEvent(ClientEventType.ErrorMessage, filePath + "の書式に誤りがあります。");
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// カラーマップの名前一覧を返す
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetColorMaps() {
            var ret = new Dictionary<string, string>();
            ret[ColorMapMaker.FocusType.None.ToString()] = "通常";
            ret[ColorMapMaker.FocusType.Low.ToString()] = "低温域強調";
            ret[ColorMapMaker.FocusType.Mid.ToString()] = "中温域強調";
            ret[ColorMapMaker.FocusType.High.ToString()] = "高温域強調";
            return ret;
        }
        /// <summary>
        /// アプリケーション設定を読み込む
        /// </summary>
        private void LoadAppConfig() {
            MainWatchBox.CellCount = AppConfig.WatchBoxCellCount;
            SubWatchBox.CellCount = AppConfig.WatchBoxCellCount;
        }

        /// <summary>
        /// サイト設定を読み込む
        /// </summary>
        public void LoadSiteConfig() {
            Config c = CurrentConfig;
            // Cof file
            AIRProxy p = AIRProxy.Instance;
            p.SetCofFilePath(c.CofFilePath);
            // メイン監視枠の設定
            MainWatchBox.Config.SetRect(c.MainWatchBoxRect);
            MainWatchBox.Config.Gap = c.MainWatchBoxGap;
            MainWatchBox.Config.Mode = c.MainWatchBoxMode;
            // サブ監視枠の設定
            SubWatchBox.Config.SetRect(c.SubWatchBoxRect);
            // カメラ関係の設定
            var support = AIRSupport.Instance;
            support.FocusType = c.ColorMapFocusType;
            support.Tmax = c.ProxyTmax;
            support.Tmin = c.ProxyTmin;
            // Watcherの設定
            Watcher.ThresholdIntercept = c.ThresholdIntercept;
            Watcher.ThresholdSlope = c.ThresholdSlope;
            Watcher.ImageOutputEnabled = c.ImageOutput;
            Watcher.ImageOutputDir = c.ImageOutDir;
            Watcher.OutputEnabled = c.CsvOutput;
            Watcher.OutputDir = c.CsvOutDir;
        }

        /// <summary>
        /// 監視枠の設定を現在のサイト設定に反映する
        /// </summary>
        /// <param name="c"></param>
        public void SaveWatchboxConfigs() {
            Config c = CurrentConfig;
            c.MainWatchBoxRect = MainWatchBox.Config.GetRect();
            c.MainWatchBoxGap = MainWatchBox.Config.Gap;
            c.MainWatchBoxMode = MainWatchBox.Config.Mode;
            c.SubWatchBoxRect = SubWatchBox.Config.GetRect();
            c.Save();
            WatchboxChanged = true;
        }

        #endregion

    }
}
