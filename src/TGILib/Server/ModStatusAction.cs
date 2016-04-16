using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// modstatusの処理
    /// </summary>
    class ModStatusAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            if (AIR.AIRProxy.Instance.State == AIR.AIRProxy.ProxyState.Initialized) {
                if (gets["Recording"] != null) {
                    bool b;
                    if (bool.TryParse(gets["Recording"], out b)) {
                        app.Watcher.Enabled = b;
                        app.StatusChanged = true;
                    }
                }
            }
            
            ResponseData["Recording"] = app.Watcher.Enabled;
            ResponseData["AIRStatus"] = AIR.AIRProxy.Instance.State;
            ResponseData["SiteDirName"] = AppConfig.SiteDirName;
            return true;
        }
    }
}
