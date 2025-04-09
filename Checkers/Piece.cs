using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    Main main;
    int i;
    void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
        string name = this.name; 
        i = int.Parse(name);
    }


    void OnMouseDown()
    {
        if (main.game.gameOver || (main.info.diff != 0 && main.game.turn %2 != 0))
            return;
        string name = this.name; //when clicking on a piece, select the held to the piece's name (which is the location)
        main.held = i;
    }

}
