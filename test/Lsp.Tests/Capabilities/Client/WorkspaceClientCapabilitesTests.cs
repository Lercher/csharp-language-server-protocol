﻿using System;
using FluentAssertions;
using Lsp.Capabilities.Client;
using Newtonsoft.Json;
using Xunit;

namespace Lsp.Tests.Capabilities.Client
{
    public class WorkspaceClientCapabilitesTests
    {
        [Theory, JsonFixture]
        public void SimpleTest(string expected)
        {
            var model = new WorkspaceClientCapabilites() {
                ApplyEdit = true,
                WorkspaceEdit = new WorkspaceEditCapability() { DocumentChanges = true},
                DidChangeConfiguration = new DidChangeConfigurationCapability() { DynamicRegistration = true },
                DidChangeWatchedFiles = new DidChangeWatchedFilesCapability() { DynamicRegistration = true },
                ExecuteCommand = new ExecuteCommandCapability() { DynamicRegistration = true },
                Symbol = new WorkspaceSymbolCapability() { DynamicRegistration = true },
            };

            var result = Fixture.SerializeObject(model);

            result.Should().Be(expected);

            var deresult = JsonConvert.DeserializeObject<WorkspaceClientCapabilites>(expected);
            deresult.ShouldBeEquivalentTo(model);
        }
    }
}
