using UnityEngine;

public class AvatarFollower : MonoBehaviour
{
    [Header("Références")]
    public Transform xrHead;       // Ta Main Camera (Les yeux)
    public Animator animator;      // L'animator du soldat

    [Header("Réglages")]
    public Vector3 offset = new Vector3(0, 0, 0); // Ajustement si les pieds flottent

    private Vector3 previousPos;

    void Start()
    {
        // On initialise la position pour le calcul de vitesse
        if (xrHead != null) transform.position = GetTargetPos();
        previousPos = transform.position;
    }

    void LateUpdate()
    {
        if (xrHead == null) return;

        // 1. Position : SNAP DIRECT (Pas de Lerp, pas de retard)
        // On prend la position X/Z de la tête, mais on garde le Y du soldat (0)
        Vector3 targetPos = xrHead.position;
        targetPos.y = transform.position.y; 
        
        // On force la position brute. Si la caméra a bougé, le soldat est déjà là.
        transform.position = targetPos;

        // 2. Rotation : On garde le lissage ici, c'est joli et ça ne cause pas de lag de position
        Vector3 lookDir = xrHead.forward;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 15f);
        }

        // 3. ANIMATION CORRIGÉE
        // Calcul de la vitesse réelle en mètres par seconde
        float rawSpeed = (transform.position - previousPos).magnitude / Time.deltaTime;

        // IMPORTANT : On ne multiplie plus par 100 !
        // On clamp la valeur entre 0 et 2 (ou la valeur max de ton Blend Tree)
        // Si tu vas à 5m/s, l'animator restera bloqué sur "Run" (2) au lieu d'exploser
        float clampedSpeed = Mathf.Clamp(rawSpeed, 0f, 2f); 

        // On lisse la valeur pour éviter que l'animation ne change trop brusquement
        // "animator.GetFloat" récupère l'ancienne valeur pour faire une moyenne vers la nouvelle
        float smoothSpeed = Mathf.Lerp(animator.GetFloat("Speed"), clampedSpeed, Time.deltaTime * 10f);

        animator.SetFloat("Speed", smoothSpeed);

        previousPos = transform.position;
    }

    Vector3 GetTargetPos()
    {
        // La position cible est la position X/Z de la tête + l'offset
        Vector3 pos = xrHead.position;
        pos.y = transform.position.y; // On force le Y actuel du soldat (le sol)
        return pos + transform.TransformVector(offset);
    }
}