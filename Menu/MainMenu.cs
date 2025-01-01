using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public GameObject startMenu;

    public void returnBTN()
    {
        this.gameObject.active = true;
        startMenu.active = false;
    }

    public void PlayBTN()
    {
        this.gameObject.active = false;
        startMenu.active = true;
    }
}
