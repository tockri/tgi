
namespace TGILib {
    /// <summary>
    /// WatchBoxから得られるセルの情報
    /// </summary>
    public class WatchCell {
        /// <summary>
        /// 平均温度
        /// </summary>
        public float Average = 0;
        /// <summary>
        /// 最高温度
        /// </summary>
        public int Max = 0;
        /// <summary>
        /// 最低温度
        /// </summary>
        public int Min = 0;
        /// <summary>
        /// 左端
        /// </summary>
        internal int Left = 0;
        /// <summary>
        /// 右端
        /// </summary>
        internal int Right = 0;
    }
}
