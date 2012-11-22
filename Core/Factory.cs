using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;

namespace Core {
    public class Factory {
        private static object _syncRoot = new object();
        private static ILoggingRepository loggingRepo;

        public Factory Instance {
            get {
                if (_instance == null) {
                    lock (_syncRoot) {
                        if (_instance != null)
                            return _instance;

                        _instance = new Factory();
                    }
                }
                return _instance;
            }
        }

        private Factory() {
            
        }
        private static Factory _instance;

        public static ILoggingRepository GetILoggingRepository() {
            if (loggingRepo == null) {
                loggingRepo = new LoggingRepository();
            }
            return loggingRepo;
        }
    }
}
