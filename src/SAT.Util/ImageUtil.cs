using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SAT.Util {
    /// <summary>
    /// 画像処理の便利クラス
    /// </summary>
    public class ImageUtil {
        private ImageUtil() {
        }

        /// <summary>
        /// BitmapをPNGデータにする
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static byte[] BitmapToPng(Bitmap bmp) {
            using (MemoryStream buf = new MemoryStream()) {
                bmp.Save(buf, System.Drawing.Imaging.ImageFormat.Png);
                return buf.ToArray();
            }
        }
    }
}
