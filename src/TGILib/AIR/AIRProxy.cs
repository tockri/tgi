using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using SAT.Util;

namespace TGILib.AIR {

    public abstract class AIRProxy {
        // ------------------------------------ const ----------------------------------------------
        /// <summary>
        /// シグナルの最大値 AIR32 = 14bit
        /// </summary>
        public const ushort Smax = 0x3FFF;
        /// <summary>
        /// カメラから得られる画像の幅 AIR32 = 660x480
        /// </summary>
        public const int ImageWidth = 320;
        /// <summary>
        /// カメラから得られる画像の高さ AIR32 = 660x480
        /// </summary>
        public const int ImageHeight = 240;

        // ------------------------------------ static ----------------------------------------------

        private static object Lock = new object();
        /// <summary>
        /// singletonインスタンス
        /// </summary>
        private static AIRProxy instance = null;
        /// <summary>
        /// singletonインスタンス
        /// </summary>
        public static AIRProxy Instance {
            get {
                if (instance == null) {
                    lock (Lock) {
                        if (instance == null) {
                            Type implClass = SimpleDIContainer.Instance.GetBoundImpl(typeof(AIRProxy));
                            instance = Activator.CreateInstance(implClass) as AIRProxy;
                        }
                    }
                }
                return instance;
            }
        }



        // ------------------------------------ enum ----------------------------------------------
        /// <summary>
        /// カメラの初期化状態
        /// </summary>
        public enum ProxyState {
            /// <summary>
            /// カメラが接続されていない
            /// </summary>
            NotConnected,
            /// <summary>
            /// 初期状態
            /// </summary>
            NotInitialized,
            /// <summary>
            /// 初期化中
            /// </summary>
            Initializing,
            /// <summary>
            /// 初期化完了
            /// </summary>
            Initialized
        }

        // ------------------------------------ instance member ----------------------------------------------
        private ProxyState state = ProxyState.NotInitialized;
        /// <summary>
        /// カメラの初期化状態
        /// </summary>
        public ProxyState State {
            get {
                return state;
            }
            protected set {
                if (state != value) {
                    state = value;
                    if (OnStateChanged != null) {
                        OnStateChanged(state);
                    }
                }
            }
        }

        /// <summary>
        /// cofファイル
        /// </summary>
        public string CofFilePath {
            get;
            protected set;
        }


        // ------------------------------------ events ----------------------------------------------
        
        /// <summary>
        /// ステータス変更
        /// </summary>
        /// <param name="state"></param>
        public delegate void StateChangeHandler(ProxyState state);
        /// <summary>
        /// ステータスが変更されたときのイベント
        /// </summary>
        public event StateChangeHandler OnStateChanged;
        protected void FireStateChanged(ProxyState newState) {
            if (OnStateChanged != null) {
                OnStateChanged(newState);
            }
        }

        /// <summary>
        /// エラー処理
        /// </summary>
        /// <param name="e"></param>
        public delegate void ErrorHandler(AIRException e);
        /// <summary>
        /// エラーが発生した時のイベント
        /// </summary>
        public event ErrorHandler OnError;
        protected void FireError(AIRException e) {
            if (OnError != null) {
                OnError(e);
            }
        }

        /// <summary>
        /// 画像を取得した
        /// </summary>
        public delegate void GetSignalsHandler(ushort[] signals);
        /// <summary>
        /// 画像を取得した時のイベント
        /// </summary>
        public event GetSignalsHandler OnGetSignals;
        protected void FireGetSignals(ushort[] signals) {
            if (OnGetSignals != null) {
                OnGetSignals(signals);
            }
        }

        // ------------------------------------ methods ----------------------------------------------
        /// <summary>
        /// singleton pattern
        /// </summary>
        protected AIRProxy() {
            
        }


        /// <summary>
        /// cofファイルを読み込む
        /// </summary>
        public abstract void SetCofFilePath(string filePath);

        /// <summary>
        /// 初期化スレッドを開始する
        /// </summary>
        public abstract void Initialize();


        /// <summary>
        /// 画像取得スレッドを開始する
        /// </summary>
        public abstract void StartImaging();


        /// <summary>
        /// 画像取得スレッドを停止する
        /// </summary>
        public abstract void StopImaging();


        /// <summary>
        /// 座標のシグナル値を返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public abstract ushort SignalAt(int x, int y);


        /// <summary>
        /// 破棄する時に停止する
        /// </summary>
        public abstract void Release();


    }
}
