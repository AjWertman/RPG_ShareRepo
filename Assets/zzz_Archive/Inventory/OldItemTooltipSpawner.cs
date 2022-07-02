using UnityEngine;
using RPGProject.Inventories;

[RequireComponent(typeof(IItemHolder))]
public class OldItemTooltipSpawner : OldTooltipSpawner
{
    public override bool CanCreateTooltip()
    {
        var item = GetComponent<IItemHolder>().GetItem();
        if (!item) return false;

        return true;
    }

    public override void UpdateTooltip(GameObject tooltip)
    {
        var itemTooltip = tooltip.GetComponent<OldItemTooltip>();
        if (!itemTooltip) return;

        var item = GetComponent<IItemHolder>().GetItem();

        itemTooltip.Setup(item);
    }
}