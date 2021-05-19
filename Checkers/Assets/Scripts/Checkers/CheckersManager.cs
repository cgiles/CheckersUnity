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
        PlacePawns();
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Generate the pawns over the board
    /// </summary>
    void PlacePawns()
    {
        for(int i = 0; i < nbPawns; i++)
        {
            Vector2Int pos = CheckersBoardCon.blackCases[i].id;
            CheckersPawnCon aPawn = Instantiate(pawn, new Vector3(pos.x, 0.25f, pos.y), Quaternion.identity,transform);
            aPawn.Init(false, pos,i+1);
            aPawn.isKing = areKings;
            pawns.Add(aPawn);
            blackPawns.Add(aPawn);
        }
        for(int i = 50-nbPawns; i < 50; i++)
        {
            Vector2Int pos = CheckersBoardCon.blackCases[i].id;
            CheckersPawnCon aPawn = Instantiate(pawn, new Vector3(pos.x,0.25f,pos.y), Quaternion.identity,transform);
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
        currentPlayer = 0;
        players[0].isWhite = true;
        players[1].isWhite = false;
        StartCoroutine(NextTurn());
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator NextTurn()
    {
        yield return new WaitForEndOfFrame();
        while (isPlaying)
        {
            foreach (GameObject g in displayers) Destroy(g);
            displayers.Clear();
            
            CheckersPawnCon pawnToPlay = null;
            CheckersCaseCon pickedCase = null;


            bool canCapture;
             playablePawns = CheckPlayablePawns(out canCapture);
            DisplayPlayablePawns(playablePawns);
            yield return new WaitForEndOfFrame();
            if (playablePawns.Count > 1)
            {
                while (pawnToPlay == null)
                {


                    CheckersPawnCon aPawn = players[currentPlayer].PickPawn();

                    if (playablePawns.Contains(aPawn)) pawnToPlay = aPawn;
                    yield return new WaitForEndOfFrame();
                }
            }
            else pawnToPlay = playablePawns[0];

            bool canCaptureMore = false;
            do
            {
                possibleCases = new List<CheckersCaseCon>();
                List<(CheckersPawnCon, List<CheckersCaseCon>)> possibleCaptures = pawnToPlay.CheckCapture();
                //possibleCases = pawnToPlay.PossibleCapture().Item1.Count > 0 ? pawnToPlay.PossibleCapture().Item1 : pawnToPlay.GetPossibleMove();
                if (possibleCaptures.Count > 0) {
                    Debug.Log("nblist : " + possibleCaptures[0].Item2.Count.ToString());
                    foreach((CheckersPawnCon,List<CheckersCaseCon>) pC in possibleCaptures)
                    {
                        possibleCases.AddRange(pC.Item2);
                    }
                }else possibleCases = pawnToPlay.GetPossibleMove();
                
                 pickedCase = null;
                DisplayPossibleCases(possibleCases);
                while (pickedCase == null)
                {
                    CheckersCaseCon aCase = null;
                    if (players[currentPlayer].PickCase() != null)
                    {
                        aCase = players[currentPlayer].PickCase();
                    }
                    else if (players[currentPlayer].PickCase() != null)
                    {
                        canCapture = false;
                        canCaptureMore = false;
                        break;
                    }
                    //Debug.Log("how many cases to move : " + possibleCases.Count.ToString());
                    if (possibleCases.Contains(aCase))
                    {
                        Debug.Log("pawn Picked");
                        pickedCase = aCase;
                    }
                    yield return new WaitForEndOfFrame();
                }
                if (canCapture)
                {
                    foreach ((CheckersPawnCon,List<CheckersCaseCon>) pC in possibleCaptures)
                    {
                        if (pC.Item2.Contains(pickedCase))
                        {
                            CapturePawn(pC.Item1);
                            pawnToPlay.MoveTo(pickedCase);
                            canCaptureMore = pawnToPlay.CheckCapture().Count > 0;
                            break;
                        }
                    }
/*Debug.Log("how many cases after capture : " + possibleCases.Count.ToString());
                    int idCapture = pawnToPlay.PossibleCapture().Item1.IndexOf(pickedCase);
                    CapturePawn(pawnToPlay.PossibleCapture().Item2[idCapture]);
                    Debug.Log("moving to");
                    
                    pawnToPlay.MoveTo(pickedCase);
                    Debug.Log("moved to");
                    canCaptureMore = pawnToPlay.PossibleCapture().Item1.Count > 0;*/
                }
                else if(pickedCase!=null)
                    pawnToPlay.MoveTo(pickedCase);
            } while (canCaptureMore);
            if (blackPawns.Count > 0 && whitePawns.Count > 0&&pickedCase!=null)
            {
                yield return new WaitForEndOfFrame();
                SwitchPlayers();
            }
            else if(blackPawns.Count == 0 || whitePawns.Count == 0)
            {
                isPlaying = false;
                Debug.Log("GameOver");
            }
            yield return new WaitForEndOfFrame();
        }
    }
    void DisplayPlayablePawns(List<CheckersPawnCon> possiblePawns)
    {
        foreach (GameObject g in displayers) Destroy(g);
        displayers.Clear();
        foreach (CheckersPawnCon g in possiblePawns)
        {
            GameObject aSphere = Instantiate(spherePawn, g.transform.position, Quaternion.identity);
            aSphere.transform.localScale = Vector3.one * 0.25f;
            displayers.Add(aSphere);
        }
    }
    void DisplayPossibleCases(List<CheckersCaseCon> possibleCases)
    {
        foreach (GameObject g in displayers) Destroy(g);
        displayers.Clear();
        foreach (CheckersCaseCon aCase in possibleCases)
        {
            GameObject aSphere = Instantiate(sphereMove, aCase.transform.position, Quaternion.identity);
            aSphere.transform.localScale = Vector3.one * 0.25f;
            displayers.Add(aSphere);
        }
    }
    void SwitchPlayers()
    {
        currentPlayer = (currentPlayer + 1) % 2;
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
}
