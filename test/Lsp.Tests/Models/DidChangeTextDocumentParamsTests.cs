﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DidChangeTextDocumentParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new DidChangeTextDocumentParams() {
                ContentChanges = new[] {
                    new TextDocumentContentChangeEvent() {
                        Range = new Range(new Position(1,1), new Position(2, 2)),
                        RangeLength = 12,
                        Text = "abc"
                    }
                },
                TextDocument = new VersionedTextDocumentIdentifier() {

                }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DidChangeTextDocumentParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
