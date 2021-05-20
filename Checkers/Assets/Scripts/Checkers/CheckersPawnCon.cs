using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPawnCon : CheckersComponent
{

    public Vector2Int position;
    public bool isKing = false;
    public bool instantDisplacement = true;
    public int caseNumber = 0;
    public Texture2D[] textures = new Texture2D[2];
    // Start is called before the first frame update
    void Start()
    {
        if (isWhite)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            GetComponent<MeshRenderer>().material.SetTexture("_TextureKing", textures[0]);
            
            
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.4f, 0.26f, 0.13f));
            GetComponent<MeshRenderer>().material.SetTexture("_TextureKing", textures[1]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// Init the pawn
    /// </summary>
    /// <param name="isW"></param>
    /// <param name="pos"></param>
    /// <param name="caseNumber"></param>
    public virtual void Init(bool isW, Vector2Int pos, int caseNumber)
    {
        isWhite = isW;
        position = pos;
        this.caseNumber = caseNumber;
        if (isWhite)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            GetComponent<MeshRenderer>().material.SetTexture("_TextureKing", textures[0]);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.4f, 0.26f, 0.13f));
            GetComponent<MeshRenderer>().material.SetTexture("_TextureKing", textures[1]);
        }
        //GetComponent<MeshRenderer>().material.SetInt("IsKing", isKing?1:0);

        CheckersBoardCon.blackCases[this.caseNumber - 1].isFree = false;
        CheckersBoardCon.blackCases[this.caseNumber - 1].pawn = this;
        transform.tag = isWhite ? "White" : "Black";
    }
    /// <summary>
    /// Checks possible move, but not the capture
    /// </summary>
    /// <returns></returns>
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
            boardArea.size = new Vector2(10, 10);
            boardArea.x = -0.5f;
            boardArea.y = -0.5f;

            for (int d = 0; d < increments.Length; d++)
            {
                Vector2Int newPos = position + increments[d];

                while (boardArea.Contains(newPos))
                {

                    if (!CheckersBoardCon.GetCaseById(newPos).isFree)
                    {
                        break;

                    }
                    else
                    {



                        cases.Add(CheckersBoardCon.GetCaseById(newPos));
                        newPos += increments[d];

                    }
                    Debug.Log(newPos.ToString() + " " + cases.Count.ToString());

                }
                Debug.Log(d.ToString() + " " + cases.Count.ToString());
            }
            Debug.Log(cases.Count);
        }

        return cases;
    }
    /// <summary>
    /// Move the pawn, and make a king if reaches the other side
    /// </summary>
    /// <param name="aCase"></param>
    public void MoveTo(CheckersCaseCon aCase)
    {
        if (instantDisplacement)
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
            if (tag == "White" && position.y == 9 && !isKing)
            {
                isKing = true;
                GetComponent<MeshRenderer>().material.SetInt("IsKing", 1);
            }
            else if (tag == "Black" && position.y == 0 && !isKing)
            {
                isKing = true;
                GetComponent<MeshRenderer>().material.SetInt("IsKing", 1);
            }
        }
        else StartCoroutine(MovingTo(aCase));
    }

    IEnumerator MovingTo(CheckersCaseCon aCase)
    {
        Debug.Log("Moving");
        CheckersCaseCon pCase = CheckersBoardCon.GetCaseByNumber(caseNumber);
        pCase.isFree = true;
        pCase.pawn = null;
        caseNumber = aCase.number;
        aCase.isFree = false;
        aCase.pawn = this;
        position = aCase.id;
        //transform.position = aCase.transform.position + Vector3.up * 0.25f;
        if (tag == "White" && position.y == 9 && !isKing)
        {
            isKing = true;
        }
        else if (tag == "Black" && position.y == 0 && !isKing)
        {
            isKing = true;
        }
        float amount = 0;
        while (amount < 1)
        {
            transform.position=Vector3.MoveTowards(transform.position, aCase.transform.position + Vector3.up * 0.25f,0.1f*Time.deltaTime);
            amount += 0.1f*Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    /// <summary>
    /// Similar to possible move, but check if we can capture
    /// </summary>
    /// <returns></returns>
    public List<(CheckersPawnCon, List<CheckersCaseCon>)> CheckCapture()
    {

        return CheckCapture(CheckersBoardCon.GetCaseById(position));
    }
    /// <summary>
    /// Similar to possible move, but check if we can capture
    /// </summary>
    /// <returns></returns>
    public List<(CheckersPawnCon, List<CheckersCaseCon>)> CheckCapture(CheckersCaseCon aCase)
    {
        List<(CheckersPawnCon, List<CheckersCaseCon>)> result = new List<(CheckersPawnCon, List<CheckersCaseCon>)>();

        List<CheckersCaseCon> possiblePawns = new List<CheckersCaseCon>();
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
                    if (!nCase.isFree && aPawn == null)
                    {
                        if (nCase.pawn.tag != tag && boardArea.Contains(newPos + increments[d]))
                        {
                            if (CheckersBoardCon.GetCaseById(newPos + increments[d]).isFree)
                            {

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
                    else if (!nCase.isFree && aPawn != null)
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
    

}
