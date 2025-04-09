using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    Camera cam;

    public Game game;

    public GameObject[] board;

    public int held;
    public GameObject mouseObject;
    public int empty = -1;

    Visuals visuals;

    public bool reverse = false;

    public Info info;

    public GameObject gameOverMenu;
    public TextMeshProUGUI winnerText;

    public void Start()
    {
        clearBoard();
        gameOverMenu.SetActive(false);
        visuals = GetComponent<Visuals>();

        game = new Game(); //create new game

        game.mustEat = info.forcedEat; //update values
        if (info.color)
        {
            reverse = true;
            game.turn = 1;
        }

        Board.getStartingBoard(ref game); //set starting locations
        Board.updateLegalMoves(ref game); //get legal moves

        mouseObject = Util.createGO(visuals.circle); //create a game object which would be hidden and will only show when the mouse is pressed
        Util.getRenderer(mouseObject).enabled = false; //hide the object
        Util.getRenderer(mouseObject).sortingOrder = 3; //make it show above all

        GameObject crw = Util.createGO(visuals.crown);
        Util.getRenderer(crw).sortingOrder = 4;
        Util.getRenderer(crw).enabled = false;
        crw.transform.parent = mouseObject.transform;


        held = empty;

        cam = Camera.main;

        board = new GameObject[64]; //create board objects
        visuals.createBoard();   
    }

    

    
    void Update()
    {
        visuals.updateBoardPos(); //update board visuals
        visuals.showBoard(game); //show pieces on board

        if (game.gameOver)
        {
            gameOverScreen();
            return;
        }

        

        if (game.turn % 2 == 0 || info.diff == 0)
        {
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
                    {
                        m.enabled = true;//show mouse piece 
                        SpriteRenderer crw = Util.getRenderer(mouseObject.transform.GetChild(0).gameObject);
                        if (Board.isKing(held, game))
                            crw.enabled = true;
                        else
                            crw.enabled = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {

                int drop = ((int)((mousePos.y + visuals.size * 4) / visuals.size)) * 8 + (int)((mousePos.x + visuals.size * 4) / visuals.size); //find drop position
                if (!drop.Equals(held))
                {
                    if (!Util.outOfBounds(drop))
                    {
                        Board.makeMove(held, drop, ref game); //make the move (function checks if possible)
                    }

                }

                held = empty;
                Util.getRenderer(mouseObject).enabled = false; //hide mouse piece 
                Util.getRenderer(mouseObject.transform.GetChild(0).gameObject).enabled = false;


            }
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 0.25f;
                game = Bot.bestMove(game, info.diff);
            }
        }
    }

    float timer = 0.25f;


    public GameObject getPiece(int i) //get visual piece
    {
        return board[i].transform.GetChild(0).gameObject;
    }
    
    public void gameOverScreen()
    {
        gameOverMenu.SetActive(true);
        if (!reverse)
        {
            if (game.winner)
                winnerText.text = "White Won!";
            else
                winnerText.text = "Black Won!";
        }
        else
        {
            if (game.winner)
                winnerText.text = "Black Won!";
            else
                winnerText.text = "White Won!";
        }
    }

    public void returnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    void clearBoard()
    {
        if (game == null || !game.gameOver)
            return;

        for (int i = 0; i < 64; i++)
        {
            if (board[i] != null)
                Destroy(board[i].gameObject);
        }
    }
}
