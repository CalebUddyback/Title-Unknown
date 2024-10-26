using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public SpriteRenderer owner;
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();


    void LateUpdate()
    {
        spriteRenderer.sprite = owner.sprite;

        transform.position = new Vector3(owner.transform.position.x, 0.001f, owner.transform.position.z + owner.transform.position.y);
    }
}
