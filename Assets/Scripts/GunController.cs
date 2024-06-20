using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField]
    private Gun currentGun;

    //연사 속도
    private float currentFireRate;

    private bool isReload = false;

    [HideInInspector] public bool isFineSightMode = false;

    private Vector3 originPos;
    private AudioSource audioSource;
    private RaycastHit hitInfo;

    [SerializeField] private Camera theCam;
    private CrossHair theCrossHair;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CancelReload()
    {
        if (isReload)
        {
            StopAllCoroutines();
            isReload = false;
        }
    }

}
