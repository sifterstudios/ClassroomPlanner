using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
                guiEvent.modifiers == EventModifiers.Shift)
            {
                HandleShiftLeftMouseDown(mousePosition);
            }

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
            {
                HandleLeftMouseDown(mousePosition);
            }

            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
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

        void SelectShapeUnderMouse()
        {
            if (_selectionInfo.MouseOverShapeIndex != -1)
            {
                _selectionInfo.SelectedShapeIndex = _selectionInfo.MouseOverShapeIndex;
                _needsRepaint = true;
            }
        }

        void Draw()
        {
            for (int shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
            {
                Shape shapeToDraw = _shapeCreator.Shapes[shapeIndex];
                bool shapeIsSelected = shapeIndex == _selectionInfo.SelectedShapeIndex;
                Color deselectedShapeColor = Color.gray;
                bool mouseIsOverShape = shapeIndex == _selectionInfo.MouseOverShapeIndex;


                for (int i = 0; i < shapeToDraw._points.Count; i++)
                {
                    Vector3 nextPoint = shapeToDraw._points[(i + 1) % shapeToDraw._points.Count];
                    if (i == _selectionInfo.LineIndex && mouseIsOverShape)
                    {
                        Handles.color = Color.red;
                        Handles.DrawLine(shapeToDraw._points[i], nextPoint);
                    }
                    else
                    {
                        Handles.color = (shapeIsSelected) ? Color.black : deselectedShapeColor;
                        Handles.DrawDottedLine(shapeToDraw._points[i], nextPoint, 4);
                    }

                    if (i == _selectionInfo.PointIndex && mouseIsOverShape)
                    {
                        Handles.color = (_selectionInfo.PointIsSelected) ? Color.black : Color.red;
                    }
                    else
                    {
                        Handles.color = (shapeIsSelected) ? Color.white : deselectedShapeColor;
                    }

                    Handles.color = Color.white;
                    Handles.DrawSolidDisc(shapeToDraw._points[i], Vector3.up, _shapeCreator.HandleRadius);
                }
            }

            _needsRepaint = false;
        }

        void OnEnable()
        {
            _shapeCreator = target as ShapeCreator;
            _selectionInfo = new SelectionInfo();
            Undo.undoRedoPerformed += OnUndoOrRedo;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoOrRedo;
        }

        void UpdateMouseOverInfo(Vector3 mousePosition)
        {
            int mouseOverPointIndex = -1;
            int mouseOverShapeindex = -1;

            for (int shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
            {
                Shape currentShape = _shapeCreator.Shapes[shapeIndex];

                for (int i = 0; i < currentShape._points.Count; i++)
                {
                    if (Vector3.Distance(mousePosition, currentShape._points[i]) < _shapeCreator.HandleRadius)
                    {
                        mouseOverPointIndex = i;
                        mouseOverShapeindex = shapeIndex;
                        break;
                    }
                }
            }

            if (mouseOverPointIndex != _selectionInfo.PointIndex ||
                mouseOverShapeindex != _selectionInfo.MouseOverShapeIndex)
            {
                _selectionInfo.MouseOverShapeIndex = mouseOverShapeindex;
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

                for (int shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
                {
                    Shape currentShape = _shapeCreator.Shapes[shapeIndex];


                    for (int i = 0; i < currentShape._points.Count; i++)
                    {
                        Vector3 nextPointInShape = currentShape._points[(i + 1) % currentShape._points.Count];
                        float dstFromMouseToLine =
                            HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(),
                                currentShape._points[i].ToXZ(),
                                nextPointInShape.ToXZ());
                        if (dstFromMouseToLine < closestLineDst)
                        {
                            closestLineDst = dstFromMouseToLine;
                            mouseOverLineIndex = i;
                            mouseOverPointIndex = shapeIndex;
                        }
                    }
                }

                if (_selectionInfo.LineIndex != mouseOverLineIndex ||
                    mouseOverShapeindex != _selectionInfo.MouseOverShapeIndex)
                {
                    _selectionInfo.MouseOverShapeIndex = mouseOverShapeindex;
                    _selectionInfo.LineIndex = mouseOverLineIndex;
                    _selectionInfo.MouseIsOverLine = mouseOverLineIndex != -1;
                    _needsRepaint = true;
                }
            }
        }

        void HandleShiftLeftMouseDown(Vector3 mousePosition)
        {
            CreateNewShape();
            CreateNewPoint(mousePosition);
        }

        void HandleLeftMouseDown(Vector3 mousePosition)
        {
            if (_shapeCreator.Shapes.Count == 0)
            {
                CreateNewShape();
            }

            SelectShapeUnderMouse();

            if (_selectionInfo.MouseIsOverPoint) SelectPointUnderMouse();
            else CreateNewPoint(mousePosition);
        }

        void CreateNewPoint(Vector3 mousePosition)
        {
            bool mouseIsOverSelectedShape = _selectionInfo.MouseOverShapeIndex == _selectionInfo.SelectedShapeIndex;
            int newPointIndex = (_selectionInfo.MouseIsOverLine && mouseIsOverSelectedShape)
                ? _selectionInfo.LineIndex + 1
                : selectedShape._points.Count;
            Undo.RecordObject(_shapeCreator, "Add point");
            selectedShape._points.Insert(newPointIndex, mousePosition);
            _selectionInfo.PointIndex = newPointIndex;
            _selectionInfo.MouseOverShapeIndex = _selectionInfo.SelectedShapeIndex;
            _needsRepaint = true;

            SelectPointUnderMouse();
        }

        void SelectPointUnderMouse()
        {
            _selectionInfo.PointIsSelected = true;
            _selectionInfo.MouseIsOverPoint = true;
            _selectionInfo.MouseIsOverLine = false;
            _selectionInfo.LineIndex = -1;

            _selectionInfo.PositionAtStartOfDrag = selectedShape._points[_selectionInfo.PointIndex];
        }

        void HandleLeftMouseUp(Vector3 mousePosition)
        {
            if (_selectionInfo.PointIsSelected)
            {
                selectedShape._points[_selectionInfo.PointIndex] = _selectionInfo.PositionAtStartOfDrag;
                Undo.RecordObject(_shapeCreator, "Move point");
                selectedShape._points[_selectionInfo.PointIndex] = mousePosition;
                _selectionInfo.PointIsSelected = false;
                _selectionInfo.PointIndex = -1;
                _needsRepaint = true;
            }
        }

        void HandleLeftMouseDrag(Vector3 mousePosition)
        {
            if (_selectionInfo.PointIsSelected)
            {
                selectedShape._points[_selectionInfo.PointIndex] = mousePosition;
                _needsRepaint = true;
            }
        }


        void CreateNewShape()
        {
            Undo.RecordObject(_shapeCreator, "Create shape");
            _shapeCreator.Shapes.Add(new Shape());
            _selectionInfo.SelectedShapeIndex = _shapeCreator.Shapes.Count - 1;
        }


        void OnUndoOrRedo()
        {
            if (_selectionInfo.SelectedShapeIndex >= _shapeCreator.Shapes.Count)
            {
                _selectionInfo.SelectedShapeIndex = _shapeCreator.Shapes.Count - 1;
            }
        }

        Shape selectedShape => _shapeCreator.Shapes[_selectionInfo.SelectedShapeIndex];
    }

    public class SelectionInfo
    {
        public int PointIndex = -1;
        public bool MouseIsOverPoint;
        public bool PointIsSelected;
        public Vector3 PositionAtStartOfDrag;
        public int LineIndex = -1;
        public bool MouseIsOverLine;
        public int SelectedShapeIndex;
        public int MouseOverShapeIndex;
    }
}