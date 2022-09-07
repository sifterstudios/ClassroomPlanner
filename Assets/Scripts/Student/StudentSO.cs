using Sifter.Degree;
using UnityEngine;

namespace Sifter.Student
{
    [CreateAssetMenu(fileName = "NewStudent", menuName = "_Entities/Student")]
    public class StudentSO : ScriptableObject
    {
        public string FirstName = "FirstName";
        public string LastName = "LastName";
        public bool AccessibilityRequirement;
        public DegreeEnum Degree;
    }
}