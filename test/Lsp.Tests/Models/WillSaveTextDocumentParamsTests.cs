﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class WillSaveTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WillSaveTextDocumentParams() {
                Reason = TextDocumentSaveReason.FocusOut,
                TextDocument = new TextDocumentIdentifier(new Uri("file:///abc/123.cs"))
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<WillSaveTextDocumentParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
