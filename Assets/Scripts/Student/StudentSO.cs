using Sifter.Degree;
using UnityEngine;

namespace Sifter.Student
{
    [CreateAssetMenu(fileName = "NewStudent", menuName = "_Entities/Student")]
    public class StudentSO : ScriptableObject
    {
        public string FirstName = "FirstName";
        public string LastName = "LastName";
        public string Email = "Email";
        public int PhoneNumber;
        public bool AccessibilityRequirement;
        public DegreeEnum Degree;
    }
}