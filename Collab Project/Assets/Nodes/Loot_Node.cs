using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot_Node : Node_Effect
{
    public List<Item> loot = new List<Item>();

    private void Awake()
    {
        ActionName = "Loot";
    }

    public override IEnumerator ImediateEffect(Character character)
    {
        yield return null;
    }

    public override IEnumerator ActivateEffect(Character character)
    {
        Item item = loot[Random.Range(0, loot.Count)];

        character.main_Camera.offset = new Vector3(0, 1, -12);

        character.animations.Clip("Award Get");

        character.award_Canvas.gameObject.SetActive(true);

        yield return character.award_Canvas.GetComponent<Award_Canvas>().WaitForSelection();

        int s = character.award_Canvas.GetComponent<Award_Canvas>().GetSelection();

        character.award_Canvas.gameObject.SetActive(false);

        character.main_Camera.offset = new Vector3(0, 1, -20);

        if (s == 1)
        {
            character.animations.Clip("Award Keep");
            yield return new WaitForSeconds(0.7f);
            print("Recieved: " + item._name);
            character.inventory.Add(item);
        }

        character.animations.Clip("Idle Down");
    }

    public override IEnumerator PassEffect(Character character)
    {
        yield return null;
    }
}
