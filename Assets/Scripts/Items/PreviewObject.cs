using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>();

    [SerializeField] int layerGround;
    const int IGNORE_RAYCAST_LAYER = 2;

    [SerializeField] Material green;
    [SerializeField] Material red;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChangeColor();
    }
    private void ChangeColor()
    {
        int nearBaseCount = 0;
        if (gameObject.name.Contains("BaseCamp")) nearBaseCount++;
        for (int i = 0; i < CharacterManager.basePos.Length; i++)
        {
            if (Vector3.Distance(transform.position, CharacterManager.basePos[i]) <= 10)
            {
                nearBaseCount++;
            }
        }
        if (nearBaseCount > 0 && colliderList.Count == 0)
        {
            SetColor(green);
        }
        else SetColor(red);
        if (gameObject.name.Contains("BaseCamp") && CharacterManager.baseCount == 3)
        {
            SetColor(red);
        }
    }
    private void SetColor(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    private void OnTriggerEnter(Collider other)
    {
        //&& other.gameObject.layer == IGNORE_RAYCAST_LAYER
        if (other.gameObject.layer != layerGround)
        {
            colliderList.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layerGround)
        {
            colliderList.Remove(other);
        }
    }

    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }

}
