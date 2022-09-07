using UnityEngine;

namespace Sifter.Tutor
{
    [CreateAssetMenu(fileName = "NewTutor", menuName = "_Entities/Tutor")]
    public class TutorSO : ScriptableObject
    {
        public string FirstName = "FirstName";
        public string LastName = "LastName";

        [Tooltip("Monday to Friday. 0 means 'Not Available', 1 means 'Available'")]
        public int WeekAvailability;

        public bool AccessibilityRequirement;
    }
}