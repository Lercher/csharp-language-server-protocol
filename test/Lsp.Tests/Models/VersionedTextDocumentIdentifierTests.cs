﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class VersionedTextDocumentIdentifierTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new VersionedTextDocumentIdentifier() {
                Uri = new Uri("file:///abc/123.cs"),
                Version = 12
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<VersionedTextDocumentIdentifier>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
