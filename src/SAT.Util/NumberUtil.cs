using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAT.Util {
    /// <summary>
    /// 数値データに関する便利メソッド
    /// </summary>
    public class NumberUtil {
        /// <summary>
        /// v1とv2の間で制限された値を返す。v1とv2の大小関係はどちらでも構わない。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int Between(int v, int v1, int v2) {
            // v1 < v2にする
            if (v2 < v1) {
                var w = v1;
                v1 = v2;
                v2 = w;
            }
            if (v < v1) {
                return v1;
            } else if (v2 < v) {
                return v2;
            } else {
                return v;
            }
        }
        /// <summary>
        /// v1とv2の間で制限された値を返す。v1とv2の大小関係はどちらでも構わない。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float Between(float v, float v1, float v2) {
            // v1 < v2にする
            if (v2 < v1) {
                var w = v1;
                v1 = v2;
                v2 = w;
            }
            if (v < v1) {
                return v1;
            } else if (v2 < v) {
                return v2;
            } else {
                return v;
            }
        }
        /// <summary>
        /// v1とv2の間で制限された値を返す。v1とv2の大小関係はどちらでも構わない。
        /// </summary>
        /// <param name="v"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double Between(double v, double v1, double v2) {
            // v1 < v2にする
            if (v2 < v1) {
                var w = v1;
                v1 = v2;
                v2 = w;
            }
            if (v < v1) {
                return v1;
            } else if (v2 < v) {
                return v2;
            } else {
                return v;
            }
        }
    }
}
