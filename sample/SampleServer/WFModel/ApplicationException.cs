using System;

namespace SampleServer
{

    public class ApplicationException : Exception
    {
        public ApplicationException(string str) : base(str) { }
    }
}