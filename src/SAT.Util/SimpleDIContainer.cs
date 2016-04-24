using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAT.Util {
    /// <summary>
    /// Dependency Injection
    /// </summary>
    public class SimpleDIContainer {
        private static object Lock = new object();
        private static SimpleDIContainer singleton;
        /// <summary>
        /// singleton instance
        /// </summary>
        public static SimpleDIContainer Instance {
            get {
                if (singleton == null) {
                    lock (Lock) {
                        if (singleton == null) {
                            singleton = new SimpleDIContainer();
                        }
                    }
                }
                return singleton;
            }
        }

        private Dictionary<Type, Type> binds = new Dictionary<Type, Type>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClass"></param>
        /// <param name="implClass"></param>
        public void Bind(Type baseClass, Type implClass) {
            if (binds.ContainsKey(baseClass)) {
                throw new Exception("already binded:" + baseClass);
            } else if (!implClass.IsSubclassOf(baseClass)) {
                throw new Exception("not subclass:" + implClass);
            }
            binds[baseClass] = implClass;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClass"></param>
        /// <returns></returns>
        public bool IsBound(Type baseClass) {
            return binds.ContainsKey(baseClass);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClass"></param>
        /// <returns></returns>
        public Type GetBoundImpl(Type baseClass) {
            if (binds.ContainsKey(baseClass)) {
                return binds[baseClass];
            } else {
                throw new Exception("binding for " + baseClass + " is not found");
            }
        }


    }
}
