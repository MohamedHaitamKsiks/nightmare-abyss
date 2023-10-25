using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimator : MonoBehaviour
{
    // fields
    [SerializeField] private Animator animator;

    // components
    private EnemyController enemyController;
    private PlayerFollower playerFollower;
    private BoxCollider boxCollider;
    
    // data
    private Rigidbody[] ragdollBodies;

    // Start is called before the first frame update
    void Start()
    {
        playerFollower = GetComponent<PlayerFollower>();
        boxCollider = GetComponent<BoxCollider>();

        // init ragdoll
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var body in ragdollBodies)
            body.isKinematic = true;
        
        // on death
        enemyController = GetComponent<EnemyController>();
        enemyController.OnDeath.AddListener(() => {

            // enable ragdoll
            foreach (var body in ragdollBodies)
                body.isKinematic = false;
            
            animator.enabled = false;
            boxCollider.enabled = false;
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.enabled) return;

        animator.SetFloat("velocity", playerFollower.MovemnetSpeed);
    }
}
