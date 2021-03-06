﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class CodeActionParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new CodeActionParams() {
                Context = new CodeActionContext() {
                    Diagnostics = new[] { new Diagnostic() {
                    Code = new DiagnosticCode("abcd"),
                    Message = "message",
                    Range = new Range(new Position(1, 1), new Position(2,2)),
                    Severity = DiagnosticSeverity.Error,
                    Source = "csharp"
                } }

                },
                Range = new Range(new Position(1, 1), new Position(2, 2)),
                TextDocument = new TextDocumentIdentifier() {
                    Uri = new Uri("file:///test/123/d.cs")
                }
            };
            var result = Fixture.SerializeObject(model);
            
            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<CodeActionParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
