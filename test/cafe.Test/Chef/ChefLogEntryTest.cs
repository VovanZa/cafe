﻿using System;
using cafe.Chef;
using FluentAssertions;
using NLog;
using Xunit;

namespace cafe.Test.Chef
{
    public class ChefLogEntryTest
    {
        [Fact]
        public void Parse_ShouldParseErrorEntry()
        {
            AssertLineCanBeParsedAndLogged("[2016-12-15T16:42:53-06:00] ERROR: Running exception handlers",
                LogLevel.Error,
                new DateTime(2016, 12, 15, 22, 42, 53).AsUtc(), "Running exception handlers");
        }

        [Fact]
        public void Parse_ShouldParseFatalEntry()
        {
            AssertLineCanBeParsedAndLogged(
                "[2016-12-15T16:42:53-06:00] FATAL: Please provide the contents of the stacktrace.out file if you file a bug report",
                LogLevel.Fatal, new DateTime(2016, 12, 15, 22, 42, 53).AsUtc(),
                "Please provide the contents of the stacktrace.out file if you file a bug report");
        }

        [Fact]
        public void Parse_ShouldParseInfoEntry()
        {
            AssertLineCanBeParsedAndLogged(
                @"[2016-12-15T16:42:53-06:00] INFO: Client key C:\chef\client.pem is not present - registering",
                LogLevel.Info,
                new DateTime(2016, 12, 15, 22, 42, 53).AsUtc(), @"Client key C:\chef\client.pem is not present - registering");
        }

        [Fact]
        public void Parse_ShouldParseOutputWithoutMetadata()
        {
            AssertLineCanBeParsedAndLogged(
                "================================================================================",
                LogLevel.Info, null,
                "================================================================================");
        }

        [Fact]
        public void Parse_ShouldParseWarnEntry()
        {
            AssertLineCanBeParsedAndLogged(
                "[2016-12-15T16:42:49-06:00] WARN: Did not find config file: C:/chef/client.rb, using command line options.",
                LogLevel.Warn, new DateTime(2016, 12, 15, 22, 42, 49).AsUtc(),
                @"Did not find config file: C:/chef/client.rb, using command line options.");
        }

        private void AssertLineCanBeParsedAndLogged(string line, LogLevel expectedLogLevel, DateTime? expectedTime,
            string expectedEntry)
        {
            var actual = ChefLogEntry.Parse(line);

            actual.Level.Should().Be(expectedLogLevel);
            actual.Time.Should().Be(expectedTime);
            actual.Entry.Should().Be(expectedEntry);

            // let's also make sure we can log
            actual.Log();
            // no exception, we're good
        }

        [Fact]
        public void Parse_ShouldDealWithError()
        {
            AssertLineCanBeParsedAndLogged("[2016-12-15T16:04:56-06:00] ERROR: Exception handlers complete",
                LogLevel.Error, new DateTime(2016, 12, 15, 22, 04, 56).AsUtc(), @"Exception handlers complete");
        }

        [Fact]
        public void Parse_ShouldParseNull()
        {
            AssertLineCanBeParsedAndLogged(null, LogLevel.Info, null, string.Empty);
        }
    }
}