using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class RhythmJudge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BeatBarController clock;
    [SerializeField] private FeverController fever;
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Image energyFill;
    [SerializeField] private Image rightIndicator;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField, Min(1f)] private float roundLengthSeconds = 45f;

    private const float PerfectWindow = 0.12f;
    private const float GoodWindow = 0.225f;

    private int energy = 100;
    private int score;
    private int combo;
    private int leftIndex;
    private int rightIndex;

    private bool gameOver;
    private bool holdingRight;
    private bool holdPerfect;
    private float holdEndBeat;
    private Color normalRightColor;

    private int maxCombo;
    private int perfectCount;
    private int goodCount;
    private int missCount;
    private bool completed;

    private void Start()
    {
        if (rightIndicator != null)
            normalRightColor = rightIndicator.color;

        leftIndex = FindNext(0, NoteSide.Left);
        rightIndex = FindNext(0, NoteSide.Right);

        UpdateEnergyUI();
        UpdateScoreUI();
        UpdateTimerUI();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (gameOver)
        {
            if (keyboard != null &&
                keyboard.rKey.wasPressedThisFrame)
            {
                SceneManager.LoadScene("Score");
            }

            return;
        }

        if (clock == null || feedbackText == null) return;

        UpdateTimerUI();

        if (clock.ElapsedSeconds >= roundLengthSeconds)
        {
            completed = true;
            EndRound("LEVEL COMPLETE - PRESS R");
            return;
        }

        float now = clock.ElapsedBeats;

        CheckMisses(now);
        if (gameOver || keyboard == null) return;

        if (keyboard.fKey.wasPressedThisFrame)
            Press(NoteSide.Left, ref leftIndex, "F", now);

        if (keyboard.jKey.wasPressedThisFrame &&
            !holdingRight)
        {
            Press(NoteSide.Right, ref rightIndex, "J", now);
        }

        UpdateHold(now, keyboard);
    }

    private void Press(
        NoteSide side,
        ref int index,
        string key,
        float now
    )
    {
        if (!InRound(index))
        {
            feedbackText.text = $"{key}: NO NOTE";
            return;
        }

        RhythmNote note = NoteAt(index);
        float targetBeat = EffectiveBeatAt(index);
        float error = Mathf.Abs(now - targetBeat);

        if (error > GoodWindow)
        {
            feedbackText.text = $"{key}: MISS";
            return;
        }

        bool perfect = error <= PerfectWindow;

        if (note.HoldBeats > 0f)
        {
            holdingRight = true;
            SetHoldIndicator(true);

            holdPerfect = perfect;
            holdEndBeat = targetBeat + note.HoldBeats;
            feedbackText.text = "J: HOLD";
            return;
        }

        Award(key, perfect, false);
        index = FindNext(index + 1, side);
    }

    private void UpdateHold(float now, Keyboard keyboard)
    {
        if (!holdingRight) return;

        if (now >= holdEndBeat && keyboard.jKey.isPressed)
        {
            CompleteHold();
        }
        else if (keyboard.jKey.wasReleasedThisFrame)
        {
            if (now >= holdEndBeat - GoodWindow)
                CompleteHold();
            else
                FailHold();
        }
    }

    private void CompleteHold()
    {
        holdingRight = false;
        SetHoldIndicator(false);

        Award("J HOLD", holdPerfect, true);
        rightIndex =
            FindNext(rightIndex + 1, NoteSide.Right);
    }

    private void FailHold()
    {
        holdingRight = false;
        SetHoldIndicator(false);

        LoseEnergy("J HOLD");
        rightIndex =
            FindNext(rightIndex + 1, NoteSide.Right);
    }

    private void CheckMisses(float now)
    {
        while (InRound(leftIndex) &&
               now > EffectiveBeatAt(leftIndex) + GoodWindow)
        {
            LoseEnergy("F");
            leftIndex =
                FindNext(leftIndex + 1, NoteSide.Left);

            if (gameOver) return;
        }

        if (holdingRight) return;

        while (InRound(rightIndex) &&
               now > EffectiveBeatAt(rightIndex) + GoodWindow)
        {
            LoseEnergy("J");
            rightIndex =
                FindNext(rightIndex + 1, NoteSide.Right);

            if (gameOver) return;
        }
    }

    private void Award(string key, bool perfect, bool hold)
    {
        score += hold
            ? (perfect ? 200 : 100)
            : (perfect ? 100 : 50);

        combo++;
        maxCombo = Mathf.Max(maxCombo, combo);

        if (perfect)
            perfectCount++;
        else
            goodCount++;

        if (key == "F" && fever != null)
            fever.RegisterLeftHit();

        feedbackText.text =
            $"{key}: {(perfect ? "PERFECT" : "GOOD")}";

        UpdateScoreUI();
    }

    private void LoseEnergy(string key)
    {
        energy = Mathf.Max(0, energy - 10);
        combo = 0;
        missCount++;

        if (key == "F" && fever != null)
            fever.RegisterLeftMiss();

        feedbackText.text = $"{key}: MISS";

        UpdateEnergyUI();
        UpdateScoreUI();

        if (energy == 0)
        {
            completed = false;
            EndRound("GAME OVER - PRESS R");
        }
    }

    private void EndRound(string message)
    {
        SetHoldIndicator(false);
        UpdateTimerUI();

        gameOver = true;
        feedbackText.text = message;
        clock.enabled = false;

        GameResult.Save(
            score,
            maxCombo,
            perfectCount,
            goodCount,
            missCount,
            completed
        );
    }

    private void SetHoldIndicator(bool holding)
    {
        if (rightIndicator == null) return;

        rightIndicator.color = holding
            ? new Color(0.35f, 0.95f, 1f, 1f)
            : normalRightColor;
    }

    private static int FindNext(int from, NoteSide side)
    {
        int length = RhythmChart.Phrase.Length;

        for (int i = from; ; i++)
        {
            if (RhythmChart.Phrase[i % length].Side == side)
                return i;
        }
    }

    private static RhythmNote NoteAt(int index)
    {
        return RhythmChart.Phrase[
            index % RhythmChart.Phrase.Length
        ];
    }

    private static float BeatAt(int index)
    {
        int length = RhythmChart.Phrase.Length;
        int repeat = index / length;

        return repeat * RhythmChart.PhraseBeats +
               NoteAt(index).Beat;
    }

    private float EffectiveBeatAt(int index)
    {
        float beat = BeatAt(index);

        return fever == null
            ? beat
            : fever.AdjustBeat(
                beat,
                NoteAt(index).Side
            );
    }

    private bool InRound(int index)
    {
        float lastBeat =
            roundLengthSeconds *
            clock.BeatsPerMinute / 60f;

        return EffectiveBeatAt(index) < lastBeat;
    }

    private void UpdateEnergyUI()
    {
        if (energyFill != null)
        {
            energyFill.rectTransform.anchorMax =
                new Vector2(1f, energy / 100f);
        }

        if (energyText != null)
            energyText.text = $"Energy: {energy}";
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";

        if (comboText != null)
            comboText.text = $"Combo: {combo}";
    }

    private void UpdateTimerUI()
    {
        if (timeText == null || clock == null) return;

        float remaining =
            Mathf.Max(0f, roundLengthSeconds - clock.ElapsedSeconds);

        int seconds = Mathf.CeilToInt(remaining);
        int minutesPart = seconds / 60;
        int secondsPart = seconds % 60;

        timeText.text =
            $"TIME {minutesPart}:{secondsPart:00}";
    }
}