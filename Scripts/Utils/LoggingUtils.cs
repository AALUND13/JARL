namespace JARL {
    public static class LoggingUtils {
        public static void LogInfo(string message) { if(ConfigHandler.DebugMode.Value) UnityEngine.Debug.Log(message); }
        public static void LogWarn(string message) { if(ConfigHandler.DebugMode.Value) UnityEngine.Debug.LogWarning(message); }
        public static void LogError(string message) { if(ConfigHandler.DebugMode.Value) UnityEngine.Debug.LogError(message); }
    }
}
