using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class EnemyController : MonoBehaviour
{
    // fields
    [SerializeField] private ParticleSystem bloodParticles;
    [SerializeField] private ParticleSystem deathBloodParticles;
    [SerializeField] private float health = 100.0f;

    // components
    private BulletTrigger[] bulletTriggers;
    private CharacterController characterController;
    private Renderer mainRenderer;

    // data
    public UnityEvent OnDeath {get; private set;} = new();
    public UnityEvent<BulletHit> OnBulletHit {get; private set;} = new();
    public bool IsDead { get; private set; } = false;
    private float destroyTimer = 5.0f;
    private float maxHealth = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        characterController = GetComponent<CharacterController>();
        bulletTriggers = GetComponentsInChildren<BulletTrigger>();

        // get main renderere
        mainRenderer = GetComponentInChildren<Renderer>();

        foreach (var trigger in bulletTriggers)
        {
            trigger.OnBulletHit.AddListener((BulletHit hit) => {
                if (IsDead) return;

                health -= hit.Damage;
                if (health < 0.0f)
                {
                    IsDead = true;
                    EmitBlood(deathBloodParticles, hit);
                    OnDeath.Invoke();
                    SlowMotionManager.SlowMotion(0.3f * hit.Damage / maxHealth, 0.2f);
                    characterController.enabled = true;
                }
                
                // blood particles
                EmitBlood(bloodParticles, hit);

                // emit signal
                OnBulletHit.Invoke(hit);
            });

        }
    }

    void Update()
    {
        if (!IsDead) return;
        
        // destroy after some time to free memory
        destroyTimer -= Time.deltaTime;
        if (destroyTimer < 0.0f && !mainRenderer.isVisible)
        {
            Destroy(gameObject);
        }
        
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
