using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movement_Canvas : MonoBehaviour
{
    private MainCamera main_Camera;

    private Character character;

    public Vector2Int selectedIndex = Vector2Int.zero;

    public List<Transform> currentPath = new List<Transform>();

    public Button rightButton, downButton, leftButton, upButton, goButton;

    public Text totalText;

    public int maxAmount = 12;

    bool doneChoosing = false;


    private void Awake()
    {
        rightButton.onClick.AddListener(() => SelectDirection(Vector2Int.right));
        downButton.onClick.AddListener(() => SelectDirection(Vector2Int.down));
        leftButton.onClick.AddListener(() => SelectDirection(Vector2Int.left));
        upButton.onClick.AddListener(() => SelectDirection(Vector2Int.up));
        goButton.onClick.AddListener(() => doneChoosing = true);
    }


    void SetUp()
    {
        totalText.text = currentPath.Count.ToString() + "/" + maxAmount.ToString();

        selectedIndex = character.currentNode;

        AvailableDirections(selectedIndex);

        goButton.interactable = false;

        doneChoosing = false;

        currentPath.Add(character.board.GetTilePos(character.currentNode));

        character.main_Camera.offset = new Vector3(0, 1, -25);
    }

    public IEnumerator ChoosingPath(Character c)
    {
        character = c;

        main_Camera = c.main_Camera;

        SetUp();

        yield return new WaitUntil(() => doneChoosing);

        currentPath.RemoveAt(0);    // Remove Start Position

        yield return new List<Transform>(currentPath);

        currentPath.Clear();     
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


    /* Putting this function in Node script maybe better */

    bool Available(Vector2Int curr, Vector2Int dir)
    {
        Vector2Int nextPos = curr + dir;

        if (character.board.GetTilePos(nextPos) == null)
            return false;

        if (currentPath.Count > 1 && (character.board.GetTilePos(nextPos) == currentPath[currentPath.Count - 2]))
            return true;

        if (currentPath.Count < maxAmount + 1)
        {
            if (character.board.boardFlowDirection)
            {
                if (character.board.GetTilePos(selectedIndex).GetComponent<Node>().Directions().Contains(dir))  //If node flows in direction
                    return true;      
            }
            else
                return true;
        }
            
        return false; 
    }

    public void SelectDirection(Vector2Int direction)
    {

        selectedIndex += direction;

        if (currentPath.Count > 1)
        {
            if (character.board.GetTilePos(selectedIndex) == currentPath[currentPath.Count - 2])
                currentPath.RemoveAt(currentPath.Count - 1);
            else
                currentPath.Add(character.board.GetTilePos(selectedIndex));
        }
        else
            currentPath.Add(character.board.GetTilePos(selectedIndex));

        if (currentPath.Count > 1)
            goButton.interactable = true;
        else
            goButton.interactable = false;


        AvailableDirections(selectedIndex);

        totalText.text = (currentPath.Count - 1).ToString() + "/" + maxAmount.ToString();

        main_Camera.target = character.board.GetTilePos(selectedIndex);

    }
}
