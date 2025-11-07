using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card_Prefab: MonoBehaviour
{
    public TextMeshProUGUI cardText, manaCost;

    public Image locked;

    public Image usableIMG;

    public Animation discardable;

    public Transform effects;

    private bool usable;
    public bool Usable
    {
        get
        {
            return usable;
        }
        set
        {
            usable = value;
            usableIMG.color = usable ?  Color.green : Color.red;
        }
    }

    public void Discardable(bool d)
    {
        discardable.gameObject.SetActive(d);

        if (d)
            discardable.Play();
        else
            discardable.Stop();
    }
}
