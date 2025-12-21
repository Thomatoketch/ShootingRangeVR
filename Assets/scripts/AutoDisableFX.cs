using UnityEngine;
using System.Collections;

public class AutoDisableFX : MonoBehaviour, IPooledObject
{
    public float duration = 1.0f;

    public void OnObjectSpawn()
    {
        StartCoroutine(DisableRoutine());
    }

    void OnEnable()
    {
        StartCoroutine(DisableRoutine());
    }

    IEnumerator DisableRoutine()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}