using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column, row, currentX, currentY, auxPreviousColumn, auxPreviousRow;

    public bool isMatched = false;
    private Board board;
    private FindMatches findMatches;
    private GameObject otherDot;
    private Vector2 firstTouchPosition, lastTouchPosition, tempPos;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = .05f;

    [Header("Powerups Stuff")]
    public bool isColumnBomb, isRowBomb;
    public GameObject rowArrow, columnArrow;
    
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //column = currentX = (int)transform.position.x;
        //row = currentY = (int)transform.position.y;
        isColumnBomb = false;
        isRowBomb = false;
    }

    //this is for testing and debug only
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = transform;
        }
    }
  
    // Update is called once per frame
    void Update()
    {
        //FindMatches();

        /*GameObject parentObject = GameObject.Find("Board");
        gameObject.name = "DOT | (" + column + "," + row + ")";
        gameObject.transform.parent = parentObject.transform;
        */
        currentX = column;
        currentY = row;

        if (Mathf.Abs(currentX - transform.position.x) > .1)
        {
            //Move towrds the target
            tempPos = new Vector2(currentX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);

            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
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
            findMatches.FindAllMatches();
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
        if (board.currentGameState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp()
    {

        lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (board.currentGameState == GameState.move)
        {
            board.currentGameState = GameState.wait;
            
            CalculateAngle();
            
            if (Mathf.Abs(lastTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(lastTouchPosition.x - firstTouchPosition.x) > swipeResist)
            {
                MovePieces();
            }
            else
            {
                board.currentGameState = GameState.move;
            }
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
            auxPreviousRow = row;
            auxPreviousColumn = column;
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;

        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            otherDot = board.allDots[column, row + 1];
            auxPreviousRow = row;
            auxPreviousColumn = column;
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;

        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            otherDot = board.allDots[column - 1, row];
            auxPreviousRow = row;
            auxPreviousColumn = column;
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            //Down Swipe
            otherDot = board.allDots[column, row - 1];
            auxPreviousRow = row;
            auxPreviousColumn = column;
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }

        StartCoroutine(CheckMoveCurrentDot());
    }

    public IEnumerator CheckMoveCurrentDot()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = auxPreviousRow;
                column = auxPreviousColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            otherDot = null;
            yield return new WaitForSeconds(.5f);
            board.currentGameState = GameState.move;
        }
    }

    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];

            if (leftDot1 != null && rightDot1 != null && gameObject !=null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag && leftDot1.tag == rightDot1.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    gameObject.GetComponent<Dot>().isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];

            if (upDot1 != null && downDot1 != null && gameObject != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag && upDot1.tag == downDot1.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    gameObject.GetComponent<Dot>().isMatched = true;
                }
            }
        }

        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            Color currentColor = mySprite.color;
            //mySprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, .5f);
            mySprite.color = new Color(255f, 255f, 255f, 1f);
            gameObject.tag = "void";
        }

    }
}
