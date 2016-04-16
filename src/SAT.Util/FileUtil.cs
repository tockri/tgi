using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SAT.Util {
    /// <summary>
    /// ファイルに関する便利メソッド。
    /// </summary>
    public class FileUtil {
        /// <summary>
        ///     ファイルまたはディレクトリ、およびその内容を新しい場所にコピーします。</summary>
        /// <param name="stSourcePath">
        ///     コピー元のディレクトリのパス。</param>
        /// <param name="stDestPath">
        ///     コピー先のディレクトリのパス。</param>
        /// <param name="bOverwrite">
        ///     コピー先が上書きできる場合は true。それ以外の場合は false。</param>param>
        public static void CopyDirectory(string stSourcePath, string stDestPath, bool bOverwrite) {
            // コピー先のディレクトリがなければ作成する
            if (!Directory.Exists(stDestPath)) {
                Directory.CreateDirectory(stDestPath);
                File.SetAttributes(stDestPath, File.GetAttributes(stSourcePath));
                bOverwrite = true;
            }

            // コピー元のディレクトリにあるすべてのファイルをコピーする
            if (bOverwrite) {
                foreach (string stCopyFrom in Directory.GetFiles(stSourcePath)) {
                    string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));
                    File.Copy(stCopyFrom, stCopyTo, true);
                }

                // 上書き不可能な場合は存在しない時のみコピーする
            } else {
                foreach (string stCopyFrom in Directory.GetFiles(stSourcePath)) {
                    string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));

                    if (!File.Exists(stCopyTo)) {
                        File.Copy(stCopyFrom, stCopyTo, false);
                    }
                }
            }

            // コピー元のディレクトリをすべてコピーする (再帰)
            foreach (string stCopyFrom in Directory.GetDirectories(stSourcePath)) {
                string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));
                CopyDirectory(stCopyFrom, stCopyTo, bOverwrite);
            }
        }
        /// <summary>
        /// ストリームからストリームへコピーする
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        public static void CopyInto(Stream src, Stream dst) {
            byte[] buffer = new byte[1024 * 1024]; // 1MB
            while (true) {
                int read = src.Read(buffer, 0, buffer.Length);
                if (read <= 0) {
                    break;
                }
                dst.Write(buffer, 0, read);
            }
        }
        /// <summary>
        /// パスを解決する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ResolvePath(string path) {
            if (Path.IsPathRooted(path)) {
                return path;
            } else {
                var curr = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var fi = new FileInfo(Path.Combine(curr, path));
                return fi.FullName;
            }
        }
    }
}
