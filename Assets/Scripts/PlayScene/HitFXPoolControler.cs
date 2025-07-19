using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFXPoolControler : MonoBehaviour
{
    public HitFXManager hitFXManager;
    public FlashManager flash1Manager;
    public FlashManager flash2Manager;
    public JudgmentTextManager judgmentTextManager;

    private FlashManager GetFlashManager(NoteController note)
    {
        return note.trackId == 0 ||
               note.trackId == 2 ||
               note.trackId == 4 ||
               note.trackId == 6 ? flash2Manager : flash1Manager;
    }

    public void Hit(NoteController note)
    {
        if (note.judgment == 0) Perfect(note);
        else if (note.judgment == -1 || note.judgment == 1) Great(note);
        else if (note.judgment == -2 || note.judgment == 2) Good(note);
        else if (note.judgment == -3 || note.judgment == 3) Bad(note);
        else if (note.judgment == -4 || note.judgment == 4) Miss(note);
    }

    public void Perfect(NoteController note)
    {
        GetFlashManager(note).HitPerfect(note.transform);
        // hitFXManager.HitPerfect(note.transform);
        judgmentTextManager.HitPerfect(note.transform);
    }

    public void Great(NoteController note)
    {
        GetFlashManager(note).HitGreat(note.transform);
        // hitFXManager.HitGood(note.transform);
        judgmentTextManager.HitGreat(note.transform);
    }

    public void Good(NoteController note)
    {
        GetFlashManager(note).HitGood(note.transform);
        // hitFXManager.HitGood(note.transform);
        judgmentTextManager.HitGood(note.transform);
    }

    public void Bad(NoteController note)
    {
        GetFlashManager(note).HitBad(note.transform);
        // hitFXManager.HitBad(note.transform);
        judgmentTextManager.HitBad(note.transform);
    }

    public void Miss(NoteController note)
    {
        // GetFlashManager(note).HitBad(note.transform);
        // hitFXManager.HitBad(note.transform);
        judgmentTextManager.HitMiss(note.transform);
    }

    public void Catch(NoteHoldController hold)
    {
        // TODO
        // hitFXManager.HitPerfect(hold.transform);
        GetFlashManager(hold).HitPerfect(hold.transform);
        judgmentTextManager.HitPerfect(hold.transform);
    }

    public void UnCatch(NoteHoldController hold)
    {
        // TODO
        judgmentTextManager.HitMiss(hold.transform);
    }
}
