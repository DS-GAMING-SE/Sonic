using BepInEx.Logging;
using SonicTheHedgehog.Modules;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace SonicTheHedgehog
{
    internal static class Log
    {
        internal static ManualLogSource _logSource;

        internal static void Init(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        internal static void Debug(object data) => _logSource.LogDebug(data);
        internal static void Error(object data)
        {
            if (Config.EnableLogs().Value)
            {
                _logSource.LogError(data);
            }
        }
        internal static void Fatal(object data) => _logSource.LogFatal(data);
        internal static void Info(object data) => _logSource.LogInfo(data);
        internal static void Message(object data)
        {
            if (Config.EnableLogs().Value)
            {
                _logSource.LogMessage(data);
            }
        }
        internal static void Warning(object data)
        {
            if (Config.EnableLogs().Value)
            {
                _logSource.LogWarning(data);
            }
        }
    }
}