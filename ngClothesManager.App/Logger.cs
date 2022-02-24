using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ngClothesManager.App {
    public static class Logger {

        public static ObservableCollection<LogEntry> Entries = new ObservableCollection<LogEntry>();

        public static Action<LogEntry> OnLogEntryAdded;

        public static void Log(string message) {
            LogEntry logEntry = new LogEntry(message);
            
            Entries.Add(logEntry);
            Console.WriteLine(message);
            OnLogEntryAdded?.Invoke(logEntry);
        }

    }
}
