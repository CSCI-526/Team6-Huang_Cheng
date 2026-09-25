using TMPro;
using UnityEngine;

public sealed class BeatBarController : MonoBehaviour
{
    [SerializeField, Min(1f)] private float beatsPerMinute = 90f;
    [SerializeField] private RectTransform beatCursor;
    [SerializeField] private TextMeshProUGUI beatText;

    private float startTime;
    public float ElapsedSeconds => Time.unscaledTime - startTime;
    public float ElapsedBeats =>
    (Time.unscaledTime - startTime) * beatsPerMinute / 60f;
    public float BeatsPerMinute => beatsPerMinute;
    private void OnEnable()
    {
        startTime = Time.unscaledTime;
    }

    private void Update()
    {
        float beatLength = 60f / beatsPerMinute;
       float elapsedBeats = ElapsedBeats;
        float beatInMeasure = Mathf.Repeat(elapsedBeats, 4f);

        if (beatCursor != null)
        {
            float halfHeight = ((RectTransform)transform).rect.height / 2f;
            float y = Mathf.Lerp(
                -halfHeight + 5f,
                halfHeight - 5f,
                beatInMeasure / 4f
            );
            beatCursor.anchoredPosition = new Vector2(0f, y);
        }

        if (beatText != null)
            beatText.text = $"BEAT {Mathf.FloorToInt(beatInMeasure) + 1}/4";
    }
}