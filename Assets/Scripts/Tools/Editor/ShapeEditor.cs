using Sifter.Tools.Geometry;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Sifter.Tools.Editor
{
    [CustomEditor(typeof(ShapeCreator))]
    public class ShapeEditor : UnityEditor.Editor
    {
        SelectionInfo _selectionInfo;
        bool _shapeChangedSinceLastRepaint;

        ShapeCreator _shapeCreator;

        Shape selectedShape => _shapeCreator.Shapes[_selectionInfo.SelectedShapeIndex];

        void OnEnable()
        {
            _shapeChangedSinceLastRepaint = true;
            _shapeCreator = target as ShapeCreator;
            _selectionInfo = new SelectionInfo();
            Undo.undoRedoPerformed += OnUndoOrRedo;
            UnityEditor.Tools.hidden = true;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoOrRedo;
            UnityEditor.Tools.hidden = false;
        }

        void OnSceneGUI()
        {
            var guiEvent = Event.current;

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
                if (_shapeChangedSinceLastRepaint) HandleUtility.Repaint();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var helpMessage =
                "Left click to add points.\nShift-left click on point to delete.\nShift-left click on empty space to create new shape.";
            EditorGUILayout.HelpBox(helpMessage, MessageType.Info);

            var shapeDeleteIndex = -1;
            _shapeCreator.ShowShapesList = EditorGUILayout.Foldout(_shapeCreator.ShowShapesList, "Show Shapes List");
            if (_shapeCreator.ShowShapesList)
                for (var i = 0; i < _shapeCreator.Shapes.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Shape " + (i + 1));

                    GUI.enabled = i != _selectionInfo.SelectedShapeIndex;
                    if (GUILayout.Button("Select")) _selectionInfo.SelectedShapeIndex = i;
                    GUI.enabled = true;

                    if (GUILayout.Button("Delete")) shapeDeleteIndex = i;
                    GUILayout.EndHorizontal();
                }

            if (shapeDeleteIndex != -1)
            {
                Undo.RecordObject(_shapeCreator, "Delete shape");
                _shapeCreator.Shapes.RemoveAt(shapeDeleteIndex);
                _selectionInfo.SelectedShapeIndex =
                    Mathf.Clamp(_selectionInfo.SelectedShapeIndex, 0, _shapeCreator.Shapes.Count - 1);
            }

            if (GUI.changed)
            {
                _shapeChangedSinceLastRepaint = true;
                SceneView.RepaintAll();
            }
        }

        void CreateNewShape()
        {
            Undo.RecordObject(_shapeCreator, "Create shape");
            _shapeCreator.Shapes.Add(new Shape());
            _selectionInfo.SelectedShapeIndex = _shapeCreator.Shapes.Count - 1;
        }

        void CreateNewPoint(Vector3 position)
        {
            var mouseIsOverSelectedShape = _selectionInfo.MouseOverShapeIndex == _selectionInfo.SelectedShapeIndex;
            var newPointIndex = _selectionInfo.MouseIsOverLine && mouseIsOverSelectedShape
                ? _selectionInfo.LineIndex + 1
                : selectedShape._points.Count;
            Undo.RecordObject(_shapeCreator, "Add point");
            selectedShape._points.Insert(newPointIndex, position);
            _selectionInfo.PointIndex = newPointIndex;
            _selectionInfo.MouseOverShapeIndex = _selectionInfo.SelectedShapeIndex;
            _shapeChangedSinceLastRepaint = true;

            SelectPointUnderMouse();
        }

        void DeletePointUnderMouse()
        {
            Undo.RecordObject(_shapeCreator, "Delete point");
            selectedShape._points.RemoveAt(_selectionInfo.PointIndex);
            _selectionInfo.PointIsSelected = false;
            _selectionInfo.MouseIsOverPoint = false;
            _shapeChangedSinceLastRepaint = true;
        }

        void SelectPointUnderMouse()
        {
            _selectionInfo.PointIsSelected = true;
            _selectionInfo.MouseIsOverPoint = true;
            _selectionInfo.MouseIsOverLine = false;
            _selectionInfo.LineIndex = -1;

            _selectionInfo.PositionAtStartOfDrag = selectedShape._points[_selectionInfo.PointIndex];
            _shapeChangedSinceLastRepaint = true;
        }

        void SelectShapeUnderMouse()
        {
            if (_selectionInfo.MouseOverShapeIndex != -1)
            {
                _selectionInfo.SelectedShapeIndex = _selectionInfo.MouseOverShapeIndex;
                _shapeChangedSinceLastRepaint = true;
            }
        }

        void HandleInput(Event guiEvent)
        {
            var mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            float drawPlaneHeight = 0;
            var dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
            var mousePosition = mouseRay.GetPoint(dstToDrawPlane);

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.Shift)
                HandleShiftLeftMouseDown(mousePosition);

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
                HandleLeftMouseDown(mousePosition);

            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0) HandleLeftMouseUp(mousePosition);

            if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 &&
                guiEvent.modifiers == EventModifiers.None)
                HandleLeftMouseDrag(mousePosition);

            if (!_selectionInfo.PointIsSelected) UpdateMouseOverInfo(mousePosition);
        }

        void HandleShiftLeftMouseDown(Vector3 mousePosition)
        {
            if (_selectionInfo.MouseIsOverPoint)
            {
                SelectShapeUnderMouse();
                DeletePointUnderMouse();
            }
            else
            {
                CreateNewShape();
                CreateNewPoint(mousePosition);
            }
        }

        void HandleLeftMouseDown(Vector3 mousePosition)
        {
            if (_shapeCreator.Shapes.Count == 0) CreateNewShape();

            SelectShapeUnderMouse();

            if (_selectionInfo.MouseIsOverPoint)
                SelectPointUnderMouse();
            else
                CreateNewPoint(mousePosition);
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
                _shapeChangedSinceLastRepaint = true;
            }
        }

        void HandleLeftMouseDrag(Vector3 mousePosition)
        {
            if (_selectionInfo.PointIsSelected)
            {
                selectedShape._points[_selectionInfo.PointIndex] = mousePosition;
                _shapeChangedSinceLastRepaint = true;
            }
        }

        void UpdateMouseOverInfo(Vector3 mousePosition)
        {
            var mouseOverPointIndex = -1;
            var mouseOverShapeIndex = -1;
            for (var shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
            {
                var currentShape = _shapeCreator.Shapes[shapeIndex];

                for (var i = 0; i < currentShape._points.Count; i++)
                    if (Vector3.Distance(mousePosition, currentShape._points[i]) < _shapeCreator.HandleRadius)
                    {
                        mouseOverPointIndex = i;
                        mouseOverShapeIndex = shapeIndex;
                        break;
                    }
            }

            if (mouseOverPointIndex != _selectionInfo.PointIndex ||
                mouseOverShapeIndex != _selectionInfo.MouseOverShapeIndex)
            {
                _selectionInfo.MouseOverShapeIndex = mouseOverShapeIndex;
                _selectionInfo.PointIndex = mouseOverPointIndex;
                _selectionInfo.MouseIsOverPoint = mouseOverPointIndex != -1;

                _shapeChangedSinceLastRepaint = true;
            }

            if (_selectionInfo.MouseIsOverPoint)
            {
                _selectionInfo.MouseIsOverLine = false;
                _selectionInfo.LineIndex = -1;
            }
            else
            {
                var mouseOverLineIndex = -1;
                var closestLineDst = _shapeCreator.HandleRadius;
                for (var shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
                {
                    var currentShape = _shapeCreator.Shapes[shapeIndex];

                    for (var i = 0; i < currentShape._points.Count; i++)
                    {
                        var nextPointInShape = currentShape._points[(i + 1) % currentShape._points.Count];
                        var dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(),
                            currentShape._points[i].ToXZ(), nextPointInShape.ToXZ());
                        if (dstFromMouseToLine < closestLineDst)
                        {
                            closestLineDst = dstFromMouseToLine;
                            mouseOverLineIndex = i;
                            mouseOverShapeIndex = shapeIndex;
                        }
                    }
                }

                if (_selectionInfo.LineIndex != mouseOverLineIndex ||
                    mouseOverShapeIndex != _selectionInfo.MouseOverShapeIndex)
                {
                    _selectionInfo.MouseOverShapeIndex = mouseOverShapeIndex;
                    _selectionInfo.LineIndex = mouseOverLineIndex;
                    _selectionInfo.MouseIsOverLine = mouseOverLineIndex != -1;
                    _shapeChangedSinceLastRepaint = true;
                }
            }
        }

        void Draw()
        {
            for (var shapeIndex = 0; shapeIndex < _shapeCreator.Shapes.Count; shapeIndex++)
            {
                var shapeToDraw = _shapeCreator.Shapes[shapeIndex];
                var shapeIsSelected = shapeIndex == _selectionInfo.SelectedShapeIndex;
                var mouseIsOverShape = shapeIndex == _selectionInfo.MouseOverShapeIndex;
                var deselectedShapeColour = Color.grey;

                for (var i = 0; i < shapeToDraw._points.Count; i++)
                {
                    var nextPoint = shapeToDraw._points[(i + 1) % shapeToDraw._points.Count];
                    if (i == _selectionInfo.LineIndex && mouseIsOverShape)
                    {
                        Handles.color = Color.red;
                        Handles.DrawLine(shapeToDraw._points[i], nextPoint);
                    }
                    else
                    {
                        Handles.color = shapeIsSelected ? Color.black : deselectedShapeColour;
                        Handles.DrawDottedLine(shapeToDraw._points[i], nextPoint, 4);
                    }

                    if (i == _selectionInfo.PointIndex && mouseIsOverShape)
                        Handles.color = _selectionInfo.PointIsSelected ? Color.black : Color.red;
                    else
                        Handles.color = shapeIsSelected ? Color.white : deselectedShapeColour;
                    Handles.DrawSolidDisc(shapeToDraw._points[i], Vector3.up, _shapeCreator.HandleRadius);
                }
            }

            if (_shapeChangedSinceLastRepaint) _shapeCreator.UpdateMeshDisplay();

            _shapeChangedSinceLastRepaint = false;
        }

        void OnUndoOrRedo()
        {
            if (_selectionInfo.SelectedShapeIndex >= _shapeCreator.Shapes.Count ||
                _selectionInfo.SelectedShapeIndex == -1)
                _selectionInfo.SelectedShapeIndex = _shapeCreator.Shapes.Count - 1;
            _shapeChangedSinceLastRepaint = true;
        }

        class SelectionInfo
        {
            public int LineIndex = -1;
            public bool MouseIsOverLine;
            public bool MouseIsOverPoint;
            public int MouseOverShapeIndex;
            public int PointIndex = -1;
            public bool PointIsSelected;
            public Vector3 PositionAtStartOfDrag;
            public int SelectedShapeIndex;
        }
    }
}
#endif