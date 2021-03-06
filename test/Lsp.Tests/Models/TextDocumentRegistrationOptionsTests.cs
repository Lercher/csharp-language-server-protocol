﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class TextDocumentRegistrationOptionsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new TextDocumentRegistrationOptions() {
                DocumentSelector = new DocumentSelector(new DocumentFilter() {
                    Language = "csharp"
                })
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<TextDocumentRegistrationOptions>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
