using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TGILib.AIR;

namespace TGIApp {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            SAT.Util.SimpleDIContainer.Instance.Bind(typeof(AIRProxy), typeof(AIR32ProxyImpl));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
