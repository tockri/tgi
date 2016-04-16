using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    class RestartAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection getParameters) {
            var app = TGIApp.Instance;
            app.Restart();
            return true;
        }
    }
}
