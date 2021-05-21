using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersManager : MonoBehaviour
{
    public CheckersPawnCon pawn;
    public GameObject sphereMove,spherePawn;
    public int nbPawns=20;
    public bool areKings = false;
    List<CheckersPawnCon> pawns=new List<CheckersPawnCon>();
    List<CheckersPawnCon> whitePawns = new List<CheckersPawnCon>();
    List<CheckersPawnCon> blackPawns = new List<CheckersPawnCon>();
    List<GameObject> displayers=new List<GameObject>();

    public static List<CheckersPawnCon> playablePawns;
    public static List<CheckersCaseCon> possibleCases;
    

    bool isPlaying = true;
    // Start is called before the first frame update
   public CheckersPlayer[] players = new CheckersPlayer[2];
    int currentPlayer = 0;
    
    void Start()
    {
        
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            CheckersRound round = new CheckersRound(pawns, true);
            round.ShowBoard();
            bool capture;
            Debug.Log("playable : "+round.GetPlayablePawn(out capture).Count.ToString());
        }
    }
    /// <summary>
    /// Generate the pawns over the board
    /// </summary>
    void PlacePawns()
    {
        foreach (CheckersPawnCon p in pawns)
        {
            Destroy(p.gameObject);
        }
        pawns.Clear();
        blackPawns.Clear();
        whitePawns.Clear(); 
        for(int i = 0; i < nbPawns; i++)
        {
            Vector2Int pos = CheckersBoardCon.blackCases[i].id;
            CheckersPawnCon aPawn = Instantiate(pawn, new Vector3(pos.x, 0.25f, pos.y), Quaternion.Euler(0,240,0),transform);
            aPawn.Init(false, pos,i+1);
            aPawn.isKing = areKings;
            pawns.Add(aPawn);
            blackPawns.Add(aPawn);
        }
        for(int i = 50-nbPawns; i < 50; i++)
        {
            Vector2Int pos = CheckersBoardCon.blackCases[i].id;
            CheckersPawnCon aPawn = Instantiate(pawn, new Vector3(pos.x,0.25f,pos.y), Quaternion.Euler(0,240, 0), transform);
            aPawn.Init(true, pos,i+1);
            pawns.Add(aPawn);
            aPawn.isKing = areKings;
            whitePawns.Add(aPawn);
        }
    }
    /// <summary>
    /// Start a new Game, launch the coroutine nextTurn()
    /// </summary>
    void StartGame()
    {
        
        PlacePawns();
        currentPlayer = 0;
        players[0].isWhite = true;
        players[0].isPlaying = true;
        players[1].isWhite = false;
        foreach (CheckersPlayer p in players)
        {
            p.Init();
        }
        StartCoroutine(NextTurn());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator NextTurn()
    {

        yield return new WaitForEndOfFrame();
        CheckersPawnCon pawnToPlay = null;
        CheckersCaseCon pickedCase = null;
        possibleCases = new List<CheckersCaseCon>();
        List<(CheckersPawnCon, List<CheckersCaseCon>)> possibleCaptures = new List<(CheckersPawnCon, List<CheckersCaseCon>)>();
        bool canCaptureMore = false;
        bool canCapture;
        while (isPlaying)
        {
            yield return new WaitForEndOfFrame();
            foreach (GameObject g in displayers) Destroy(g);
            displayers.Clear();




            
            playablePawns = CheckPlayablePawns(out canCapture);
            DisplayPlayablePawns(playablePawns);




            if (playablePawns.Count > 1)
            {
                CheckersPawnCon aPawn = players[currentPlayer].PickPawn();
                if (playablePawns.Contains(aPawn))
                {
                    pawnToPlay = aPawn;

                }
            }
            else
            {
                pawnToPlay = playablePawns[0];
            }

            if (pawnToPlay != null)
            {
                possibleCases = new List<CheckersCaseCon>();
                possibleCaptures = pawnToPlay.CheckCapture();
                if (possibleCaptures.Count > 0)
                {
                   
                    foreach ((CheckersPawnCon, List<CheckersCaseCon>) pC in possibleCaptures)
                    {
                        possibleCases.AddRange(pC.Item2);
                    }
                }
                else possibleCases = pawnToPlay.GetPossibleMove();
            }
            if (possibleCases.Count > 0) DisplayPossibleCases(possibleCases);
          
            if ((pawnToPlay != null && pickedCase == null) || (pawnToPlay != null /*&& pickedCase == null*/ && canCaptureMore))
            {
                CheckersCaseCon aCase = null;
                aCase = players[currentPlayer].PickCase();
                if (possibleCases.Contains(aCase))
                {
                    pickedCase = aCase;
                    if (canCapture || canCaptureMore)
                    {
                        Debug.Log(possibleCaptures.Count);

                        foreach ((CheckersPawnCon, List<CheckersCaseCon>) pC in possibleCaptures)
                        {
                            if (pC.Item2.Contains(pickedCase))
                            {
                                CapturePawn(pC.Item1);
                                pawnToPlay.MoveTo(pickedCase);
                                canCaptureMore = pawnToPlay.CheckCapture().Count > 0;
                                canCapture = false;
                                pickedCase = null;

                            }
                        }
                        if (!canCaptureMore)
                        {
                            pawnToPlay = null;
                            pickedCase = null;
                            playablePawns.Clear();
                            possibleCaptures.Clear();
                            possibleCases.Clear();
                            SwitchPlayers();
                        }
                    }
                    else
                    {
                        pawnToPlay.MoveTo(pickedCase);
                        pawnToPlay = null;
                        pickedCase = null;
                        playablePawns.Clear();
                        possibleCaptures.Clear();
                        possibleCases.Clear();
                        canCaptureMore = false;

                        SwitchPlayers();
                        yield return new WaitForEndOfFrame();
                    }

                }
            }
            else if (pawnToPlay != null && pickedCase != null) SwitchPlayers();
        }
    }
    /// <summary>
    /// show the paws the player can play
    /// </summary>
    /// <param name="possiblePawns"></param>
    void DisplayPlayablePawns(List<CheckersPawnCon> possiblePawns)
    {
        foreach (GameObject g in displayers) Destroy(g);
        displayers.Clear();
        foreach (CheckersPawnCon g in possiblePawns)
        {
            GameObject aSphere = Instantiate(spherePawn, g.transform.position, Quaternion.identity);
            aSphere.transform.localScale = Vector3.one * 0.5f;
            displayers.Add(aSphere);
        }
    }
    /// <summary>
    /// Show which case is available for the selected pawn
    /// </summary>
    /// <param name="possibleCases"></param>
    void DisplayPossibleCases(List<CheckersCaseCon> possibleCases)
    {
       // foreach (GameObject g in displayers) Destroy(g);
      //  displayers.Clear();
        foreach (CheckersCaseCon aCase in possibleCases)
        {
            GameObject aSphere = Instantiate(sphereMove, aCase.transform.position, Quaternion.identity);
            aSphere.transform.localScale = Vector3.one * 0.25f;
            displayers.Add(aSphere);
        }
    }
    /// <summary>
    /// simply switch the players
    /// </summary>
    void SwitchPlayers()
    {
        IsGameOver();
        players[currentPlayer].isPlaying = false;
        currentPlayer = (currentPlayer + 1) % 2;
        players[currentPlayer].isPlaying = true;
    }
    void CapturePawn(CheckersPawnCon prey)
    {
        Debug.Log("capturing !");
        CheckersCaseCon pCase = CheckersBoardCon.GetCaseByNumber(prey.caseNumber);

        pawns.Remove(prey);
        if (prey.isWhite)
        {
            whitePawns.Remove(prey);
        }
        else
        {
            blackPawns.Remove(prey);
        }
        pCase.isFree = true;
        pCase.pawn = null;
        Destroy(prey.gameObject);
    }   
   public List<CheckersPawnCon> CheckPlayablePawns(out bool canCapture)
    {
        canCapture = false;
        List<CheckersPawnCon> playablePawns = new List<CheckersPawnCon>();
        List<CheckersPawnCon> predatorPawns = new List<CheckersPawnCon>();
        if (currentPlayer == 0)
        {
            foreach(CheckersPawnCon w in whitePawns)
            {
                if (w.CheckCapture().Count > 0)//if (w.PossibleCapture().Item1.Count > 0)
                {
                    predatorPawns.Add(w);
                    canCapture = true;
                }
                else if (w.GetPossibleMove().Count > 0)
                {
                    playablePawns.Add(w);
                }
            }
        }
        else
        {
            foreach(CheckersPawnCon b in blackPawns)
            {
                if (b.CheckCapture().Count > 0)//if (b.PossibleCapture().Item1.Count > 0)
                {
                    predatorPawns.Add(b);
                    canCapture = true;

                }
                else if (b.GetPossibleMove().Count > 0)
                {
                    playablePawns.Add(b);
                }
                       
            }


        }
        return predatorPawns.Count>0?predatorPawns:playablePawns;
    }
    bool IsGameOver()
    {
       
        if (blackPawns.Count == 0 || whitePawns.Count == 0)
        {
            StopAllCoroutines();
            isPlaying = false;
            return true;
        }
        else return false;
    }
}
