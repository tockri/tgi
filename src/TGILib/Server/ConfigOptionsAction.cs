using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// configOptionsの処理
    /// </summary>
    class ConfigOptionsAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection getParameters) {
            TGIApp app = TGIApp.Instance;
            ResponseData["ThresholdSet"] = app.GetThresholdNames();
            ResponseData["ColorMap"] = app.GetColorMaps();
            ResponseData["CofFile"] = app.GetCofFileNames();
            return true;
        }
    }
}
