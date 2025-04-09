using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    public TextMeshProUGUI diff;
    public Button colorBTN;

    public Button startButton;
    public Button backButton;

    private Color startColor;
    private Color endColor;
    private float tColor;

    public Image eatSlider;
    public Image eatBar;

    public Vector3 offset;
    private Vector3 eatSliderOff;
    private Vector3 eatSliderOn;
    

    public Color colorOffCircle;
    public Color colorOnCircle;
    public Color colorOffBar;
    public Color colorOnBar;


    public float timeToSwitchSlider;
    public float timeToSwitchColor;

    private bool eat;
    private bool color; //false = white; true = black


    float t;
    Vector3 startPos;
    Vector3 target;

    Color startCirc;
    Color endCirc;
    Color startBar;
    Color endBar;

    public Info info;

    void Start()
    {
        startColor = endColor = Color.white; //setup starting variables for animations

        target = startPos = eatSlider.transform.position;
        eatSliderOff = startPos;
        eatSliderOn = startPos + offset;

        eatSlider.color = colorOffCircle;
        eatBar.color = colorOffBar;
        startCirc = endCirc = colorOffCircle;
        startBar = endBar = colorOffBar;
    }

    void Update()
    {
        if (timeToSwitchSlider < 0) //minimum time
            timeToSwitchSlider = 0.01f;
        if (timeToSwitchColor < 0)
            timeToSwitchColor = 0.01f;

        t += Time.deltaTime / timeToSwitchSlider;
        tColor += Time.deltaTime / timeToSwitchColor; //do animations
         
        colorBTN.image.color = Color.Lerp(startColor, endColor, tColor); 

        eatSlider.transform.position = Vector3.Lerp(startPos, target, t);
        eatSlider.color = Color.Lerp(startCirc, endCirc, t);
        eatBar.color = Color.Lerp(startBar, endBar, t);

    }

    public void updateDiff(float val)
    {
        if (val == 1)
            diff.text = "vs Player";
        else
            diff.text = (val-1).ToString();
    }

    public void setColorBTN()
    {
        if (!(colorBTN.image.color == Color.white || colorBTN.image.color == Color.black))
            return;
        color = !color;
        if (color) //setup animation
        {
            startColor = Color.white;
            endColor = Color.black;
        }
        else
        {
            startColor = Color.black;
            endColor = Color.white;
        }
        tColor = 0;
    }

    public void eatToggle()
    {
        Vector3 pos = eatSlider.transform.position;
        if (!(pos == eatSliderOff || pos == eatSliderOn))
            return;
        startPos = pos;
        if (eat) { //setup animation
            target = startPos - offset;

            startCirc = colorOnCircle;
            endCirc = colorOffCircle;
            startBar = colorOnBar;
            endBar = colorOffBar;
        }
        else
        {
            target = startPos + offset;

            startCirc = colorOffCircle;
            endCirc = colorOnCircle;
            startBar = colorOffBar;
            endBar = colorOnBar;
        }
        t = 0;
        eat = !eat;
    }

    public void startGame()
    {
        if (diff.text.Equals("vs Player"))
            info.diff = 0;
        else
            info.diff = int.Parse(diff.text);
        info.color = color;
        info.forcedEat = eat;
        SceneManager.LoadScene("Checkers");
    }
}
