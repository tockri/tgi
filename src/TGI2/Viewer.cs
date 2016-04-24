using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TGIApp {
    public partial class Viewer : UserControl {
        private bool PaintBg = true;

        private Bitmap bmp = new Bitmap(320, 240);

        public Bitmap Bmp {
            get {
                return bmp;
            }
        }

        public Viewer() {
            InitializeComponent();
        }

        public void RepaintOnce() {
            PaintBg = false;
            Invoke(new MethodInvoker(() => {
                Invalidate();
            }));
        }

        protected override void OnPaintBackground(PaintEventArgs e) {
            if (PaintBg) {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            lock (bmp) {
                g.DrawImage(bmp, 0, 0, Width, Height);
            }
        }

    }
}
