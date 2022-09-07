using System.Collections.Generic;
using Sifter.Degree;
using Sifter.Room;
using UnityEngine;

namespace Sifter.Subject
{
    [CreateAssetMenu(fileName = "NewSubject", menuName = "_Entities/Subject")]
    public class SubjectSO : ScriptableObject
    {
        public string SubjectName = "SubjectName";
        public List<DegreeEnum> DegreesTeachingThisSubject = new();
        public List<RoomFacility> RoomFacilityRequirements = new();
    }
}