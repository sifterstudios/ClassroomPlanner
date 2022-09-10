using Sifter;
using Sifter.Tools;
using Sifter.Tools.Geometry;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
    SelectionInfo selectionInfo;
    bool shapeChangedSinceLastRepaint;

    ShapeCreator shapeCreator;

    Shape SelectedShape => shapeCreator.Shapes[selectionInfo.selectedShapeIndex];

    void OnEnable()
    {
        shapeChangedSinceLastRepaint = true;
        shapeCreator = target as ShapeCreator;
        selectionInfo = new SelectionInfo();
        Undo.undoRedoPerformed += OnUndoOrRedo;
        Tools.hidden = true;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndoOrRedo;
        Tools.hidden = false;
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
            if (shapeChangedSinceLastRepaint) HandleUtility.Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var helpMessage =
            "Left click to add points.\nShift-left click on point to delete.\nShift-left click on empty space to create new shape.";
        EditorGUILayout.HelpBox(helpMessage, MessageType.Info);

        var shapeDeleteIndex = -1;
        shapeCreator.ShowShapesList = EditorGUILayout.Foldout(shapeCreator.ShowShapesList, "Show Shapes List");
        if (shapeCreator.ShowShapesList)
            for (var i = 0; i < shapeCreator.Shapes.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Shape " + (i + 1));

                GUI.enabled = i != selectionInfo.selectedShapeIndex;
                if (GUILayout.Button("Select")) selectionInfo.selectedShapeIndex = i;
                GUI.enabled = true;

                if (GUILayout.Button("Delete")) shapeDeleteIndex = i;
                GUILayout.EndHorizontal();
            }

        if (shapeDeleteIndex != -1)
        {
            Undo.RecordObject(shapeCreator, "Delete shape");
            shapeCreator.Shapes.RemoveAt(shapeDeleteIndex);
            selectionInfo.selectedShapeIndex =
                Mathf.Clamp(selectionInfo.selectedShapeIndex, 0, shapeCreator.Shapes.Count - 1);
        }

        if (GUI.changed)
        {
            shapeChangedSinceLastRepaint = true;
            SceneView.RepaintAll();
        }
    }

    void CreateNewShape()
    {
        Undo.RecordObject(shapeCreator, "Create shape");
        shapeCreator.Shapes.Add(new Shape());
        selectionInfo.selectedShapeIndex = shapeCreator.Shapes.Count - 1;
    }

    void CreateNewPoint(Vector3 position)
    {
        var mouseIsOverSelectedShape = selectionInfo.mouseOverShapeIndex == selectionInfo.selectedShapeIndex;
        var newPointIndex = selectionInfo.mouseIsOverLine && mouseIsOverSelectedShape
            ? selectionInfo.lineIndex + 1
            : SelectedShape._points.Count;
        Undo.RecordObject(shapeCreator, "Add point");
        SelectedShape._points.Insert(newPointIndex, position);
        selectionInfo.pointIndex = newPointIndex;
        selectionInfo.mouseOverShapeIndex = selectionInfo.selectedShapeIndex;
        shapeChangedSinceLastRepaint = true;

        SelectPointUnderMouse();
    }

    void DeletePointUnderMouse()
    {
        Undo.RecordObject(shapeCreator, "Delete point");
        SelectedShape._points.RemoveAt(selectionInfo.pointIndex);
        selectionInfo.pointIsSelected = false;
        selectionInfo.mouseIsOverPoint = false;
        shapeChangedSinceLastRepaint = true;
    }

    void SelectPointUnderMouse()
    {
        selectionInfo.pointIsSelected = true;
        selectionInfo.mouseIsOverPoint = true;
        selectionInfo.mouseIsOverLine = false;
        selectionInfo.lineIndex = -1;

        selectionInfo.positionAtStartOfDrag = SelectedShape._points[selectionInfo.pointIndex];
        shapeChangedSinceLastRepaint = true;
    }

    void SelectShapeUnderMouse()
    {
        if (selectionInfo.mouseOverShapeIndex != -1)
        {
            selectionInfo.selectedShapeIndex = selectionInfo.mouseOverShapeIndex;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleInput(Event guiEvent)
    {
        var mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
        float drawPlaneHeight = 0;
        var dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
        var mousePosition = mouseRay.GetPoint(dstToDrawPlane);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
            HandleShiftLeftMouseDown(mousePosition);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseDown(mousePosition);

        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0) HandleLeftMouseUp(mousePosition);

        if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
            HandleLeftMouseDrag(mousePosition);

        if (!selectionInfo.pointIsSelected) UpdateMouseOverInfo(mousePosition);
    }

    void HandleShiftLeftMouseDown(Vector3 mousePosition)
    {
        if (selectionInfo.mouseIsOverPoint)
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
        if (shapeCreator.Shapes.Count == 0) CreateNewShape();

        SelectShapeUnderMouse();

        if (selectionInfo.mouseIsOverPoint)
            SelectPointUnderMouse();
        else
            CreateNewPoint(mousePosition);
    }

    void HandleLeftMouseUp(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape._points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
            Undo.RecordObject(shapeCreator, "Move point");
            SelectedShape._points[selectionInfo.pointIndex] = mousePosition;

            selectionInfo.pointIsSelected = false;
            selectionInfo.pointIndex = -1;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void HandleLeftMouseDrag(Vector3 mousePosition)
    {
        if (selectionInfo.pointIsSelected)
        {
            SelectedShape._points[selectionInfo.pointIndex] = mousePosition;
            shapeChangedSinceLastRepaint = true;
        }
    }

    void UpdateMouseOverInfo(Vector3 mousePosition)
    {
        var mouseOverPointIndex = -1;
        var mouseOverShapeIndex = -1;
        for (var shapeIndex = 0; shapeIndex < shapeCreator.Shapes.Count; shapeIndex++)
        {
            var currentShape = shapeCreator.Shapes[shapeIndex];

            for (var i = 0; i < currentShape._points.Count; i++)
                if (Vector3.Distance(mousePosition, currentShape._points[i]) < shapeCreator.HandleRadius)
                {
                    mouseOverPointIndex = i;
                    mouseOverShapeIndex = shapeIndex;
                    break;
                }
        }

        if (mouseOverPointIndex != selectionInfo.pointIndex || mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
        {
            selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
            selectionInfo.pointIndex = mouseOverPointIndex;
            selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;

            shapeChangedSinceLastRepaint = true;
        }

        if (selectionInfo.mouseIsOverPoint)
        {
            selectionInfo.mouseIsOverLine = false;
            selectionInfo.lineIndex = -1;
        }
        else
        {
            var mouseOverLineIndex = -1;
            var closestLineDst = shapeCreator.HandleRadius;
            for (var shapeIndex = 0; shapeIndex < shapeCreator.Shapes.Count; shapeIndex++)
            {
                var currentShape = shapeCreator.Shapes[shapeIndex];

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

            if (selectionInfo.lineIndex != mouseOverLineIndex ||
                mouseOverShapeIndex != selectionInfo.mouseOverShapeIndex)
            {
                selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
                selectionInfo.lineIndex = mouseOverLineIndex;
                selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
                shapeChangedSinceLastRepaint = true;
            }
        }
    }

    void Draw()
    {
        for (var shapeIndex = 0; shapeIndex < shapeCreator.Shapes.Count; shapeIndex++)
        {
            var shapeToDraw = shapeCreator.Shapes[shapeIndex];
            var shapeIsSelected = shapeIndex == selectionInfo.selectedShapeIndex;
            var mouseIsOverShape = shapeIndex == selectionInfo.mouseOverShapeIndex;
            var deselectedShapeColour = Color.grey;

            for (var i = 0; i < shapeToDraw._points.Count; i++)
            {
                var nextPoint = shapeToDraw._points[(i + 1) % shapeToDraw._points.Count];
                if (i == selectionInfo.lineIndex && mouseIsOverShape)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(shapeToDraw._points[i], nextPoint);
                }
                else
                {
                    Handles.color = shapeIsSelected ? Color.black : deselectedShapeColour;
                    Handles.DrawDottedLine(shapeToDraw._points[i], nextPoint, 4);
                }

                if (i == selectionInfo.pointIndex && mouseIsOverShape)
                    Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
                else
                    Handles.color = shapeIsSelected ? Color.white : deselectedShapeColour;
                Handles.DrawSolidDisc(shapeToDraw._points[i], Vector3.up, shapeCreator.HandleRadius);
            }
        }

        if (shapeChangedSinceLastRepaint) shapeCreator.UpdateMeshDisplay();

        shapeChangedSinceLastRepaint = false;
    }

    void OnUndoOrRedo()
    {
        if (selectionInfo.selectedShapeIndex >= shapeCreator.Shapes.Count || selectionInfo.selectedShapeIndex == -1)
            selectionInfo.selectedShapeIndex = shapeCreator.Shapes.Count - 1;
        shapeChangedSinceLastRepaint = true;
    }

    public class SelectionInfo
    {
        public int lineIndex = -1;
        public bool mouseIsOverLine;
        public bool mouseIsOverPoint;
        public int mouseOverShapeIndex;

        public int pointIndex = -1;
        public bool pointIsSelected;
        public Vector3 positionAtStartOfDrag;
        public int selectedShapeIndex;
    }
}
#endif