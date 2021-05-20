using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersAI :CheckersPlayer
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override CheckersPawnCon PickPawn()
    {   if (isPlaying)
            return CheckersManager.playablePawns[Random.Range(0, CheckersManager.playablePawns.Count)];
        else return null;
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
public class CheckersRound
{
  
}
