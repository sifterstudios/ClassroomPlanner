using UnityEngine;

namespace Sifter.Tools.Logger
{
    public static class SifterLog
    {
        public static void Print(string message)
        {
            Debug.Log(message);
        }
    }
}