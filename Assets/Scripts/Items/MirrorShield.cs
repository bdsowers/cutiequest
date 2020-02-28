using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class MirrorShield : MonoBehaviour
{
    public Item parentItem;

    private int mDurability = 5;

    public void Reflect(Projectile projectile)
    {
        projectile.gameObject.SetLayerRecursive(LayerMask.NameToLayer("PlayerProjectile"));
        projectile.GetComponent<ConstantTranslation>().direction *= -1;

        mDurability--;
        if (mDurability <= 0)
        {
            Destroy(gameObject);
            GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

            string message = "Mirror Shield Broke!";
            NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, message, NumberPopupReason.Good, 0.25f);
        }
    }
}
