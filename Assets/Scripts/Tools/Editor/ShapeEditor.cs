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
        SelectionInfo _selectionInfo;

        void OnSceneGUI()
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.Repaint)
            {
                Draw();
            }
            else if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
            else
            {
                HandleInput(guiEvent);
                if (_needsRepaint)
                {
                    HandleUtility.Repaint();
                    _needsRepaint = false;
                }
            }


            Draw();

            if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            }
        }

        void HandleInput(Event guiEvent)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneHeight = 0;
            float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
            Vector3 mousePosition = mouseRay.GetPoint((dstToDrawPlane));

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDown(mousePosition);
            }

            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseUp(mousePosition);
            }

            if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDrag(mousePosition);
            }

            if (!_selectionInfo.MouseIsOverPoint) UpdateMouseOverInfo(mousePosition);
            
        }

        void Draw()
        {
            for (int i = 0; i < _shapeCreator._points.Count; i++)
            {
                Vector3 nextPoint = _shapeCreator._points[(i + 1) % _shapeCreator._points.Count];
                if (i == _selectionInfo.LineIndex)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(_shapeCreator._points[i], nextPoint);
                }
                else
                {
                    Handles.color = Color.black;
                    Handles.DrawDottedLine(_shapeCreator._points[i], nextPoint, 4);
                }

                if (i == _selectionInfo.PointIndex)
                {
                    Handles.color = (_selectionInfo.PointIsSelected) ? Color.black : Color.red;
                }
                else
                {
                    Handles.color = Color.white;
                }

                Handles.color = Color.white;
                Handles.DrawSolidDisc(_shapeCreator._points[i], Vector3.up, _shapeCreator.HandleRadius);
            }

            _needsRepaint = false;
        }

        void OnEnable()
        {
            _shapeCreator = target as ShapeCreator;
            _selectionInfo = new SelectionInfo();
        }

        void UpdateMouseOverInfo(Vector3 mousePosition)
        {
            int mouseOverPointIndex = -1;

            for (int i = 0; i < _shapeCreator._points.Count; i++)
            {
                if (Vector3.Distance(mousePosition, _shapeCreator._points[i]) < _shapeCreator.HandleRadius)
                {
                    mouseOverPointIndex = i;
                    break;
                }
            }

            if (mouseOverPointIndex != _selectionInfo.PointIndex)
            {
                _selectionInfo.PointIndex = mouseOverPointIndex;
                _selectionInfo.MouseIsOverPoint = mouseOverPointIndex != -1;

                _needsRepaint = true;
            }

            if (_selectionInfo.MouseIsOverPoint)
            {
                _selectionInfo.MouseIsOverLine = false;
                _selectionInfo.LineIndex = -1;
            }
            else
            {
                int mouseOverLineIndex = -1;
                float closestLineDst = _shapeCreator.HandleRadius;
                for (int i = 0; i < _shapeCreator._points.Count; i++)
                {
                    Vector3 nextPointInShape = _shapeCreator._points[(i + 1) % _shapeCreator._points.Count];
                    float dstFromMouseToLine =
                        HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(), _shapeCreator._points[i].ToXZ(),
                            nextPointInShape.ToXZ());
                    if (dstFromMouseToLine < closestLineDst)
                    {
                        closestLineDst = dstFromMouseToLine;
                        mouseOverLineIndex = i;
                    }
                }

                if (_selectionInfo.LineIndex != mouseOverLineIndex)
                {
                    _selectionInfo.LineIndex = mouseOverLineIndex;
                    _selectionInfo.MouseIsOverLine = mouseOverLineIndex != -1;
                    _needsRepaint = true;
                }
            }
        }

        void HandleLeftMouseDown(Vector3 mousePosition)
        {
            if (!_selectionInfo.MouseIsOverPoint)
            {
                int newPointIndex = (_selectionInfo.MouseIsOverLine)
                    ? _selectionInfo.LineIndex + 1
                    : _shapeCreator._points.Count;
                Undo.RecordObject(_shapeCreator, "Add point");
                _shapeCreator._points.Insert(newPointIndex, mousePosition);
                _selectionInfo.PointIndex = newPointIndex;
            }

            _selectionInfo.PointIsSelected = true;
            _selectionInfo.PositionAtStartOfDrag = mousePosition;
            _needsRepaint = true;
        }

        void HandleLeftMouseUp(Vector3 mousePosition)
        {
            if (_selectionInfo.PointIsSelected)
            {
                _shapeCreator._points[_selectionInfo.PointIndex] = _selectionInfo.PositionAtStartOfDrag;
                Undo.RecordObject(_shapeCreator,  "Move point");
                _shapeCreator._points[_selectionInfo.PointIndex] = mousePosition;
                _selectionInfo.PointIsSelected = false;
                _selectionInfo.PointIndex = -1;
                _needsRepaint = true;
            }
        }

        void HandleLeftMouseDrag(Vector3 mousePosition)
        {
            if (_selectionInfo.PointIsSelected)
            {
                _shapeCreator._points[_selectionInfo.PointIndex] = mousePosition;
                _needsRepaint = true;
            }
        }
    }

    public class SelectionInfo
    {
        public int PointIndex = -1;
        public bool MouseIsOverPoint;
        public bool PointIsSelected;
        public Vector3 PositionAtStartOfDrag;
        public int LineIndex = -1;
        public bool MouseIsOverLine;
    }
}