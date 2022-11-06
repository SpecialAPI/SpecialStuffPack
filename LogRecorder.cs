using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialStuffPack
{
    public class LogRecorder : ILogListener
    {
        public void Dispose()
        {
        }

        public void LogEvent(object sender, LogEventArgs eventArgs)
        {
            lines.Add(eventArgs.ToString());
        }

        public static List<string> lines = new();
    }
}
