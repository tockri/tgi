using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine;
using SuperWebSocket;
using SAT.Util;
using System.Diagnostics;

namespace TGILib.Server {
    public class WSServer : IDisposable {
        public readonly int Port;

        private WebSocketServer Server;

        private readonly List<WebSocketSession> Sessions = new List<WebSocketSession>();

        private object Lock = "";

        private bool isSending = false;

        public WSServer(int port) {
            Port = port;
        }
        /// <summary>
        /// セッション接続リクエスト
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        private void Server_NewRequestReceived(WebSocketSession session, SuperWebSocket.Protocol.IWebSocketFragment requestInfo) {
            Logger.debug("New Request");
        }


        /// <summary>
        /// セッション接続時のイベントリスナ
        /// </summary>
        /// <param name="session"></param>
        void Server_NewSessionConnected(WebSocketSession session) {
            if (Sessions.Count >= AppConfig.MaxSessionCount) {
                Logger.debug("already full");
                session.Send("ConnectionRefused\t\"接続数の上限に達しているため接続できません。\"");
                session.Close(CloseReason.SocketError);
            } else {
                Logger.debug(session.RemoteEndPoint, "Session connected");
                Sessions.Add(session);
            }
        }
        /// <summary>
        /// セッション切断時のイベントリスナ
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void Server_SessionClosed(WebSocketSession session, CloseReason value) {
//            Debug.WriteLine(session.RemoteEndPoint, "Closed");
            Logger.debug(session.RemoteEndPoint, "Session closed");
            Sessions.Remove(session);
        }
        /// <summary>
        /// メッセージ受信時のイベントリスナ
        /// </summary>
        /// <param name="session"></param>
        /// <param name="value"></param>
        void Server_NewMessageReceived(WebSocketSession session, string value) {
        }
        /// <summary>
        /// サーバー開始
        /// </summary>
        public void Start() {
            Server = new WebSocketServer();
            Server.NewSessionConnected += Server_NewSessionConnected;
            Server.NewMessageReceived += Server_NewMessageReceived;
            Server.SessionClosed += Server_SessionClosed;
            var rootConfig = new RootConfig();
            var serverConfig = new ServerConfig() {
                Port = Port,
                Ip = "Any",
                MaxConnectionNumber = 10,       /// 最大ユーザセッション数
                Mode = SocketMode.Tcp,
                Name = "TGI Server"
            };
            if (!Server.Setup(rootConfig, serverConfig, new SocketServerFactory())) {
                Logger.error("WebSocket Server cannot setup(" + Port + ")");
            }
            if (!Server.Start()) {
                Logger.error("WebSocket Server failed to start");
            }
        }

        /// <summary>
        /// サーバー停止
        /// </summary>
        public void Stop() {
            if (Server != null) {
                Server.Stop();
            }
            Sessions.Clear();
        }

        public void CloseAllSessions() {
            lock (Lock) {
                Sessions.ForEach((s) => {
                    s.Close();
                });
                Sessions.Clear();
            }
        }

        /// <summary>
        /// バイナリデータを送信する
        /// </summary>
        /// <param name="data"></param>
        public void SendToAllClients(byte[] data) {
            if (isSending) {
                return;
            }
            lock (Lock) {
                isSending = true;

                Parallel.ForEach(Sessions, (s) => {
                    try {
                        s.Send(data, 0, data.Length);
                    } catch (Exception ex) {
                        Logger.debug(ex, "Error on sending data : " + s.RemoteEndPoint.ToString()); 
                        Sessions.Remove(s);
                        s.Close();
                    }
                });
                isSending = false;
            }
        }
        /// <summary>
        /// 文字列を送信する
        /// </summary>
        /// <param name="message"></param>
        public void SendToAllClients(string message) {
            lock (Lock) {
                isSending = true;
                Parallel.ForEach(Sessions, (s) => {
                    try {
                        s.Send(message);
                    } catch (Exception ex) {
                        Logger.debug(ex, "Error on sending message : " + s.RemoteEndPoint.ToString());
                        Sessions.Remove(s);
                        s.Close();
                    }
                });
                isSending = false;
            }
        }

        /// <summary>
        /// 破棄する
        /// </summary>
        public void Dispose() {
            Stop();
            if (Server != null) {
                Server.Dispose();
            }
        }
    }

}
