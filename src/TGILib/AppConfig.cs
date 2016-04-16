using SAT.Util;
using System.IO;
using System.Drawing;
using System.Reflection;


namespace TGILib {
    /// <summary>
    /// アプリケーションの設定値
    /// </summary>
    internal class AppConfig {
        /// <summary>
        /// プロパティを読み込むためのインスタンス
        /// </summary>
        private static FileProperties Properties {
            get {
                return FileProperties.Default;
            }
        }

        /// <summary>
        /// staticコンストラクタ
        /// </summary>
        static AppConfig() {
            var iniPath = FileUtil.ResolvePath("tgi.ini");
            Properties.Load(iniPath);
        }

        /// <summary>
        /// HTTPサーバのポート
        /// </summary>
        public static int HttpPort {
            get {
                return Properties.GetInt("HttpPort", 80);
            }
        }
        /// <summary>
        /// WebSocketサーバのポート
        /// </summary>
        public static int WebSocketPort {
            get {
                return Properties.GetInt("WebSocketPort", 8082);
            }
        }
        /// <summary>
        /// HTTPサーバのドキュメントルート
        /// </summary>
        public static string HttpDocRoot {
            get {
                return FileUtil.ResolvePath(Properties.GetString("HttpDocRoot", @".\www"));
            }
        }
        /// <summary>
        /// サイト設定のルートディレクトリ
        /// </summary>
        public static string SiteConfigRoot {
            get {
                var res = FileUtil.ResolvePath(Properties.GetString("SiteConfigRoot", null));
                if (res != null && Directory.Exists(res)) {
                    return res;
                } else {
                    return FileUtil.ResolvePath(@"..\data\sites");
                }
            }
        }
        /// <summary>
        /// 使用中のサイト設定
        /// </summary>
        public static string SiteDirName {
            get {
                return Properties.GetString("SiteDirName", "default");
            }
            set {
                Properties.SetValue("SiteDirName", value);
            }
        }

        /// <summary>
        /// cofファイルのルートディレクトリ
        /// </summary>
        public static string CofRoot {
            get {
                return FileUtil.ResolvePath(Properties.GetString("CofRoot", @"C:\Vcam2\Install"));
            }
        }
        /// <summary>
        /// 閾値判定変数セットのファイル名
        /// </summary>
        public static string ThresholdSetFile {
            get {
                const string DEF = @"..\data\thresholdset.csv";
                var str = Properties.GetString("ThresholdSetFile", DEF);
                return FileUtil.ResolvePath(str);
            }
        }

        
        /// <summary>
        /// WatchBoxのセル分割数
        /// </summary>
        public static int WatchBoxCellCount {
            get {
                return Properties.GetInt("WatchBoxCellCount", 10);
            }
        }
        /// <summary>
        /// 正常値を示すセル数の最小値
        /// </summary>
        public static int WatchBoxSafeCellCount {
            get {
                return Properties.GetInt("WatchBoxSafeCellCount", 3);
            }
        }
        /// <summary>
        /// 保存する
        /// </summary>
        public static void Save() {
            Properties.Save();
        }
    }
}
