using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    //[Range(-30, 0)]
    public float value = -30.0f;
    public float speed;

    public float margin = 40.0f;

    public RectTransform top, bottom, left, right, center;

    // Update is called once per frame
    void Update()
    {
        CheckIsAiming();
    }
    void CheckIsAiming()
    {
        if (ThirdPersonController.Instance.ReturnSpeed() >= 0.3f)
            value = -30.0f;
        else
            value = -70.0f;

        if (ThirdPersonController.Instance.ReturnIsAiming() == true)
        {
            float topValue, bottomValue, leftValue, rightValue;

            topValue = Mathf.Lerp(top.position.y, center.position.y + margin + value, speed * Time.deltaTime);
            bottomValue = Mathf.Lerp(bottom.position.y, center.position.y - margin - value, speed * Time.deltaTime);
            leftValue = Mathf.Lerp(left.position.x, center.position.x - margin - value, speed * Time.deltaTime);
            rightValue = Mathf.Lerp(right.position.x, center.position.x + margin + value, speed * Time.deltaTime);

            top.position = new Vector2(top.position.x, topValue);
            bottom.position = new Vector2(bottom.position.x, bottomValue);

            left.position = new Vector2(leftValue, center.position.y);
            right.position = new Vector2(rightValue, center.position.y);
        }
        else
        {
            float topValue, bottomValue, leftValue, rightValue;

            topValue = Mathf.Lerp(top.position.y, center.position.y + margin, speed * Time.deltaTime);
            bottomValue = Mathf.Lerp(bottom.position.y, center.position.y - margin, speed * Time.deltaTime);
            leftValue = Mathf.Lerp(left.position.x, center.position.x - margin, speed * Time.deltaTime);
            rightValue = Mathf.Lerp(right.position.x, center.position.x + margin, speed * Time.deltaTime);

            top.position = new Vector2(top.position.x, topValue);
            bottom.position = new Vector2(bottom.position.x, bottomValue);

            left.position = new Vector2(leftValue, center.position.y);
            right.position = new Vector2(rightValue, center.position.y);
        }
    }

}
