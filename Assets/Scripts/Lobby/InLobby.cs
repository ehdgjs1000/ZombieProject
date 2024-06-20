using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InLobby : MonoBehaviour
{
    public GameObject plainPrefab;
    [SerializeField] GameObject plainStartPos;

    public GameObject[] vfxPos;
    public GameObject[] vfxPrefabs;

    float plainSpawnTime;

    [SerializeField] AudioClip audioClip;
    AudioSource audioSource;


    public void PlayBtnOnClick()
    {
        SceneManager.LoadScene("IntroScene");
    }
    public void EndGameBtnOnClick()
    {
        Application.Quit();
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
        plainSpawnTime = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        plainSpawnTime -= Time.deltaTime;
        if (plainSpawnTime < 0)
        {
            Instantiate(plainPrefab, plainStartPos.transform.position, Quaternion.identity);
            spawnExplosionVFX();
            plainSpawnTime = 4.2f;
        }

    }
    void spawnExplosionVFX()
    {
        int ranPos = Random.Range(0, vfxPos.Length);
        

        int ranVFX = Random.Range(0, vfxPrefabs.Length);

        GameObject go = Instantiate(vfxPrefabs[ranVFX], vfxPos[ranPos].transform.position, Quaternion.identity);
        Destroy(go, 4.2f);

    }


}
