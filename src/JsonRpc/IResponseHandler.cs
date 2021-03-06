﻿using System.Threading.Tasks;

namespace JsonRpc
{
    /// <summary>
    ///
    /// Server --> -->
    ///               |
    /// Client <-- <--
    ///
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface IResponseHandler<TResponse> : IJsonRpcHandler
    {
        Task Handle(TResponse request);
    }
}