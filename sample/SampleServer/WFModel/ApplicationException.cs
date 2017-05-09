using System;

namespace SampleServer.WFModel
{

    public class ApplicationException : Exception
    {
        public ApplicationException(string str) : base(str) { }
    }
}