using UnityEngine;

public class Target : MonoBehaviour, IPooledObject
{
    public void OnObjectSpawn()
    {
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision cible avec : {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            
            ObjectPoolManager.Instance.SpawnFromPool("HitEffect", hitPosition, Quaternion.identity);

            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.AddScore(1); 
                Debug.Log("Score ajouté !");
            }
            else
            {
                Debug.LogError("GameplayManager introuvable !");
            }

            if (TargetSpawner.Instance != null)
            {
                TargetSpawner.Instance.RegisterTargetDespawn();
            }

            collision.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}