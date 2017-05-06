using System;
using System.Diagnostics;
using System.IO;

namespace JsonRpc
{
    public static class Tracer
    {
        private static TextWriter tw = null;
        private static Stopwatch sw;

        public static void Connect(Stream channel)
        {
            Connect(new StreamWriter(channel));
        }

        public static void Connect(TextWriter writer)
        {
            tw = writer;
            sw = Stopwatch.StartNew();
        }

        public static void Do(Action<TextWriter> writeLineAction)
        {
            if (tw != null && writeLineAction != null)
            {
                tw.Write(sw.Elapsed.ToString());
                writeLineAction.Invoke(tw);
            }                
        }
    }
}
