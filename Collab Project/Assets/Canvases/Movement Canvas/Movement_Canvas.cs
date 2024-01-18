using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement_Canvas : MonoBehaviour
{
    public MainCamera main_Camera;

    public Character character;

    public Vector2Int selectedIndex = Vector2Int.zero;

    public List<Transform> currentPath = new List<Transform>();

    public List<Transform> submittedPath = null;

    public Button rightButton, downButton, leftButton, upButton, goButton;

    public Text totalText;

    public int maxAmount = 12;


    private void OnEnable()
    {
        totalText.text = currentPath.Count.ToString() + "/" + maxAmount.ToString();

        selectedIndex = character.currentNode;

        AvailableDirections(selectedIndex);

        goButton.interactable = false;

        submittedPath = null;

        character.main_Camera.offset = new Vector3(0, 1, -25);
    }

    private void Update()
    {

    }

    public void AvailableDirections(Vector2Int currentlySelectedIndex)
    {
        rightButton.interactable = false;
        downButton.interactable = false;
        leftButton.interactable = false;
        upButton.interactable = false;

        rightButton.interactable = Available(currentlySelectedIndex, Vector2Int.right);
      
        downButton.interactable = Available(currentlySelectedIndex, Vector2Int.down);
 
        leftButton.interactable = Available(currentlySelectedIndex, Vector2Int.left);
      
        upButton.interactable = Available(currentlySelectedIndex, Vector2Int.up);
    }

    bool Available(Vector2Int curr, Vector2Int dir)
    {
        Vector2Int nextPos = curr + dir;

        bool result = false;


        if (character.board.GetNodePos(nextPos) != null && currentPath.Count < maxAmount)
        {
            if (character.board.boardFlowDirection)
            {
                if (character.board.GetNodePos(selectedIndex).GetComponent<Node>().Directions().Contains(dir))
                    result = true;

                if (currentPath.Count > 1 && character.board.GetNodePos(nextPos) == currentPath[currentPath.Count - 2])
                    result = true;
                else if (character.board.GetNodePos(nextPos) == character.board.GetNodePos(character.currentNode))
                    result = true;
            }
            else
            {
                result = true;
            }
        }
        else
        {
            if (currentPath.Count > 1 && character.board.GetNodePos(nextPos) == currentPath[currentPath.Count - 2])
                result = true;
        }
            

        return result; 
    }

    public void SelectMovementAmount(int x)
    {
        Vector2Int direction = Vector2Int.zero;

        switch (x)
        {
            case 3:
                direction = Vector2Int.right;
                break;
            case 6:
                direction = Vector2Int.down;
                break;
            case 9:
                direction = Vector2Int.left;
                break;
            case 12:
                direction = Vector2Int.up;
                break;
            default:
                Debug.Log("Wrong Button input");
                break;

        }

        if (character.board.GetNodePos(selectedIndex + direction) == null)
        {
            return;
        }


        if (currentPath.Count > 1)
        {
            if (character.board.GetNodePos(selectedIndex + direction) != currentPath[currentPath.Count - 2])
            {
                currentPath.Add(character.board.GetNodePos(selectedIndex + direction));
            }
            else
            {
                currentPath.RemoveAt(currentPath.Count - 1);
            }
        }
        else if (currentPath.Count == 1)
        {
            if (character.board.GetNodePos(selectedIndex + direction) != character.board.GetNodePos(character.currentNode))
            {
                currentPath.Add(character.board.GetNodePos(selectedIndex + direction));
            }
            else
            {
                currentPath.RemoveAt(currentPath.Count - 1);
                goButton.interactable = false;
            }
        }
        else
        {
            currentPath.Add(character.board.GetNodePos(character.currentNode + direction));
            goButton.interactable = true;
        }

        selectedIndex += direction;


        AvailableDirections(selectedIndex);


        totalText.text = currentPath.Count.ToString() + "/" + maxAmount.ToString();

        main_Camera.target = character.board.GetNodePos(selectedIndex);

    }

    public void SubmitPath()
    {
        if (currentPath.Count > 0)
        {
            submittedPath = new List<Transform>(currentPath);
            currentPath.Clear();
            character.main_Camera.offset = new Vector3(0, 1, -20);
            gameObject.SetActive(false);
        }
        else
        {
            print("No Path chosen");
        }
    }

}
