using System.Diagnostics;
using UnityEngine;

public struct JudgeStat
{
    private readonly ComboCounter comboCounter;
    public readonly int totalNumOfNotes;
    public int currNumOfNotes;

    public int perfect;
    public int great;
    public int good;
    public int bad;
    public int miss;
    public int early;
    public int late;

    public int holdCatch;
    public int holdUncatch;

    public bool isFullCombo;

    public JudgeStat(
        ComboCounter comboCounter,
        int totalNumOfNotes,
        int currNumOfNotes = 0,
        int perfect = 0,
        int great = 0,
        int good = 0,
        int bad = 0,
        int miss = 0,
        int early = 0,
        int late = 0,
        int holdCatch = 0,
        int holdUncatch = 0,
        bool isFullCombo = true)
    {
        this.comboCounter = comboCounter;
        this.totalNumOfNotes = totalNumOfNotes;
        this.currNumOfNotes = currNumOfNotes;
        this.perfect = perfect;
        this.great = great;
        this.good = good;
        this.bad = bad;
        this.miss = miss;
        this.early = early;
        this.late = late;
        this.holdCatch = holdCatch;
        this.holdUncatch = holdUncatch;
        this.isFullCombo = isFullCombo;
    }

    public void Reset()
    {
        currNumOfNotes = 0;
        perfect = 0;
        great = 0;
        good = 0;
        bad = 0;
        miss = 0;
        early = 0;
        late = 0;
        holdCatch = 0;
        holdUncatch = 0;
        isFullCombo = true;
    }

    public void Stat(NoteController note)
    {
        if (note == null) return;
        if (!note.IsHit) return;

        switch (note.judgment)
        {
            case -4: AddMiss(); break;
            case -3: AddBad(); AddLate(); break;
            case -2: AddGood(); AddLate(); break;
            case -1: AddGreat(); AddLate(); break;
            case 0: AddPerfect(); break;
            case 1: AddGreat(); AddEarly(); break;
            case 2: AddGood(); AddEarly(); break;
            case 3: AddBad(); AddEarly(); break;
        }
    }

    public void Stat(NoteHoldController hold)
    {
        if (hold == null) return;
        if (!hold.ToHold) return;

        switch (hold.judgment)
        {
            case -4: AddMiss(); break;
            case -3: AddBad(); AddLate(); break;
            case -2: AddGood(); AddLate(); break;
            case -1: AddGreat(); AddLate(); break;
            case 0: AddPerfect(); break;
            case 1: AddGreat(); AddEarly(); break;
            case 2: AddGood(); AddEarly(); break;
            case 3: AddBad(); AddEarly(); break;
            case 4: throw new System.Exception("Should not stat note whose judment is 4 (early unhitted).");
        }
    }

    private void AddPerfect() { perfect++; currNumOfNotes++; comboCounter.Combo(); }
    private void AddGreat() { great++; currNumOfNotes++; comboCounter.Combo(); }
    private void AddGood() { good++; currNumOfNotes++; comboCounter.Combo(); }
    private void AddBad() { bad++; currNumOfNotes++; comboCounter.Break(); isFullCombo = false; }
    private void AddMiss() { miss++; currNumOfNotes++; comboCounter.Break(); isFullCombo = false; }
    public void AddCatch() { holdCatch++; comboCounter.Combo(); }
    public void AddUncatch() { holdUncatch++; comboCounter.Break(); isFullCombo = false; }
    public void AddEarly() { early++; }
    public void AddLate() { late++; }

    public readonly float GetAcc()
    {
        if (currNumOfNotes == 0) return 0f;
        float currAcc = (perfect * 1f +
                great * 2f / 3f +
                good * 1f / 3f +
                bad * 0f
        ) / currNumOfNotes;

        float k = Mathf.Clamp01((float)currNumOfNotes / (totalNumOfNotes / 2));
        return k * currAcc;
    }

    public override readonly string ToString()
    {
        return $"JudgeStat:\n" +
               $"  Notes Hit: {currNumOfNotes} / {totalNumOfNotes}\n" +
               $"  Judgments: Perfect={perfect}, Great={great}, Good={good}, Bad={bad}, Miss={miss}\n" +
               $"  Timing Offset: Early={early}, Late={late}\n" +
               $"  Hold: Catch={holdCatch}, Uncatch={holdUncatch}\n" +
               $"  Full Combo: {(isFullCombo ? "Yes" : "No")}\n" +
               $"  Accuracy: {GetAcc():P2}";
    }
}
