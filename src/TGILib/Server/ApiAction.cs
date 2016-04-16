using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using SAT.Util;

namespace TGILib.Server {
    public abstract class ApiAction : IAction {
        /// <summary>
        /// 成功
        /// </summary>
        public bool Success {
            get;
            private set;
        }
        /// <summary>
        /// レスポンスで返すデータ
        /// </summary>
        public Dictionary<string, object> ResponseData {
            get;
            private set;
        }
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage {
            get;
            protected set;
        }
        /// <summary>
        /// アクションメソッドを実行する
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void Invoke(string method, HttpListenerRequest req, HttpListenerResponse res) {
            ResponseData = new Dictionary<string, object>();
            try {
                Success = InvokeInner(method, UriUtil.GetParams(req.RawUrl, Encoding.UTF8));
            } catch (Exception ex) {
                Logger.warn(ex, "Invoke failed.");
                ErrorMessage = ex.Message;
                Success = false;
            }
            res.ContentType = "application/json; charset=utf-8";
            ResponseData["Success"] = Success;
            ResponseData["ErrorMessage"] = ErrorMessage;
            JsonWriter json = new JsonWriter();
            json.ReadableFormat = false;
            json.Write(res.OutputStream, ResponseData);
            res.OutputStream.Flush();

        }
        /// <summary>
        /// アクションメソッド実処理
        /// </summary>
        /// <param name="method"></param>
        /// <param name="getParameters"></param>
        protected abstract bool InvokeInner(string method, NameValueCollection getParameters);
    }
}
