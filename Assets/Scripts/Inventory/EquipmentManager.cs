using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] GameObject staffObject = null;
    [SerializeField] GameObject swordObject = null;

    private void Awake()
    {
        swordObject.SetActive(false);
        staffObject.SetActive(false);
    }
    public void EquipWeapon(bool _isSword)
    {
        if (_isSword) swordObject.SetActive(true);
        else staffObject.SetActive(true);
    }
}
