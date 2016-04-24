using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using SAT.Util;

namespace TGILib.AIR {
    /// <summary>
    /// 実際にAIR32に接続する
    /// </summary>
    public class AIR32ProxyImpl : AIRProxy {
        /// <summary>
        /// 
        /// </summary>
        private const int FP_WIDTH = 660;
        /// <summary>
        /// 
        /// </summary>
        private const int FP_HEIGHT = 480;
        /// <summary>
        /// VCamCalTblに必要なサイズ
        /// </summary>
        private const int VCamCalTblSize = FP_WIDTH * FP_HEIGHT * sizeof(short)
            + FP_WIDTH * FP_HEIGHT * 2 * sizeof(short)
            + FP_WIDTH * FP_HEIGHT * sizeof(short)
            + 633856;

        /// <summary>
        /// USBデバイスを検索するためのクエリ文字列
        /// </summary>
        private const string DEVICE_ID_QUERY = @"USB\\VID_167C&PID_0032\\%";
        /// <summary>
        /// cofファイル保存ディレクトリ
        /// </summary>
        private const string AIR5_DIR = @"C:\Vcam2\Install\";

        private const string AIR5_DLL = AIR5_DIR + "AIR5.dll";

        // ------------------------------------ DLL extern ----------------------------------------------

        /// <summary>
        /// データを受信したり送信したりするマルチメソッド
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="buf"></param>
        /// <param name="count"></param>
        /// <param name="status"></param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void USB2_xfer2(byte* endpoint, byte* buf, long* count, byte* status);
        /// <summary>
        /// 初期化メソッドその０
        /// </summary>
        /// <param name="tbl">CVCamCalTbl</param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void USB2_VCamCalTbl(IntPtr tbl);
        /// <summary>
        /// 初期化メソッドその１
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fileName"></param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void USB2_fpgaload(IntPtr status, IntPtr fileName);
        /// <summary>
        /// 初期化メソッドその２
        /// </summary>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void USB2_Initfpa();
        /// <summary>
        /// 初期化メソッドその３
        /// </summary>
        /// <param name="status"></param>
        //        [DllImport(@"C:\Vcam2\Install\AIR5.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //        private static extern void USB2_OptVload(IntPtr status);
        /// <summary>
        /// Cold Calibration
        /// </summary>
        /// <param name="pBuf"></param>
        /// <param name="pCount"></param>
        /// <param name="average"></param>
        //        [DllImport(@"C:\Vcam2\Install\AIR5.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //        private static unsafe extern void USB2_ColdCal(byte* status, ushort* count, ushort* average);
        /// <summary>
        /// キャリブレーションの結果を保存する
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="status"></param>
        //        [DllImport(@"C:\Vcam2\Install\AIR5.dll", SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        //        private static unsafe extern void USB2_SaveCof(char* filename, byte* status);
        /// <summary>
        /// キャリブレーション結果ファイルをロードする
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="status"></param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void USB2_UploadCof(IntPtr filename, IntPtr status);
        /// <summary>
        /// correctの画像を取得する
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="count"></param>
        /// <param name="status"></param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static unsafe extern void USB2_GetImage(ushort* buf, long* count, byte* status);

