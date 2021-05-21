using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersAI : CheckersPlayer
{
    CheckersPawnCon pickedPawn;
    CheckersCaseCon pickedCase;
    bool pPrun = false;
    bool pCrun = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isPlaying)
        {
            pickedPawn = null;
            pickedCase = null;
        }
    }
    public override CheckersPawnCon PickPawn()
    {
        if (isPlaying)

            return CheckersManager.playablePawns[Random.Range(0, CheckersManager.playablePawns.Count)];
        else return null;
    }
    IEnumerator _PickPawn()
    {
        yield return new WaitForEndOfFrame();
        pickedPawn = CheckersManager.playablePawns[Random.Range(0, CheckersManager.playablePawns.Count)];
    }
    public override CheckersCaseCon PickCase()
    {
        if (isPlaying)
            return CheckersManager.possibleCases[Random.Range(0, CheckersManager.possibleCases.Count)];
        else return null;
    }
    public override void Init()
    {
        base.Init();
    }
}
