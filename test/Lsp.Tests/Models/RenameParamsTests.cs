﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class RenameParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new RenameParams() {
                NewName = "new name",
                Position = new Position(1, 2),
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<RenameParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
