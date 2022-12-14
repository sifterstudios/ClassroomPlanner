using System.Collections.Generic;
using Sifter.Subject;
using UnityEngine;

namespace Sifter.Degree
{
    [CreateAssetMenu(fileName = "NewDegree", menuName = "_Entities/Degree")]
    public class DegreeSO : ScriptableObject
    {
        public DegreeEnum Degree;
        public List<SubjectSO> Subjects = new();
    }
}