using System;
using UnityEngine;

public class ComobSysWeapon : ScriptableObject
{
    [SerializeField] GameObject weaponPrefab = null;
    [SerializeField] float baseDamage = 20f;

    [SerializeField] WeaponTransformInfo weaponTransformInfo = null;

    public GameObject GetWeaponPrefab()
    {
        return weaponPrefab;
    }

    public float GetBaseDamage()
    {
        return baseDamage;
    }

    public WeaponTransformInfo GetWeaponTransformInfo()
    {
        return weaponTransformInfo;
    }
}

[Serializable]
public class WeaponTransformInfo
{
    [SerializeField] Vector3 sheathedPosition = Vector3.zero;
    [SerializeField] Vector3 sheathedRotation = Vector3.zero;

    [SerializeField] Vector3 unsheathedPosition = Vector3.zero;
    [SerializeField] Vector3 unsheathedRotation = Vector3.zero;

    public Vector3 GetSheathedPosition()
    {
        return sheathedPosition;
    }

    public Vector3 GetSheathedRotation()
    {
        return sheathedRotation;
    }

    public Vector3 GetUnsheathedPosition()
    {
        return unsheathedPosition;
    }

    public Vector3 GetUnsheathedRotation()
    {
        return unsheathedRotation;
    }
}
