﻿using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;
using Xunit.Sdk;

namespace JsonRpc.Tests
{
    public class MediatorTestsRequestHandlerOfTRequest
    {
        [Method("workspace/executeCommand")]
        public interface IExecuteCommandHandler : IRequestHandler<ExecuteCommandParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class ExecuteCommandParams
        {
            public string Command { get; set; }
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var executeCommandHandler = Substitute.For<IExecuteCommandHandler>();

            var collection = new HandlerCollection { executeCommandHandler };
            var mediator = new RequestRouter(collection);

            var id = Guid.NewGuid().ToString();
            var @params = new ExecuteCommandParams() { Command = "123" };
            var request = new Request(id, "workspace/executeCommand", JObject.Parse(JsonConvert.SerializeObject(@params)));

            var response = await mediator.RouteRequest(request);

            await executeCommandHandler.Received(1).Handle(Arg.Any<ExecuteCommandParams>(), Arg.Any<CancellationToken>());


        }
    }
}