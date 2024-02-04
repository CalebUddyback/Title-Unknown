using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Award_Canvas : MonoBehaviour
{

    public IEnumerator WaitForSelection()
    {
        yield return new WaitUntil(() => Selection != 0);
    }

    public void SetSelection(int i) => Selection = i;

    public int GetSelection()
    {
        int x = Selection;
        Selection = 0;

        return x;
    }

    private int Selection { get; set; } = 0;
}
