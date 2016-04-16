using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// selectSiteの処理
    /// </summary>
    class SelectsiteAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            if (gets["DirName"] != null) {
                app.SetCurrentConfigName(gets["DirName"]);
            } else {
                throw new Exception("DirNameが指定されていません。");
            }
            return true;
        }
    }
}
