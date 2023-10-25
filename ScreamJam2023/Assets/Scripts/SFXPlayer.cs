using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SFXPlayer : MonoBehaviour
{

    [SerializeField] private float pitchVariation = 0.0f;
    public AudioSource Source {get; private set;}
    private float initialPitch = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Source = GetComponent<AudioSource>();
        initialPitch = Source.pitch;
    }

    public void Play()
    {
        Source.pitch = initialPitch + Random.Range(-1.0f, 1.0f) * pitchVariation;
        Source.Play();
    }

}
