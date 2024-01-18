using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public MainCamera main_Camera;

    public Board board;

    public Vector2Int currentNode = Vector2Int.zero;

    [HideInInspector]
    public AnimationController animations;

    public Canvas dice_Canvas, movement_Canvas, item_Canvas;

    public List<string> inventory = new List<string>();

    public int gold = 0;

    private void Start()
    {
        animations = transform.Find("Sprites").GetComponent<AnimationController>();

        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        dice_Canvas.gameObject.SetActive(true);

        dice_Canvas.GetComponent<Dice_Canvas>().ResetResults();

        yield return new WaitUntil(() => dice_Canvas.GetComponent<Dice_Canvas>().GetResults != 0);

        dice_Canvas.gameObject.SetActive(false);

        int diceResults = dice_Canvas.GetComponent<Dice_Canvas>().GetResults;

        movement_Canvas.GetComponent<Movement_Canvas>().maxAmount = diceResults;

        movement_Canvas.gameObject.SetActive(true);

        yield return new WaitUntil(() => movement_Canvas.GetComponent<Movement_Canvas>().submittedPath != null);

        var movementPath = movement_Canvas.GetComponent<Movement_Canvas>().submittedPath;   

        //Use MovementPath for pathtracking (knockback frm node effect

        main_Camera.target = transform;

        yield return StartCoroutine(main_Camera.LerpMove(1f));

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(Moving(movementPath));



        //StartCoroutine(Moving(1));
    }


    //MOVE METHOD TO MOVEMENT CANVAS

    IEnumerator Moving(List<Transform> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            //Transform nextNode = (currentNode + dir < nodes.transform.childCount) ? nodes.GetNodePos(currentNode + dir) : nodes.GetNodePos(0);

            Transform nextNode = path[i];

            FaceNextDirection(nextNode);

            while (MoveToNextNode(nextNode))
            {
                yield return null;
            }

            currentNode = board.GetNodeIndex(nextNode);

            if (i != path.Count - 1)
            {
                if (board.GetNodePos(currentNode).GetComponent<Node_Effect>() != null)
                    yield return StartCoroutine(board.GetNodePos(currentNode).GetComponent<Node_Effect>().PassEffect(this));
            }
        }

        animations.Clip("Idle " + IsFacing);

        Debug.Log("Done Moving");

        StartCoroutine(FaceCamera());
    }

    bool MoveToNextNode(Transform targetNode)
    {
        float speed = 15f;

        return targetNode.position != (transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime));
    }

    public string IsFacing { get; set; } = "Down";

    IEnumerator FaceCamera()
    {
        if (IsFacing != "Up")
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.15f);

            if (Random.Range(0, 2) == 1)
                animations.Clip("Idle Right");
            else
                animations.Clip("Idle Left");

            yield return new WaitForSeconds(0.15f);
        }

        animations.Clip("Idle Down");

        yield return new WaitForSeconds(0.3f);

        if (board.GetNodePos(currentNode).GetComponent<Node_Effect>() != null)
            yield return StartCoroutine(board.GetNodePos(currentNode).GetComponent<Node_Effect>().LandOnEffect(this));
        else
            print("No Effect");

        StartCoroutine(Movement());
    }

    void FaceNextDirection(Transform targetNode)
    {
        if (transform.position.y == targetNode.position.y)
        {
            if (transform.position.x < targetNode.position.x)
            {
                animations.Clip("Run Right");
                IsFacing = "Right";
            }

            if (transform.position.x > targetNode.position.x)
            {
                animations.Clip("Run Left");
                IsFacing = "Left";
            }
        }


        if (transform.position.x == targetNode.position.x)
        {
            if (transform.position.y < targetNode.position.y)
            {
                animations.Clip("Run Up");
                IsFacing = "Up";
            }

            if (transform.position.y > targetNode.position.y)
            {
                animations.Clip("Run Down");
                IsFacing = "Down";
            }
        }
    }
}
