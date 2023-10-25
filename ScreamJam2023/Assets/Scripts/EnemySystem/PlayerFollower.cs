using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController), typeof(BulletTrigger))]
public class PlayerFollower : MonoBehaviour
{   
    // contst
    private const float ROTATION_SPEED = 10.0f;

    // fields
    [SerializeField] private float acceleration = 0.0f;
    [SerializeField] private float maxSpeed = 0.0f;
    [SerializeField] private float tickRate = 16.0f;

    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;

    [SerializeField] private float viewDistance = 200.0f;
    [SerializeField] private Transform eyesPosition;
    [SerializeField] private LayerMask eyesLayerMask;

    // components
    private CharacterController characterController;
    private EnemyController enemyController;

    // data
    public float MovemnetSpeed {get; private set;} = 0.0f;
    private GameObject player = null;
    private Vector3 velocity = Vector3.zero;
    private Vector3 target = Vector3.zero;
    private float followTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        characterController = GetComponent<CharacterController>();
        enemyController = GetComponent<EnemyController>();
        enemyController.OnBulletHit.AddListener(OnBulletHit);

        target = transform.position;
    }

    private void OnBulletHit(BulletHit hit)
    {
        velocity += 0.1f * hit.Damage * Vector3.Scale(hit.Direction, GetLockVector());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.IsDead) return;

        // update target at tick
        followTimer -= Time.deltaTime;
        if (followTimer < 0.0f)
        {
            UpdateTarget();
            followTimer = 1.0f / tickRate;
        }

        var lockVector = GetLockVector();
        var direction = Vector3.Scale(target - transform.position, lockVector).normalized;
        
        // check if there is no wall between enemy and player
        var playerVisible =  Vector3.Distance(transform.position, target) < viewDistance && !Physics.Raycast(eyesPosition.position, direction, viewDistance, eyesLayerMask);

        // move to target
        var targetSpeed = playerVisible? maxSpeed: 0.0f;
        velocity += (direction * targetSpeed - velocity) * (acceleration * Time.deltaTime);

        // look at velocity
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * ROTATION_SPEED);

        // apply velocity
        velocity = Vector3.Scale(velocity, lockVector);
        MovemnetSpeed = velocity.magnitude / maxSpeed;
        characterController.SimpleMove(velocity);
    }

    void UpdateTarget()
    {
        target = player.transform.position;
    }

    Vector3 GetLockVector()
    {
        return new Vector3(lockX ? 0.0f : 1.0f, lockY ? 0.0f : 1.0f, lockZ ? 0.0f : 1.0f);
    }

}
