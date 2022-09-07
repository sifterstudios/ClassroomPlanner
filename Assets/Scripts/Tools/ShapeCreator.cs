using System.Collections.Generic;
using Sifter.Tools.Geometry;
using UnityEngine;

namespace Sifter
{
    public class ShapeCreator : MonoBehaviour
    {
        public MeshFilter MeshFilter;
        
        [HideInInspector] public List<Shape> Shapes = new List<Shape>();

        [HideInInspector]
        public bool ShowShapesList;
        
        public float HandleRadius = .5f;


        public void UpdateMeshDisplay()
        {
            CompositeShape compShape = new CompositeShape(Shapes);
            MeshFilter.mesh = compShape.GetMesh();
        }
    }
}
