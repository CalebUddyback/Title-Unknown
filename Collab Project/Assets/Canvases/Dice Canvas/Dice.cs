using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public Text text;

    public Coroutine rolling = null;

    public void StartRolling() => rolling = StartCoroutine(Roll(Random.Range(0.05f, 0.2f)));

    public void StartRolling(float speed) => rolling = StartCoroutine(Roll(speed));

    private bool isRolling = false;


    IEnumerator Roll(float speed)
    {
        GetResult = 6;

        isRolling = true;

        while (isRolling)
        {
            GetResult = (GetResult + 1 > 6) ? 1 : GetResult += 1;
            text.text = GetResult.ToString();
            yield return new WaitForSeconds(speed);
        }
    }

    public void StopRoll() => isRolling = false;

    public int GetResult { get; private set; } = -1;

}
