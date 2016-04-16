using System;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace SAT.Util {
    /// <summary>
    /// JSONを書き出すユーティリティ
    /// </summary>
    public class JsonWriter {
        /// <summary>
        /// インデント文字列
        /// </summary>
        private const string INDENT = "  ";
        /// <summary>
        /// エスケープパターン
        /// </summary>
        private static Regex EscapePattern = new Regex(@"[\n\r""']");
        /// <summary>
        /// 読みやすく改行入れるフラグ
        /// </summary>
        public bool ReadableFormat {
            get;
            set;
        }
        /// <summary>
        /// 文字コード
        /// </summary>
        public Encoding Encoding {
            get;
            set;
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JsonWriter() {
            ReadableFormat = false;
            Encoding = new UTF8Encoding(false);
        }
        /// <summary>
        /// JSON文字列に変換する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Encode(object obj) {
            using (MemoryStream dst = new MemoryStream()) {
                Write(dst, obj);
                byte[] bytes = dst.ToArray();
                return Encoding.GetString(bytes);
            }
        }
        /// <summary>
        /// JSONをストリームに書き出す
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="obj"></param>
        public void Write(Stream dst, object obj) {
            using (StreamWriter wr = new StreamWriter(dst, Encoding)) {
                WriteInner(wr, obj, 0);
                wr.Close();
            }
        }
        /// <summary>
        /// 書き出す実処理
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="obj"></param>
        /// <param name="depth"></param>
        private void WriteInner(StreamWriter dst, object obj, int depth) {
            if (obj == null) {
                dst.Write("null");
            } else  if (obj is IDictionary) {
                var dic = obj as IDictionary;
                dst.Write("{");
                nl(dst, depth + 1);
                var keys = dic.Keys;
                int i = 0;
                foreach (var key in keys) {
                    var value = dic[key];
                    dst.Write("\"");
                    dst.Write(key.ToString());
                    dst.Write("\": ");
                    WriteInner(dst, value, depth + 1);
                    if (i == keys.Count - 1) {
                        nl(dst, depth);
                    } else {
                        dst.Write(",");
                        nl(dst, depth + 1);
                    }
                    i++;
                }
                dst.Write("}");
            } else if (obj is IList) {
                var list = obj as IList;
                dst.Write("[");
                nl(dst, depth + 1);
                for (int i = 0; i < list.Count; i++) {
                    var elem = list[i];
                    WriteInner(dst, elem, depth + 1);
                    if (i == list.Count - 1) {
                        nl(dst, depth);
                    } else {
                        dst.Write(",");
                        nl(dst, depth + 1);
                    }
                }
                dst.Write("]");
            } else if (obj is string || obj is Enum) {
                // 文字列
                dst.Write("\"");
                dst.Write(Escape(obj.ToString()));
                dst.Write("\"");
            } else if (obj is bool) {
                // bool
                dst.Write(obj.ToString().ToLower());
            } else if (IsNumeric(obj)) {
                // 数値型
                dst.Write(obj.ToString());
            } else {
                // その他のオブジェクト＝publicフィールドとプロパティのみを出力
                Type t = obj.GetType();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (var fi in t.GetFields()) {
                    if (IsWritableType(fi.FieldType)) {
                        try {
                            dic[fi.Name] = fi.GetValue(obj);
                        } catch (Exception) { 
                        }
                    }
                }
                foreach (var pi in t.GetProperties()) {
                    if (IsWritableType(pi.PropertyType)) {
                        try {
                            dic[pi.Name] = pi.GetValue(obj, null);
                        } catch (Exception) {
                        }
                    }
                }
                WriteInner(dst, dic, depth);
            }
        }
        /// <summary>
        /// 数値化どうかを返す
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private bool IsNumeric(object o) {
            return o is IConvertible;
        }
        /// <summary>
        /// 書き出し可能
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private bool IsWritableType(Type t) {
            Type[] types = new Type[] {
                typeof(IDictionary),
                typeof(IList),
                typeof(IConvertible),
                typeof(Enum),
                typeof(string),
            };
            foreach (var tp in types) {
                if (tp.IsAssignableFrom(t)) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 改行、クオート記号のエスケープ
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string Escape(string str) {
            return EscapePattern.Replace(str, (m) => {
                switch (m.Value) {
                    case "\n":
                        return @"\n";
                    case "\r":
                        return @"\r";
                    case "\"":
                        return @"\""";
                    case "'":
                        return @"\'";
                    default:
                        return m.Value;
                }
            });
        }
        /// <summary>
        /// 改行してインデントする
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="depth"></param>
        private void nl(StreamWriter dst, int depth) {
            if (ReadableFormat) {
                dst.Write("\n");
                for (int i = 0; i < depth; i++) {
                    dst.Write(INDENT);
                }
            }
        }
    }


}
