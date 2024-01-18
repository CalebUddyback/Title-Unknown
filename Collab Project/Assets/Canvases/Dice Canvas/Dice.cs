using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    public Text text;

    public float rollSpeed = 0.2f;

    public void StartRoll() => StartCoroutine(Roll());

    private IEnumerator Roll()
    {
        IsRolling = true;

        GetResult = 1;

        while (true)
        {
            text.text = GetResult.ToString();

            yield return new WaitForSeconds(rollSpeed);

            if (!IsRolling)
                break;

            if (GetResult + 1 > 6)
                GetResult = 1;
            else
                GetResult++;           
        }
    }

    public void StopRoll() => IsRolling = false;

    public bool IsRolling { get; private set; } = false;

    public int GetResult { get; private set; } = -1;

}
