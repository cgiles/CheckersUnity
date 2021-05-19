using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPawnCon : MonoBehaviour
{
    public bool isWhite = false;
    public Vector2Int id;
    public Vector2Int position;
    public bool isKing = false;
    public int caseNumber = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (isWhite)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.4f, 0.26f, 0.13f));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Init(bool isW,Vector2Int pos,int caseNumber)
    {
        isWhite = isW;
        position = pos;
        this.caseNumber = caseNumber;
        if (isWhite)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.4f,0.26f,0.13f));
        }
        CheckersBoardCon.blackCases[this.caseNumber-1].isFree = false;
        CheckersBoardCon.blackCases[this.caseNumber-1].pawn = this;
        transform.tag = isWhite ? "White" : "Black";
    }
    public List<CheckersCaseCon> GetPossibleMove()
    {
        

        List<CheckersCaseCon> cases = new List<CheckersCaseCon>();
        
        int nbUnit = caseNumber % 10;
        if (!isKing)
        {
            if (nbUnit != 6)
            {
                if (isWhite)
                {
                    CheckersCaseCon aCase = CheckersBoardCon.GetCaseById(position.x - 1, position.y + 1);
                    if (aCase.isFree) cases.Add(aCase);
                }
                else
                {
                    CheckersCaseCon aCase = CheckersBoardCon.GetCaseById(position.x - 1, position.y - 1);
                    if (aCase.isFree) cases.Add(aCase);

                }
            }
            if (nbUnit != 5)
            {
                if (isWhite)
                {
                    CheckersCaseCon aCase = CheckersBoardCon.GetCaseById(position.x + 1, position.y + 1);
                    if (aCase.isFree) cases.Add(aCase);


                }
                else
                {
                    CheckersCaseCon aCase = CheckersBoardCon.GetCaseById(position.x + 1, position.y - 1); ;
                    if (aCase.isFree) cases.Add(aCase);
                }
            }
        }
        else
        {
            Vector2Int[] increments = new Vector2Int[4]
            {
                new Vector2Int(-1,1),
                new Vector2Int(-1,-1),
                new Vector2Int(1,-1),
                new Vector2Int(1,1)
                
            };
            Rect boardArea = new Rect();
            boardArea.size=new Vector2(10, 10);
            boardArea.x = -0.5f;
            boardArea.y = -0.5f;
            
            for(int d = 0; d < increments.Length; d++)
            {
                Vector2Int newPos = position+increments[d];

                while (boardArea.Contains(newPos))
                {

                    if (!CheckersBoardCon.GetCaseById(newPos).isFree)
                    {
                        break;
                        if (CheckersBoardCon.GetCaseById(newPos).pawn.tag != tag)
                        {
                            if (!CheckersBoardCon.GetCaseById(newPos + increments[d]).isFree)
                            {
                                break;
                            }

                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {



                        cases.Add(CheckersBoardCon.GetCaseById(newPos));
                        newPos += increments[d];

                    }
                    Debug.Log(newPos.ToString() + " " + cases.Count.ToString());
                    
                }
                Debug.Log(d.ToString()+" "+ cases.Count.ToString());
            }
            Debug.Log(cases.Count);
        }

        return cases;
    }
    public void MoveTo(CheckersCaseCon aCase)
    {
        Debug.Log("Moving");
        CheckersCaseCon pCase = CheckersBoardCon.GetCaseByNumber(caseNumber);
        pCase.isFree = true;
        pCase.pawn = null;
        caseNumber = aCase.number;
        aCase.isFree = false;
        aCase.pawn = this;
        position = aCase.id;
        transform.position = aCase.transform.position + Vector3.up * 0.25f;
        if (tag == "White"&&position.y==9&&!isKing)
        {
            isKing = true;
        }
        else if (tag == "Black" && position.y == 0 && !isKing)
        {
            isKing = true;
        }
    }
   
    public List<(CheckersPawnCon, List<CheckersCaseCon>)> CheckCapture()
    {
       
        return CheckCapture(CheckersBoardCon.GetCaseById(position));
    }
    public List<(CheckersPawnCon,List<CheckersCaseCon>)> CheckCapture(CheckersCaseCon aCase)
    {
        List<(CheckersPawnCon, List<CheckersCaseCon>)> result = new List<(CheckersPawnCon, List<CheckersCaseCon>)>();
        
        List<CheckersCaseCon> possiblePawns=new List<CheckersCaseCon>();
        List<CheckersCaseCon> neighborCases = new List<CheckersCaseCon>();

        if (!isKing)
        {
            if (aCase.id.x > 1)
            {
                if (aCase.id.y < 8) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x - 1, aCase.id.y + 1));
                if (aCase.id.y > 1) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x - 1, aCase.id.y - 1));
            }
            if (aCase.id.x < 8)
            {
                if (aCase.id.y < 8) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x + 1, aCase.id.y + 1));
                if (aCase.id.y > 1) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x + 1, aCase.id.y - 1));
            }
            foreach (CheckersCaseCon neightbor in neighborCases)
            {
                if (!neightbor.isFree && neightbor.pawn.tag != tag)
                {
                    CheckersPawnCon anOpponent = neightbor.pawn;
                    Vector2Int diff = anOpponent.position - position;
                    Vector2Int nextCaseId = anOpponent.position + diff;
                    if (CheckersBoardCon.GetCaseById(nextCaseId).isFree)
                    {
                        possiblePawns.Add(CheckersBoardCon.GetCaseById(nextCaseId));
                        result.Add((anOpponent, possiblePawns));
                    }
                }
            }

        }
        else
        {
            Vector2Int[] increments = new Vector2Int[4]
       {
                new Vector2Int(-1,1),
                new Vector2Int(-1,-1),
                new Vector2Int(1,-1),
                new Vector2Int(1,1)

       };
            Rect boardArea = new Rect();
            boardArea.size = new Vector2(10, 10);
            boardArea.x = -0.5f;
            boardArea.y = -0.5f;
            CheckersPawnCon aPawn = null;
            for (int d = 0; d < increments.Length; d++)
            {
                Vector2Int newPos = position + increments[d];
                aPawn = null;
                List<CheckersCaseCon> possibleCase = new List<CheckersCaseCon>();
                while (boardArea.Contains(newPos))
                {
                    CheckersCaseCon nCase = CheckersBoardCon.GetCaseById(newPos);
                    if (!nCase.isFree&&aPawn==null)
                    {
                        if (nCase.pawn.tag != tag && boardArea.Contains(newPos + increments[d])) 
                        {
                            if (CheckersBoardCon.GetCaseById(newPos + increments[d]).isFree){
                               
                                    aPawn = nCase.pawn;
                                    newPos += increments[d];
                                nCase = CheckersBoardCon.GetCaseById(newPos);
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else if(!nCase.isFree && aPawn != null)
                    {
                        break;
                    }
                    if (aPawn != null) possibleCase.Add(nCase);
                    newPos += increments[d];
                }
                if (aPawn != null) result.Add((aPawn, possibleCase));
            }
        }
        return result;
    }
    public (List<CheckersCaseCon>, List<CheckersPawnCon>) PossibleCapture()
    {
        return PossibleCapture(CheckersBoardCon.GetCaseById(position));
    }
    public (List<CheckersCaseCon>, List<CheckersPawnCon>) PossibleCapture(CheckersCaseCon aCase )
    {
        List<CheckersCaseCon> possibleCases = new List<CheckersCaseCon>();
        List<CheckersCaseCon> neighborCases = new List<CheckersCaseCon>();
       List<CheckersPawnCon> opponentPawns = new List<CheckersPawnCon>();
        if (!isKing)
        {
            int nbUnit = aCase.number % 10;

            if (aCase.id.x>1)
            {
                if (aCase.id.y < 8) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x - 1, aCase.id.y + 1));
                if (aCase.id.y > 1) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x - 1, aCase.id.y - 1));
            }
            if (aCase.id.x<8)
            {
                if (aCase.id.y < 8) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x + 1, aCase.id.y + 1));
                if (aCase.id.y > 1) neighborCases.Add(CheckersBoardCon.GetCaseById(aCase.id.x + 1, aCase.id.y - 1));
            }

            for (int i = 0; i < neighborCases.Count; i++)
            {

                if (!neighborCases[i].isFree && neighborCases[i].pawn.tag != transform.tag)
                {

                    CheckersPawnCon opponentPawn = neighborCases[i].pawn;
                    Vector2Int diff = opponentPawn.position - position;
                    Vector2Int nextCaseId = opponentPawn.position + diff;
                    if (CheckersBoardCon.GetCaseById(nextCaseId).isFree)
                    {

                        possibleCases.Add(CheckersBoardCon.GetCaseById(nextCaseId));
                        opponentPawns.Add(opponentPawn);
                    }
                } 
            }
        }
        else
        {
            Vector2Int[] increments = new Vector2Int[4]
          {
                new Vector2Int(-1,1),
                new Vector2Int(-1,-1),
                new Vector2Int(1,-1),
                new Vector2Int(1,1)

          };
            Rect boardArea = new Rect();
            boardArea.size = new Vector2(10, 10);
            boardArea.x = -0.5f;
            boardArea.y = -0.5f;
            int pOpponentsNB = 0;
            int oppenentCaptured=0;
            for (int d = 0; d < increments.Length; d++)
            {
                Vector2Int newPos = position + increments[d];
                List<CheckersCaseCon> possibleCasesT = new List<CheckersCaseCon>();
                while (boardArea.Contains(newPos))
                {
                  
                    if (!CheckersBoardCon.GetCaseById(newPos).isFree)
                    {
                      
                        if (CheckersBoardCon.GetCaseById(newPos).pawn.tag != tag&&boardArea.Contains(newPos + increments[d]))
                        {
                            if (CheckersBoardCon.GetCaseById(newPos + increments[d]).isFree && boardArea.Contains(newPos + increments[d]))
                            {
                                possibleCasesT.Clear();
                                opponentPawns.Add(CheckersBoardCon.GetCaseById(newPos).pawn);
                                oppenentCaptured++;
                                newPos += increments[d];
                            }
                            else
                            {
                                break;
                            }
                        }
                        else if(CheckersBoardCon.GetCaseById(newPos).pawn.tag == tag)
                        {
                            break;
                        }
                    }



                    possibleCasesT.Add(CheckersBoardCon.GetCaseById(newPos));

                    newPos += increments[d];
                  

                }
                if (pOpponentsNB == 0 && opponentPawns.Count > 0)
                {
                    Debug.Log("King can capture"+opponentPawns[0].position.ToString());
                    possibleCases.Clear();
                    possibleCases.AddRange(possibleCasesT);
                    pOpponentsNB = opponentPawns.Count;
                }else if (opponentPawns.Count >= pOpponentsNB&& pOpponentsNB > 0)
                {
                    pOpponentsNB = opponentPawns.Count;
                    opponentPawns.Add(opponentPawns[oppenentCaptured - 1]);
                    possibleCases.AddRange(possibleCasesT);
                }else if (pOpponentsNB == 0 && opponentPawns.Count == 0)
                {
                    possibleCases.AddRange(possibleCasesT);
                }
            }
            Debug.Log(opponentPawns.Count.ToString() + "paws for : " + possibleCases.Count.ToString());
        
    }if (opponentPawns.Count == 0) possibleCases.Clear();
        return (possibleCases,opponentPawns);
    }

}
