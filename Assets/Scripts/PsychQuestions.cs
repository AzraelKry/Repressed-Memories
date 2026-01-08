using UnityEngine;

[System.Serializable]
public class PsychQuestion
{
    [TextArea(2, 4)] public string questionText;
    public string choiceA;
    public string choiceB;

    public int scoreA;  // +1, 0, or -1 depending on answer
    public int scoreB;
}

