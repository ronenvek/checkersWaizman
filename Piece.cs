using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    Main main;
    void Awake()
    {
        main = GameObject.Find("Main").GetComponent<Main>();
    }

    void OnMouseDown()
    {
        string name = this.name; //when clicking on a piece, select the held to the piece's name (which is the location)
        int i = int.Parse(name);
        main.held = i;
    }

}
