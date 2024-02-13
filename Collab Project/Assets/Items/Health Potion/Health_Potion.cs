using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Potion : Item
{
    [Header("Item specifics")]
    public GameObject pulse;

    public override IEnumerator Use(Character character)
    {
        GameObject p = Instantiate(pulse, new Vector2(character.transform.position.x, character.transform.position.y + 1), Quaternion.identity);

        p.GetComponent<SpriteRenderer>().color = Color.green;

        character.Health(10);

        print("Health increased!");

        yield return new WaitForSeconds(0.2f);
    }
}
