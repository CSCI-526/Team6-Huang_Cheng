using System.Collections.Generic;
using UnityEngine;
using TMPro;

public sealed class ChartNoteSpawner : MonoBehaviour
{
    [SerializeField] private BeatBarController clock;
    [SerializeField] private FeverController fever;
    [SerializeField] private RectTransform leftNoteTemplate;
    [SerializeField] private RectTransform rightNoteTemplate;
    [SerializeField] private RectTransform leftTarget;
    [SerializeField] private RectTransform rightTarget;
    [SerializeField, Min(0.1f)] private float travelBeats = 2f;
    [SerializeField, Min(1f)] private float roundLengthSeconds = 45f;

    private sealed class VisibleNote
    {
        public RectTransform Rect;
        public RectTransform Label;
        public float Beat;
        public float Hold;
        public float StartY;
        public float TargetY;
        public float BaseHeight;
        public float PixelsPerBeat;
    }

    private readonly List<VisibleNote> visible = new List<VisibleNote>();
    private int nextNoteIndex;

    private void Awake()
    {
        if (leftNoteTemplate != null)
            leftNoteTemplate.gameObject.SetActive(false);

        if (rightNoteTemplate != null)
            rightNoteTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (clock == null || !clock.enabled) return;

        float now = clock.ElapsedBeats;
        RhythmNote[] phrase = RhythmChart.Phrase;
        float roundEndBeat =
            roundLengthSeconds * clock.BeatsPerMinute / 60f;

        while (true)
        {
            int repeat = nextNoteIndex / phrase.Length;
            RhythmNote entry = phrase[nextNoteIndex % phrase.Length];

            float beat =
                repeat * RhythmChart.PhraseBeats + entry.Beat;

            if (fever != null)
                beat = fever.AdjustBeat(beat, entry.Side);

            if (beat >= roundEndBeat) break;
            if (now < beat - travelBeats) break;

            Spawn(entry, beat);
            nextNoteIndex++;
        }

        for (int i = visible.Count - 1; i >= 0; i--)
        {
            VisibleNote note = visible[i];

            if (now > note.Beat + note.Hold + 0.225f)
            {
                Destroy(note.Rect.gameObject);
                visible.RemoveAt(i);
                continue;
            }

            float progress = Mathf.Clamp01(
                (now - (note.Beat - travelBeats)) / travelBeats
            );

            float headY =
                Mathf.Lerp(note.StartY, note.TargetY, progress);

            float remainingHold = Mathf.Clamp(
                note.Beat + note.Hold - now,
                0f,
                note.Hold
            );

            float height =
                note.BaseHeight +
                remainingHold * note.PixelsPerBeat;

            note.Rect.sizeDelta =
                new Vector2(note.Rect.sizeDelta.x, height);

            note.Rect.anchoredPosition = new Vector2(
                note.Rect.anchoredPosition.x,
                headY + (height - note.BaseHeight) / 2f
            );

            if (note.Label != null)
            {
                note.Label.anchoredPosition =
                    new Vector2(
                        0f,
                        -(height - note.BaseHeight) / 2f
                    );
            }
        }
    }

    private void Spawn(RhythmNote entry, float beat)
    {
        bool left = entry.Side == NoteSide.Left;

        RectTransform template =
            left ? leftNoteTemplate : rightNoteTemplate;

        RectTransform target =
            left ? leftTarget : rightTarget;

        if (template == null || target == null) return;

        RectTransform copy =
            Instantiate(template, template.parent);

        FallingNotePreview oldPreview =
            copy.GetComponent<FallingNotePreview>();

        if (oldPreview != null)
            oldPreview.enabled = false;

        copy.gameObject.SetActive(true);
        if (entry.HoldBeats > 0f)
{
    TextMeshProUGUI label =
        copy.Find("KeyLabel")?.GetComponent<TextMeshProUGUI>();

    if (label != null)
    {
        label.text = "HOLD";
        label.fontSize = 24;
    }
}

        visible.Add(new VisibleNote
        {
            Rect = copy,
            Label = copy.Find("KeyLabel") as RectTransform,
            Beat = beat,
            Hold = entry.HoldBeats,
            StartY = template.anchoredPosition.y,
            TargetY = target.anchoredPosition.y,
            BaseHeight = template.rect.height,
            PixelsPerBeat =
                (template.anchoredPosition.y -
                 target.anchoredPosition.y) / travelBeats
        });
    }
}