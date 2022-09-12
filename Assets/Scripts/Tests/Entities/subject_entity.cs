using System.Collections.Generic;
using NUnit.Framework;
using Sifter.Room;
using Sifter.Subject;
using UnityEngine;

namespace Tests.Entities
{
    public class subject_entity
    {
        [Test]
        public void gameobject_changes_name_when_so_is_added()
        {
            var go = new GameObject().AddComponent<SubjectEntity>();
            var oldName = go.name;
            var sE = go.GetComponent<SubjectEntity>();
            var sSO = ScriptableObject.CreateInstance<SubjectSO>();
            sSO.SubjectName = "TheAwesomestName";
            sE._subjectSo = sSO;
            sE.RenameIfSOAdded();
            Assert.AreEqual(sSO.SubjectName, go.name);
        }

        [Test]
        public void populate_applicable_rooms()
        {
            var go = new GameObject().AddComponent<SubjectEntity>();
            var sE = go.GetComponent<SubjectEntity>();
            var sSO = ScriptableObject.CreateInstance<SubjectSO>();
            sSO.RoomFacilityRequirements.Add(RoomFacility.BigStudio);
            sE._subjectSo = sSO;
            sE._applicableRooms = new List<RoomSO>();
            sE.PopulateApplicableRooms();
            Assert.That(sE._applicableRooms[0].RoomFacilities.Contains(RoomFacility.BigStudio));
            // Assert.(LogType.Warning)
        }
    }
}