using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public struct BulletHit
{
    public Vector3 Position;
    public Vector3 Direction;
    public Vector3 Normal;
    public float Damage;

    public override readonly string ToString()
    {
        return string.Format("BulletHit:\n\nPosition = {0}\nDirection = {1}\nNormal = {2}\nDamage = {3}\n", 
        Position, Direction, Normal, Damage);
    } 
}   

public class BulletTrigger : MonoBehaviour
{
    // event with 3 params position, direction, normal, damage.
    public UnityEvent<BulletHit> OnBulletHit = new();

    // Start is called before the first frame update
    void Start()
    {
        OnBulletHit.AddListener((BulletHit hit) => {
            Debug.Log(hit.ToString());
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
