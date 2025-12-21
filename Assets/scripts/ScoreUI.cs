using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreUI : MonoBehaviour
{
    [Header("Réglages")]
    public TextMeshProUGUI scoreText;
    public float refreshRate = 0.2f;

    void Start()
    {
        if (scoreText == null) Debug.LogError("ScoreUI : Le champ TextMeshPro n'est pas assigné !");
        StartCoroutine(UpdateScoreRoutine());
    }

    IEnumerator UpdateScoreRoutine()
    {
        while (true)
        {
            if (GameplayManager.Instance != null && scoreText != null)
            {
                int score = GameplayManager.Instance.GetScore();
                scoreText.text = "Score : " + score;
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}