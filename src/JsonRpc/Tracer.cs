using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace JsonRpc
{
    public static class Tracer
    {
        private static TextWriter tw = null;
        private static Stopwatch sw;
        private static ISet<int> deactivated = new HashSet<int>();

        public static void Connect(Stream channel)
        {
            Connect(new StreamWriter(channel));
        }

        public static void Connect(TextWriter writer)
        {
            tw = writer;
            sw = Stopwatch.StartNew();
        }

        public static void Deactivate(IEnumerable<int> list)
        {
            foreach (var i in list)
                deactivated.Add(i);
        }

        public static void Do(int i, Action<TextWriter> writeLineAction)
        {
            if (tw != null && writeLineAction != null && !deactivated.Contains(i))
            {
                tw.Write(sw.Elapsed.ToString());
                writeLineAction.Invoke(tw);
            }                
        }
    }
}
