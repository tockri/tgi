using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TGILib;
using TGILib.AIR;
using SAT.Util;
using System.Diagnostics;

namespace TGI2Test {
    class Program {

        static void Main(string[] args) {
            SAT.Util.SimpleDIContainer.Instance.Bind(typeof(AIRProxy), typeof(AIRProxyMock));
            AIRProxy p = AIRProxy.Instance;
            p.OnStateChanged += p_OnStateChanged;
            TGIApp app = TGIApp.Instance;
            app.Init();
            Console.WriteLine("「q」を入力すると終了します。");
            while (true) {
                var command = Console.ReadLine();
                if (command == "q") {
                    app.Dispose();
                    break;
                }
            }
        }
        
        static void p_OnStateChanged(AIRProxy.ProxyState state) {
            switch (state) {
                case AIRProxy.ProxyState.NotConnected:
                    Console.WriteLine("デバイスが接続されていません");
                    break;
                case AIRProxy.ProxyState.NotInitialized:
                    Console.WriteLine("初期化されていません");
                    break;
                case AIRProxy.ProxyState.Initializing:
                    Console.WriteLine("初期化中...");
                    break;
                case AIRProxy.ProxyState.Initialized:
                    Console.WriteLine("初期化完了");
                    break;
                default:
                    Console.WriteLine("Unknown State : " + state);
                    break;
            }
        }
    }
}
