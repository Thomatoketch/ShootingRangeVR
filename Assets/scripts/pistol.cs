using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    [Header("Références au Gameplay")]
    public Transform firePoint; // L'objet vide pour déterminer la sortie du canon
    public GameObject bullet; // Votre préfabriqué de projectile
    public float bulletSpeed = 20f;

    [Header("Références aux Particules (Muzzle Flash)")]
    public GameObject muzzleFlashPrefabPCVR; // Votre préfabriqué d'effet de tir (version riche)
    public GameObject muzzleFlashPrefabQuest; // Votre préfabriqué d'effet de tir (version optimisée)
    
    // Pour la détection de la plateforme (simplifié pour l'exemple)
    private bool isQuestBuild = false;

    void Start()
    {
        // Une manière simple et efficace de détecter si on est sur une build Android (typiquement pour Quest)
        // Cette portion de code ne sera incluse que si la plateforme de build est Android.
        #if UNITY_ANDROID
        isQuestBuild = true;
        #endif
    }

    public void FireBullet()
    {
        //--- Instantiate a bullet and fire it forward
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.tag = "Bullet"; // S'assurer que le tag est bien "bullet"
        spawnedBullet.transform.position = firePoint.position;
        spawnedBullet.GetComponent<Rigidbody>().AddForce(firePoint.forward * bulletSpeed, ForceMode.Impulse);
        
        TriggerMuzzleFlash();

        //--- Destroy the bullet after 5 seconds
        Destroy(spawnedBullet, 5);
    }

    private void TriggerMuzzleFlash()
    {
        // Choisir la bonne version du prefab selon la plateforme ciblée pour le build
        GameObject selectedMuzzleFlash = isQuestBuild ? muzzleFlashPrefabQuest : muzzleFlashPrefabPCVR;

        if (selectedMuzzleFlash != null)
        {
            // Instancier l'effet à la position du FirePoint
            // L'effet de particules est un objet temporaire
            GameObject flashInstance = Instantiate(selectedMuzzleFlash, firePoint.position, firePoint.rotation);

            // Optionnel : s'assurer qu'il démarre et est nettoyé
            ParticleSystem ps = flashInstance.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                
                // Détruire l'objet une fois que l'effet a terminé pour libérer la mémoire.
                // Utilisez la durée de vie maximale de l'effet de particules + un petit délai.
                Destroy(flashInstance, ps.main.duration + 0.1f);
            }
            else
            {
                // Si ce n'est pas un système de particules, on peut juste le détruire après un court instant.
                 Destroy(flashInstance, 0.5f);
            }
        }
    }
}