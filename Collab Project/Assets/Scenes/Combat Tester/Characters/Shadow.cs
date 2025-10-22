using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour
{
    public SpriteRenderer owner;
    private SpriteRenderer spriteRenderer => GetComponent<SpriteRenderer>();

    private float y;

    private void Awake()
    {
        transform.localPosition = new Vector2(0, 0.01f);

        y = transform.position.y - owner.transform.position.y;
    }

    void LateUpdate()
    {
        spriteRenderer.sprite = owner.sprite;

        transform.position = new Vector3(owner.transform.position.x, 0 + y, owner.transform.position.z + owner.transform.position.y);
    }
}
