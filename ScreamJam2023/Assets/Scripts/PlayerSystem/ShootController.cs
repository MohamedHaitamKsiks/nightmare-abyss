using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using Random = UnityEngine.Random;

[RequireComponent(typeof(InventoryManager), typeof(EquipmentController), typeof(PlayerController))]
public class ShootController : MonoBehaviour
{
    private const float SHOOT_BUFFER = 0.3f;

    [SerializeField] private Transform cameraTranform;
    [SerializeField] private GunHolder gunHolder;
    [SerializeField] private LayerMask layer;
    [SerializeField] BulletCounter bulletCounter;


    private InventoryManager inventoryManager;
    private EquipmentController equipmentController;
    private PlayerController playerController;

    private bool canShoot = true;
    private float shootBuffer = 0.0f;


    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        equipmentController = GetComponent<EquipmentController>();
        playerController = GetComponent<PlayerController>();

        // gun holder animation finish
        gunHolder.OnShootAnimationEnd.AddListener(() =>
        {
            canShoot = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        // skip when pause
        if (PauseManager.IsPaused()) return;

        // shoot buffer
        shootBuffer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire"))
        {
            shootBuffer = SHOOT_BUFFER;
        }

        var weapon = equipmentController.GetCurrentWeapon();

        // shoot
        if (weapon != null && shootBuffer > 0.01f)
        {
            if (!canShoot) return;
            if (inventoryManager.Items[weapon.ID] == 0) return;

            // remove bullet 
            if (weapon.UseBullets)
            {
                inventoryManager.Items[weapon.ID] -= 1;
                bulletCounter.UpdateValueAnimated(inventoryManager.Items[weapon.ID]);
            }

            canShoot = false;
            shootBuffer = 0.0f;

            if (weapon.Delai > 0.01f)
            {
                StartCoroutine(ShootDelay(weapon.Delai));
            }
            else
            {
                Shoot();
            }

            gunHolder.Shoot();
        }
    }

    IEnumerator ShootDelay(float delay)
    {
        
        yield return new WaitForSeconds(delay);
        Shoot();
    }

    void Shoot()
    {
        var weapon = equipmentController.GetCurrentWeapon();
        // shoot all bullets
        for (int i = 0; i < weapon.BulletCount; i++)
            ShootOneBullet();
        // apply movement recoil
        playerController.Push(cameraTranform.forward, -weapon.MovementRecoil);

    }

    void ShootOneBullet()
    {
        var weapon = equipmentController.GetCurrentWeapon();

        // send ray cast
        var spreadRotation = 
            Quaternion.AngleAxis(Random.Range(-1.0f, 1.0f) * weapon.BulletSpread, cameraTranform.up) *
            Quaternion.AngleAxis(Random.Range(-1.0f, 1.0f) * weapon.BulletSpread, cameraTranform.forward);

        var shootDirection = spreadRotation * cameraTranform.forward;

        if (!Physics.Raycast(cameraTranform.position, shootDirection , out RaycastHit hit, weapon.Range, layer)) return;


        // create bullet hit info
        BulletHit bulletHit = new()
        {
            Position = hit.point,
            Direction = shootDirection,
            Normal = hit.normal,
            Damage = weapon.Damage * (1.0f - hit.distance / weapon.Range)
        };


        // on raycast hit
        if (hit.collider.gameObject.TryGetComponent(out BulletTrigger bulletTrigger))
        {
            // send signal
            bulletTrigger.OnBulletHit.Invoke(bulletHit);
        }

        // push if has rigid body
        if (hit.collider.gameObject.TryGetComponent(out Rigidbody rigidbody))
        {
            StartCoroutine(PushRigidBody(rigidbody, bulletHit));
        } 

    }

    // hit colliders next frame
    IEnumerator PushRigidBody(Rigidbody rigidbody, BulletHit bulletHit)
    {
        yield return new WaitForFixedUpdate();
        rigidbody.AddForceAtPosition(bulletHit.Direction * (bulletHit.Damage * 1.0f), bulletHit.Position, ForceMode.VelocityChange);
    }
}
