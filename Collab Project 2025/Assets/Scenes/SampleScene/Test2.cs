using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public SkillScriptTest[] skills;

    // Start is called before the first frame update
    void Start()
    {
        skills = transform.GetChild(0).GetComponents<SkillScriptTest>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("skill");
            StartCoroutine(skills[0].Action());
        }
    }
}
