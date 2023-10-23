using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimator : MonoBehaviour
{
    // fields
    [SerializeField] private Animator animator;

    // components
    private EnemyController enemyController;
    
    // data
    private Rigidbody[] ragdollBodies;

    // Start is called before the first frame update
    void Start()
    {
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
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
