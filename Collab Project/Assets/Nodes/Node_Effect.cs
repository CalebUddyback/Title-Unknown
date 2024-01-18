using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node_Effect : MonoBehaviour
{
    public abstract IEnumerator LandOnEffect(Character character);

    public abstract IEnumerator PassEffect(Character character);
}
