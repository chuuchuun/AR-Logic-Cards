using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "LogicLink/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Basic Info")]
    public string levelName = "New Level";
    [TextArea(2, 4)]
    public string description = "Build a circuit that...";
    public string successMessage = "Great! Level complete.";

    [Header("Validation")]
    // Optional: names of cards that must be present (e.g., "Input_zero_card", "NOT_card")
    public string[] requiredCardNames;
    // If you want more precise validation, you can add expected truth table, but we'll keep simple.

    [TextArea(2, 4)]
    public string hint = "No hint available.";
}