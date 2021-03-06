﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CompletionItemKindTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = CompletionItemKind.Color;
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CompletionItemKind>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
