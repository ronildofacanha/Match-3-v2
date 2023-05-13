using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentmatches = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot != null)
                {
                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isRowBomb
                                    || leftDot.GetComponent<Dot>().isRowBomb
                                    || rightDot.GetComponent<Dot>().isRowBomb)
                                {
                                    currentmatches.Union(GetRowPieces(j));
                                }

                                if (!currentmatches.Contains(leftDot))
                                {
                                    currentmatches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;

                                if (!currentmatches.Contains(rightDot))
                                {
                                    currentmatches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;
                                if (!currentmatches.Contains(currentDot))
                                {
                                    currentmatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;

                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null && downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (!currentmatches.Contains(upDot))
                                {
                                    currentmatches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;

                                if (!currentmatches.Contains(downDot))
                                {
                                    currentmatches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;

                                if (!currentmatches.Contains(currentDot))
                                {
                                    currentmatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                }
            }
        }

    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int j = 0; j < board.height; j++)
        {
            if (board.allDots[column, j] != null)
            {
                dots.Add(board.allDots[column, j]);
                board.allDots[column, j].GetComponent<Dot>().isMatched = true;            }
        }

        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                dots.Add(board.allDots[i,row]);
                board.allDots[i,row].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }
}
