using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGILib.AIR {
    /// <summary>
    /// サーモグラフィカメラのデバイスで例外が発生した
    /// </summary>
    public class AIRException : Exception {
        public enum ErrorType {
            /// <summary>
            /// 初期化失敗
            /// </summary>
            InitializeError,
            /// <summary>
            /// デバイスが接続されていないエラー
            /// </summary>
            DeviceNotConnectedError,
            /// <summary>
            /// デバイス通信系エラー
            /// </summary>
            DeviceError,
            /// <summary>
            /// 不明なエラー
            /// </summary>
            UnknownError,
        }

        public readonly ErrorType Type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public AIRException(ErrorType type, string message, Exception inner)
            : base(message, inner) {
            Type = type;
        }
    }
}
