using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TGILib.AIR {
    /// <summary>
    /// mock instance
    /// </summary>
    public class AIRProxyMock :AIRProxy {
        private Timer timer;
        public override void Initialize() {
            if (State == ProxyState.NotInitialized) {
                new Thread(() => {
                    Thread.Sleep(3000);
                    State = ProxyState.Initialized;
                }).Start();
            }
        }

        public override void Release() {
            
        }

        public override void SetCofFilePath(string filePath) {
            CofFilePath = filePath;
        }

        private ushort[] signals = new ushort[ImageWidth * ImageHeight];


        public override ushort SignalAt(int x, int y) {
            return signals[y * ImageWidth + x];
        }

        public override void StartImaging() {
            timer = new Timer(new TimerCallback((t) => {
                var r = new Random();
                for (int i = 0; i < signals.Length; i++) {
                    signals[i] = (ushort)r.Next();
                }
                FireGetSignals(signals);
            }), this, 500, 1000);
        }

        public override void StopImaging() {
            timer.Dispose();
            timer = null;
        }
    }
}
