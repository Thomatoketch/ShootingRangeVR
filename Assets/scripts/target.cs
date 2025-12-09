using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Target : MonoBehaviour, IPooledObject
{
    [Header("Addressables")]
    public string hitEffectKey = "HitEffectFX"; // Le nom donné dans le groupe Addressable
    private string currentLabel;

    void Start()
    {
        // Détection simple comme dans le pistolet
        #if UNITY_ANDROID
            currentLabel = "Quest"; 
        #else
            currentLabel = "PCVR";
        #endif
    }

    // Cette fonction est appelée automatiquement par le PoolManager quand la cible réapparaît
    public void OnObjectSpawn()
    {
        // Réinitialisation de l'état (utile si tu as des animations ou des PV)
        // Par exemple: GetComponent<Collider>().enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Vector3 hitPosition = collision.contacts[0].point;
            
            TriggerHitEffect(hitPosition);

            // Le script PooledBullet sur la balle s'occupera de stopper sa vélocité au prochain Spawn
            collision.gameObject.SetActive(false);

            // 2. On désactive la cible (elle retourne dans le pool "Target")
            gameObject.SetActive(false);
        }
    }

    private void TriggerHitEffect(Vector3 hitPosition)
    {
        // Chargement dynamique selon la plateforme
        Addressables.LoadAssetsAsync<GameObject>(new [] { hitEffectKey, currentLabel }, 
            (obj) => {
                SpawnFX(obj, hitPosition);
            }, Addressables.MergeMode.Intersection);
    }

    private void SpawnFX(GameObject fxPrefab, Vector3 position)
    {
        if (fxPrefab == null) return;

        GameObject hitInstance = Instantiate(fxPrefab, position, Quaternion.identity);
        
        ParticleSystem ps = hitInstance.GetComponent<ParticleSystem>();
        float duration = (ps != null) ? ps.main.duration : 1.0f;
        Destroy(hitInstance, duration + 0.1f); 
    }
}