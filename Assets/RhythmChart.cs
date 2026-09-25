public enum NoteSide
{
    Left,   // F：反拍
    Right   // J：正拍
}

public struct RhythmNote
{
    public float Beat;
    public NoteSide Side;
    public float HoldBeats; // 0 表示短按

    public RhythmNote(float beat, NoteSide side, float holdBeats = 0f)
    {
        Beat = beat;
        Side = side;
        HoldBeats = holdBeats;
    }
}

public static class RhythmChart
{
    public const float PhraseBeats = 64f;

    public static readonly RhythmNote[] Phrase =
    {
        new RhythmNote(2f,    NoteSide.Right),
        new RhythmNote(2.5f,  NoteSide.Left),
        new RhythmNote(3.5f,  NoteSide.Left),
        new RhythmNote(5f,    NoteSide.Right, 1f),
        new RhythmNote(5.5f,  NoteSide.Left),
        new RhythmNote(7f,    NoteSide.Right),
        new RhythmNote(8.5f,  NoteSide.Left),
        new RhythmNote(9f,    NoteSide.Right),
        new RhythmNote(10f,   NoteSide.Right),
        new RhythmNote(11.5f, NoteSide.Left),
        new RhythmNote(13f,   NoteSide.Right, 1.5f),
        new RhythmNote(13.5f, NoteSide.Left),
        new RhythmNote(15.5f, NoteSide.Left),

        new RhythmNote(17f,   NoteSide.Right),
        new RhythmNote(18.5f, NoteSide.Left),
        new RhythmNote(20f,   NoteSide.Right, 1f),
        new RhythmNote(20.5f, NoteSide.Left),
        new RhythmNote(22f,   NoteSide.Right),
        new RhythmNote(23.5f, NoteSide.Left),
        new RhythmNote(25f,   NoteSide.Right),
        new RhythmNote(26f,   NoteSide.Right),
        new RhythmNote(27.5f, NoteSide.Left),
        new RhythmNote(29f,   NoteSide.Right, 1f),
        new RhythmNote(29.5f, NoteSide.Left),
        new RhythmNote(31f,   NoteSide.Right),

        new RhythmNote(33f,   NoteSide.Right),
        new RhythmNote(34.5f, NoteSide.Left),
        new RhythmNote(35.5f, NoteSide.Left),
        new RhythmNote(36f,   NoteSide.Right),
        new RhythmNote(37f,   NoteSide.Right, 1f),
        new RhythmNote(37.5f, NoteSide.Left),
        new RhythmNote(39.5f, NoteSide.Left),
        new RhythmNote(41f,   NoteSide.Right),
        new RhythmNote(42.5f, NoteSide.Left),
        new RhythmNote(44f,   NoteSide.Right, 1.5f),
        new RhythmNote(44.5f, NoteSide.Left),
        new RhythmNote(46f,   NoteSide.Right),
        new RhythmNote(47.5f, NoteSide.Left),

        new RhythmNote(49f,   NoteSide.Right),
        new RhythmNote(49.5f, NoteSide.Left),
        new RhythmNote(51f,   NoteSide.Right, 1f),
        new RhythmNote(51.5f, NoteSide.Left),
        new RhythmNote(53.5f, NoteSide.Left),
        new RhythmNote(55f,   NoteSide.Right),
        new RhythmNote(56f,   NoteSide.Right),
        new RhythmNote(57.5f, NoteSide.Left),
        new RhythmNote(59f,   NoteSide.Right, 1f),
        new RhythmNote(59.5f, NoteSide.Left),
        new RhythmNote(61f,   NoteSide.Right),
        new RhythmNote(62.5f, NoteSide.Left)
    };
}