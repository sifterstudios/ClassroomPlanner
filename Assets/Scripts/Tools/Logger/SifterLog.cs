using UnityEngine;

namespace Sifter.Tools.Logger
{
    public static class SifterLog
    {
        public static void Print(string message)
        {
            Debug.Log(message);
        }

        public static void Warning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }
    }
}