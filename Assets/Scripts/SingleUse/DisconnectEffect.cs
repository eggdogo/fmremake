using System.Collections;
using UnityEngine;

public class DisconnectEffect : MonoBehaviour //bye bye time
{
    public float launchSpeed = 1f;
    public float spinSpeed = 360f;
    public float lifetime = 3f;

    public void StartLaunch()
    {
        StartCoroutine(LaunchRoutine());
    }

    IEnumerator LaunchRoutine()
    {
        float timer = 0f;

        while (timer < lifetime)
        {
            transform.position += Vector3.up * launchSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}