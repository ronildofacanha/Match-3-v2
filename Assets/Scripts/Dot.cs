using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column, row, myColumn, myRow, currentX,currentY;
    public float swipeAngle = 0;
    private float swipeResist = .05f;
    public bool isMatched = false;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition, lastTouchPosition,tempPos;
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        myColumn = column = currentX = (int)transform.position.x;
        myRow = row = currentY = (int)transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();

        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            Color currentColor = mySprite.color;
            //mySprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, .5f);
            mySprite.color = new Color(0f,0f,0f, .5f);
            gameObject.tag = "void";
        }

        currentX = column;
        currentY = row;

        if (Mathf.Abs(currentX - transform.position.x) > .1)
        {
            //Move towrds the target
            tempPos = new Vector2(currentX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);

            if (board.allDots[column,row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(currentX, transform.position.y);
            transform.position = tempPos;
        } 

        if (Mathf.Abs(currentY - transform.position.y) > .1)
        {
            //Move towrds the target
            tempPos = new Vector2(transform.position.x, currentY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
        }
        else
        {
            //Directly set the position
            tempPos = new Vector2(transform.position.x, currentY);
            transform.position = tempPos;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
     if(Mathf.Abs(lastTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(lastTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            CalculateAngle();
            MovePieces();
        }
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(lastTouchPosition.y - firstTouchPosition.y, lastTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Right Swipe
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCurrentDot());
    }

    public IEnumerator CheckMoveCurrentDot()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null) { 
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = myRow;
                column = myColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
        }
    } 

    void FindMatches()
    {
        if(column>0 && column < board.width-1)
        {
            GameObject leftDot = board.allDots[column-1, row];
            GameObject rightDot = board.allDots[column+1, row];

            if(leftDot != null && rightDot != null)
            {
                if(leftDot.tag == gameObject.tag && rightDot.tag == gameObject.tag)
                {
                    leftDot.GetComponent<Dot>().isMatched = true;
                    rightDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row>0 && row < board.width - 1)
        {
            GameObject downdot = board.allDots[column, row-1];
            GameObject upDot = board.allDots[column, row + 1];

            if(downdot != null && upDot != null)
            {
                if(downdot.tag ==gameObject.tag && upDot.tag == gameObject.tag)
                {
                    downdot.GetComponent<Dot>().isMatched = true;
                    upDot.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }
}
