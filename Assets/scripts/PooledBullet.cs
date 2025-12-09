using UnityEngine;

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
        // Reset complet de la physique obligatoire
        if(rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // Lance le timer de désactivation
        Invoke("DisableObject", lifeTime);
    }

    void DisableObject()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke(); // Sécurité
    }
    
    // Si tu touches une cible, le script Target s'occupera de désactiver la balle
    // Mais il faut remplacer Destroy(collision.gameObject) par collision.gameObject.SetActive(false) dans Target.cs
}