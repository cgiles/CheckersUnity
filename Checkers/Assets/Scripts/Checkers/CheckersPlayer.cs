using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isWhite = true;
    public  string  playerTag;
    public bool isPlaying = false;
    Camera cam;
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// Pick a pawn, by clicking on it
    /// </summary>
    /// <returns></returns>
    public virtual CheckersPawnCon PickPawn()
    {
       
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            
            if (Physics.Raycast(ray,out hit,Mathf.Infinity))
            {
             
                if (hit.collider.GetComponent<CheckersPawnCon>())
                {

                    if (hit.collider.tag == playerTag)
                    {

                        return hit.collider.GetComponent<CheckersPawnCon>();
                    }
                    else return null;
                }
                else
                {
                    return null;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// pick a case by clicking on it
    /// </summary>
    /// <returns></returns>
   public virtual CheckersCaseCon PickCase()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButton(0))
        {
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.GetComponent<CheckersCaseCon>())
                {
                    return hit.collider.GetComponent<CheckersCaseCon>();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        return null;
    }
    /// <summary>
    /// Init the player
    /// </summary>
    public virtual void Init()
    {
        cam = Camera.main;
        playerTag = isWhite ? "White" : "Black";
    }
}
