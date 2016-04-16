using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SAT.Util {
    /// <summary>
    /// �t�@�C���Ɋւ���֗����\�b�h�B
    /// </summary>
    public class FileUtil {
        /// <summary>
        ///     �t�@�C���܂��̓f�B���N�g���A����т��̓��e��V�����ꏊ�ɃR�s�[���܂��B</summary>
        /// <param name="stSourcePath">
        ///     �R�s�[���̃f�B���N�g���̃p�X�B</param>
        /// <param name="stDestPath">
        ///     �R�s�[��̃f�B���N�g���̃p�X�B</param>
        /// <param name="bOverwrite">
        ///     �R�s�[�悪�㏑���ł���ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>param>
        public static void CopyDirectory(string stSourcePath, string stDestPath, bool bOverwrite) {
            // �R�s�[��̃f�B���N�g�����Ȃ���΍쐬����
            if (!Directory.Exists(stDestPath)) {
                Directory.CreateDirectory(stDestPath);
                File.SetAttributes(stDestPath, File.GetAttributes(stSourcePath));
                bOverwrite = true;
            }

            // �R�s�[���̃f�B���N�g���ɂ��邷�ׂẴt�@�C�����R�s�[����
            if (bOverwrite) {
                foreach (string stCopyFrom in Directory.GetFiles(stSourcePath)) {
                    string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));
                    File.Copy(stCopyFrom, stCopyTo, true);
                }

                // �㏑���s�\�ȏꍇ�͑��݂��Ȃ����̂݃R�s�[����
            } else {
                foreach (string stCopyFrom in Directory.GetFiles(stSourcePath)) {
                    string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));

                    if (!File.Exists(stCopyTo)) {
                        File.Copy(stCopyFrom, stCopyTo, false);
                    }
                }
            }

            // �R�s�[���̃f�B���N�g�������ׂăR�s�[���� (�ċA)
            foreach (string stCopyFrom in Directory.GetDirectories(stSourcePath)) {
                string stCopyTo = Path.Combine(stDestPath, Path.GetFileName(stCopyFrom));
                CopyDirectory(stCopyFrom, stCopyTo, bOverwrite);
            }
        }
        /// <summary>
        /// �X�g���[������X�g���[���փR�s�[����
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
        /// �p�X����������
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
