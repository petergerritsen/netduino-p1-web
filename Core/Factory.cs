using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Persistence;

namespace Core
{
    public static class Factory
    {
        public static ILoggingRepository GetILoggingRepository() {
            return new LoggingRepository();
        }
    }
}
