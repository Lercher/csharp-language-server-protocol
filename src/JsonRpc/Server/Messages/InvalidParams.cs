﻿namespace JsonRpc.Server.Messages
{
    public class InvalidParams : Error
    {
        public InvalidParams() : this(null) { }
        public InvalidParams(object id) : base(id, new ErrorMessage(-32602, "Invalid params")) { }
    }
}