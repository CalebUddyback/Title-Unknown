using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Target_Arrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Combat_Character owner;

    public Color hoverColor = Color.white;
    public Color unHoverColor = new Color(0, 0, 0, 0.5f);

    public TextMeshProUGUI hitChance;

    public enum State { View, Target, React };
    public State state;

    public Transform symbols;

    public Transform view, target, pulse;

    private void Start()
    {
        Shrink();
    }

    private void LateUpdate()
    {
        GetComponent<RectTransform>().anchoredPosition = owner.TurnController.mainCamera.UIPosition(owner.outcome_Bubble_Pos.position);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovering(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(state == State.View)
            StartCoroutine(owner.deck.Raise(true, true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovering(false);
    }

    public IEnumerator SetState(State newState, bool grow)
    {
        //yield return Shrink();

        foreach (Transform s in symbols)
            s.gameObject.SetActive(false);

        state = newState;

        switch (state)
        {
            case State.View:
                view.gameObject.SetActive(true);
                break;

            case State.Target:
                target.gameObject.SetActive(true);
                break;

            case State.React:
                pulse.gameObject.SetActive(true);
                state = State.View;
                goto case State.View;
        }

        if(grow)
            yield return Grow();

        yield return null;
    }

    public void Shrink()
    {
        //float timer = 0;
        //float maxTime = 0.1f;
        //
        //while (timer < maxTime)
        //{
        //    symbols.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timer / maxTime);
        //
        //    timer += Time.deltaTime;
        //
        //    yield return null;
        //}

        symbols.GetComponent<Image>().raycastTarget = false;

        symbols.transform.localScale = Vector3.zero;

    }

    IEnumerator Grow()
    {
        float timer = 0;
        float maxTime = 0.1f;

        while (timer < maxTime)
        {
            symbols.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        symbols.transform.localScale = Vector3.one;

        symbols.GetComponent<Image>().raycastTarget = true;
    }

    public void Hovering(bool x)
    {

        if (x)
            owner.TurnController.CharacterHoveringOver = owner;
        else
            owner.TurnController.CharacterHoveringOver = null;

        Highlight(x);
    }

    public void Highlight(bool x)
    {
        if (x)
        {
            symbols.GetComponent<Image>().color = hoverColor;

            if (state == State.Target)
                hitChance.gameObject.SetActive(true);
        }
        else
        {
            symbols.GetComponent<Image>().color = unHoverColor;

            if (state == State.Target)
                hitChance.gameObject.SetActive(false);
        }

        if (!gameObject.activeSelf)
        {
            owner.animationController.Outline(Color.clear);
            return;
        }

        if (x) 
            owner.animationController.OutlineAppear(Color.white);
        else
            owner.animationController.OutlineDisappear(Color.white);

    }
}
