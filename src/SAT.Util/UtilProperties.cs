using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SAT.Util {
    /// <summary>
    /// SAT.Utilで利用するプロパティを簡単に取るクラス
    /// </summary>
    public class UtilProperties {
        /// <summary>
        /// プロパティを読み込むためのインスタンス
        /// </summary>
        public static FileProperties Properties {
            get;
            set;
        }
        static UtilProperties() {
            Properties = FileProperties.Default;
        }

        /// <summary>
        /// Loggerが利用するディレクトリ
        /// </summary>
        public static string LogDirectory {
            get {
                string defDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Properties.GetString("SAT.Util.Logger.LogDir", defDir);
            }
        }
        /// <summary>
        /// Loggerのデフォルトレベル
        /// </summary>
        public static string LogLevel {
            get {
                return Properties.GetString("SAT.Util.Logger.LogLevel", "INFO");
            }
        }
        /// <summary>
        /// 古いログを削除する日数のしきい値
        /// デフォルト30
        /// </summary>
        public static int LogCleanThreshold {
            get {
                return Properties.GetInt("SAT.Util.Logger.CleanThreshold", 30);
            }
        }

    }
}
