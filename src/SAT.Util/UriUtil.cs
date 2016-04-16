using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace SAT.Util {
    /// <summary>
    /// Uriに関する便利メソッド
    /// </summary>
    public class UriUtil {
        /// <summary>
        /// インスタンス化不可
        /// </summary>
        private UriUtil() {
        }
        /// <summary>
        /// GETパラメータを返す
        /// </summary>
        /// <param name="rawUri">絶対パスのURL文字列</param>
        /// <param name="enc">文字コード</param>
        /// <returns></returns>
        public static NameValueCollection GetParams(string rawUri, Encoding enc) {
            var ret = new NameValueCollection();
            var m = Regex.Match(rawUri, @"\?([^\?]*)$");
            if (m.Success) {
                return HttpUtility.ParseQueryString(m.Groups[1].Value, enc);
            }
            return ret;
        }
    }
}
