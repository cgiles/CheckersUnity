using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersRound
{
    int[,] board = new int[10, 10];
    int[,] pBoard = new int[10, 10];
    int white = 1;
    int whiteKing = 3;
    int black = 2;
    int blackKing = 4;
    bool whiteIsPlaying = false;
    bool canCapture;
    List<Vector2Int> playablePawns = new List<Vector2Int>();
    public CheckersRound(List<CheckersPawnCon> pawns, bool isWhitePlaying)
    {
        whiteIsPlaying = isWhitePlaying;
        foreach (CheckersPawnCon p in pawns)
        {
            int isKing = p.isKing ? 2 : 0;
            board[p.position.x, p.position.y] = p.isWhite ? white : black + isKing;
        }
        pBoard = board;
        playablePawns = GetPlayablePawn(out canCapture);

    }
    /// <summary>
    /// Return the list of all the playable pawns at this round;
    /// </summary>
    /// <param name="canCapture"></param>
    /// <returns></returns>
    public List<Vector2Int> GetPlayablePawn(out bool canCapture)
    {
        canCapture = false;
        List<Vector2Int> pawns = new List<Vector2Int>();
        List<Vector2Int> predator = new List<Vector2Int>();
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (whiteIsPlaying)
                {
                    if (board[x, y] % 2 == 1)
                    {
                        Vector2Int pos = new Vector2Int(x, y);
                        if (PawnCanMove(pos).Count > 0)
                        {

                            pawns.Add(pos);

                        }
                        if (PawnCanCapture(pos).Count > 0)
                        {
                            predator.Add(pos);
                            canCapture = true;
                        }

                    }
                }
                else
                {
                    if (board[x, y] % 2 == 0 && board[x, y] > 0)
                    {
                        Vector2Int pos = new Vector2Int(x, y);
                        if (PawnCanMove(pos).Count > 0)
                        {
                            pawns.Add(pos);
                        }
                        if (PawnCanCapture(pos).Count > 0)
                        {
                            predator.Add(pos);
                            canCapture = true;
                        }

                    }
                }

            }
        }
        return predator.Count > 0 ? predator : pawns;

    }
    /// <summary>
    /// Return if a pawn at the position XY can move
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public List<Vector2Int> PawnCanMove(Vector2Int pos)
    {
        List<Vector2Int> results = new List<Vector2Int>();
        if (whiteIsPlaying)
        {
            if (GetBoardAt(pos) == 1)
            {
                Vector2Int[] increment = new Vector2Int[2]
                {
               new Vector2Int(1,1),
                new Vector2Int(-1,1)

                };
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];

                    if (nPos.x > 0 && nPos.x < 9)
                        if (GetBoardAt(nPos) == 0)
                        {
                            results.Add(pos);
                        }
                }
            }
            else if (GetBoardAt(pos) == 3)
            {
                Vector2Int[] increment = new Vector2Int[4]
           {
              new Vector2Int(1,1),
              new Vector2Int(-1,1),
              new Vector2Int(1,-1),
              new Vector2Int(-1,-1)

           };
                Rect boardArea = new Rect();
                boardArea.size = new Vector2(10, 10);
                boardArea.x = -0.5f;
                boardArea.y = -0.5f;
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];
                    while (GetBoardAt(nPos) == 0 && boardArea.Contains(nPos))
                    {
                        results.Add(nPos);
                        nPos += increment[i];
                    }
                }
            }



        }
        else
        {
            if (GetBoardAt(pos) == 2)
            {
                Vector2Int[] increment = new Vector2Int[2]
             {
                new Vector2Int(1,-1),
                new Vector2Int(-1,-1)

             };
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];

                    if (nPos.x > 0 && nPos.x < 9)
                        if (GetBoardAt(nPos) == 0)
                        {
                            results.Add(pos);
                        }
                }
            }
            else if (GetBoardAt(pos) == 4)
            {
                Vector2Int[] increment = new Vector2Int[4]
          {
              new Vector2Int(1,1),
              new Vector2Int(-1,1),
              new Vector2Int(1,-1),
              new Vector2Int(-1,-1)

          };
                Rect boardArea = new Rect();
                boardArea.size = new Vector2(10, 10);
                boardArea.x = -0.5f;
                boardArea.y = -0.5f;
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];
                    while (GetBoardAt(nPos) == 0 && boardArea.Contains(nPos))
                    {
                        results.Add(nPos);
                        nPos += increment[i];
                    }
                }
            }
        }
        return results;
    }
    /// <summary>
    /// Return if a pawn at this position XY can capture and if yes their prey.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public List<(Vector2Int, List<Vector2Int>)> PawnCanCapture(Vector2Int pos)
    {
        List<Vector2Int> possibleCasesToMove = new List<Vector2Int>();
        Vector2Int possibleCapture = new Vector2Int();
        List<(Vector2Int, List<Vector2Int>)> result = new List<(Vector2Int, List<Vector2Int>)>();
        Vector2Int[] increment = new Vector2Int[4]
           {
               new Vector2Int(1,1),
               new Vector2Int(-1,1),
               new Vector2Int(1,-1),
               new Vector2Int(-1,-1)

           };
        //   if (pos.x < 8 && pos.x > 1 && pos.y < 8 && pos.y > 1)
        if (whiteIsPlaying)
        {
            if (GetBoardAt(pos) == 1)
            {
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];

                    if (GetBoardAt(nPos) % 2 == 0 && GetBoardAt(nPos) > 0 && GetBoardAt(nPos + increment[i]) == 0)
                    {

                        possibleCapture = nPos;
                        possibleCasesToMove.Add(nPos + increment[i]);
                        result.Add((possibleCapture, possibleCasesToMove));
                        possibleCasesToMove.Clear();
                    }
                }
            }
            else if (GetBoardAt(pos) == 3)
            {
                Rect boardArea = new Rect();
                boardArea.size = new Vector2(10, 10);
                boardArea.x = -0.5f;
                boardArea.y = -0.5f;
                Vector2Int aPawn = new Vector2Int();
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];
                    aPawn = new Vector2Int(-1, 1);
                    List<Vector2Int> possibleCase = new List<Vector2Int>();
                    while (boardArea.Contains(nPos))
                    {
                        int nCase = GetBoardAt(nPos);
                        if (nCase > 0 && aPawn.x == -1)
                        {
                            if (nCase % 2 == 0 && nCase > 0 && boardArea.Contains(nPos + increment[i]))
                            {
                                if (GetBoardAt(nPos + increment[i]) == 0)
                                {
                                    aPawn = nPos;
                                    nPos += increment[i];
                                    nCase = GetBoardAt(nPos);
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
                        else if (nCase > 0 && aPawn.x >= 0)
                        {
                            break;
                        }
                        if (aPawn.x >= 0) possibleCase.Add(nPos);
                        nPos += increment[i];
                    }
                    if (aPawn.x >= 0) result.Add((aPawn, possibleCase));
                }


            }

        }
        else
        {
            if (GetBoardAt(pos) == 2)
            {
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];
                    if (GetBoardAt(nPos) % 2 == 1 && GetBoardAt(nPos + increment[i]) == 0)
                    {
                        possibleCapture = nPos;
                        possibleCasesToMove.Add(nPos + increment[i]);
                        result.Add((possibleCapture, possibleCasesToMove));
                        possibleCasesToMove.Clear();
                    }
                }
            }
            else if (GetBoardAt(pos) == 3)
            {
                Rect boardArea = new Rect();
                boardArea.size = new Vector2(10, 10);
                boardArea.x = -0.5f;
                boardArea.y = -0.5f;
                Vector2Int aPawn = new Vector2Int();
                for (int i = 0; i < increment.Length; i++)
                {
                    Vector2Int nPos = pos + increment[i];
                    aPawn = new Vector2Int(-1, 1);
                    List<Vector2Int> possibleCase = new List<Vector2Int>();
                    while (boardArea.Contains(nPos))
                    {
                        int nCase = GetBoardAt(nPos);
                        if (nCase > 0 && aPawn.x == -1)
                        {
                            if (nCase % 2 == 1 && boardArea.Contains(nPos + increment[i]))
                            {
                                if (GetBoardAt(nPos + increment[i]) == 0)
                                {
                                    aPawn = nPos;
                                    nPos += increment[i];
                                    nCase = GetBoardAt(nPos);
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
                        else if (nCase > 0 && aPawn.x >= 0)
                        {
                            break;
                        }
                        if (aPawn.x >= 0) possibleCase.Add(nPos);
                        nPos += increment[i];
                    }
                    if (aPawn.x >= 0) result.Add((aPawn, possibleCase));
                }
            }

        }

        return result;
    }
    /// <summary>
    /// Return the round's board to the debug
    /// </summary>
    public void ShowBoard()
    {
        string line = "";
        for (int y = 9; y >= 0; y--)
        {
            line += "|";
            for (int x = 0; x < 10; x++)
            {
                line += board[x, y].ToString() + "|";
            }
            line += "\n";
        }
        Debug.Log(line);
    }
    /// <summary>
    /// return the value of case at Position XY
    /// </summary>
    /// <param name="pos"> Vector2Int which is the position of the case to test</param>
    /// <returns></returns>
    public int GetBoardAt(Vector2Int pos)
    {
        if (pos.x > 9 || pos.x < 0 || pos.y > 9 || pos.y < 0) return -1;
        else return board[pos.x, pos.y];
    }
    /// <summary>
    /// Return an array containing the number of pawns and kings of the color.
    /// </summary>
    /// <param name="isWhite"></param>
    /// <returns></returns>
    public int[] GetNumberOfPawnsKings(bool isWhite)
    {
        int[] nbPawnsKings = new int[2];
        if (isWhite)
        {
            foreach (int i in board)
            {
                if (i == 1) nbPawnsKings[0]++;
                else if (i == 3) nbPawnsKings[1]++;
            }
        }
        else
        {
            foreach (int i in board)
            {
                if (i == 1) nbPawnsKings[0]++;
                else if (i == 3) nbPawnsKings[1]++;
            }
        }
        return nbPawnsKings;
    }/// <summary>
     /// Generate all the possible next rounds !
     /// </summary>
     /// <returns></returns>
    public List<CheckersRound> GenerateRounds()
    {
        List<CheckersRound> result = new List<CheckersRound>();
        for (int i = 0; i < playablePawns.Count; i++)
        {
            if (!canCapture)
            {
                List<Vector2Int> possibleDest = PawnCanMove(playablePawns[i]);
                foreach (Vector2Int pD in possibleDest)
                {

                }
            }
        }

        return result;

    }
    /// <summary>
    /// Simply move the pawn designed by it's position, to the destination, return false and do nothing if the destination is busy
    /// </summary>
    /// <param name="pawn"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public bool MovePawnTo(Vector2Int pawn, Vector2Int destination)
    {

        int pawnColor = GetBoardAt(pawn);
        if (GetBoardAt(destination) == 0)
        {
            board[destination.x, destination.y] = pawnColor;
            board[pawn.x, pawn.y] = 0;
            return true;
        }
        else return false;
    }
    /// <summary>
    /// Simply move the predator to the destination and erase the prey !
    /// </summary>
    /// <param name="predator"></param>
    /// <param name="prey"></param>
    /// <param name="destination"></param>
    public void CaptureAndMovePawnTo(Vector2Int predator, Vector2Int prey, Vector2Int destination)
    {
        int predatorColor = GetBoardAt(predator);
        board[destination.x, destination.y] = predatorColor;
        board[prey.x, prey.y] = 0;
        board[predator.x, predator.y] = 0;
    }
    public void SaveBoard()
    {
        pBoard = board;
    }
    public void UndoMove()
    {
        board = pBoard;
    }
}
