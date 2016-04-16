using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Data.Common;

namespace SAT.Util {
    /// <summary>
    /// ログ出力ユーティリティ。
    /// <para>ログ出力先としてファイルとコンソールを選択できる。</para>
    /// <para>ログ出力先としてファイルを選択する場合、出力フォルダはアプリケーション構成ファイルの
    /// SAT.Util.Logger.logDirに従う。</para>
    /// <para>ログレベルをコード内で</para>
    /// </summary>
    public class Logger {

        /// <summary>
        /// レベルを3種類
        /// </summary>
        public enum LogLevel {
            /// <summary>
            /// エラーのみをログ出力するレベル
            /// </summary>
            ERROR = 1,
            /// <summary>
            /// 警告、エラーを出力する
            /// </summary>
            WARN = 2,
            /// <summary>
            /// 通常運用レベル
            /// </summary>
            INFO = 3, 
            /// <summary>
            /// デバッグレベル。
            /// </summary>
            DEBUG = 4, 
            /// <summary>
            /// ログを一切出さない設定
            /// </summary>
            NONE = 0,
            /// <summary>
            /// アプリケーション構成ファイルの記述に従う
            /// </summary>
            CONFIG = -1
        }
        /// <summary>
        /// 出力先。ファイルまたはコンソール
        /// </summary>
        public enum LogOutput {
            /// <summary>
            /// コンソール出力
            /// </summary>
            CONSOLE, 
            /// <summary>
            /// ファイル出力
            /// </summary>
            FILE
        }
        /// <summary>
        /// ログレベル
        /// </summary>
        public static LogLevel Level = LogLevel.CONFIG;
        /// <summary>
        /// 出力先ディレクトリ
        /// </summary>
        private static string logDir = UtilProperties.LogDirectory;
        /// <summary>
        /// 初回出力時のフラグ
        /// </summary>
        private static bool firstTime = true;
        /// <summary>
        /// 出力先。デフォルトはファイル
        /// </summary>
        public static LogOutput Output = LogOutput.FILE;
        private static bool initialized = false;
        /// <summary>
        /// 最後にディレクトリの掃除をした時刻
        /// </summary>
        private static DateTime lastCleaned = DateTime.MinValue;
        /// <summary>
        /// この日数以上経ったログファイルを削除する
        /// </summary>
        private static int cleanThreshold = UtilProperties.LogCleanThreshold;
        /// <summary>
        /// 1日1回Cleanする
        /// </summary>
        private static TimeSpan cleanInterval = new TimeSpan(24, 0, 0);
        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private static object Lock = DateTime.Now.ToString();

        /// <summary>
        /// 初期化
        /// </summary>
        private static void Initialize() {
            if (initialized) {
                return;
            }
            switch (UtilProperties.LogLevel.ToUpper()) {
                case "NONE":
                    Level = LogLevel.NONE;
                    break;
                case "ERROR":
                    Level = LogLevel.ERROR;
                    break;
                case "INFO":
                    Level = LogLevel.INFO;
                    break;
                case "WARN":
                    Level = LogLevel.WARN;
                    break;
                case "DEBUG":
                    Level = LogLevel.DEBUG;
                    break;
                default:
                    Level = LogLevel.INFO;
                    break;
            }
            if (!Directory.Exists(logDir)) {
                Directory.CreateDirectory(logDir);
            }
            initialized = true;
        }

        /// <summary>
        /// レベルがERRORまたはINFOまたはDEBUGのとき（いつでも）出力する
        /// </summary>
        /// <param name="o"></param>
        /// <param name="message"></param>
        public static void error(object o, string message) {
            Log(LogLevel.ERROR, o, message);
        }
        /// <summary>
        /// レベルがERRORまたはINFOまたはDEBUGのとき（いつでも）出力する
        /// </summary>
        /// <param name="message"></param>
        public static void error(string message) {
            error(null, message);
        }
        /// <summary>
        /// レベルがWARNのとき（いつでも）出力する
        /// </summary>
        /// <param name="o"></param>
        /// <param name="message"></param>
        public static void warn(object o, string message) {
            Log(LogLevel.WARN, o, message);
        }
        /// <summary>
        /// レベルがWARNのとき（いつでも）出力する
        /// </summary>
        /// <param name="message"></param>
        public static void warn(string message) {
            warn(null, message);
        }



