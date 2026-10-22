using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationController : MonoBehaviour
{
    public Coroutine coroutine;

    public bool keyFrame = false;
    public bool endFrame = false;

    public Transform instatiatePoint;

    public void Clip(string clipName, float playSpeed)
    {
        coroutine = StartCoroutine(Playing(clipName));
        PlaySpeed(playSpeed);
    }

    public void PlaySpeed(float s)
    {
        GetComponent<Animator>().speed = s;
    }

    IEnumerator Playing(string clipName)
    {
        GetComponent<Animator>().Play(clipName, -1, 0);

        yield return null;

        yield return new WaitUntil(() => endFrame == true);

        yield return null;

        endFrame = false;
    }

    public void KeyFrame()
    {
        keyFrame = true;
    }

    public void EndFrame()
    {
        endFrame = true;
    }

    public IEnumerator Flash()
    {
        ColorFill(1, Color.white);

        float timer = 0;
        float maxTime = 2f / 12f; // target frame / frame rate

        while (timer < maxTime)
        {
            float i = Mathf.Lerp(1f, 0f, timer / maxTime);

            ColorFill(i, Color.white);

            timer += Time.deltaTime * GetComponent<Animator>().speed;

            yield return null;
        }

        ColorFill(0, Color.white);
    }

    public void ColorFill(float amount, Color color)
    {
        GetComponent<SpriteRenderer>().material.SetFloat("Fill_Amount", amount);
        GetComponent<SpriteRenderer>().material.SetColor("Fill_Color", color);
    }

    public void Opacity(float amount)
    {
        Color col = GetComponent<SpriteRenderer>().color;

        GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, amount);
    }

    public void Outline(float opacity, Color color)
    {
        GetComponent<SpriteRenderer>().material.SetFloat("Outline_Opacity", opacity);
        GetComponent<SpriteRenderer>().material.SetColor("Outline_Color", color);
    }
}
