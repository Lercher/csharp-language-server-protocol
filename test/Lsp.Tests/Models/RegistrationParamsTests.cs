﻿using System;
using System.Collections.Generic;
using FluentAssertions;
using Lsp.Models;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Models
{
    public class RegistrationParamsTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new RegistrationParams() {
                Registrations = new[] {  new Registration() {
                    Id = "abc",
                    Method = "method",
                    RegisterOptions = new Dictionary<string, object>()
                } }
            };
            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<RegistrationParams>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
