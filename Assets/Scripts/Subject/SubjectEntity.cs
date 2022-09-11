using System.Collections.Generic;
using Sifter.Room;
using Sifter.Tools.Logger;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sifter.Tools.Editor;
#endif

namespace Sifter.Subject
{
    public class SubjectEntity : MonoBehaviour
    {
        public SubjectSO _subjectSo;
        public List<RoomSO> _applicableRooms;
#if UNITY_EDITOR
        void OnValidate()
        {
            if (_subjectSo != null) gameObject.name = _subjectSo.SubjectName;
        }

        [Button(ButtonSizes.Medium, ButtonStyle.FoldoutButton)]
        void PopulateApplicableRooms()
        {
            var allRooms = EditorExtensions.GetAllSOInstances<RoomSO>();
            foreach (var room in allRooms)
            foreach (var roomFacility in _subjectSo.RoomFacilityRequirements)
            {
                var checkedFacilities = 0;
                if (room.RoomFacilities.Contains(roomFacility)) checkedFacilities++;

                if (checkedFacilities == _subjectSo.RoomFacilityRequirements.Count && !_applicableRooms.Contains(room))
                {
                    _applicableRooms.Add(room);
                    SifterLog.Print($"{room} was added to Applicable rooms!");
                }
                else if (_applicableRooms.Contains(room))
                {
                    SifterLog.Print($"{room} Has already been added to the list.");
                }
                else
                {
                    SifterLog.Print($"{room} is NOT applicable.");
                }
            }
        }
#endif
    }
}