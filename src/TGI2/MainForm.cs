using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using TGILib;
using TGILib.AIR;
using SAT.Util;
using System.Diagnostics;

namespace TGITest {
    public partial class MainForm : Form {
        private enum WM : uint {
            WM_DEVICECHANGE = 0x0219,
        }
        private Bitmap bmp = new Bitmap(AIRProxy.ImageWidth, AIRProxy.ImageHeight);
        private Bitmap mapBmp;

        private TGIApp app = TGIApp.Instance;

        public MainForm() {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            AIRProxy p = AIRProxy.Instance;
            p.OnStateChanged += new AIRProxy.StateChangeHandler(p_OnStateChanged);
            app.Init();
            mapBmp = AIRSupport.Instance.CreateColormapImage();
        }

        void p_OnStateChanged(AIRProxy.ProxyState state) {
            Invoke(new MethodInvoker(() => {
                switch (state) {
                    case AIRProxy.ProxyState.NotConnected:
                        MessageLabel.Text = "デバイスが接続されていません";
                        break;
                    case AIRProxy.ProxyState.NotInitialized:
                        MessageLabel.Text = "初期化されていません";
                        break;
                    case AIRProxy.ProxyState.Initializing:
                        MessageLabel.Text = "初期化中...";
                        break;
                    case AIRProxy.ProxyState.Initialized:
                        MessageLabel.Text = "初期化完了";
                        break;
                    default:
                        MessageLabel.Text = "Unknown State : " + state;
                        break;
                }
            }));
        }

        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            app.Dispose();
        }



        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawImage(mapBmp, 10, 270);
        }

        protected override void WndProc(ref Message m) {
            AIRProxy p = AIRProxy.Instance;
            switch ((WM)m.Msg) {
                case WM.WM_DEVICECHANGE:
                    p.Initialize();
                    break;
                default:
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
