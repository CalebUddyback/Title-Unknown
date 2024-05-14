using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Energy_Potion : Item
{
    [Header("Item specifics")]
    public GameObject pulse;
    readonly int amount = 3;

    public override IEnumerator Use(Character character)
    {
        GameObject p = Instantiate(pulse, new Vector2(character.transform.position.x, character.transform.position.y + 1), Quaternion.identity);

        p.GetComponent<SpriteRenderer>().color = Color.blue;

        print("Movement increased!");

        CoroutineWithData cd = new CoroutineWithData(character, character.MovementPhase(amount));

        yield return cd.coroutine;

        yield return character.Moving((List<Transform>)cd.result);

        // DAMAGE OR MOVEMENT HERE

        if (character.board.GetTilePos(character.currentNode).GetComponent<Node_Effect>() != null)
            yield return character.board.GetTilePos(character.currentNode).GetComponent<Node_Effect>().ImediateEffect(character);
        else
            print("No Imediate Node Effect");




        yield return character.FaceDirection("Down", false);

        yield return new WaitForSeconds(0.3f);
    }
}
