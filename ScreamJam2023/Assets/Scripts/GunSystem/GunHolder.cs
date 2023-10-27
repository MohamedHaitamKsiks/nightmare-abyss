using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GunHolder : MonoBehaviour
{

    private const float RECOIL_ANIMATION_SPEED = 10.0f;
    private const float RECOIL_BASE_ANGLE = -32.0f;
    private const float RECOIL_BASE_POSITION = 0.2f;
    private const float SWAY_SPEED = 5.0f;
    private const float SWAY_SMOOTH = 10.0f;
    private const float MOVE_ANIMATION_SPEED = 8.0f;
    private const float MOVE_ANIMATION_AMP = 0.3f;
    private const float CHANGE_ANIMATION_SPEED = 10.0f;
    private const float CHANGE_DOWN_ANGLE = 90.0f;

    [SerializeField] private Transform cameraTransform;
    
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite cursor;
    [SerializeField] private Sprite cursorActive;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private BulletCounter bulletCounter;

    // data
    private Gun gun = null; 
    private Weapon weapon = null;
    private string weaponID = "";

    // animation
    // movement animation
    public Vector2 SwayVelocity = Vector2.zero; 
    public float MovementSpeed = 0.0f;
    private float gunAnimationTime = 0.0f;
    private float gunAnimationSpeed = 0.0f;
    private float gunAnimationAmp = 0.0f;
    
    // attack animation
    public UnityEvent OnShootAnimationEnd {get; private set;} = new();
    private float gunRecoilTime = 0.0f;
    private Vector3 gunRecoilPosition = Vector3.zero;
    private bool isGunShooting = false;

    // chenge gun animation
    private bool isGunUp = false;
    public bool IsChangingGun {get; private set;} = false;
    private float gunChangingScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // gun changing animation
        var gunChangingTargetScale = isGunUp ? 0.0f : 1.0f;
        gunChangingScale = Mathf.Lerp(gunChangingScale, gunChangingTargetScale, Time.unscaledDeltaTime * CHANGE_ANIMATION_SPEED);

        // change gun when down
        const float changeGunPrecision = 0.1f;
        if (IsChangingGun && !isGunUp && Mathf.Abs(gunChangingScale - gunChangingTargetScale) < changeGunPrecision)
        {
            if (gun != null)
            {
                Destroy(gun.gameObject);
                gun = null;
            }

            if (weapon != null)
            {
                gun = Instantiate(weapon.WeaponPrefab, transform).GetComponent<Gun>();
                bulletCounter.UpdateValue(inventoryManager.Items[weaponID]);
                isGunUp = true;
                IsChangingGun = false;
            }

        }

        // cache gun transform
        if (gun == null || weapon == null) return;

        var gunTransform = gun.transform;

        // gun sway
        float swayX = -(Input.GetAxis("Mouse X") + SwayVelocity.x) * SWAY_SMOOTH;
        float swayY = (Input.GetAxis("Mouse Y") + SwayVelocity.y) * SWAY_SMOOTH;

        Quaternion targetRotation = Quaternion.Euler(0.0f, swayX, swayY);        
        gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, targetRotation, SWAY_SPEED * Time.deltaTime);

        // recoil animation
        if (gunRecoilTime > weapon.ShootCurve[weapon.ShootCurve.length - 1].time && isGunShooting)
        {
            // snap camera to rotation
            OnShootAnimationEnd.Invoke();
            cameraTransform.localRotation = Quaternion.identity;
            isGunShooting = false;
        }

        var gunRecoilValue = weapon.ShootCurve.Evaluate(gunRecoilTime);
        var gunRecoilAngle = RECOIL_BASE_ANGLE * weapon.Recoil * gunRecoilValue;
        gunRecoilPosition =  Vector3.left * (-gunRecoilValue * weapon.Recoil * RECOIL_BASE_POSITION);

        // camera recoil
        var gunRecoilCameraAngle = 0.2f * RECOIL_BASE_ANGLE * weapon.Recoil * gunRecoilValue;
        cameraTransform.localRotation = Quaternion.Euler(gunRecoilCameraAngle, 0.0f, 0.0f);

        gunRecoilTime += Time.deltaTime * weapon.AnimationSpeed;

        // gun movement animation
        gunAnimationSpeed = Mathf.Lerp(gunAnimationSpeed, MOVE_ANIMATION_SPEED * MovementSpeed, Time.deltaTime * MOVE_ANIMATION_SPEED);
        gunAnimationTime += Time.deltaTime * gunAnimationSpeed;
        gunAnimationAmp = Mathf.Lerp(gunAnimationAmp, MOVE_ANIMATION_AMP * MovementSpeed, Time.deltaTime * MOVE_ANIMATION_AMP);

        var gunOffset = Mathf.Sin(gunAnimationTime) * gunAnimationAmp;
        gunTransform.localPosition = gunRecoilPosition + new Vector3(0.0f, Mathf.Abs(gunOffset * 0.5f) - 0.1f + gunChangingScale, gunOffset);

        // apply holder transform
        var gunChangingAngle = gunChangingScale * CHANGE_DOWN_ANGLE;
        transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 90.0f, gunChangingAngle) + weapon.RotationEuler * gunRecoilAngle);
    }

    public void Shoot()
    {
        if (weapon == null) return;

        // cache gun transform
        gunRecoilTime = 0.0f;
        isGunShooting = true;
        cameraTransform.localRotation = Quaternion.identity;

        gun.Shoot();
    }

    public void ChangeWeaponTo(string newWeaponID)
    {
        if (IsChangingGun) return;

        IsChangingGun = true;
        isGunUp = false;

        weaponID = newWeaponID;
        Debug.Log(weaponID);
        weapon = (weaponID == "")? null: (Weapon) ItemManager.GetItem(weaponID);
    }
}
