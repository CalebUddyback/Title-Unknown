using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DialogueResponse
{
    public string response;
    public Node_Effect[] responseEffects;
    public int nextSection;
}

[System.Serializable]
public class DialogueSection
{
    public bool expression;
    public string dialogue;
    public DialogueResponse[] responses;
}

[System.Serializable]
public class DialogueBranch
{
    public string branchName;
    public int branchId;
    public DialogueSection[] section;
    public bool endOnFinal;
}

public class NPC_Dialogue : MonoBehaviour
{
    public DialogueBranch[] branches;
    public string npcName;
}
