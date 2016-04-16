using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace TGILib.Server {
    public interface IAction {
        /// <summary>
        /// アクションメソッドを実行する
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        void Invoke(string method, HttpListenerRequest req, HttpListenerResponse res);
    }
}
