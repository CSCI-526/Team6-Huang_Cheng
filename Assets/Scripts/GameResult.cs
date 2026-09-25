public static class GameResult
{
    public static int Score { get; private set; }
    public static int MaxCombo { get; private set; }
    public static int Perfect { get; private set; }
    public static int Good { get; private set; }
    public static int Miss { get; private set; }
    public static bool Completed { get; private set; }

    public static float Accuracy
    {
        get
        {
            int total = Perfect + Good + Miss;
            if (total == 0) return 0f;

            // Perfect 算 1 次准确命中，Good 算半次。
            return (Perfect + Good * 0.5f) / total * 100f;
        }
    }

    public static void Clear()
    {
        Score = 0;
        MaxCombo = 0;
        Perfect = 0;
        Good = 0;
        Miss = 0;
        Completed = false;
    }

    public static void Save(
        int score,
        int maxCombo,
        int perfect,
        int good,
        int miss,
        bool completed
    )
    {
        Score = score;
        MaxCombo = maxCombo;
        Perfect = perfect;
        Good = good;
        Miss = miss;
        Completed = completed;
    }
}