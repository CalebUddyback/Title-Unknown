using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;

    public float smoothTime = .5f;

    private Vector3 velocity;

    private Camera cam;

    public bool fixedTime = false;

    public SpriteRenderer blackOutSprite;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (!fixedTime)
        {
            if (target != null)
            {
                Vector3 newPosition = target.position + offset;
                transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
            }
        }
    }

    public IEnumerator LerpMove(Transform newTarget, float speed )
    {
        target = newTarget;

        fixedTime = true;

        Vector3 startPositon = transform.position;

        Vector3 newPosition = target.position + offset;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;

        while (lerp < 1.0f && speed > 0.0f)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / speed);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            transform.position = Vector3.Lerp(startPositon, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPosition;

        fixedTime = false;
    }

    public void LerpMoveVoid(Vector3 newTarget, float speed) => StartCoroutine(LerpMoveIE(newTarget, speed));

    public IEnumerator LerpMoveIE(Vector3 newTarget, float speed)
    {
        fixedTime = true;

        Vector3 startPositon = transform.position;

        Vector3 newPosition = newTarget + offset;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;

        while (lerp < 1.0f && speed > 0.0f)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / speed);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            transform.position = Vector3.Lerp(startPositon, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPosition;

        fixedTime = false;
    }

    public IEnumerator Reset(float delay)
    {
        fixedTime = true;

        Vector3 startPositon = transform.position;

        Vector3 newPosition = new Vector3(0, 0.55f, -2.5f);

        float speed = 0.5f;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;


        yield return new WaitForSeconds(delay);

        while (lerp < 1.0f && speed > 0.0f)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / speed);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            transform.position = Vector3.Lerp(startPositon, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPosition;

        fixedTime = false;
    }

    public void BlackOut(float targetAlpha, float speed) => StartCoroutine(BlackingOut(targetAlpha, speed));

    public IEnumerator BlackingOut(float targetAlpha, float speed)
    {
        Color currentColor = blackOutSprite.color;

        float elapsedTime = 0.0f;

        while (elapsedTime < speed)
        {

            currentColor.a = Mathf.Lerp(currentColor.a, targetAlpha, elapsedTime / speed);

            blackOutSprite.color = currentColor;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        currentColor.a = targetAlpha;
        blackOutSprite.color = currentColor;
    }
}
