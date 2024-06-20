using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionVfx : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] AudioClip[] audioClips;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(audioClips[0],0.5f);
        Destroy(this.gameObject, 1.0f);
    }
}
