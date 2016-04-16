using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SAT.Util;

namespace ColorMapTest {
    public partial class TestForm : Form {
        private Color[] map1 = ColorMapMaker.Create(ColorMapMaker.FocusType.None);
        private Bitmap bmp1 = new Bitmap(256, 10);
        private Color[] map2 = ColorMapMaker.Create(ColorMapMaker.FocusType.Low);
        private Bitmap bmp2 = new Bitmap(256, 10);
        private Color[] map3 = ColorMapMaker.Create(ColorMapMaker.FocusType.Mid);
        private Bitmap bmp3 = new Bitmap(256, 10);
        private Color[] map4 = ColorMapMaker.Create(ColorMapMaker.FocusType.High);
        private Bitmap bmp4 = new Bitmap(256, 10);

        public TestForm() {
            InitializeComponent();
        }
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Make(bmp1, map1);
            Make(bmp2, map2);
            Make(bmp3, map3);
            Make(bmp4, map4);
        }

        private void Make(Bitmap bmp, Color[] map) {
            lock (bmp) {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                        ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                unsafe {
                    byte* p = (byte*)data.Scan0;
                    for (int y = 0; y < 10; y++) {
                        for (int x = 0; x < map.Length; x++) {
                            Color c = map[x];
                            p[0] = c.B;
                            p[1] = c.G;
                            p[2] = c.R;
                            p += 3;
                        }
                    }
                }
                bmp.UnlockBits(data);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.DrawImage(bmp1, 10, 10);
            g.DrawImage(bmp2, 10, 30);
            g.DrawImage(bmp3, 10, 50);
            g.DrawImage(bmp4, 10, 70);
        }
    }
}
