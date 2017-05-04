using System;
using System.IO;

namespace JsonRpc
{
    public static class Tracer
    {
        private static TextWriter tw = null;

        public static void Connect(Stream channel)
        {
            Connect(new StreamWriter(channel));
        }

        public static void Connect(TextWriter writer)
        {
            tw = writer;
        }

        public static void Do(Action<TextWriter> writeLineAction)
        {
            if (tw != null && writeLineAction != null)
                writeLineAction.Invoke(tw);
        }
    }
}
