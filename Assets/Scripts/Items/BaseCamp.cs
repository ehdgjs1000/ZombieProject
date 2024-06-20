using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCamp : MonoBehaviour
{
    private float baseCampDistance = 20.0f;
    Vector3 basePos;
    private void Awake()
    {
        basePos = transform.position;
    }

    public void SaveBasePos()
    {

    }

}
