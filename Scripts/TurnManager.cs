// TurnManager.cs
// --------------------------------------------------
// Provides a simple turn counter and a revert mechanism using Git.
// Attach this script to any GameObject in the scene (e.g., a manager object).
// --------------------------------------------------
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

public class TurnManager : MonoBehaviour {
    private static int currentTurn = 0;
    public static int CurrentTurn => currentTurn;

    // Call this method after any significant action (e.g., placing a building) to increment the turn.
    public static void Step() {
        currentTurn++;
        UnityEngine.Debug.Log($"[Turn] 현재 turn: {currentTurn}");
    }

#if UNITY_EDITOR
    // Menu item to display the current turn number.
    [MenuItem("Tools/Turn/Show Current Turn")] 
    private static void ShowCurrentTurn() {
        EditorUtility.DisplayDialog("Turn Info", $"현재 turn: {currentTurn}", "OK");
    }

    // Simple method to perform git reset. Use only in the project root.
    public static void RevertToTurn(int targetTurn) {
        if (targetTurn < 0 || targetTurn > currentTurn) {
            UnityEngine.Debug.LogError($"잘못된 turn 번호: {targetTurn}");
            return;
        }
        // Calculate how many commits to go back.
        int stepsBack = currentTurn - targetTurn;
        string gitDir = Path.Combine(Application.dataPath, "DeskCityAR");
        var psi = new System.Diagnostics.ProcessStartInfo {
            FileName = "git",
            Arguments = $"reset --hard HEAD~{stepsBack}",
            WorkingDirectory = gitDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        var proc = System.Diagnostics.Process.Start(psi);
        proc.WaitForExit();
        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(stdout)) UnityEngine.Debug.Log(stdout);
        if (!string.IsNullOrEmpty(stderr)) UnityEngine.Debug.LogWarning(stderr);
        // Refresh Unity asset database after revert.
        AssetDatabase.Refresh();
        currentTurn = targetTurn; // Update turn count.
        UnityEngine.Debug.Log($"[Turn] Reverted to turn {currentTurn}");
    }
#endif
}
