using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using SAT.Util;
using System.IO;

namespace TGILib.AIR {
    /// <summary>
    /// AIRProxyから得られるushort配列を
    /// 様々な形に変換するクラス
    /// </summary>
    public class AIRSupport {
        #region constants
        /// <summary>
        /// シグナルの最大値 AIR32 = 14bit
        /// </summary>
        private const ushort Smax = AIRProxy.Smax;
        /// <summary>
        /// 画像の横幅
        /// </summary>
        private const int ImageWidth = AIRProxy.ImageWidth;
        /// <summary>
        /// 画像の高さ
        /// </summary>
        private const int ImageHeight = AIRProxy.ImageHeight;
        #endregion

        #region properties
        /// <summary>
        /// カラーマップの種類
        /// </summary>
        private ColorMapMaker.FocusType focusType;
        /// <summary>
        /// カラーマップの種類
        /// </summary>
        public ColorMapMaker.FocusType FocusType {
            get {
                return focusType;
            }
            set {
                if (focusType != value) {
                    focusType = value;
                    ColorMap = ColorMapMaker.Create(value);
                }
            }
        }
        /// <summary>
        /// カラーマップ
        /// </summary>
        private Color[] ColorMap = ColorMapMaker.Create(ColorMapMaker.FocusType.None);

        #endregion

        /// <summary>
        /// シグナルが最大値の時の温度
        /// </summary>
        public int Tmax = 300; // Visualエディタ用のデフォルト値
        /// <summary>
        /// シグナルが最小値の時の温度
        /// </summary>
        public int Tmin = -20; // Visualエディタ用のデフォルト値

        #region singleton
        /// <summary>
        /// インスタンス
        /// </summary>
        private static AIRSupport singleton = new AIRSupport();
        /// <summary>
        /// シングルトンインスタンス
        /// </summary>
        public static AIRSupport Instance {
            get {
                return singleton;
            }
        }
        private AIRSupport() {
        }
        #endregion

        /// <summary>
        /// AIR32のシグナル値から色に変換する
        /// </summary>
        /// <param name="sig"></param>
        /// <returns></returns>
        public Color SignalToColor(ushort sig) {
            double s1 = Smax;
            double s2 = 0 + (Smax - s1);
            int i1 = 0;
            int i2 = ColorMap.Length - 1;
            int idx = (int)Math.Round((i2 - i1) / (s2 - s1) * (s2 - sig));
            if (idx < i1) {
                idx = i1;
            } else if (i2 < idx) {
                idx = i2;
            }
            return ColorMap[idx];
        }

        /// <summary>
        /// サーモグラフィのシグナルを温度に変換する
        /// </summary>
        /// <param name="sig">シグナル(14bit)</param>
        /// <returns>Tmin～Tmax</returns>
        public int SignalToTemp(ushort sig) {
            int t = (int)((Tmax - Tmin) * sig / (double)Smax + Tmin);
            //Debug.WriteLine("Signal:" + sig + " -> Temp: " + t, "SignalToTemp");
            return t;
        }
        /// <summary>
        /// 指定した点の温度を返す
        /// </summary>
        /// <param name="x">x座標</param>
        /// <param name="y">y座標</param>
        /// <returns></returns>
        public int GetTempAt(ushort[] signals, int x, int y) {
            return SignalToTemp(signals[ImageWidth * y + x]);
        }
        /// <summary>
        /// ビットマップに移す
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="bmp"></param>
        public void WriteBitmap(ushort[] signals, Bitmap bmp) {
            lock (bmp) {
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), 
                    ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                int length = signals.Length;
                unsafe {
                    byte* p = (byte*)data.Scan0;
                    for (int i = 0; i < length; i++) {
                        Color c = SignalToColor(signals[i]);
                        p[0] = c.B;
                        p[1] = c.G;
                        p[2] = c.R;
                        p += 3;
                    }
                }
                bmp.UnlockBits(data);
            }
        }
        /// <summary>
        /// 色マップの画像を作って返す
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateColormapImage() {
            Bitmap img = new Bitmap(320, 40);
            using (Graphics g = Graphics.FromImage(img)) {
                g.DrawImage(ColorMapMaker.MakeImage(ColorMap, 20), 32, 0);
                var font = new Font("Arial", 7);
                // 温度の文字列
                for (int i = 0; i <= 4; i++) {
                    int x = 256 / 4 * i;
                    int t = (Tmax - Tmin) / 4 * i + Tmin;
                    g.DrawString(t.ToString() + "℃", font, Brushes.Black, new Point(x + 20, 22));
                }
            }
            return img;
        }

    }
}
