using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sifter.Tools
{
    [CustomEditor(typeof(ShapeCreator))]
    public class ShapeEditor : Editor
    {
        ShapeCreator _shapeCreator;
        bool _needsRepaint;
        void OnSceneGUI()
        {
            Event guiEvent = Event.current;

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneHeight = 0;
            float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
            Vector3 mousePosition = mouseRay.GetPoint((dstToDrawPlane));

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                _shapeCreator._points.Add(mousePosition);
                _needsRepaint = true;

            }

            if (_needsRepaint)
            {
                HandleUtility.Repaint();
                _needsRepaint = false;
            }

            for (int i = 0; i < _shapeCreator._points.Count; i++)
            {
                Handles.DrawSolidDisc(_shapeCreator._points[i], Vector3.up, .5f);
            }

            if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            
            
            
        }

        void OnEnable()
        {
            _shapeCreator = target as ShapeCreator;
        }
    }
}
