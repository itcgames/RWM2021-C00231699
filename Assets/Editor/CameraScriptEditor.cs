using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DynamicCameraController)), CanEditMultipleObjects]
public class CameraScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DynamicCameraController controller = (DynamicCameraController)target;
        EditorGUILayout.LabelField("=========== Camera Follow ===========");


        EditorGUILayout.LabelField("=========== Axis Fixing ==============");
        controller.useBoundaryPositions = EditorGUILayout.Toggle("Use BoundaryPositions:", controller.useBoundaryPositions);
        if (controller.useBoundaryPositions == true)
        {
            controller.boundaryTopLeft = EditorGUILayout.Vector2Field("Boundary Top Left:", controller.boundaryTopLeft);
            controller.boundaryBottomRight = EditorGUILayout.Vector2Field("Boundary Bottom Right:", controller.boundaryBottomRight);
        }

    }

}
