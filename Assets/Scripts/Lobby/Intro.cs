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
        introText.text = "알수 없는 바이러스가 퍼졌다....";
        yield return new WaitForSeconds(4.0f);
        introText.text = "바이러스를 해결 할 수 있는 곳이 있다고 들었다";
        yield return new WaitForSeconds(4.0f);
        introText.text = "그곳으로 향하던 도중...";
        yield return new WaitForSeconds(4.0f);
        introText.text = "지진이 발생하고 난 기절했다";
        yield return new WaitForSeconds(4.0f);

        SceneManager.LoadScene("GameScene");
    }

}
