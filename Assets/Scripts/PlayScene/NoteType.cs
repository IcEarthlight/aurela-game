public enum NoteType
{
    TAP = 1,
    AIRTAP = 2,
    HOLD = 4,
    AIRHOLD = 8,
    ISGROUND = TAP | HOLD,
    ISAIR = AIRTAP | AIRHOLD,
    ISTAP = TAP | AIRTAP,
    ISHOLD = HOLD | AIRHOLD
}
