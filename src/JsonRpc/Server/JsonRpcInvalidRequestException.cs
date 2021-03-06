﻿using System;

namespace JsonRpc.Server
{
    public class JsonRpcInvalidRequestException : JsonRpcException
    {
        public JsonRpcInvalidRequestException()
        {
        }

        public JsonRpcInvalidRequestException(string message) : base(message)
        {
        }

        public JsonRpcInvalidRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        //protected JsonRpcInvalidRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        //{
        //}
    }
}