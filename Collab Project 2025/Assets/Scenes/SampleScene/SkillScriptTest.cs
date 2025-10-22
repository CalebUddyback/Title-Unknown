using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillScriptTest : MonoBehaviour
{
    public string name_;
     
    public IEnumerator Action()
    {
        Debug.Log("Actions started");

        yield return new WaitForSeconds(1);

        Debug.Log("Actions done");

        yield return null;
    }
}
