using UnityEngine;

public class VRRigSync : MonoBehaviour
{
    [Header("Sources XR")]
    public Transform xrHead;
    public Transform xrLeftHand;
    public Transform xrRightHand;

    [Header("Tes Targets IK")]
    public Transform targetHead;
    public Transform targetLeftHand;
    public Transform targetRightHand;

    [Header("Réglages Main DROITE")]
    public Vector3 rightHandPosOffset;
    public Vector3 rightHandRotOffset; // Tes valeurs actuelles qui marchent

    [Header("Réglages Main GAUCHE")]
    public Vector3 leftHandPosOffset;
    public Vector3 leftHandRotOffset;  // À régler séparément !

    [Header("Stabilisation")]
    public float smoothness = 10f;

    void LateUpdate()
    {
        if(targetHead != null && xrHead != null)
            MapTransform(targetHead, xrHead, Vector3.zero, Vector3.zero);

        // On utilise les offsets GAUCHE
        if(targetLeftHand != null && xrLeftHand != null)
            MapTransform(targetLeftHand, xrLeftHand, leftHandRotOffset, leftHandPosOffset);

        // On utilise les offsets DROITE
        if(targetRightHand != null && xrRightHand != null)
            MapTransform(targetRightHand, xrRightHand, rightHandRotOffset, rightHandPosOffset);
    }

    void MapTransform(Transform target, Transform source, Vector3 rotOffset, Vector3 posOffset)
    {
        // Calcul de la position cible (Manette + Offset)
        Vector3 targetPos = source.TransformPoint(posOffset);
        Quaternion targetRot = source.rotation * Quaternion.Euler(rotOffset);

        // Au lieu de téléporter, on lisse le mouvement (Damping C3)
        // Time.deltaTime * smoothness permet d'aller vite mais sans à-coups
        target.position = Vector3.Lerp(target.position, targetPos, Time.deltaTime * smoothness);
        target.rotation = Quaternion.Slerp(target.rotation, targetRot, Time.deltaTime * smoothness);
    }
}