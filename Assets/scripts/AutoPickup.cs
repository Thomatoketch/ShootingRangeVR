using UnityEngine;
using System.Collections;

public class AutoPickup : MonoBehaviour
{
    [Header("Réglages")]
    public float delayBeforeAttach = 0.5f; // Temps (en sec) pour que la main atteigne l'arme

    private bool hasBeenPickedUp = false;

    // Cette fonction se lance toute seule quand on marche dans le Trigger
    void OnTriggerEnter(Collider other)
    {
        // 1. Vérification : On ne ramasse qu'une fois, et seulement si c'est le Player
        if (hasBeenPickedUp) return;
        
        // On vérifie si l'objet qui nous touche est bien le soldat (par son tag ou son nom)
        if (other.CompareTag("Player")) 
        {
            hasBeenPickedUp = true;
            
            // 2. Récupérer les composants du soldat
            // On cherche l'Animator et le point d'attache (WeaponSocket)
            Animator playerAnim = other.GetComponentInChildren<Animator>();
            
            // Astuce : On cherche le socket dans les enfants de la main droite
            // Assure-toi que ton objet vide dans la main s'appelle bien "WeaponSocket"
            Transform socket = other.transform.FindDeepChild("WeaponSocket"); 

            if (playerAnim != null && socket != null)
            {
                // 3. Lancer l'animation
                playerAnim.SetTrigger("Equip");      // 
                playerAnim.SetBool("IsEquipped", true); // [cite: 37]

                // 4. Attacher l'arme (avec un petit délai pour faire joli)
                StartCoroutine(AttachWeaponRoutine(socket));
            }
            else
            {
                Debug.LogError("Erreur : Impossible de trouver l'Animator ou le WeaponSocket sur le joueur !");
            }
        }
    }

    IEnumerator AttachWeaponRoutine(Transform socket)
{
    // 1. Attendre le bon moment de l'animation
    yield return new WaitForSeconds(delayBeforeAttach);

    // 2. COUPER LA PHYSIQUE (C'est ça qui manque !)
    Rigidbody rb = GetComponent<Rigidbody>();
    Collider col = GetComponent<Collider>();

    if (rb != null)
    {
        rb.isKinematic = true; // L'arme ne subit plus la gravité ni les chocs
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero; // On stoppe tout mouvement résiduel
    }

    if (col != null)
    {
        col.enabled = false; // On désactive le collider pour qu'il ne tape pas le joueur
    }

    // 3. Attacher l'arme
    transform.SetParent(socket);
    
    // 4. Reset position (pour qu'elle soit bien calée)
    transform.localPosition = Vector3.zero;
    transform.localRotation = Quaternion.identity;
}
}

// Petite extension pour trouver le Socket même s'il est caché loin dans la hiérarchie
public static class TransformDeepChildExtension
{
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        foreach(Transform child in aParent)
        {
            if(child.name == aName) return child;
            var result = child.FindDeepChild(aName);
            if (result != null) return result;
        }
        return null;
    }
}