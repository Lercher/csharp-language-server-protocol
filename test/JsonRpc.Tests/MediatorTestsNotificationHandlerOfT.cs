﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using JsonRpc.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NSubstitute;
using Xunit;

namespace JsonRpc.Tests
{
    public class MediatorTestsNotificationHandlerOfT
    {
        [Method("$/cancelRequest")]
        public interface ICancelRequestHandler : INotificationHandler<CancelParams> { }

        [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
        public class CancelParams
        {
            public object Id { get; set; }
        }

        [Fact]
        public async Task ExecutesHandler()
        {
            var cancelRequestHandler = Substitute.For<ICancelRequestHandler>();

            var collection = new HandlerCollection { cancelRequestHandler };
            var mediator = new RequestRouter(collection);

            var @params = new CancelParams() { Id = Guid.NewGuid() };
            var notification = new Notification("$/cancelRequest", JObject.Parse(JsonConvert.SerializeObject(@params)));

            mediator.RouteNotification(notification);

            await cancelRequestHandler.Received(1).Handle(Arg.Any<CancelParams>());
        }

    }
}