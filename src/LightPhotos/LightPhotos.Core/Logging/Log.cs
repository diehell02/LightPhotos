// Copyright 2013-2015 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Serilog;
using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;
using Serilog.Exceptions;
using System.Diagnostics;
using System.Net.Sockets;

namespace LightPhotos.Core.Logging;

/// <summary>
/// An optional static entry point for logging that can be easily referenced
/// by different parts of an application. To configure the <see cref="Log"/>
/// set the Logger static property to a logger instance.
/// </summary>
/// <example>
/// Log.Logger = new LoggerConfiguration()
///     .WithConsoleSink()
///     .CreateLogger();
///
/// var thing = "World";
/// Log.Logger.Information("Hello, {Thing}!", thing);
/// </example>
/// <remarks>
/// The methods on <see cref="Log"/> (and its dynamic sibling <see cref="ILogger"/>) are guaranteed
/// never to throw exceptions. Methods on all other types may.
/// </remarks>
public static class Log
{
    static Log()
    {
        var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss:fff}] [{Level:u3}] [{ThreadId}] {Message}{Exception}{NewLine}";
        var processId = Environment.ProcessId.ToString();
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
        var logFileName = $"log_{processId}_{timestamp}.txt";
        Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.WithExceptionDetails()
            .Enrich.FromLogContext()
            .Enrich.WithThreadId()
#if DEBUG
                .MinimumLevel.Debug()
                .WriteTo.Debug()
#else
            .MinimumLevel.Information()
#endif
            .WriteTo.File(Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), logFileName),
            rollOnFileSizeLimit: true,
            outputTemplate: outputTemplate)
            .CreateLogger();
        Log.Information("Serilog is inited");
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void Verbose(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Verbose, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </example>
    public static void Verbose(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Verbose, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </example>
    public static void Debug(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Debug, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </example>
    public static void Debug(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Debug, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </example>
    public static void Information(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Information, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </example>
    public static void Information(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Information, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </example>
    public static void Warning(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Warning, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </example>
    public static void Warning(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Warning, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </example>
    public static void Error(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Error, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </example>
    public static void Error(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Error, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level.
    /// </summary>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Fatal("Process terminating.");
    /// </example>
    public static void Fatal(string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Fatal, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }

    /// <summary>
    /// Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception.
    /// </summary>
    /// <param name="exception">Exception related to the event.</param>
    /// <param name="messageTemplate">Message template describing the event.</param>
    /// <example>
    /// Log.Fatal(ex, "Process terminating.");
    /// </example>
    [MessageTemplateFormatMethod("messageTemplate")]
    public static void Fatal(Exception? exception, string messageTemplate, [CallerFilePath] string? callerFilePath = null, [CallerMemberName] string? callerMemberName = null, [CallerLineNumber] int callerLineNumber = 0)
    {
        Serilog.Log.Write(LogEventLevel.Fatal, exception, $"[{{@CallerFilePath:l}}] [{{@CallerMemberName:l}}:{{@CallerLineNumber}}] {messageTemplate}", Path.GetFileName(callerFilePath), callerMemberName, callerLineNumber);
    }
}