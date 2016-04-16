using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SAT.Util {
    /// <summary>
    /// SAT.Util�ŗ��p����v���p�e�B���ȒP�Ɏ��N���X
    /// </summary>
    public class UtilProperties {
        /// <summary>
        /// �v���p�e�B��ǂݍ��ނ��߂̃C���X�^���X
        /// </summary>
        public static FileProperties Properties {
            get;
            set;
        }
        static UtilProperties() {
            Properties = FileProperties.Default;
        }

        /// <summary>
        /// Logger�����p����f�B���N�g��
        /// </summary>
        public static string LogDirectory {
            get {
                string defDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Properties.GetString("SAT.Util.Logger.LogDir", defDir);
            }
        }
        /// <summary>
        /// Logger�̃f�t�H���g���x��
        /// </summary>
        public static string LogLevel {
            get {
                return Properties.GetString("SAT.Util.Logger.LogLevel", "INFO");
            }
        }
        /// <summary>
        /// �Â����O���폜��������̂������l
        /// �f�t�H���g30
        /// </summary>
        public static int LogCleanThreshold {
            get {
                return Properties.GetInt("SAT.Util.Logger.CleanThreshold", 30);
            }
        }

    }
}
