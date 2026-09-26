using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI maxComboText;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Start()
    {
        scoreText.text = $"Score:  {GameResult.Score}";
        accuracyText.text = $"Accuracy:  {GameResult.Accuracy:F1}%";
        maxComboText.text = $"Max Combo:  {GameResult.MaxCombo}";

        resultText.text = GameResult.Completed
            ? "COMPLETE"
            : "FAIL";
    }
}