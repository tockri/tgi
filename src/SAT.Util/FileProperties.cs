using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SAT.Util {
    /// <summary>
    /// テキストファイルを読み書きするプロパティクラス
    /// </summary>
    public class FileProperties {
        private static FileProperties defProperties;
        /// <summary>
        /// デフォルトのインスタンス
        /// </summary>
        public static FileProperties Default {
            get {
                if (defProperties == null) {
                    defProperties = new FileProperties();
                }
                return defProperties;
            }
        }

        /// <summary>
        /// 読み込んだ情報を記録する
        /// </summary>
        private class Entry {
            private string _line;
            /// <summary>
            /// 行表現
            /// </summary>
            public string Line {
                get { return _line; }
                set { 
                    _line = value;
                    if (_line.IndexOf("=") > 0 && !_line.StartsWith("#")) {
                        string[] kv = _line.Split('=');
                        _key = _line.Substring(0, _line.IndexOf("="));
                        _value = _line.Substring(_line.IndexOf("=") + 1);
                        _value = _value.Replace(@"\n", "\n").Replace(@"\r", "\r").Replace(@"￥", @"\");
                    }
                }
            }

            private string _value;
            /// <summary>
            /// 値
            /// </summary>
            public string Value {
                get {
                    return _value; 
                }
                set {
                    _value = value != null ? value : "";
                    Line = _key + "=" + _value.Replace(@"\", @"￥").Replace("\r", @"\r").Replace("\n", @"\n");
                }
            }

            private string _key;
            /// <summary>
            /// キー
            /// </summary>
            public string Key {
                get {
                    return _key;
                }
                set {
                    _key = value;
                    Line = _key + "=" + _value;
                }
            }
            /// <summary>
            /// 一行を読み込むコンストラクタ
            /// </summary>
            /// <param name="expr"></param>
            public Entry(string expr) {
                Line = expr;
            }
            /// <summary>
            /// キーと値を設定するコンストラクタ
            /// </summary>
            /// <param name="key"></param>
            /// <param name="value"></param>
            public Entry(string key, string value) {
                _key = key;
                Value = value;
            }
        }

        private Dictionary<string, Entry> map = new Dictionary<string, Entry>();
        private List<Entry> lines = new List<Entry>();
        private string loadedFile;
        /// <summary>
        /// 読み込まれているファイルパス
        /// </summary>
        public string LoadedFilePath {
            get {
                return loadedFile;
            }
        }
        /// <summary>
        /// 文字列のプロパティを返す
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public string GetString(string key, string defValue) {
            if (map.ContainsKey(key)) {
                return map[key].Value;
            } else {
                return defValue;
            }
        }
        /// <summary>
        /// 整数のプロパティを返す
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public int GetInt(string key, int defValue) {
            if (map.ContainsKey(key)) {
                return int.Parse(map[key].Value);
            } else {
                return defValue;
            }
        }
        /// <summary>
        /// 小数のプロパティを返す
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public double GetDouble(string key, double defValue) {
            if (map.ContainsKey(key)) {
                return double.Parse(map[key].Value);
            } else {
                return defValue;
            }
        }

        /// <summary>
        /// 真偽値のプロパティを返す
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defValue"></param>
        /// <returns></returns>
        public bool GetBool(string key, bool defValue) {
            if (map.ContainsKey(key)) {
                return bool.Parse(map[key].Value);
            } else {
                return defValue;
            }
        }
        /// <summary>
        /// プロパティを設定する
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetValue(string key, object value) {
            if (map.ContainsKey(key)) {
                map[key].Value = value.ToString();
            } else {
                if (value == null) {
                    value = "";
                }
                Entry e = new Entry(key, value.ToString());
                map[key] = e;
                lines.Add(e);
            }
        }

        /// <summary>
        /// 全てを削除する
        /// </summary>
        public void Clear() {
            map = new Dictionary<string, Entry>();
            lines = new List<Entry>();
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="filePath"></param>
        public void Load(string filePath) {
            Clear();
            loadedFile = filePath;
            if (File.Exists(filePath)) {
                foreach (string expr in File.ReadAllLines(filePath)) {
                    Entry e = new Entry(expr);
                    if (e.Key != null && map.ContainsKey(e.Key)) {
                        map[e.Key].Value = e.Value;
                    } else {
                        lines.Add(e);
                        if (e.Key != null) {
                            map[e.Key] = e;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 保存する
        /// </summary>
        public void Save() {
            SaveTo(loadedFile);
        }
        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveTo(string filePath) {
            using (StreamWriter fout = new StreamWriter(filePath)) {
                foreach (Entry e in lines) {
                    fout.WriteLine(e.Line);
                }
                fout.Close();
            }
        }
    }
}
