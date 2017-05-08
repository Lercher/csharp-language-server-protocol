using System.Threading.Tasks;
using JsonRpc;
using Lsp.Models;
// ReSharper disable CheckNamespace

namespace Lsp.Protocol
{
    public static class LogMessageExtensions
    {
        public static void LogMessage(this ILanguageServer mediator, LogMessageParams @params)
        {
            mediator.SendNotification("window/logMessage", @params);
        }

        public static void LogMessage(this ILanguageServer mediator, MessageType type, string msg)
        {
            mediator.LogMessage(new LogMessageParams() { Type = type, Message = msg });
        }

        public static void LogMessage(this ILanguageServer mediator, string msg)
        {
            mediator.LogMessage(MessageType.Info,  msg);
        }

    }
}