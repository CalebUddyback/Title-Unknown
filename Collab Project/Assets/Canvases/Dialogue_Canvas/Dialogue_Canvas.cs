using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Canvas : MonoBehaviour
{
    public Image portrait;

    public Text textBox;

    public Transform responses;

    public GameObject responseButton;

    int next = -100;

    public bool autoContinue = false;

    public IEnumerator PrintText(NPC_Dialogue npc_dialogue, int branch)
    {
        int i = 0;

        next = -100;

        while (i < npc_dialogue.branches[branch].section.Length && next != -1)
        {
            textBox.text = npc_dialogue.branches[branch].section[i].dialogue;

            next = -100;

            for (int c = 0; c < responses.childCount; c++)
            {
                Destroy(responses.GetChild(c).gameObject);
            }

            foreach (DialogueResponse response in npc_dialogue.branches[branch].section[i].responses)
            {
                GameObject currResponse = Instantiate(responseButton, responses);

                currResponse.transform.GetChild(0).GetComponent<Text>().text = response.response;

                currResponse.GetComponent<Button>().onClick.AddListener(() => Response(response.nextSection));
            }

            yield return new WaitUntil(() => next != -100);

            if (next > 0)
                i = next;
            else
            {
                switch (next)
                {
                    case -1:
                        print("Dialogue Ended");
                        if(npc_dialogue.branches[branch].section[i].responses[0].responseEffects != null)
                            yield return npc_dialogue.branches[branch].section[i].responses[0].responseEffects;
                        break;

                    default:
                        Debug.Log("<color=red>Error:</color> Invalid 'Next Section'. Please Change 'Next Section' Of A Response: " + branch + " " + i);
                        break;

                }
            }    
        }
    }

    public void Response(int n)
    {
        if (n == -100)
        {
            Debug.Log("<color=red>Error:</color> '-100' Is The Dialogue Waiting Flag.");
            next = 101;
        }
        else
            next = n;

    }
}
