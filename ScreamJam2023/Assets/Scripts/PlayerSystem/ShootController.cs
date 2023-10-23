using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class ShootController : MonoBehaviour
{
    private const float MAX_RANGE = 10000.0f;

    [SerializeField] private Transform cameraTranform;
    [SerializeField] private GunHolder gunHolder;
    [SerializeField] private LayerMask layer;
    [SerializeField] private ParticleSystem impactParticels;

    private InventoryManager inventoryManager;
    private EquipmentController equipmentController;

    // Start is called before the first frame update
    void Start()
    {
        inventoryManager = GetComponent<InventoryManager>();
        equipmentController = GetComponent<EquipmentController>();
    }

    // Update is called once per frame
    void Update()
    {
        // skip when pause
        if (PauseManager.IsPaused()) return;

        // shoot
        if (equipmentController.GetCurrentWeapon() != null && Input.GetButtonDown("Fire"))
        {
            Shoot();
            gunHolder.Shoot();
        }
    }

    void Shoot()
    {
        var weapon = equipmentController.GetCurrentWeapon();

        // send ray cast
        if (!Physics.Raycast(cameraTranform.position, cameraTranform.forward, out RaycastHit hit, weapon.Range, layer)) return;

        // add impact particles
        impactParticels.transform.SetPositionAndRotation(
            hit.point, 
            Quaternion.LookRotation(hit.normal)
        );   
        impactParticels.Play();

        // on raycast hit
        if (!hit.collider.gameObject.TryGetComponent(out BulletTrigger bulletTrigger)) return;

        // send bullet hit event
        BulletHit bulletHit = new() {
            Position = hit.point,
            Direction = cameraTranform.forward,
            Normal = hit.normal,
            Damage = weapon.Damage * (1.0f - hit.distance / weapon.Range)
        };
        bulletTrigger.OnBulletHit.Invoke(bulletHit);
    }
}
