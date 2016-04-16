using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime;
using Microsoft.Win32;

namespace SAT.Util {
    /// <summary>
    /// Webサーバーの実装
    /// </summary>
    public class WebServer {

        /// <summary>
        /// リクエストとレスポンスを代理するデリゲート
        /// </summary>
        public delegate void RequestHandler(HttpListenerRequest req, HttpListenerResponse res);


        private Thread serviceThread;
        private volatile bool running = false;
        private HttpListener listener;
        /// <summary>
        /// DocumentRoot以下にファイルが存在しないパスへのリクエスト時に呼ばれるリスナ
        /// </summary>
        public event RequestHandler OnRequest;

        /// <summary>
        /// ファイルを置いておくためのルートディレクトリ
        /// </summary>
        public string DocumentRoot {
            get;
            set;
        }
        /// <summary>
        /// 待ち受けポート
        /// </summary>
        public int ListenPort {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="port"></param>
        public WebServer(int port) {
            ListenPort = port;
        }
        /// <summary>
        /// 拡張子からcontentTypeに変換する
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        private string ContentTypeForExtension(string ext) {
            // 特別なmime-type
            switch (ext) {
                case ".js":
                    return "text/javascript";
                case ".css":
                    return "text/css";
                default:
                    break;
            }

            const string defType = "application/octet-stream";
            var key = Registry.ClassesRoot.OpenSubKey(ext);
            if (key == null) {
                return defType;
            }
            var mimeType = key.GetValue("Content Type");
            return mimeType != null ? mimeType.ToString() : defType;
        }

        /// <summary>
        /// リクエストを処理する
        /// </summary>
        /// <param name="context"></param>
        private void ProcessRequest(HttpListenerContext context) {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            response.AddHeader("Access-Control-Allow-Origin", "*");
            response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
            response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
            string path = Regex.Replace(request.RawUrl, @"\?.+$", "");
            string filePath = Path.Combine(DocumentRoot, path.Trim('/').Replace("/", @"\"));
            if (Directory.Exists(filePath)) {
                filePath = Path.Combine(filePath, "index.html");
            }
            if (File.Exists(filePath)) {
                // 存在するファイルへのアクセス
                response.AddHeader("Cache-Control", "private,max-age=60");
                response.ContentType = ContentTypeForExtension(Path.GetExtension(filePath));
                using (var src = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                    Stream os = response.OutputStream;
                    FileUtil.CopyInto(src, os);
                    os.Flush();
                }
            } else {
                response.AddHeader("Cache-Control", "no-cache,no-store,must-revalidate");
                response.AddHeader("Pragma", "no-cache");
                if (OnRequest != null) {
                    OnRequest(request, response);
                }
            }
            response.Close();
        }

        private void Run() {
            using (listener = new HttpListener()) {
                listener.Prefixes.Add("http://*:" + ListenPort + "/");
                try {
                    listener.Start();
                } catch (Exception ex) {
                    Logger.error(ex, "プロセスを停止します。サービスの設定により再起動させてください。");
                    running = false;
                    Environment.Exit(1);
                }

                while (running) {
                    try {
                        HttpListenerContext context = listener.GetContext();
                        Thread t = new Thread(new ThreadStart(() => {
                            try {
                                ProcessRequest(context);
                            } catch (Exception ex) {
                                Logger.warn(ex, "Webアクセス失敗");
                                context.Response.StatusCode = 400;
                            }
                        }));
                        t.Start();
                    } catch (ThreadInterruptedException) {
                        break;
                    } catch (Exception ex) {
                        Logger.warn(ex, "HttpListenerでエラーが発生しました。");
                        Thread.Sleep(100);
                    }
                }
                serviceThread = null;
                try {
                    listener.Stop();
                    listener.Close();
                } catch (Exception) {
                }
            }
        }
        /// <summary>
        /// 起動する
        /// </summary>
        public void Start() {
            if (!running) {
                running = true;
                serviceThread = new Thread(Run);
                serviceThread.Start();
                Logger.info("WebServer Started.");
            }
        }
        /// <summary>
        /// 停止する
        /// </summary>
        public void Stop() {
            if (running) {
                running = false;
                if (listener != null) {
                    Logger.info("Stopping listener...");
                    listener.Abort();
                }
                if (serviceThread != null) {
                    try {
                        serviceThread.Interrupt();
                    } catch (Exception) {
                    }
                }
            }
        }
    }
}
