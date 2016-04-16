using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// configの処理
    /// </summary>
    class DuplicateAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            Config c;
            if (gets["DirName"] != null) {
                c = app.Duplicate(gets["DirName"]);
            } else {
                throw new Exception("DirNameが指定されていません。");
            }
            ResponseData["Values"] = ConfigAction.ConfigValues(c);
            return true;
        }
    }
}
