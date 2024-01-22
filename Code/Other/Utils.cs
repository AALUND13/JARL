namespace JARL
{
    public static class Utils
    {
        public static bool logging = true;

        public static void LogInfo(string message) { if (logging) UnityEngine.Debug.Log(message); }
        public static void LogWarn(string message) { if (logging) UnityEngine.Debug.LogWarning(message); }
        public static void LogError(string message) { if (logging) UnityEngine.Debug.LogError(message); }
    }
}