        /// <summary>
        /// Syncronize ?
        /// </summary>
        /// <param name="status"></param>
        [DllImport(AIR5_DLL, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void USB2_SyncOn(IntPtr status);
        /// <summary>
        /// AIRから取得したシグナル配列
        /// </summary>
        private static ushort[] signals = new ushort[ImageWidth * ImageHeight];
        /// <summary>
        /// USB2_VCamCalTblに渡すポインタ。バッファサイズはVCamCalTblSize
        /// </summary>
        private IntPtr pVCamCalTbl;
        /// <summary>
        /// ステータスチェックを行うタイマー
        /// </summary>
        private Timer statusTimer;
        /// <summary>
        /// 画像取得を行うタイマー
        /// </summary>
        private Timer imagingTimer;
        /// <summary>
        /// 画像取得を一時停止するフラグ
        /// </summary>
        private bool imagingPaused;
        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private object Lock = DateTime.Now.ToString();

        private AIR32ProxyImpl singleton;

        /// <summary>
        /// コンストラクタは１回しか呼ばれてはいけない
        /// </summary>
        public AIR32ProxyImpl() {
            if (singleton != null) {
                throw new Exception("Constructor called twice.");
            }
            singleton = this;
        }

        /// <summary>
        /// cofファイルを読み込む
        /// </summary>
        /// <returns></returns>
        private byte UploadCofInner() {
            lock (Lock) {
                IntPtr pStatus = Marshal.AllocCoTaskMem(1);     // unsigned char*
                Marshal.WriteByte(pStatus, 0);
                IntPtr pCofFile = Marshal.StringToHGlobalAnsi(CofFilePath);
                try {
                    if (System.IO.File.Exists(CofFilePath)) {
                        Debug.WriteLine("UpdateCof:" + CofFilePath);
                        USB2_UploadCof(pCofFile, pStatus);
                        Debug.WriteLine("End.");
                    } else {
                        throw new FileNotFoundException("ファイルが存在しません。：" + CofFilePath);
                    }
                    return Marshal.ReadByte(pStatus);
                } finally {
                    Marshal.FreeCoTaskMem(pStatus);
                    Marshal.FreeHGlobal(pCofFile);
                }
            }
        }

        /// <summary>
        /// cofファイルを読み込む
        /// </summary>
        public override void SetCofFilePath(string filePath) {
            if (CofFilePath == filePath) {
                return;
            }
            try {
                if (!File.Exists(filePath)) {
                    throw new FileNotFoundException("ファイルが存在しません。:" + filePath);
                }
                CofFilePath = filePath;
                if (State == ProxyState.Initialized) {
                    imagingPaused = true;
                    if (UploadCofInner() != 0) {
                        throw new Exception("デバイスが失敗ステータスを返しました");
                    }
                }
            } catch (Exception e) {
                Logger.warn(e, "UploadCofFile失敗");
                AIRException ae = new AIRException(AIRException.ErrorType.DeviceError, "ファイルのロードに失敗しました。", e);
                FireError(ae);
            } finally {
                imagingPaused = false;
            }
        }


        /// <summary>
        /// デバイスが接続されているか確認する
        /// </summary>
        private bool CheckConnected() {
            var dev = USBDeviceUtil.GetDevice(DEVICE_ID_QUERY);
            if (dev == null) {
                State = ProxyState.NotConnected;
                return false;
            } else {
                if (CofFilePath == null) {
                    // DeviceIDの最後の部分からcofファイルのファイル名を得る
                    // 例：USB\VID_167C&PID_0032\201500
                    CofFilePath = Path.Combine(AIR5_DIR, Path.GetFileNameWithoutExtension(dev.DeviceID) + ".cof");
                }
                if (State == ProxyState.NotConnected) {
                    // 未接続→接続に変化したとき
                    State = ProxyState.NotInitialized;
                }
                return true;
            }
        }


        /// <summary>
        /// 初期化する。
        /// 初期化開始時にOnInitializeStartedイベントが発生する。
        /// 初期化完了時にOnInitializeCompletedイベントが発生する。
        /// </summary>
        private void InitializeInner() {
            // 何度も使うステータスチェック用関数を定義
            Func<IntPtr, bool> Check = (p) => {
                byte s = Marshal.ReadByte(p);
                if (s != 0) {
                    throw new AIRException(AIRException.ErrorType.InitializeError, "初期化に失敗しました。", null);
                }
                return true;
            };
            try {
                // 初期化開始
                if (State != ProxyState.NotInitialized) {
                    return;
                }
                State = ProxyState.Initializing;
                lock (Lock) {
                    IntPtr pStatus = Marshal.AllocCoTaskMem(1);     // unsigned char*
                    IntPtr pFilename = Marshal.StringToHGlobalAnsi(AIR5_DIR);
                    try {
                        // メモリ領域を確保する
                        Marshal.FreeHGlobal(pVCamCalTbl);
                        pVCamCalTbl = Marshal.AllocHGlobal(VCamCalTblSize); // 3168256
                        USB2_VCamCalTbl(pVCamCalTbl);

                        // FPGA Load
                        USB2_fpgaload(pStatus, pFilename);
                        Check(pStatus);

                        // Init FPA
                        USB2_Initfpa();

                        // Cofファイル読み込み
                        Marshal.WriteByte(pStatus, UploadCofInner());
                        Check(pStatus);

                        //USB2_OptVload(pStatus); 必要なさそう？
                        //status = Marshal.ReadByte(pStatus);

                        // Sync Mode ? APIが同期になるっぽい？
                        USB2_SyncOn(pStatus);
                        Check(pStatus);

                        // GetImageの動作モード指定
                        // 5byte目の0はUNCORRECTEDらしい。0x0bにするとCORRECTEDらしい。リファレンス無し。
                        XferInner(new byte[] { 8, 0x2a, 2, 0x18, 0x0b });
                        Check(pStatus);
                    } catch (AIRException ae) {
                        throw ae;
                    } catch (AccessViolationException ave) {
                        throw new AIRException(AIRException.ErrorType.DeviceError, "初期化に失敗しました。カメラの接続を確認してください。", ave);
                    } catch (Exception e) {
                        throw new AIRException(AIRException.ErrorType.InitializeError, e.Message, e);
                    } finally {
                        Marshal.FreeCoTaskMem(pStatus);
                        Marshal.FreeHGlobal(pFilename);
                    }
                }
                // 初期化成功
                State = ProxyState.Initialized;
            } catch (AIRException ae) {
                State = ProxyState.NotInitialized;
                Logger.warn(ae, "初期化失敗");
                FireError(ae);
            }
        }


        /// <summary>
        /// 初期化スレッドを開始する
        /// </summary>
        public override void Initialize() {
            bool processing = false;
            statusTimer = new Timer(new TimerCallback((o) => {
                if (processing) {
                    return;
                }
                processing = true;
                switch (State) {
                    case ProxyState.NotConnected:
                    case ProxyState.NotInitialized:
                        if (CheckConnected()) {
                            InitializeInner();
                        }
                        break;
                    case ProxyState.Initializing:
                    case ProxyState.Initialized:
                        // nop
                        break;
                    // nop;
                    default:
                        break;
                }
                processing = false;
            }), null, 0, 1000);
        }

        /// <summary>
        /// USB2_xfer2を実行して失敗時はAIRExceptionをthrowする。
        /// endpoint = 1固定。
        /// </summary>
        /// <param name="command">コマンドのbyte配列</param>
        private void XferInner(byte[] command) {
            unsafe
            {
                fixed (byte* pCommand = command)
                {
                    XferInner(1, pCommand, command.Length);
                }
            }
        }



        /// <summary>
        /// USB2_xfer2を実行して失敗時はAIRExceptionをthrowする
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="pCommand"></param>
        /// <param name="count"></param>
        private unsafe void XferInner(byte endpoint, byte* pCommand, long count) {
            byte status = 0;
            try {
                USB2_xfer2(&endpoint, pCommand, &count, &status);
            } catch (Exception ae) {
                throw new AIRException(AIRException.ErrorType.DeviceError, "カメラとの通信時にエラーが発生", ae);
            }
            if (status != 0) {
                throw new AIRException(AIRException.ErrorType.DeviceError, "USB2が失敗しました。", null);
            }
        }
        /// <summary>
        /// 画像取得スレッドを開始する
        /// </summary>
        public override void StartImaging() {
            if (imagingTimer != null) {
                return;
            }
            bool processing = false;
            imagingTimer = new Timer(new TimerCallback((o) => {
                if (processing) {
                    return;
                } else {
                    processing = true;
                }
                if (State == ProxyState.Initialized) {
                    GetSignals();
                } else {
                    StopImaging();
                }
                processing = false;
            }), null, 0, 30);
        }
        /// <summary>
        /// 画像取得スレッドを停止する
        /// </summary>
        public override void StopImaging() {
            if (imagingTimer != null) {
                imagingTimer.Dispose();
                imagingTimer = null;
            }
        }


        /// <summary>
        /// USB2_xfer2を使用して1フレームの画像を受信する。
        /// </summary>
        /// <param name="image">受信バッファ</param>
        /// <returns></returns>
        private void GetSignals() {
            if (imagingPaused) {
                return;
            }
            if (State != ProxyState.Initialized) {
                return;
            }
            try {
                lock (Lock) {
                    // 1 Frame らしい。詳細不明。サンプルコードまる写し。リファレンス無し。
                    XferInner(new byte[] { 8, 0x2a, 2, 0x18, 2 });
                    // GetImageではなくこちらをつかうらしい。サンプルコードまる写し。
                    unsafe
                    {
                        fixed (ushort* pSignals = signals)
                        {
                            XferInner(5, (byte*)pSignals, signals.Length * 2);
                        }
                    }
                    //ushort[] esig = new ushort[signals.Length];
                    //Array.Copy(signals, esig, signals.Length);
                    try {
                        FireGetSignals(signals);
                    } catch (Exception ex) {
                        Logger.warn(ex, "OnGetImage event error");
                    }
                }
            } catch (AIRException ae) {
                State = ProxyState.NotInitialized;
                FireError(ae);
            }

        }
        /// <summary>
        /// 座標のシグナル値を返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override ushort SignalAt(int x, int y) {
            return signals[ImageWidth * y + x];
        }

        /// <summary>
        /// 破棄する時に停止する
        /// </summary>
        public override void Release() {
            StopImaging();
            if (statusTimer != null) {
                statusTimer.Dispose();
                statusTimer = null;
            }
            State = ProxyState.NotInitialized;
        }

    }
}