        /// <summary>
        /// レベルがINFOまたはDEBUGのときだけ出力する
        /// </summary>
        /// <param name="o"></param>
        /// <param name="message"></param>
        /// 
        public static void info(object o, string message) {
            Log(LogLevel.INFO, o, message);
        }
        /// <summary>
        /// レベルがINFOまたはDEBUGのときだけ出力する
        /// </summary>
        /// <param name="message"></param>
        public static void info(string message) {
            info(null, message);
        }

        /// <summary>
        /// レベルがDEBUGのときだけ出力する
        /// </summary>
        /// <param name="o"></param>
        /// <param name="message"></param>
        public static void debug(object o, string message) {
            Log(LogLevel.DEBUG, o, message);
        }
        /// <summary>
        /// レベルがDEBUGのときだけ出力する
        /// </summary>
        /// <param name="message"></param>
        public static void debug(string message) {
            debug(null, message);
        }

        /// <summary>
        /// ログを書き出す実処理。
        /// </summary>
        /// <param name="obj">任意のオブジェクト</param>
        /// <param name="level">レベル</param>
        /// <param name="message">メッセージ</param>
        private static void Log(LogLevel level, object obj, string message) {
            Initialize();
            if (level <= Level) {
                string todayStr = System.DateTime.Now.ToString("yyyy-MM-dd");
                StringWriter sw = new StringWriter();
                if (firstTime) {
                    sw.WriteLine("####################################################");
                    firstTime = false;
                }
                string now = DateTime.Now.ToString("HH:mm:ss");
                sw.WriteLine("-----------------------");
                sw.WriteLine(now + "[" + level + "]   " + message);
                if (obj != null) {
                    sw.WriteLine(Inspect(obj, 0));
                }
                if (Output == LogOutput.FILE) {
                    lock (Lock) {
                        using (StreamWriter file = new StreamWriter(logDir + "/log_" + todayStr + ".txt", true)) {
                            file.Write(sw.ToString());
                        }
                    }
                } else {
                    Console.Write(sw.ToString());
                }
            }
            Clean();
        }
        /// <summary>
        /// objectを再帰的にたどって文字列にする
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="depth">スタック深さ</param>
        /// <returns></returns>
        private static string Inspect(object obj, int depth) {
            string spacer = "";
            for (int i = 0; i < depth; i++) {
                spacer += "  ";
            }
            if (obj is string) {
                return spacer + obj.ToString();
            } else if (obj is DbCommand) {
                // SQLコマンドをログ出力する
                DbCommand comm = (DbCommand)obj;
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\nSQL : ").Append(comm.CommandText);
                foreach (DbParameter p in comm.Parameters) {
                    sb.Append("\n").Append(p.ParameterName).Append(" : ").Append(p.Value != null ? p.Value.ToString() : "");
                }
                sb.Append("\n}");
                return sb.ToString();
            } else if (obj is IDictionary) {
                // Dictionaryを出力
                StringBuilder sb = new StringBuilder();
                sb.Append("{\n");
                foreach (DictionaryEntry ent in (IDictionary)obj) {
                    sb.Append(spacer).Append(ent.Key).Append(" : ").Append(Inspect(ent.Value, depth + 1)).Append("\n");
                }
                sb.Append("}");
                return sb.ToString();
            } else  if (obj is IEnumerable) {
                // Listを出力
                StringBuilder sb = new StringBuilder();
                sb.Append("[\n");
                foreach (object o in (IEnumerable)obj) {
                    sb.Append(spacer).Append(Inspect(o, depth + 1)).Append("\n");
                }
                sb.Append("]");
                return sb.ToString();
            } else {
                // それ以外のオブジェクトを出力
                return spacer + obj.ToString();
            }

        }
        /// <summary>
        /// 古いログファイルを削除する
        /// </summary>
        private static void Clean() {
            try {
                if (DateTime.Now - lastCleaned > cleanInterval) {
                    DirectoryInfo dir = new DirectoryInfo(logDir);
                    DateTime limit = DateTime.Today.AddDays(-cleanThreshold);
                    foreach (FileInfo fi in dir.GetFiles("log_*.txt")) {
                        if (fi.LastWriteTime < limit) {
                            try {
                                fi.Delete();
                            } catch (Exception) {
                            }
                        }
                    }
                    lastCleaned = DateTime.Now;
                }
            } catch (Exception) {
            }
        }
    }
}