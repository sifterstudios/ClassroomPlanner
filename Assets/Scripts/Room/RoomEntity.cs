using UnityEngine;

namespace Sifter.Room
{
    public class RoomEntity : MonoBehaviour
    {
        [SerializeField] RoomSO _roomSO;


        void OnValidate()
        {
            if (_roomSO != null && gameObject.name != _roomSO.RoomName) gameObject.name = _roomSO.RoomName;
        }
    }
}