using UnityEngine;

public class PlainCtrl : MonoBehaviour
{
    Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlain();
    }
    void MovePlain()
    {
        rigid.velocity = transform.forward * 100f;
        Destroy(gameObject, 4.0f);
    }


}
