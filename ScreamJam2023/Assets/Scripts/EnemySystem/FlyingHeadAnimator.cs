using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingHeadAnimator : MonoBehaviour
{
    // head body
    [SerializeField] private Rigidbody headBody;

    // components
    private EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        
        enemyController.OnDeath.AddListener(() => {
            headBody.isKinematic = false;
        }); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
