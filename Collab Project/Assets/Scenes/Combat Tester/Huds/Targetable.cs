using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Targetable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public Turn_Controller Turn_Controller;

    public Transform trackingPoint;

    public GameObject selectionArrow;

    // Start is called before the first frame update

    private void Start()
    {
        Turn_Controller = GameObject.Find("Fight Controller").GetComponent<Turn_Controller>();

        selectionArrow = Instantiate(selectionArrow, Turn_Controller.target_Arrows.transform);
    }

    private void OnEnable()
    {
        selectionArrow.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targPos = trackingPoint.position;

        Vector3 camForward = Turn_Controller.mainCamera.transform.forward;

        Vector3 camPos = Turn_Controller.mainCamera.transform.position + camForward;

        float distanceInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);

        if (distanceInFrontOfCamera < 0f)
            targPos -= camForward * distanceInFrontOfCamera;

        selectionArrow.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(Turn_Controller.mainCamera.cam, targPos);
    }

    private void OnDisable()
    {
        selectionArrow.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Turn_Controller.target_Manager.Hovering = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Turn_Controller.target_Manager.Hovering = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //TurnController.characterTurn.chosenAction.chosen_Targets.Add(transform);

    }
}
