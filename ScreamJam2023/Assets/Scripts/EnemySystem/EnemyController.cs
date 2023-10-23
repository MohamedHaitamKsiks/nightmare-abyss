using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    // fields
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private ParticleSystem deathBloodParticles;

    // components
    private BoxCollider boxCollider;
    private BulletTrigger bulletTrigger;
    private CharacterController characterController;

    // data
    public UnityEvent OnDeath;
    private float health = 100.0f;

    // Start is called before the first frame update
    void Start()
    {
        bulletTrigger = GetComponent<BulletTrigger>();
        boxCollider = GetComponent<BoxCollider>();
        characterController = GetComponent<CharacterController>();

        bulletTrigger.OnBulletHit.AddListener((BulletHit hit) => {
            health -= hit.Damage;
            if (health < 0.0f)
            {
                EmitBlood(deathBloodParticles, hit);
                OnDeath.Invoke();
                boxCollider.enabled = false;
                characterController.enabled = true;

                return;
            }
            
            // blood particles
            EmitBlood(bloodParticles, hit);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // play blood particles
    void EmitBlood(ParticleSystem particles, BulletHit hit)
    {
        particles.transform.SetPositionAndRotation(
            hit.Position,
            Quaternion.LookRotation(hit.Normal)
        );
        particles.Play();
    }
}
