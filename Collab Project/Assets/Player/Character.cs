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

    public Canvas dice_Canvas, movement_Canvas, actions_Canvas, inventory_canvas, award_Canvas;

    [Header("Personal Stats")]      // Put into scriptable object

    public List<Item> inventory = new List<Item>();

    public int gold = 0;

    private int health = 50;

    public UnityEngine.UI.Image healthBar;      // Control by gamecontroller
    public UnityEngine.UI.Text healthTxt;

    private void Start()
    {
        animations = transform.Find("Sprites").GetComponent<AnimationController>();

        healthBar.fillAmount = GetHealth() / 100f;
        healthTxt.text = GetHealth().ToString();

        StartCoroutine(Phases());
    }

    IEnumerator Phases()
    {
        animations.Clip("Idle Down");


        CoroutineWithData cd = new CoroutineWithData(this, DicePhase());     
        yield return cd.coroutine;
        
        int diceResults = (int)cd.result;



        cd = new CoroutineWithData(this, MovementPhase(diceResults));
        yield return cd.coroutine;
        
        //Use MovementPath for pathtracking (knockback from node effect)
        
        List<Transform> movementPath = (List<Transform>)cd.result;
        
        
        yield return new WaitForSeconds(0.2f);
        
        
        yield return Moving(movementPath);



        // DAMAGE OR MOVEMENT HERE

        if (board.GetTilePos(currentNode).GetComponent<Node_Effect>() != null)
            yield return board.GetTilePos(currentNode).GetComponent<Node_Effect>().ImediateEffect(this);
        else
            print("No Imediate Node Effect");




        yield return FaceDirection("Down", false);

        yield return new WaitForSeconds(0.3f);




        cd = new CoroutineWithData(this, ActionPhase());
        yield return cd.coroutine;

        




        yield return new WaitForSeconds(3f);

        //yield break;

        //UNCOMMENT BELOW TO LOOP 

        StartCoroutine(Phases());
        
        yield return null;
    }

    IEnumerator DicePhase()
    {
        dice_Canvas.gameObject.SetActive(true);

        dice_Canvas.GetComponent<Dice_Canvas>().SetDice(1);

        /* Delay because destroying in SetDice() does not happen quick enough */

        yield return new WaitForEndOfFrame();  

        /* Dont use CoroutineWithData so we can retrieve DiceTotal after the fact multiple times */

        yield return dice_Canvas.GetComponent<Dice_Canvas>().RollDice();

        int diceResults = dice_Canvas.GetComponent<Dice_Canvas>().GetDiceTotal;

        yield return new WaitForSeconds(0.5f);

        dice_Canvas.gameObject.SetActive(false);

        yield return diceResults;
    }

    public IEnumerator MovementPhase(int max)
    {
        movement_Canvas.GetComponent<Movement_Canvas>().maxAmount = max;

        movement_Canvas.gameObject.SetActive(true);

        CoroutineWithData cd = new CoroutineWithData(this, movement_Canvas.GetComponent<Movement_Canvas>().ChoosingPath(this));

        yield return cd.coroutine;

        List<Transform> movementPath = (List<Transform>)cd.result;

        main_Camera.offset = new Vector3(0, 1, -20);

        movement_Canvas.gameObject.SetActive(false);

        yield return main_Camera.LerpMove(transform, 1f);

        yield return movementPath;
    }

    public IEnumerator Moving(List<Transform> path)
    {

        for (int i = 0; i < path.Count; i++)
        {
            Transform nextNode = path[i];

            FaceNextNode(nextNode);

            while (MoveToNextNode(nextNode))
            {
                yield return null;
            }

            currentNode = board.GetTileIndex(nextNode);

            if (i != path.Count - 1)
            {
                if (board.GetTilePos(currentNode).GetComponent<Node_Effect>() != null)
                    yield return StartCoroutine(board.GetTilePos(currentNode).GetComponent<Node_Effect>().PassEffect(this));
            }
        }

        animations.Clip("Idle " + IsFacing);
    }

    bool MoveToNextNode(Transform targetNode)
    {
        float speed = 15f;

        return targetNode.position != (transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime));
    }

    public string IsFacing { get; set; } = "Down";

    void FaceNextNode(Transform targetNode)
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

    public IEnumerator FaceDirection(string targetDirection, bool immediate)
    {
        string[] directions = { "Right", "Down", "Left", "Up" };

        if (immediate)
        {
            animations.Clip("Idle " + targetDirection);
        }
        else
        {
            int currIndex = -1;

            int targetIndex = -1;

            for (int i = 0; i < directions.Length; i++)
            {
                if (directions[i] == targetDirection)
                {
                    targetIndex = i;
                }

                if (directions[i] == IsFacing)
                {
                    currIndex = i;
                }
            }

            if (currIndex == -1)
                animations.Clip("Idle " + directions[targetIndex]);

            int numOfTurns = targetIndex - currIndex;

            int turnDirection = 0;

            switch (Mathf.Abs(numOfTurns))
            {
                case 1:
                    turnDirection = (numOfTurns > 0) ? 1 : -1;
                    break;
                case 2:
                    turnDirection = (Random.Range(0, 2) == 1) ? 1 : -1;
                    break;
                case 3:
                    turnDirection = (numOfTurns < 0) ? 1 : -1;
                    numOfTurns = 1;
                    break;
            }

            //print(directions[currIndex] + " To " + directions[targetIndex] + ":" + Mathf.Abs(numOfTurns) +  " dir:" + turnDirection);


            for (int i = 1; i <= Mathf.Abs(numOfTurns); i++)
            {
                yield return new WaitForSeconds(0.15f);
                animations.Clip("Idle " + directions[Mod(currIndex + (i * turnDirection), directions.Length)]);
            }
        }

        IsFacing = targetDirection;
    }

    int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    IEnumerator ActionPhase()
    {
        Node_Effect nodeEffect = board.GetTilePos(currentNode).GetComponent<Node_Effect>();


        while (true)
        {

            actions_Canvas.gameObject.SetActive(true);
            actions_Canvas.GetComponent<CanvasGroup>().interactable = true;

            CoroutineWithData cd = new CoroutineWithData(this, actions_Canvas.GetComponent<Action_Canvas>().ChooseAction(this, nodeEffect));
            yield return cd.coroutine;

            string chosenAction = (string)cd.result;





            if (chosenAction == "node")
            {
                actions_Canvas.gameObject.SetActive(false);

                yield return new WaitForSeconds(1f);

                if (nodeEffect != null)
                {
                    yield return StartCoroutine(nodeEffect.ActivateEffect(this));
                    nodeEffect.isSearched = true;
                }
                else
                    print("No Effect");

                break;

            }
            else if (chosenAction == "item")
            {
                actions_Canvas.GetComponent<CanvasGroup>().interactable = false;
                inventory_canvas.gameObject.SetActive(true);

                cd = new CoroutineWithData(this, inventory_canvas.GetComponent<Inventory_Canvas>().ChooseItem(this));
                yield return cd.coroutine;

                int chosenItem = (int)cd.result;

                if (chosenItem == -1)
                {
                    print("Returned");
                    inventory_canvas.gameObject.SetActive(false);
                }
                else
                {
                    actions_Canvas.gameObject.SetActive(false);
                    inventory_canvas.gameObject.SetActive(false);
                    yield return inventory[chosenItem].GetComponent<Item>().Use(this);
                    inventory.RemoveAt(chosenItem);

                    break;
                }
            }
            else if (chosenAction == "rest")
            {
                actions_Canvas.gameObject.SetActive(false);
                animations.Clip("Rest");

                break;
            }
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public void Health(int change)
    {
        if (health + change > 100)
            health = 100;
        else if (health + change <= 0)
            Death();
        else
            health += change;

        healthBar.fillAmount = GetHealth() / 100f;
        healthTxt.text = GetHealth().ToString();
    }

    public void Death()
    {
        print("DEAD");
    }
}
