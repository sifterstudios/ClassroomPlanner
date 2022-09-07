using System.Collections.Generic;
using Sifter.Floor;
using UnityEngine;

namespace Sifter.Room
{
    [CreateAssetMenu(fileName = "NewRoom", menuName = "_Entities/Room")]
    public class RoomSO : ScriptableObject
    {
        public string RoomName = "RoomName";
        public List<RoomFacility> RoomFacilities = new();
        public FloorEnum Floor;
    }
}