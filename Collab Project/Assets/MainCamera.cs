using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamera : MonoBehaviour
{
    public Transform target;    // ONLY USED IN BOARD GAME

    public Vector3 offset;

    public float smoothTime = 0.5f;

    private Vector3 velocity;

    private Camera cam => GetComponent<Camera>();

    public bool fixedTime = false;


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

    /**THIS CODE IS USED ONLY IN BOARD GAME**/

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
}
