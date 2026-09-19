using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Outcome_String : MonoBehaviour
{
    public TextMeshProUGUI text;

    public IEnumerator Display(string str, Color clr)
    {
        text.color = clr;

        text.text = str;

        yield return new WaitForSeconds(0.167f); // Grow length

        yield return new WaitForSeconds(0.5f);  // view window

        text.GetComponent<Animation>().Play("Fade");
        yield return new WaitWhile(() => text.GetComponent<Animation>().isPlaying);

        Destroy(gameObject);
    }
}
