using System;
using System.Drawing;
using NU = SAT.Util.NumberUtil;
using System.Diagnostics;

namespace TGILib {

    public class WatchBox {
        /// <summary>
        /// 設定用オブジェクト
        /// </summary>
        public class ConfigValues {
            private WatchBox wb;
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="instance"></param>
            internal ConfigValues(WatchBox instance) {
                wb = instance;
            }
            /// <summary>
            /// モード
            /// </summary>
            public string Mode {
                get {
                    return wb.mode.ToString();
                }
                set {
                    try {
                        wb.mode = (BoxMode)Enum.Parse(typeof(BoxMode), value);
                    } catch (Exception) {
                        wb.mode = BoxMode.Single;
                    }
                }
            }
            /// <summary>
            /// Doubleモードの場合のみ、間隔
            /// </summary>
            public int Gap {
                get {
                    return wb.mode == BoxMode.Double ? wb.gap : 0;
                }
                set {
                    if (wb.gap != value) {
                        wb.gap = NU.Between(value, 0, wb.rect.Width - wb.CellCount);
                    }
                }
            }

            /// <summary>
            /// Rectangleオブジェクトを返す
            /// </summary>
            /// <returns></returns>
            public Rectangle GetRect() {
                return wb.rect;
            }
            /// <summary>
            /// Rectangle
            /// </summary>
            /// <param name="rect"></param>
            public void SetRect(Rectangle rect) {
                wb.rect = rect;
            }


            /// <summary>
            /// 左端座標
            /// </summary>
            public int Left {
                get {
                    return wb.rect.Left;
                }
                set {
                    if (wb.rect.Left != value) {
                        wb.rect.Location = new Point(NU.Between(value, 0, MaxWidth - wb.rect.Width), wb.rect.Location.Y);
                    }
                }
            }
            /// <summary>
            /// 上端座標
            /// </summary>
            public int Top {
                get {
                    return wb.rect.Top;
                }
                set {
                    if (wb.rect.Top != value) {
                        wb.rect.Location = new Point(wb.rect.Location.X, NU.Between(value, 0, MaxHeight - wb.rect.Height));
                    }
                }
            }
            /// <summary>
            /// 幅
            /// </summary>
            public int Width {
                get {
                    return wb.rect.Width;
                }
                set {
                    if (wb.rect.Width != value) {
                        wb.rect.Width = NU.Between(value, 0, MaxWidth - wb.rect.Left);
                    }
                }
            }
            /// <summary>
            /// 高さ
            /// </summary>
            public int Height {
                get {
                    return wb.rect.Height;
                }
                set {
                    if (wb.rect.Height != value) {
                        wb.rect.Height = NU.Between(value, 0, MaxHeight - wb.rect.Top);
                    }
                }
            }
        }
        public readonly ConfigValues Config;

        // ------------------------ const --------------------------
        private const int MaxWidth = AIR.AIRProxy.ImageWidth;
        private const int MaxHeight = AIR.AIRProxy.ImageHeight;

        // ------------------------ enum ---------------------------


        /// <summary>
        /// Boxのモード
        /// </summary>
        public enum BoxMode {
            Double,
            Single
        }


        // -------------------------- instance ---------------------
        private WatchCell[] cells;
        /// <summary>
        /// セル
        /// </summary>
        public WatchCell[] WatchCells {
            get {
                return cells;
            }
        }
        /// <summary>
        /// セル分割数。2の倍数でなければならない。
        /// </summary>
        public int CellCount {
            get {
                return cells != null ? cells.Length : 0;
            }
            set {
                if (CellCount != value && 2 <= value && value % 2 == 0) {
                    cells = new WatchCell[value];
                    for (int i = 0; i < cells.Length; i++) {
                        cells[i] = new WatchCell();
                    }
                }
            }
        }
        /// <summary>
        /// 座標
        /// </summary>
        private Rectangle rect = new Rectangle(0, 0, 15, 15);
        /// <summary>
        /// 間隔
        /// </summary>
        private int gap = 10;
        /// <summary>
        /// モード
        /// </summary>
        private BoxMode mode = BoxMode.Single;
        /// <summary>
        /// 表示・非表示のプロパティ
        /// </summary>
        public bool Enabled {
            get;
            set;
        }

        /// <summary>
        /// 全てのセルの最高温度
        /// </summary>
        public int AllMax {
            get {
                int m = int.MinValue;
                foreach (WatchCell c in cells) {
                    m = Math.Max(m, c.Max);
                }
                return m;
            }
        }

        /// <summary>
        /// 全てのセルの最低温度
        /// </summary>
        public int AllMin {
            get {
                int m = int.MaxValue;
                foreach (WatchCell c in cells) {
                    m = Math.Min(m, c.Min);
                }
                return m;
            }
        }

        /// <summary>
        /// 全てのセルの平均温度
        /// </summary>
        public float AllAverage {
            get {
                float m = 0;
                foreach (WatchCell c in cells) {
                    m += c.Average;
                }
                return m / cells.Length;
            }
        }


        // ------------------------ methods ----------------------------

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="lv"></param>
        public WatchBox() {
            CellCount = 10;
            Config = new ConfigValues(this);
        }


        /// <summary>
        /// WatchCell一つ分の温度情報を収集する
        /// </summary>
        /// <param name="c"></param>
        /// <param name="temp"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void ReadWatchCell(int c, ushort[] signals, int left, int right) {
            int height = rect.Height;
            int max = int.MinValue;
            int min = int.MaxValue;
            int sum = 0;
            var sp = AIR.AIRSupport.Instance;
            //Debug.Write("cell[" + c + "]:");
            for (int x = left; x < right; x++) {
                for (int y = 0; y < height; y++) {
                    int t = sp.GetTempAt(signals, x, y);
                    //Debug.Write(t + ",");
                    sum += t;
                    max = max < t ? t : max;
                    min = min > t ? t : min;
                }
            }
            cells[c].Max = max;
            cells[c].Min = min;
            cells[c].Average = sum / (float)((right - left) * height);
            //Debug.WriteLine("" + cells[c].Max + ":" + cells[c].Min + ":" + cells[c].Average + ", ");
        }

        /// <summary>
        /// 温度を監視する
        /// </summary>
        /// <param name="temp">温度の配列</param>
        internal void Watch(ushort[] signals) {
            if (Enabled) {
                if (mode == BoxMode.Single) {
                    float cellWidth = rect.Width / (float)cells.Length;
                    for (int c = 0; c < cells.Length; c++) {
                        int left = (int)Math.Round(c * cellWidth) + rect.Left;
                        int right = (int)Math.Round(left + cellWidth);
                        ReadWatchCell(c, signals, left, right);
                    }
                } else {
                    // Double
                    float cellWidth = (rect.Width - gap) / (float)cells.Length;
                    for (int c = 0; c < cells.Length; c++) {
                        int left = (int)Math.Round(c * cellWidth) + (c < cells.Length / 2 ? 0 : gap) + rect.Left;
                        int right = (int)Math.Round(left + cellWidth);
                        ReadWatchCell(c, signals, left, right);
                    }
                }
            }
        }
    }
}
