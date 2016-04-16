using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using TGILib;
using TGILib.AIR;
using System.Drawing;
using SAT.Util;

namespace TGILib.Server {
    class ColorMapAction : IAction {
        /// <summary>
        /// アクションメソッドを実行する
        /// </summary>
        /// <param name="req"></param>
        /// <param name="res"></param>
        public void Invoke(string method, HttpListenerRequest req, HttpListenerResponse res) {
            AIRSupport support = AIRSupport.Instance;
            byte[] img = ImageUtil.BitmapToPng(support.CreateColormapImage());
            res.ContentType = "image/png";
            res.OutputStream.Write(img, 0, img.Length);
            res.OutputStream.Flush();
        }

    }
}
