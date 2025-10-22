using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_Camera : MonoBehaviour
{
    public Vector3 offset;

    public Camera cam => transform.GetChild(0).GetComponent<Camera>();

    public SpriteRenderer blackOutSprite, whiteOutSprite;

    public float idleSwaySpeed = 3f;
    public float idleSwayDistance = 0.05f;

    private void Start()
    {
        StartCoroutine(IdleSway());
    }

    public void SetIdleSway(float distance)
    {
        idleSwayDistance = distance;
    }

    public void ResetIdleSway()
    {
        idleSwayDistance = 0.05f;
    }

    private IEnumerator IdleSway()
    {
        int direction = 1;

        while (true)
        {

            Vector3 startPosition = cam.transform.localPosition;

            Vector3 newPosition = Vector3.right * idleSwayDistance * direction;

            float lerp = 0.0f;

            float smoothLerp = 0.0f;

            float elapsedTime = 0.0f;

            while (elapsedTime < idleSwaySpeed)
            {

                lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / idleSwaySpeed);
                smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
                cam.transform.localPosition = Vector3.Lerp(startPosition, newPosition, smoothLerp);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            direction *= -1;
        }
    }

    private IEnumerator EaseToCenter()
    {
        Vector3 startPosition = cam.transform.localPosition;
        Vector3 newPosition = Vector3.zero;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;

        float duration = 0.25f;

        while (elapsedTime < duration)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            cam.transform.localPosition = Vector3.Lerp(startPosition, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        cam.transform.localPosition = newPosition;
    }

    public void MoveTo(Vector3 newTarget, float duration) => StartCoroutine(MovingTo(newTarget, duration));

    public IEnumerator MovingTo(Vector3 newTarget, float duration)
    {
        Vector3 startPositon = transform.position;

        Vector3 newPosition = newTarget + offset;

        if (startPositon == newPosition)
            yield break;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            transform.position = Vector3.Lerp(startPositon, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPosition;

        //idle = true;
    }

    public IEnumerator Reset(float delay)
    {

        Vector3 startPositon = transform.position;

        Vector3 newPosition = new Vector3(0, 0.45f, -2.5f);

        if (startPositon == newPosition)
            yield break;

        float duration = 0.5f;

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;


        yield return new WaitForSeconds(delay);

        while (elapsedTime < duration)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            transform.position = Vector3.Lerp(startPositon, newPosition, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPosition;
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

    public void WhiteOut(Combat_Character owner, Combat_Character target, float speed) => StartCoroutine(WhitingOut(owner, target, speed));

    public IEnumerator WhitingOut(Combat_Character owner, Combat_Character target, float speed)
    {
        SpriteRenderer ownerRenderer = owner.transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer targetRenderer = target.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Material ownerMat = ownerRenderer.material;

        ownerRenderer.material = targetRenderer.material;

        Color currentBGColor = Color.white;

        whiteOutSprite.color = Color.white;

        ownerRenderer.color = Color.black;

        targetRenderer.color = Color.black;

        yield return new WaitForSeconds(speed);

        currentBGColor.a = 0;
        whiteOutSprite.color = currentBGColor;

        ownerRenderer.color = Color.white;

        ownerRenderer.material = ownerMat;

        targetRenderer.color = Color.white;
    }

    public void Shake()
    {
        StartCoroutine(Shaking());
    }

    IEnumerator Shaking()
    {
        // This shake makes it hard to see damge numbers, put damage numbers on to screen canvas for better clarity

        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        float duration = 1f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = startPos + Random.insideUnitSphere * 0.2f * (duration - elapsedTime);
            yield return null;
        }

        transform.position = startPos;
    }


    public Vector3 UIPosition(Vector3 objectPos)
    {
        // Use Bellow if Ui elements should stay within Border around screen

        //Vector3 camForward = Character.TurnController.mainCamera.transform.forward;

        //Vector3 camPos = Character.TurnController.mainCamera.transform.position + camForward;

        //float distanceInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);

        //if (distanceInFrontOfCamera < 0f)
        //    targPos -= camForward * distanceInFrontOfCamera;

        return RectTransformUtility.WorldToScreenPoint(cam, objectPos);
    }
}
