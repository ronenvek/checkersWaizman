using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visuals : MonoBehaviour
{
    public Color bright = new Color(240 / 255f, 217 / 255f, 181 / 255f);
    public Color dark = new Color(181 / 255f, 136 / 255f, 99 / 255f);
    public float size = 1f;
    public float pieceRatio = 0.8f;

    public Color topColor = Color.black;
    public Color bottomColor = Color.white;

    private Main main;

    Sprite circle;
    Sprite rect;
    Sprite crown;

    float height;
    float width;

    void Awake()
    {
        main = GetComponent<Main>();

        rect = main.rect;
        circle = main.circle;
        crown = main.crown;

        Camera cam = Camera.main;
        height = cam.orthographicSize * 2f;
        width = height * cam.aspect;
    }

    public void createBoard()
    {
        string[] name = { "a", "b", "c", "d", "e", "f", "g", "h" };
        int k = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++) //create board visuals
            {
                Color c = dark;
                if ((i + j) % 2 == 0)
                    c = bright; //choose color


                main.board[k] = Util.createGO(rect); //create square and set its color and name, add to array
                main.board[k].GetComponent<SpriteRenderer>().color = c;
                main.board[k].name = name[j] + (i + 1);

                GameObject g = Util.createGO(circle); //create circle and make it appear above square, give it collider, script, and name, set as child of square
                Util.getRenderer(g).sortingOrder = 1;
                g.name = (k).ToString();
                g.AddComponent<CircleCollider2D>();
                g.AddComponent<Piece>();
                g.transform.parent = main.board[k].transform;
                

                GameObject crw = Util.createGO(crown);
                Util.getRenderer(crw).sortingOrder = 2;

                crw.transform.parent = g.transform;
                k++;
            }
        }
    }

    public void updateBoardPos()
    {
        size = Mathf.Max(size, 0.1f); //set max and min size
        size = Mathf.Min(height / 8, width / 8, size);

        pieceRatio = Mathf.Max(0.1f, pieceRatio); //max and min piece ratio 
        pieceRatio = Mathf.Min(0.9f, pieceRatio);

        Util.scale(main.mouseObject, size * pieceRatio);

        int k = 0;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Vector2 rectpos = new Vector2(j * size - size * 3.5f, i * size - size * 3.5f); 
                
                GameObject go = main.board[k];
                Util.getRenderer(go).color = dark;
                if ((i + j) % 2 == 0)
                {
                    Util.getRenderer(go).color = bright;
                }
                go.transform.position = rectpos;//update square location and color
                Util.scale(go, size); //change scale
                Util.localScale(main.getPiece(k), pieceRatio); //scale piece based on square

                k++;
            }
        }
    }

    public void highlight(int i, Color color, float ratio) //highlight a square (to show that its possible to move there)
    {
        GameObject p = main.getPiece(i);
        SpriteRenderer r = Util.getRenderer(p);
        r.sprite = main.hexagon;
        r.color = color;
        Util.localScale(p, ratio);
    }


    public void showBoard(Game game) //shows the pieces (need to change the name of the function)
    {
        ulong m = 1;
        int k = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                
                SpriteRenderer p = Util.getRenderer(main.getPiece(k));
                SpriteRenderer crw = Util.getRenderer(main.getPiece(k).transform.GetChild(0).gameObject);
                p.sprite = null;
                crw.enabled = false;

                if (k != main.held) //dont show piece if its held 
                {
                    if ((game.top & m) != 0)
                    {
                        p.sprite = circle; //set sprite and color
                        p.color = topColor;
                        if (main.reverse)
                            p.color = bottomColor;


                    }
                    if ((game.bottom & m) != 0)
                    {
                        p.sprite = circle;
                        p.color = bottomColor;
                        if (main.reverse)
                            p.color = topColor;

                    }

                    if ((game.kings & m) != 0)
                        crw.enabled = true;
                    
    
                }
                
                m <<= 1;
                k++;
            }
        }

        if (main.held != -1){
            highlightLegalMoves(game, 0, 18, 0); //check if move is legal for held piece and highlight if it is
            highlightLegalMoves(game, 1, 14, 0);
            highlightLegalMoves(game, 2, -18, 0);
            highlightLegalMoves(game, 3, -14, 0);
            highlightLegalMoves(game, 4, -18, 1);
            highlightLegalMoves(game, 5, -14, 1);
            highlightLegalMoves(game, 6, 18, 1);
            highlightLegalMoves(game, 7, 14, 1);
            highlightLegalMoves(game, 8, 9, 0);
            highlightLegalMoves(game, 9, 7, 0);
            highlightLegalMoves(game, 10, -9, 0);
            highlightLegalMoves(game, 11, -7, 0);
            highlightLegalMoves(game, 12, -9, 1);
            highlightLegalMoves(game, 13, -7, 1);
            highlightLegalMoves(game, 14, 9, 1);
            highlightLegalMoves(game, 15, 7, 1);
        }
    }

    private void highlightLegalMoves(Game game, int loc, int offset, int turn)
    {
        if (Board.isMovesArrayIndex(main.held, loc, game) && game.turn % 2 == turn)
            highlight(main.held + offset, Color.red, pieceRatio * pieceRatio);
    }

}
