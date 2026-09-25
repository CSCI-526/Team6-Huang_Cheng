using UnityEngine;

public sealed class FallingNotePreview : MonoBehaviour
{
    [SerializeField] private BeatBarController clock;
    [SerializeField] private RectTransform target;
    [SerializeField, Min(0f)] private float startOffsetBeats;

    private RectTransform note;
    private float startY;

    private void Awake()
    {
        note = (RectTransform)transform;
        startY = note.anchoredPosition.y;
    }

    private void Update()
{
    if (clock == null || target == null) return;

    float elapsed = clock.ElapsedBeats - startOffsetBeats;

    if (elapsed < 0f)
    {
        note.localScale = Vector3.zero;
        return;
    }

    note.localScale = Vector3.one;

    float progress = Mathf.Repeat(elapsed, 2f) / 2f;
    float y = Mathf.Lerp(startY, target.anchoredPosition.y, progress);
    note.anchoredPosition = new Vector2(target.anchoredPosition.x, y);
}
}