using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Model;

namespace Core.Persistence
{
    public interface ILoggingRepository
    {
        IEnumerable<LogEntry> GetEntries();
        LogEntry GetEntry(int id);
        LogEntry AddEntry(LogEntry entry);
        User GetUserById(int id);
        User GetUserByApiKey(string apiKey);
    }
}
