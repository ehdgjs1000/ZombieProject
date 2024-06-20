using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] Text introText;
    [SerializeField] Text explainText;

    private void Start()
    {
        StartCoroutine(PlayIntroText());
    }
    IEnumerator PlayIntroText()
    {
        introText.text = "�˼� ���� ���̷����� ������....";
        yield return new WaitForSeconds(4.0f);
        introText.text = "���̷����� �ذ� �� �� �ִ� ���� �ִٰ� �����";
        yield return new WaitForSeconds(4.0f);
        introText.text = "�װ����� ���ϴ� ����...";
        yield return new WaitForSeconds(4.0f);
        introText.text = "������ �߻��ϰ� �� �����ߴ�";
        yield return new WaitForSeconds(4.0f);

        SceneManager.LoadScene("GameScene");
    }

}
