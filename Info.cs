using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Info : ScriptableObject
{
    [Range(0, 9)]
    public int diff;
    public bool color;
    public bool forcedEat;
}
