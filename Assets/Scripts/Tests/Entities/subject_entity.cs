using NUnit.Framework;
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
            Assert.AreEqual(sSO.SubjectName, go.name);
        }
    }
}