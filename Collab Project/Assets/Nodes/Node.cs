using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField]
    private bool right = false, down = false, left = false, up = false;

    public List<Vector2> Directions()
    {
        List<Vector2> directions = new List<Vector2>();

        if (right)
            directions.Add(Vector2.right);
        if (down)
            directions.Add(Vector2.down);
        if (left)
            directions.Add(Vector2.left);
        if (up)
            directions.Add(Vector2.up);

        return directions;
    }

    private void OnValidate()
    {
       transform.GetChild(0).GetComponent<Node_Canvas>().rightArrow.gameObject.SetActive(right);
       transform.GetChild(0).GetComponent<Node_Canvas>().downArrow.gameObject.SetActive(down);
       transform.GetChild(0).GetComponent<Node_Canvas>().leftArrow.gameObject.SetActive(left);
       transform.GetChild(0).GetComponent<Node_Canvas>().upArrow.gameObject.SetActive(up);
    }
}
