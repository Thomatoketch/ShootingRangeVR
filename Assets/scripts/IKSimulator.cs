using UnityEngine;

public class IKSimulator : MonoBehaviour {
    public Transform cameraRef; // Ta caméra FPS
    public Transform t_Head, t_RightHand, t_LeftHand;
    
    // Position de l'arme par rapport à la caméra
    public Vector3 weaponOffset = new Vector3(0.2f, -0.3f, 0.5f); 
    public float damping = 10f; // Vitesse de lissage

    void LateUpdate() {
        float dt = Time.deltaTime;

        // 1. La Tête suit la caméra (Lerp pour le damping) [cite: 67]
        t_Head.position = Vector3.Lerp(t_Head.position, cameraRef.position, dt * damping);
        t_Head.rotation = Quaternion.Slerp(t_Head.rotation, cameraRef.rotation, dt * damping);

        // 2. La Main Droite suit une position virtuelle devant la caméra
        Vector3 targetHandPos = cameraRef.TransformPoint(weaponOffset);
        t_RightHand.position = Vector3.Lerp(t_RightHand.position, targetHandPos, dt * damping);
        t_RightHand.rotation = Quaternion.Slerp(t_RightHand.rotation, cameraRef.rotation, dt * damping);

        // 3. La Main Gauche suit la droite (ou reste fixe par rapport à l'arme)
        // ... (Logique similaire selon ton arme)
    }
}