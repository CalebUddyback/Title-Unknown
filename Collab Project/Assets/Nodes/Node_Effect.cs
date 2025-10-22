using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node_Effect : MonoBehaviour
{
    public bool isSearched = false;

    public string ActionName { get; set; }

    public abstract IEnumerator ImediateEffect(Character character);

    public abstract IEnumerator ActivateEffect(Character character);

    public abstract IEnumerator PassEffect(Character character);
}
