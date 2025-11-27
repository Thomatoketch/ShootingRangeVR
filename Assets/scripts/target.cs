using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Références aux Particules (Impact)")]
    // Références pour assigner les Prefabs optimisés dans l'Inspector
    public GameObject hitEffectPrefabPCVR;
    public GameObject hitEffectPrefabQuest;

    // Pour la détection de la plateforme
    private bool isQuestBuild = false;

    void Start()
    {
        // Détection de la plateforme de compilation : Android (Meta Quest) est la cible mobile.
        #if UNITY_ANDROID
        isQuestBuild = true;
        #endif
    }

    void OnCollisionEnter(Collision collision)
    {
        // Vérifie que l'objet entrant en collision est bien une balle
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Récupère le point d'impact précis
            Vector3 hitPosition = collision.contacts[0].point;
            
            TriggerHitEffect(hitPosition);

            // Nettoyage : Détruit la balle immédiatement après l'impact pour éviter 
            // toute nouvelle collision ou d'autres problèmes de physique.
            Destroy(collision.gameObject); 
        }
    }

    private void TriggerHitEffect(Vector3 hitPosition)
    {
        // 1. Choisir le préfab d'effet approprié pour la plateforme
        GameObject selectedHitEffect = isQuestBuild ? hitEffectPrefabQuest : hitEffectPrefabPCVR;

        if (selectedHitEffect != null)
        {
            // 2. Instancier l'effet à la position exacte de l'impact
            GameObject hitInstance = Instantiate(selectedHitEffect, hitPosition, Quaternion.identity);

            // 3. Nettoyage : Détruire l'instance une fois l'effet terminé
            ParticleSystem ps = hitInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                // Détruire l'objet après la durée de vie du système de particules + un petit délai
                Destroy(hitInstance, ps.main.duration + 0.1f);
            }
            else
            {
                 // Si l'objet n'a pas de ParticleSystem, le détruire après un court instant par défaut
                 Destroy(hitInstance, 1.0f);
            }
        }
    }
}