using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

[CustomEditor(typeof(InputBasics))]
public class InputBasicsEditor : OdinEditor
{
    string foldoutText = "Drag and Swipe Vector";
    bool showVectorFoldout = false;

    float circleRadius = 1f; 
    float circlePadding = 10;
    Vector2 circleCenter;
    float circlePixelRadius;

    public override void OnInspectorGUI()
    {
        InputBasics input = (InputBasics)target;
        DrawDefaultInspector();

        // Show Debug Rectangle in Inspector
        showVectorFoldout = EditorGUILayout.Foldout(showVectorFoldout, foldoutText);
        if (showVectorFoldout)
        {
            // Draw Rectangle
            Rect rect = GUILayoutUtility.GetRect(150, 150);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            Handles.BeginGUI();

            // Draw Circle
            circleCenter = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
            circlePixelRadius = Mathf.Min(rect.width, rect.height) / 2 - circlePadding;
            Handles.color = Color.white;
            Handles.DrawWireDisc(circleCenter, Vector3.forward, circlePixelRadius);

            // Draw Vectors
            if (input.hasSwiped)
                DrawVector(rect, input.swipeVector/input.ScreenDiagonal, Color.red);
            if (input.isPressing)
                DrawVector(rect, input.dragVector/input.ScreenDiagonal, Color.blue);
        }
    }

    void DrawVector(Rect rect, Vector2 vector, Color color)
    {
        // Draw Vector
        Handles.color = color;
        Vector2 scaledVector = Vector2.ClampMagnitude(vector, circleRadius);  // Use constant radius
        Vector2 vectorEnd = circleCenter + new Vector2(scaledVector.x * circlePixelRadius / circleRadius, -scaledVector.y * circlePixelRadius / circleRadius);

        Handles.DrawLine(circleCenter, vectorEnd);

        // Draw Vector head
        Vector2 direction = (vectorEnd - circleCenter).normalized;
        Vector2 right = Quaternion.Euler(0, 0, -30) * direction;
        Vector2 left = Quaternion.Euler(0, 0, 30) * direction;

        Handles.DrawLine(vectorEnd, vectorEnd - right * 10);
        Handles.DrawLine(vectorEnd, vectorEnd - left * 10);

        // Show Vector Size
        Handles.Label(vectorEnd, vector.magnitude.ToString("F2"));

        Handles.EndGUI();
    }
}
