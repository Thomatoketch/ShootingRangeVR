using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Game State")]
    private int currentScore = 0;

    void Awake()
    {
        // Initialisation du Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Méthode pour ajouter des points
    public void AddScore(int amount)
    {
        currentScore += amount;
        // Optionnel : Debug.Log("Nouveau Score : " + currentScore);
    }

    // Méthode pour récupérer le score actuel (pour l'UI)
    public int GetScore()
    {
        return currentScore;
    }
}