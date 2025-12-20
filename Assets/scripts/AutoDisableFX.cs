using UnityEngine;
using System.Collections;

public class AutoDisableFX : MonoBehaviour
{
    public float duration = 1.0f;

    void OnEnable()
    {
        StartCoroutine(DisableRoutine());
    }

    IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject); 
    }
}