using TMPro;
using UnityEngine;

public sealed class FeverController : MonoBehaviour
{
    [SerializeField] private BeatBarController clock;
    [SerializeField] private TextMeshProUGUI feverText;
    [SerializeField] private TextMeshProUGUI leftKeyText;
    [SerializeField, Min(1)] private int hitsNeeded = 4;
    [SerializeField, Min(0.1f)] private float previewBeats = 2f;
    [SerializeField, Min(1f)] private float durationBeats = 4f;

    private const float TimingGrace = 0.225f;

    private int streak;
    private bool scheduled;
    private float startBeat;
    private float endBeat;

    public float AdjustBeat(float chartBeat, NoteSide side)
    {
        if (scheduled &&
            side == NoteSide.Left &&
            chartBeat >= startBeat &&
            chartBeat < endBeat)
        {
            return chartBeat + 0.5f;
        }

        return chartBeat;
    }

    public void RegisterLeftHit()
    {
        if (clock == null) return;

        float now = clock.ElapsedBeats;

        // 等待中的 FEVER 和正在进行的 FEVER 都不累计下一轮。
        if (scheduled && now <= endBeat + TimingGrace) return;

        streak++;

        if (streak >= hitsNeeded)
        {
            streak = 0;

            // 从至少提前两拍的下一小节开始，给音符留下预告时间。
            startBeat =
                Mathf.CeilToInt((now + previewBeats) / 4f) * 4f;
            endBeat = startBeat + durationBeats;
            scheduled = true;
        }

        UpdateLabels();
    }

    public void RegisterLeftMiss()
    {
        streak = 0;
        UpdateLabels();
    }

    private void Update()
    {
        if (clock != null && clock.enabled)
            UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (clock == null) return;

        float now = clock.ElapsedBeats;
        bool active = scheduled &&
                      now >= startBeat &&
                      now <= endBeat + TimingGrace;

        if (feverText != null)
        {
            if (active)
            {
                float remaining = Mathf.Max(0f, endBeat - now);
                feverText.text = $"FEVER {remaining:0.0}  F ON BEAT";
            }
            else if (scheduled && now < startBeat)
            {
                int beatsUntilStart = Mathf.CeilToInt(startBeat - now);
                feverText.text = $"FEVER IN {beatsUntilStart} BEATS";
            }
            else
            {
                feverText.text = $"F STREAK {streak}/{hitsNeeded}";
            }
        }

        if (leftKeyText != null)
        {
            leftKeyText.text = active
                ? "F / ON BEAT\nON BLUE LINE"
                : "F / OFF BEAT\nBETWEEN BLUE LINES";
        }
    }
}