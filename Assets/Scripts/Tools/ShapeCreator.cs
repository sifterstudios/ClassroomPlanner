using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sifter
{
    public class ShapeCreator : MonoBehaviour
    {
        [HideInInspector] public List<Shape> Shapes = new List<Shape>();

        public float HandleRadius = .5f;
    }

    [System.Serializable]
    public class Shape
    {
        public List<Vector3> _points = new List<Vector3>();
    }
}
