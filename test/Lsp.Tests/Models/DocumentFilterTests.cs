﻿using System;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class DocumentFilterTests
    {
        [Theory, JsonFixture]
        public void Empty(string expected)
        {
            var model = new DocumentFilter();
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void OnlyLanguage(string expected)
        {
            var model = new DocumentFilter() {
                Language = "csharp"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void OnlyScheme(string expected)
        {
            var model = new DocumentFilter() {
                Scheme = "abc"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void OnlyPattern(string expected)
        {
            var model = new DocumentFilter() {
                Pattern = "123**"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void Mixed(string expected)
        {
            var model = new DocumentFilter() {
                Pattern = "123**",
                Language = "csharp"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }

        [Theory, JsonFixture]
        public void Full(string expected)
        {
            var model = new DocumentFilter() {
                Pattern = "123**",
                Language = "csharp",
                Scheme = "abc"
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<DocumentFilter>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
