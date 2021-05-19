using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoardCon : MonoBehaviour
{
    [HideInInspector]
    static public List<CheckersCaseCon> cases = new List<CheckersCaseCon>();
    static public List<CheckersCaseCon> blackCases = new List<CheckersCaseCon>();
    public CheckersCaseCon checkersCase;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        CreateBoard();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void CreateBoard()
    {
        bool even = true;
        int caseName = 46;
        for (int y = 0; y <10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                Vector3 casePos = new Vector3(x, 0,y);
                CheckersCaseCon aCase = Instantiate(checkersCase, casePos, Quaternion.identity,transform);
                if (even)
                {
                    aCase.number = caseName;
                    caseName++;
                    blackCases.Add(aCase);
                }
                aCase.SetId(new Vector2Int(x, y),!even);
                aCase.gameObject.name = "Case" + x.ToString() + "_" + y.ToString();
                cases.Add(aCase);
                even = !even;
            }
            caseName -= 10;
           even = !even;
        }
        blackCases.Sort(delegate (CheckersCaseCon caseA, CheckersCaseCon caseB) {
            if (caseA.number == 0 && caseB.number == 0) return 0;
            else if (caseA.number == 0) return -1;
            else if (caseB.number == 0) return 1;
            else return caseA.number.CompareTo(caseB.number);
        });

    }
    static public CheckersCaseCon GetCaseByNumber(int nb)
    {
        return blackCases[nb - 1];
    }
    static public CheckersCaseCon GetCaseById(int x,int y)
    {

        return cases[x + y * 10];
    }
    static public CheckersCaseCon GetCaseById(Vector2Int id)
    {
        return GetCaseById(id.x, id.y);
    }
}
