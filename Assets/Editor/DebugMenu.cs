using UnityEngine;
using UnityEditor;

public static class DebugMenu
{
    [MenuItem("Debug/Convert to Global Position")]
    public static void PrintGlobalPosition()
    {
        if (Selection.activeGameObject != null)
        {
            Selection.activeTransform.position = Selection.activeTransform.localPosition;
            //Debug.Log(Selection.activeGameObject.name + " is at " + Selection.activeGameObject.transform.position);
        }
    }
}