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

        while(endFrame != true)
        {
            yield return null;

            if (keyFrame)
            {
                yield return null;
                keyFrame = false;
            }
        }

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
        ColorFill(Color.white);

        float timer = 0;
        float maxTime = 2f / 12f; // target frame / frame rate

        while (timer < maxTime)
        {
            float i = Mathf.Lerp(1f, 0f, timer / maxTime);

            ColorFill(new Color(1f,1f,1f,i));

            timer += Time.deltaTime * GetComponent<Animator>().speed;

            yield return null;
        }

        ColorFill(Color.clear);
    }

    public void ColorFill(Color color)
    {
        GetComponent<SpriteRenderer>().material.SetColor("Fill_Color", color);
    }

    public void Opacity(float amount)
    {
        Color col = GetComponent<SpriteRenderer>().color;

        GetComponent<SpriteRenderer>().color = new Color(col.r, col.g, col.b, amount);
    }

    public void Outline(Color color)
    {
        GetComponent<SpriteRenderer>().material.SetColor("Outline_Color", color);
    }

    public IEnumerator OutlineAppear(Color color)
    {
        Outline(color);

        GetComponent<SpriteRenderer>().material.SetFloat("Outline_Thickness", 2);

        float timer = 0;
        float maxTime = 0.2f;

        while (timer <= maxTime)
        {
            float i = Mathf.Lerp(2, 1, timer / maxTime);

            GetComponent<SpriteRenderer>().material.SetFloat("Outline_Thickness", i);

            timer += Time.deltaTime;

            yield return null;
        }

        GetComponent<SpriteRenderer>().material.SetFloat("Outline_Thickness", 1);
    }

    public IEnumerator OutlineDisappear(Color color)
    {
        Outline(color);

        float timer = 0;
        float maxTime = 0.2f;

        while (timer <= maxTime)
        {
            float i = Mathf.Lerp(1, 0, timer / maxTime);

            Outline(new Color(color.r, color.g, color.b ,i));

            timer += Time.deltaTime;

            yield return null;
        }

        Outline(Color.clear);
    }
}
