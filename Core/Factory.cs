using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;

namespace Core
{
    public static class Factory
    {
        private static ILoggingRepository loggingRepo;

        public static ILoggingRepository GetILoggingRepository() {
            //if (loggingRepo == null)
            //    loggingRepo = new LoggingRepository();

            return new LoggingRepository(); ;
        }
    }
}
