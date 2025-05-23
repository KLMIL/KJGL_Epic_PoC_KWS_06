using System.Collections;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform Target; // 카메라가 따라갈 대상
    public Vector3 Offset = new Vector3(0, 2, -10);
    public float FollowSpeed = 5f;

    [Header("Shake Settings")]
    Vector3 _originalPos;
    Coroutine _shakeCoroutine;


    private void LateUpdate()
    {
        Vector3 targetPos = Target.position + Offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSpeed * Time.deltaTime);
    }

    public void ShakeCamera(float duration = 0.2f, float magnitude = 0.1f)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        _originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = _originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localEulerAngles = _originalPos;
    }
}
