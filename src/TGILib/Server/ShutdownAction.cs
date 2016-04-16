using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// statusの処理
    /// </summary>
    class ShutdownAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection getParameters) {
            TGIApp app = TGIApp.Instance;
            app.ShutdownPC();
            return true;
        }
    }
}
