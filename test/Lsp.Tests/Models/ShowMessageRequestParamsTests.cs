﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ShowMessageRequestParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ShowMessageRequestParams() {
                Message = "message",
                Actions = new Container<MessageActionItem>(new MessageActionItem() {
                    Title = "abc"
                }),
                Type = MessageType.Error
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ShowMessageRequestParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
