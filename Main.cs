using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Main : MonoBehaviour
{
    Camera cam;

    public Sprite circle;
    public Sprite rect;
    public Sprite hexagon;

    public Game game;

    public GameObject[] board;

    public int held;
    public GameObject mouseObject;
    public int empty = -1;

    Visuals visuals;

    int turn = 0;
    public bool reverse = false;
    public bool mustEat = false;

    void Start()
    {
        visuals = GetComponent<Visuals>();

        game = new Game(); //create new game
        game.mustEat = mustEat; //update values
        Board.getStartingBoard(ref game); //set starting locations
        Board.updateLegalMoves(ref game); //get legal moves

        mouseObject = Util.createGO(circle); //create a game object which would be hidden and will only show when the mouse is pressed
        Util.getRenderer(mouseObject).sprite = null; //hide the object
        Util.getRenderer(mouseObject).sortingOrder = 2; //make it show above all

        held = empty;

        cam = Camera.main;

        board = new GameObject[64]; //create board objects
        visuals.createBoard();   
    }

    

    
    void Update()
    {
        visuals.updateBoardPos(); //update board visuals
        visuals.showBoard(game); //show pieces on board

        Vector3 p = Input.mousePosition;//update position of the mouse piece
        p.z = 10;
        Vector3 mousePos = cam.ScreenToWorldPoint(p);
        mouseObject.transform.position = mousePos; 

        if (Input.GetMouseButtonDown(0))
        {
            if (held != empty)
            {
                SpriteRenderer r = Util.getRenderer(getPiece(held)); //hide clicked piece
                SpriteRenderer m = Util.getRenderer(mouseObject);
                m.color = r.color;
                if (!Board.isEmpty(held, game))
                    m.sprite = circle;//show mouse piece 
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            int drop = ((int) ((mousePos.y + visuals.size * 4) / visuals.size)) * 8 + (int)((mousePos.x + visuals.size * 4) / visuals.size); //find drop position
            if (!drop.Equals(held))
            {
                if (!Util.outOfBounds(drop) )
                    Board.makeMove(held, drop, ref game); //make the move (function checks if possible)
            }
            
            held = empty;
            Util.getRenderer(mouseObject).sprite = null; //hide mouse piece 

        }

    }

    public GameObject getPiece(int i) //get visual piece
    {
        return board[i].transform.GetChild(0).gameObject;
    }

}
