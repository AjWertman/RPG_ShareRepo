using UnityEngine;

/// <summary>
/// Handles a characters equipment.
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    //Refactor
    [SerializeField] GameObject staffObject = null;
    [SerializeField] GameObject swordObject = null;

    bool hasSwordEquipped = false;

    public void EquipWeapon(bool _isSword)
    {
        hasSwordEquipped = _isSword;

        if (_isSword) swordObject.SetActive(true);
        else staffObject.SetActive(true);
    }

    public bool HasSwordEquipped()
    {
        return hasSwordEquipped;
    }

    //From Combo manager

    //[SerializeField] Transform sheatheParent = null;
    //[SerializeField] Transform unsheatheParent = null;

    //[SerializeField] ComobSysWeapon startWeapon = null;
    //WeaponBehavior equippedWeapon = null;

    //public event Action<WeaponBehavior> onWeaponEquip;

    //private void Start()
    //{
    //    EquipWeapon(startWeapon);
    //}

    //public void EquipWeapon(ComobSysWeapon newWeapon)
    //{
    //    WeaponBehavior weaponBehavior = CreateWeapon(newWeapon);
    //    weaponBehavior.Initialize(newWeapon);

    //    WeaponTransformInfo weaponTransformInfo = weaponBehavior.GetWeaponTransformInfo();

    //    weaponBehavior.transform.localPosition = weaponTransformInfo.GetSheathedPosition();
    //    weaponBehavior.transform.localEulerAngles = weaponTransformInfo.GetSheathedRotation();

    //    equippedWeapon = weaponBehavior;
    //    onWeaponEquip(weaponBehavior);
    //}

    //public void Sheath()
    //{
    //    SetSheathTransform(true);
    //}

    //void Unsheath()
    //{
    //    SetSheathTransform(false);
    //}

    //private void SetSheathTransform(bool shouldSheath)
    //{
    //    WeaponTransformInfo weaponTransformInfo = equippedWeapon.GetWeaponTransformInfo();
    //    Transform newParent = null;
    //    Vector3 newPosition = Vector3.zero;
    //    Vector3 newRotation = Vector3.zero;

    //    if (shouldSheath)
    //    {
    //        newParent = sheatheParent;
    //        newPosition = weaponTransformInfo.GetSheathedPosition();
    //        newRotation = weaponTransformInfo.GetSheathedRotation();
    //    }
    //    else
    //    {
    //        newParent = unsheatheParent;
    //        newPosition = weaponTransformInfo.GetUnsheathedPosition();
    //        newRotation = weaponTransformInfo.GetUnsheathedRotation();
    //    }

    //    equippedWeapon.transform.parent = newParent;
    //    equippedWeapon.transform.localPosition = newPosition;
    //    equippedWeapon.transform.localEulerAngles = newRotation;
    //}

    //private WeaponBehavior CreateWeapon(ComobSysWeapon newWeapon)
    //{
    //    GameObject weaponInstance = Instantiate(newWeapon.GetWeaponPrefab(), sheatheParent);
    //    return weaponInstance.GetComponent<WeaponBehavior>();
    //}
}
