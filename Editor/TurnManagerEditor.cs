// TurnManagerEditor.cs
// --------------------------------------------------
// Editor window for the TurnManager allowing the user to input a target turn
// and revert the project state using Git reset.
// --------------------------------------------------
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TurnManagerEditor : EditorWindow {
    private int targetTurn = 0;

    [MenuItem("Tools/Turn/Revert To Specific Turn...")]
    public static void ShowWindow() {
        GetWindow<TurnManagerEditor>("Turn Revert").Show();
    }

    private void OnGUI() {
        GUILayout.Label("Revert Project to a Specific Turn", EditorStyles.boldLabel);
        targetTurn = EditorGUILayout.IntField("Target Turn", targetTurn);
        if (GUILayout.Button("Revert")) {
            if (targetTurn < 0) {
                EditorUtility.DisplayDialog("Error", "Turn number cannot be negative.", "OK");
                return;
            }
            // Call the TurnManager method to perform git reset.
            TurnManager.RevertToTurn(targetTurn);
        }
    }
}
#endif
