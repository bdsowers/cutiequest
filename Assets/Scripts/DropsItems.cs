using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class DropsItems : MonoBehaviour
{
    [System.Serializable]
    public struct DropData
    {
        public string itemName;
        public int rate;
        public int amount;
    }

    // These should be sorted lowest to highest
    public DropData[] coinDropData;
    public DropData[] heartDropData;
    public DropData[] arbitraryDrops;
    public bool scatter;

    public void Drop()
    {
        int numCoinsToDrop = NumCurrencyToDrop(coinDropData);
        int numHeartsToDrop = NumCurrencyToDrop(heartDropData);
        
        if (GoldDiggerQuirk.quirkEnabled)
        {
            Debug.Log("Enabled");
            numCoinsToDrop += numHeartsToDrop * 3;
            numHeartsToDrop = 0;
        }

        if (numCoinsToDrop > 0)
        {
            DropItems("CollectableCoin", numCoinsToDrop);
        }

        if (numHeartsToDrop > 0)
        {
            DropItems("CollectableHeart", numHeartsToDrop);
        }

        for (int i = 0; i < arbitraryDrops.Length; ++i)
        {
            int value = Random.Range(0, 100);
            if (value < arbitraryDrops[i].rate)
            {
                DropItems(arbitraryDrops[i].itemName, arbitraryDrops[i].amount);
            }
        }
    }

    private void DropItems(string prefabName, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            GameObject newItem = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefabName));
            Vector3 sourcePosition = transform.position;
            Vector3 endPosition = transform.position + VectorHelper.RandomNormalizedXZVector3() * 0.3f;

            if (scatter)
            {
                int randX = Random.Range(-1, 2);
                int randZ = Random.Range(-1, 2);
                if (randX == 0 && randZ == 0) randX = 1;

                endPosition += new Vector3(randX, 0f, randZ);
            }

            newItem.transform.position = sourcePosition;

            RevealWhenAvatarIsClose reveal = newItem.GetComponentInChildren<RevealWhenAvatarIsClose>();
            if (reveal != null)
            {
                reveal.enabled = false;
            }

            Collectable collectable = newItem.GetComponent<Collectable>();
            if (collectable != null)
            {
                collectable.PlayDropAnimation(sourcePosition, endPosition, i != 0);
            }
        }
    }

    private int NumCurrencyToDrop(DropData[] dropData)
    {
        if (dropData == null)
            return 0;

        int val = Random.Range(0, 100);
        for (int i = 0; i < dropData.Length; ++i)
        {
            if (val <= dropData[i].rate)
            {
                return dropData[i].amount;
            }
        }
        return 0;
    }
}
