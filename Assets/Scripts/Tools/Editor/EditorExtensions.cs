#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Sifter.Tools.Editor
{
    public static class EditorExtensions
    {
        public static List<T> GetAllSOInstances<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name).ToList();
            var a = new List<T>();
            foreach (var t in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(t);
                a.Add(AssetDatabase.LoadAssetAtPath<T>(path));
            }

            return a;
        }
    }
}
#endif