using System.Collections.Generic;
using Sifter.Room;
using UnityEditor;
using UnityEngine;

namespace Sifter.Subject
{
    public class SubjectEntity : MonoBehaviour
    {
        [SerializeField] SubjectSO _subjectSo;
        public List<RoomSO> _applicableRooms;
        void OnValidate()
        {
            if (_subjectSo = null)
            {
                return;
            }
            gameObject.name = _subjectSo.SubjectName;
        }
        void PopulateList()
        {
            string[] assetNames = AssetDatabase.FindAssets("t:RoomSO", new []{})
        }
    }
}