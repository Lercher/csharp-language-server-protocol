using Lsp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleServer
{
    public class CollectingTextWriter : System.IO.TextWriter, IDisposable
    {
        private System.IO.TextWriter tw;
        private int maxLineNumber1;
        public readonly List<Diagnostic> list = new List<Diagnostic>();

        public CollectingTextWriter(System.IO.TextWriter tw, int maxLineNumber1)
        {
            this.tw = tw;
            this.maxLineNumber1 = maxLineNumber1;
        }

        public override Encoding Encoding => tw.Encoding;

        public override void Write(char value)
        {
            tw.Write(value);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            tw.WriteLine(format, arg0, arg1, arg2);
            if (arg0 is int line && arg1 is int col && arg2 is string msg)
            {
                var pos = new Position(line - 1, col - 1);
                var range = new Range(pos, pos);
                list.Add(new Diagnostic() { Source = "Coco/R", Message = msg, Range = range, Severity = DiagnosticSeverity.Error });
            }
        }

        public override void WriteLine(string value)
        {
            WriteLine(string.Empty, maxLineNumber1 + 1, 1, value);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            tw.Dispose();
        }
    }
}
