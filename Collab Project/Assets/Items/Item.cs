using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public string _name;
    public Sprite icon;
    public bool usable;
    public int value;


    public abstract IEnumerator Use(Character character);
}
