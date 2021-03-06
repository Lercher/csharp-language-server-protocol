﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class ExecuteCommandParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new ExecuteCommandParams() {
                Arguments = new ObjectContainer(1, "2"),
                Command = "command"
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<ExecuteCommandParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
