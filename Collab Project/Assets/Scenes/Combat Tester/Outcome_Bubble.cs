using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Outcome_Bubble : MonoBehaviour
{
    public Text text;
    public Canvas canvas;

    private void Start()
    {
        StartCoroutine(Playing());
    }

    IEnumerator Playing()
    {
        yield return new WaitWhile(() => GetComponent<Animation>().isPlaying);

        Destroy(gameObject);
    }
}
