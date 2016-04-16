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
    class WatchboxAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection getParameters) {
            TGIApp app = TGIApp.Instance;
            ResponseData["Main"] = app.MainWatchBox.Config;
            ResponseData["Sub"] = app.SubWatchBox.Config;
            return true;
        }
    }
}
