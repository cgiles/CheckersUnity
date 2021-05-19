using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersCaseCon : MonoBehaviour
{
    public Vector2Int id;
    public int number;
    public bool isFree = true;
    public bool isWhite = false;
    public CheckersPawnCon pawn = null;
    
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetId(Vector2Int anId,bool isWhite)
    {
        this.isWhite = isWhite;
        id = anId; 
        if (isWhite)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.2f,0.2f,0.2f));
        }
    }
    public override string ToString()
    {
        return number.ToString();
    }
}
