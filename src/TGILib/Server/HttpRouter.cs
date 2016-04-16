using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAT.Util;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace TGILib.Server {
    /// <summary>
    /// ルーティングする
    /// </summary>
    public class HttpRouter {
        public HttpRouter(WebServer serv) {
            serv.OnRequest += new WebServer.RequestHandler(serv_OnRequest);
        }
        /// <summary>
        /// リクエストの処理
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void serv_OnRequest(HttpListenerRequest req, HttpListenerResponse res) {
            // ?以降をすべて除去したパス
            string path = Regex.Replace(req.RawUrl, @"\?.+$", "");
            IAction action = null;
            string method = "";
            if (path.StartsWith("/api/")) {
                string baseName = path.Substring(5);
                string[] elems = baseName.Split('-');
                action = GetAction(elems[0]);
                if (elems.Length > 1) {
                    method = elems[1];
                }
            } else if (path == "/colormap") {
                action = new ColorMapAction();
            }
            if (action != null) {
                res.StatusCode = 200;
                action.Invoke(method, req, res);
            } else {
                NotFound(res);
            }
        }

        /// <summary>
        /// 404 Not Foundを返す
        /// </summary>
        /// <param name="res"></param>
        private void NotFound(HttpListenerResponse res) {
            res.StatusCode = 404;
            res.ContentType = "text/html; charset=utf-8";
            res.StatusDescription = "Not Found";
            using (StreamWriter sw = new StreamWriter(res.OutputStream, Encoding.UTF8)) {
                sw.WriteLine("<html><body>Not found.</body></html>");
            }
        }


        /// <summary>
        /// Action実装クラスのインスタンスを返す
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private ApiAction GetAction(string baseName) {
            switch (baseName) {
                case "status":
                    return new StatusAction();
                case "watchbox":
                    return new WatchboxAction();
                case "modwatchbox":
                    return new ModWatchboxAction();
                case "configOptions":
                    return new ConfigOptionsAction();
                case "config":
                    return new ConfigAction();
                case "modconfig":
                    return new ModConfigAction();
                case "sites":
                    return new SitesAction();
                case "modstatus":
                    return new ModStatusAction();
                case "signaltemp":
                    return new SignaltempAction();
                case "duplicate":
                    return new DuplicateAction();
                case "selectSite":
                    return new SelectsiteAction();
                case "restart":
                    return new RestartAction();
                case "shutdown":
                    return new ShutdownAction();
                default:
                    return null;
            }
        }
    }
}
