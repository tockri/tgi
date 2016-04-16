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
    class StatusAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection getParameters) {
            TGIApp app = TGIApp.Instance;
            ResponseData["Recording"] = app.Watcher.Enabled;
            ResponseData["AIRStatus"] = AIR.AIRProxy.Instance.State;
            ResponseData["SiteDirName"] = AppConfig.SiteDirName;
            return true;
        }
    }
}
