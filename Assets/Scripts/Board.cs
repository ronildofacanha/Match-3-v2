using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;

    private TB[,] allTile;
    void Start()
    {
        allTile = new TB[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }

    private bool MatchesAt(int x, int y, GameObject piece)
    {
        if (x > 1 && y > 1)
        {
            if (allDots[x - 1, y].tag == piece.tag && allDots[x - 2, y].tag == piece.tag)
            {
                return true;
            }
            if (allDots[x, y - 1].tag == piece.tag && allDots[x, y - 2].tag == piece.tag)
            {
                return true;
            }

            if (x <= 1 || y <= 1)
            {
                if (x >= 1)
                {
                    if (allDots[x - 1, y].tag == piece.tag && allDots[x - 2, y].tag == piece.tag)
                    {
                        return true;
                    }
                }

                if (y >= 1)
                {
                    if (allDots[x, y - 1].tag == piece.tag && allDots[x, y - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    void Update()
    {

    }

    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPos = new Vector2(i, j);
                GameObject TB = Instantiate(tilePrefab, tempPos, Quaternion.identity);
                TB.transform.parent = transform;
                TB.name = "(" + i + "," + j + ")";
                // Dots
                
                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;

                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }

                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.transform.parent = transform;
                dot.name = TB.name;
                allDots[i, j] = dot;
            }
        }
    }

    private void DestroyMatchesAt(int x, int y)
    {
        if (allDots[x, y].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[x, y]);
            allDots[x, y] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for(int j=0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(RefillBoardCo());
    }

    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j=0; j< height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    piece.transform.parent = transform;
                    piece.name = "PEÇA NOVA";
                    allDots[i, j] = piece;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator RefillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
    }
}
