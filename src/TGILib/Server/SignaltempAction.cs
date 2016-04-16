using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;
using TGILib.AIR;
using System.Diagnostics;
using System.Drawing;

namespace TGILib.Server {
    /// <summary>
    /// statusの処理
    /// </summary>
    class SignaltempAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            var air = AIRProxy.Instance;
            if (air.State != AIRProxy.ProxyState.Initialized) {
                ErrorMessage = "現在この操作は行えません。";
                return false;
            }
            switch (method) {
                case "step0":
                    return Step0(gets);
                case "step1":
                case "step2":
                    return XYStep(gets);
                case "submit":
                    return Submit(gets);
                default:
                    throw new Exception("Unknown method : " + method);
            }
        }

        private string Hex(Color c) {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
        /// <summary>
        /// 処理開始
        /// </summary>
        /// <param name="gets"></param>
        /// <returns></returns>
        private bool Step0(NameValueCollection gets) {
            return true;
        }
        /// <summary>
        /// 座標に対応するシグナルと色を返す
        /// </summary>
        /// <param name="gets"></param>
        /// <returns></returns>
        private bool XYStep(NameValueCollection gets) {
            int x = int.Parse(gets["x"]);
            int y = int.Parse(gets["y"]);
            var air = AIRProxy.Instance;
            ushort sig = air.SignalAt(x, y);
            ResponseData["Signal"] = sig;
            var support = AIRSupport.Instance;
            ResponseData["Color"] = Hex(support.SignalToColor(sig));
            ResponseData["Temp"] = support.SignalToTemp(sig);
            return true;
        }
        /// <summary>
        /// TmaxとTminを決定する
        /// </summary>
        /// <param name="gets"></param>
        /// <returns></returns>
        private bool Submit(NameValueCollection gets) {
            int t1 = int.Parse(gets["temp1"]);
            int t2 = int.Parse(gets["temp2"]);
            sort(ref t1, ref t2);
            int s1 = int.Parse(gets["signal1"]);
            int s2 = int.Parse(gets["signal2"]);
            sort(ref s1, ref s2);
            Debug.WriteLine(string.Format("temp1={0},temp2={1},signal1={2},signal2={3}", t1, t2, s1, s2));

            TGIApp app = TGIApp.Instance;
            Config c = app.CurrentConfig;
            AIRSupport support = AIRSupport.Instance;
            // 計算
            double R = (t2 - t1) / (double)(s2 - s1);
            // 反映
            support.Tmin = c.ProxyTmin = (int)(t1 - R * s1);
            support.Tmax = c.ProxyTmax = (int)(t2 + R * (AIRProxy.Smax - s2));
            c.Save();
            return true;
        }
        private void sort(ref int i1, ref int i2) {
            if (i2 < i1) {
                int w = i1;
                i1 = i2;
                i2 = w;
            }
        }

    }
}
