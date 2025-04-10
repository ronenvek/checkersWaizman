
using UnityEngine;

public class Util
{


    public static GameObject createGO(Sprite sprite) //create empty game object
    {
        GameObject go = new GameObject();
        go.AddComponent<SpriteRenderer>();
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        return go;
    }
    public static void scale(GameObject go, float size) //scale game object by value
    {
        Vector2 s = go.transform.lossyScale;
        s.x = size;
        s.y = size;
        go.transform.localScale = s;
    }

    public static void localScale(GameObject go, float scale) //scale relative to game object
    {
        go.transform.localScale = new Vector3(scale, scale, scale);
    }


    public static SpriteRenderer getRenderer(GameObject go) //get renderer of game object
    {
        return go.GetComponent<SpriteRenderer>();
    }

    public static bool outOfBounds(int i) //check if location is within bounds
    {
        return i < 0 || i > 63;
    }

    private static int[] bitCount = {0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4};

    public static int count(ulong num)
    {
        if (num == 0)
            return 0;
        return bitCount[num & 15] + count(num >> 4);
    }
}
