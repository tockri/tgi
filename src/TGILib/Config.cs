using SAT.Util;
using System;
using System.IO;
using System.Drawing;
using System.Reflection;


namespace TGILib {
    /// <summary>
    /// アプリケーションの設定値
    /// </summary>
    public class Config {
        private const string IniFileName = "config.ini";
        private const string NameFileName = "name.txt";

        /// <summary>
        /// プロパティを読み込むためのインスタンス
        /// </summary>
        private readonly FileProperties Properties;
        /// <summary>
        /// サイト名を返す
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static string SiteNameFor(string dirPath) {
            var path = Path.Combine(dirPath, NameFileName);
            if (File.Exists(path)) {
                return File.ReadAllText(path);
            } else {
                return "";
            }
        }
        /// <summary>
        /// サイト設定をコピーする
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="newDirPath"></param>
        public static void Copy(string dirPath, string newDirPath) {
            if (Directory.Exists(newDirPath)) {
                return;
            }
            Directory.CreateDirectory(newDirPath);
            var srcConfigPath = Path.Combine(dirPath, IniFileName);
            if (File.Exists(srcConfigPath)) {
                File.Copy(srcConfigPath, Path.Combine(newDirPath, IniFileName));
            }
        }

        /// <summary>
        /// シングルトン
        /// </summary>
        public Config(string baseDir) {
            if (!Directory.Exists(baseDir)) {
                Directory.CreateDirectory(baseDir);
            }
            Properties = new FileProperties();
            string filePath = Path.Combine(baseDir, IniFileName);
            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "");
            }
            Properties.Load(filePath);
        }

        /// <summary>
        /// iniファイルが存在するディレクトリ。
        /// iniファイルが読み込まれていない場合は、アセンブリの場所
        /// </summary>
        private string BaseDir {
            get {
                // iniファイルのディレクトリ
                return Path.GetDirectoryName(Properties.LoadedFilePath);
            }
        }

        /// <summary>
        /// ディレクトリ名
        /// </summary>
        public string DirName {
            get {
                return Path.GetFileName(BaseDir);
            }
        }

        /// <summary>
        /// cofファイルのパス
        /// </summary>
        public string CofFilePath {
            get {
                return Properties.GetString("CofFilePath", null);
            }
            set {
                Properties.SetValue("CofFilePath", value);
            }
        }

        /// <summary>
        /// サイト名
        /// </summary>
        public string SiteName {
            get {
                return SiteNameFor(BaseDir);
            }
            set {
                var path = Path.Combine(BaseDir, NameFileName);
                File.WriteAllText(path, value);
            }
        }
        /// <summary>
        /// 接合者
        /// </summary>
        public string SitePerson {
            get {
                return Properties.GetString("SitePerson", null);
            }
            set {
                Properties.SetValue("SitePerson", value);
            }
        }
        /// <summary>
        /// シートタイプ
        /// </summary>
        public string SheetType {
            get {
                return Properties.GetString("SiteSheetType", null);
            }
            set {
                Properties.SetValue("SiteSheetType", value);
            }
        }
        /// <summary>
        /// 接合温度
        /// </summary>
        public string FusionTemp {
            get {
                return Properties.GetString("SiteFusionTemp", null);
            }
            set {
                Properties.SetValue("SiteFusionTemp", value);
            }
        }
        /// <summary>
        /// 速度
        /// </summary>
        public string FusionSpeed {
            get {
                return Properties.GetString("SiteFusionSpeed", null);
            }
            set {
                Properties.SetValue("SiteFusionSpeed", value);
            }
        }
        /// <summary>
        /// 圧力
        /// </summary>
        public string FusionPressure {
            get {
                return Properties.GetString("SiteFusionPressure", null);
            }
            set {
                Properties.SetValue("SiteFusionPressure", value);
            }
        }
        /// <summary>
        /// メモ
        /// </summary>
        public string Memo {
            get {
                return Properties.GetString("SiteMemo", null);
            }
            set {
                Properties.SetValue("SiteMemo", value);
            }
        }


        /// <summary>
        /// シグナル最大値の時の温度
        /// </summary>
        public int ProxyTmax {
            get {
                return Properties.GetInt("ProxyTmax", 100);
            }
            set {
                Properties.SetValue("ProxyTmax", value);
            }
        }

        /// <summary>
        /// シグナル最小値の時の温度
        /// </summary>
        public int ProxyTmin {
            get {
                return Properties.GetInt("ProxyTmin", 0);
            }
            set {
                Properties.SetValue("ProxyTmin", value);
            }
        }

        /// <summary>
        /// 閾値自動判定式の傾き
        /// </summary>
        public double ThresholdSlope {
            get {
                return Properties.GetDouble("ThresholdSlope", 0);
            }
            set {
                Properties.SetValue("ThresholdSlope", value);
            }
        }
        /// <summary>
        /// 閾値自動判定式の切片
        /// </summary>
        public double ThresholdIntercept {
            get {
                return Properties.GetDouble("ThresholdIntercept", 0);
            }
            set {
                Properties.SetValue("ThresholdIntercept", value);
            }
        }


        /// <summary>
        /// ファイル出力ディレクトリ
        /// </summary>
        public string CsvOutDir {
            get {
                return Path.Combine(BaseDir, "CSV");
            }
        }

        /// <summary>
        /// ファイル出力を行う
        /// </summary>
        public bool CsvOutput {
            get {
                return Properties.GetBool("CsvOutEnable", true);
            }
            set {
                Properties.SetValue("CsvOutEnable", value);
            }
        }

        /// <summary>
        /// 画像ファイル出力ディレクトリ
        /// </summary>
        public string ImageOutDir {
            get {
                return Path.Combine(BaseDir, "image");
            }
        }

        /// <summary>
        /// 画像ファイル出力を行う
        /// </summary>
        public bool ImageOutput {
            get {
                return Properties.GetBool("ImageOutEnable", true);
            }
            set {
                Properties.SetValue("ImageOutEnable", value);
            }
        }

        /*
        /// <summary>
        /// グラフを表示するかどうか
        /// </summary>
        public bool ShowGraphWindow {
            get {
                return Properties.GetBool("ShowGraphWindow", true);
            }
            set {
                Properties.SetValue("ShowGraphWindow", value);
            }
        }
        /// <summary>
        /// グラフの位置
        /// </summary>
        public Point GraphWindow1Location {
            get {
                return new Point(Properties.GetInt("GraphWindow1X", 100), Properties.GetInt("GraphWindow1Y", 100));
            }
            set {
                Properties.SetValue("GraphWindow1X", value.X);
                Properties.SetValue("GraphWindow1Y", value.Y);
            }
        }
        /// <summary>
        /// グラフの位置
        /// </summary>
        public Point GraphWindow2Location {
            get {
                return new Point(Properties.GetInt("GraphWindow2X", 100), Properties.GetInt("GraphWindow2Y", 300));
            }
            set {
                Properties.SetValue("GraphWindow2X", value.X);
                Properties.SetValue("GraphWindow2Y", value.Y);
            }
        }
        */

        /// <summary>
        /// box1の枠
        /// </summary>
        public Rectangle MainWatchBoxRect {
            get {
                return new Rectangle(
                    Properties.GetInt("MainWatchBoxRectX", 10),
                    Properties.GetInt("MainWatchBoxRectY", 20),
                    Properties.GetInt("MainWatchBoxRectWidth", 300),
                    Properties.GetInt("MainWatchBoxRectHeight", 30)
                );
            }
            set {
                Properties.SetValue("MainWatchBoxRectX", value.X);
                Properties.SetValue("MainWatchBoxRectY", value.Y);
                Properties.SetValue("MainWatchBoxRectWidth", value.Width);
                Properties.SetValue("MainWatchBoxRectHeight", value.Height);
            }
        }

        /// <summary>
        /// boxの枠の間隔
        /// </summary>
        public int MainWatchBoxGap {
            get {
                return Properties.GetInt("MainWatchBoxGap", 10);
            }
            set {
                Properties.SetValue("MainWatchBoxGap", value);
            }
        }
        /// <summary>
        /// boxのdouble mode
        /// </summary>
        public string MainWatchBoxMode {
            get {
                return Properties.GetString("WatchBoxMode", "single");
            }
            set {
                if ("single".Equals(value, System.StringComparison.CurrentCultureIgnoreCase)
                    || "double".Equals(value, System.StringComparison.CurrentCultureIgnoreCase)) {
                    Properties.SetValue("WatchBoxMode", value);
                } else {
                    Properties.SetValue("WatchBoxMode", "single");
                }
            }
        }

        /// <summary>
        /// box2の枠
        /// </summary>
        public Rectangle SubWatchBoxRect {
            get {
                return new Rectangle(
                    Properties.GetInt("SubWatchBoxRectX", 10),
                    Properties.GetInt("SubWatchBoxRectY", 10),
                    Properties.GetInt("SubWatchBoxRectWidth", 30),
                    Properties.GetInt("SubWatchBoxRectHeight", 30)
                );
            }
            set {
                Properties.SetValue("SubWatchBoxRectX", value.X);
                Properties.SetValue("SubWatchBoxRectY", value.Y);
                Properties.SetValue("SubWatchBoxRectWidth", value.Width);
                Properties.SetValue("SubWatchBoxRectHeight", value.Height);
            }
        }

        /// <summary>
        /// アラーム音
        /// </summary>
        public bool AlermSound {
            get {
                return Properties.GetBool("WatcherAlermSound", true);
            }
            set {
                Properties.SetValue("WatcherAlermSound", value);
            }
        }

        /// <summary>
        /// カラーマップ種類
        /// </summary>
        public ColorMapMaker.FocusType ColorMapFocusType {
            get {
                var val = Properties.GetString("ColorMapFocusType", "None");
                try {
                    return (ColorMapMaker.FocusType)Enum.Parse(typeof(ColorMapMaker.FocusType), val);
                } catch (Exception) {
                    return ColorMapMaker.FocusType.None;
                }
            }
            set {
                Properties.SetValue("ColorMapFocusType", value.ToString());
            }
        }
        /// <summary>
        /// 保存する
        /// </summary>
        public void Save() {
            Properties.Save();
        }

    }
}
