using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    private AudioSource source;

    void Awake()
    {
        //source = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    //public void StepNoise()
    //{
    //    if (!source.isPlaying)
    //        source.PlayOneShot(source.clip);
    //}
}

