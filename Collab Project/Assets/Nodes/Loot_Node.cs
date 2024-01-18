using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot_Node : Node_Effect
{
    public List<string> loot = new List<string> {"flower", "rock", "sword"};


    public override IEnumerator LandOnEffect(Character character)
    {
        int i = Random.Range(0, loot.Count);

        string item = loot[i];

        //yield return StartCoroutine(character.Award(item));


        character.main_Camera.offset = new Vector3(0, 1, -12);

        character.animations.Clip("Award Get");

        character.item_Canvas.gameObject.SetActive(true);

        yield return character.item_Canvas.GetComponent<Item_Canvas>().WaitForSelection();

        int s = character.item_Canvas.GetComponent<Item_Canvas>().GetSelection();

        character.item_Canvas.gameObject.SetActive(false);

        character.main_Camera.offset = new Vector3(0, 1, -20);

        if (s == 1)
        {
            character.animations.Clip("Award Keep");
            yield return new WaitForSeconds(0.7f);
            print("Recieved: " + item);
            character.inventory.Add(item);
        }

        character.animations.Clip("Idle Down");



        print("Node Effect Done");
    }

    public override IEnumerator PassEffect(Character character)
    {
        yield return null;
    }
}
