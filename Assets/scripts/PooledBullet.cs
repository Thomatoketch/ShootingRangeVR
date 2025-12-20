using UnityEngine;
using System.Collections;

public class PooledBullet : MonoBehaviour, IPooledObject
{
    public float lifeTime = 5f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnObjectSpawn()
    {
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        StartCoroutine(DeactivateAfterTime());
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        StopAllCoroutines(); 
    }
}