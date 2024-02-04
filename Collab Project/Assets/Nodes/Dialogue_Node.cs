using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Node : Node_Effect
{
    public NPC_Dialogue npc_Dialogue;

    private readonly string npcDirection = "Left";

    private void Awake()
    {
        ActionName = "Discuss";
    }

    public override IEnumerator ImediateEffect(Character character)
    {
        GameObject dialogueBox = GameObject.Find("Main Camera").transform.GetChild(0).gameObject;

        yield return character.FaceDirection(npcDirection, false);

        dialogueBox.SetActive(true);

        yield return dialogueBox.GetComponent<Dialogue_Canvas>().PrintText(npc_Dialogue, 0);

        dialogueBox.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        isSearched = true;

        yield return null;
    }

    public override IEnumerator ActivateEffect(Character character)
    {
        GameObject dialogueBox = GameObject.Find("Main Camera").transform.GetChild(0).gameObject;

        yield return character.FaceDirection(npcDirection, false);

        dialogueBox.SetActive(true);



        CoroutineWithData cd = new CoroutineWithData(this, dialogueBox.GetComponent<Dialogue_Canvas>().PrintText(npc_Dialogue, 1));

        yield return cd.coroutine;

        Node_Effect dialogue_Result = (Node_Effect)cd.result;
        
        //yield return dialogueBox.GetComponent<Dialogue_Canvas>().PrintText(npc_Dialogue, 1);

        dialogueBox.SetActive(false);

        yield return dialogue_Result.ActivateEffect(character);

        yield return new WaitForSeconds(0.5f);

        isSearched = true;

        yield return null;
    }

    public override IEnumerator PassEffect(Character character)
    {
        yield return null;
    }
}
