using System.Collections.Generic;
using Sifter.Degree;
using UnityEngine;

namespace Sifter.Subject
{
    [CreateAssetMenu(fileName = "NewElectiveSubject", menuName = "_Entities/Elective Subject")]
    public class ElectiveSO : SubjectSO
    {
        public List<DegreeSO> ElectableBy = new();
    }
}