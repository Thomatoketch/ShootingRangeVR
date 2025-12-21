using UnityEngine;

public class PooledBullet : MonoBehaviour, IPooledObject
{
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
    }
}