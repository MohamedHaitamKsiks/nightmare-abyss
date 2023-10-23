using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunHolder : MonoBehaviour
{

    private const float RECOIL_ANIMATION_SPEED = 10.0f;
    private const float SWAY_SPEED = 5.0f;
    private const float SWAY_SMOOTH = 10.0f;
    private const float MOVE_ANIMATION_SPEED = 8.0f;
    private const float MOVE_ANIMATION_AMP = 0.3f;
    private const float CHANGE_ANIMATION_SPEED = 30.0f;
    private const float CHANGE_DOWN_ANGLE = 90.0f;

    [SerializeField] private Transform cameraTransform;
    
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite cursor;
    [SerializeField] private Sprite cursorActive;

    // data
    private Gun gun = null; 
    private Weapon weapon = null;
    private string weaponID = "";

    // animation
    public Vector2 SwayVelocity = Vector2.zero; 
    public float MovementSpeed = 0.0f;
    private float time = 0.0f;
    private Vector3 gunRecoilPosition = Vector3.zero;
    private float gunAnimationSpeed = 0.0f;
    private float gunAnimationAmp = 0.0f;
    private bool isGunUp = false;
    private bool isChangingGun = false;
    private float gunChangingAngle = CHANGE_DOWN_ANGLE;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // gun changing animation
        var gunChangingTargetAngle = isGunUp ? 0.0f : CHANGE_DOWN_ANGLE;
        gunChangingAngle = Mathf.Lerp(gunChangingAngle, gunChangingTargetAngle, Time.unscaledDeltaTime * CHANGE_ANIMATION_SPEED);

        // change gun when down
        const float changeGunPrecision = 0.1f;
        if (isChangingGun && !isGunUp && Mathf.Abs(gunChangingAngle - gunChangingTargetAngle) < changeGunPrecision)
        {
            if (gun != null)
            {
                Destroy(gun.gameObject);
                gun = null;
            }

            if (weapon != null)
            {
                gun = Instantiate(weapon.WeaponPrefab, transform).GetComponent<Gun>();
                isGunUp = true;
            }

        }

        // cache gun transform
        if (gun == null) return;

        var gunTransform = gun.transform;

        // gun sway
        float swayX = -(Input.GetAxis("Mouse X") + SwayVelocity.x) * SWAY_SMOOTH;
        float swayY = (Input.GetAxis("Mouse Y") + SwayVelocity.y) * SWAY_SMOOTH;

        Quaternion targetRotation = Quaternion.Euler(0.0f, swayX, swayY);        
        gunTransform.localRotation = Quaternion.Slerp(gunTransform.localRotation, targetRotation, SWAY_SPEED * Time.deltaTime);
        gunRecoilPosition = Vector3.Lerp(gunRecoilPosition, Vector3.zero, Time.deltaTime * RECOIL_ANIMATION_SPEED);

        // recoil animation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0.0f, 90.0f, gunChangingTargetAngle), Time.deltaTime * RECOIL_ANIMATION_SPEED);
        cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, Quaternion.identity, Time.deltaTime * RECOIL_ANIMATION_SPEED);
    
        // cursor animation
        cursorImage.sprite = (gunTransform.localPosition.magnitude > 0.05f)? cursorActive: cursor;

        // gun animation
        gunAnimationSpeed = Mathf.Lerp(gunAnimationSpeed, MOVE_ANIMATION_SPEED * MovementSpeed, Time.deltaTime * MOVE_ANIMATION_SPEED);
        time += Time.deltaTime * gunAnimationSpeed;

        gunAnimationAmp = Mathf.Lerp(gunAnimationAmp, MOVE_ANIMATION_AMP * MovementSpeed, Time.deltaTime * MOVE_ANIMATION_AMP);

        var gunOffset = Mathf.Sin(time) * gunAnimationAmp;
        gunTransform.localPosition = gunRecoilPosition + Vector3.forward * gunOffset + Vector3.up * (Mathf.Abs(gunOffset * 0.5f) - 0.1f);

    }

    public void Shoot()
    {
        if (weapon == null) return;

        // cache gun transform
        var gunTransform = gun.transform;

        // animate callback
        transform.localRotation *= Quaternion.AngleAxis(-32.0f * weapon.Recoil, Vector3.forward);
        cameraTransform.localRotation *= Quaternion.Euler(-10.0f, 0.0f, Random.Range(-5.0f, 5.0f));
        gunTransform.localPosition -= Vector3.back * 0.5f;

        gun.Shoot();
    }

    public void ChangeWeaponTo(string newWeaponID)
    {
        isChangingGun = true;
        isGunUp = false;

        weaponID = newWeaponID;
        weapon = (weaponID == "")? null: (Weapon) ItemManager.GetItem(weaponID);
    }
}
