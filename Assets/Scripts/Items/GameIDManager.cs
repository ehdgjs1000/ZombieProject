using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIDManager : MonoBehaviour
{
    public static GameIDManager instance;

    public List<string> isIngridentPicked = new List<string>();
    public List<string> isGenericPharmed = new List<string>();
    public List<string> isZombieDie = new List<string>();


    private void Awake()
    {
        instance = this;
    }
}
