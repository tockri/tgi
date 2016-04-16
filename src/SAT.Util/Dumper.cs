using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SAT.Util {
    /// <summary>
    /// �I�u�W�F�N�g�̓��e�������o���f�o�b�O�p�N���X
    /// </summary>
    public class Dumper {
        /// <summary>
        /// �I�u�W�F�N�g�̑S�Ẵv���p�e�B�������o��
        /// </summary>
        /// <param name="target">�C�ӂ̃I�u�W�F�N�g</param>
        /// <returns>������</returns>
        public static string Dump(object target) {
            Hashtable t = new Hashtable(new ObjectComparator());
            return DumpInner(target, 0, t);
        }
        /// <summary>
        /// �C�ӂ̃I�u�W�F�N�g�𕶎���ɃV���A���C�Y����
        /// </summary>
        /// <param name="target"></param>
        /// <param name="depth"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private static string DumpInner(object target, int depth, Hashtable t) {
            if (target == null) {
                return "";
            } else if (t.ContainsKey(target)) {
                return "(same : " + target.GetHashCode() + ")";
            } else {
                t[target] = true;
                StringBuilder sb = new StringBuilder();
                sb.Append("(" + target.GetHashCode() + ") ");
                if (target is string 
                    || target is int 
                    || target is long 
                    || target is double 
                    || target is float) {
                    sb.Append(target.ToString());
                } else if (target is IEnumerable) {
                    sb.Append("[");
                    foreach (object e in (IEnumerable)target) {
                        sb.Append(e.ToString()).Append(",");
                    }
                    sb.Append("]\n");
                } else {
                    PropertyInfo[] attrs = target.GetType().GetProperties();
                    foreach (PropertyInfo pi in attrs) {
                        string inner;
                        try {
                            object v = pi.GetValue(target, null);
                            inner = DumpInner(v, depth + 1, t);
                        } catch (Exception e) {
                            inner = "(" + e.Message + ")";
                        }
                        sb.Append(pi.Name).Append(" : ").Append(inner).Append("\n");
                    }
                }
                return sb.ToString();
            }

        }
        private class ObjectComparator : IEqualityComparer {
            new public bool Equals(object a, object b) {
                return Object.ReferenceEquals(a, b);
            }
            public int GetHashCode(object a) {
                return a.GetHashCode();
            }
        }
    }
}
