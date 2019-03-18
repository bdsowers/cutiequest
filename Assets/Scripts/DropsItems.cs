using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class DropsItems : MonoBehaviour
{
    [System.Serializable]
    public struct DropData
    {
        public int rate;
        public int amount;
    }

    // These should be sorted lowest to highest
    public DropData[] coinDropData;
    public DropData[] heartDropData;

    public void Drop()
    {
        int numCoinsToDrop = NumCurrencyToDrop(coinDropData);
        int numHeartsToDrop = NumCurrencyToDrop(heartDropData);
        
        if (numCoinsToDrop > 0)
        {
            DropItems("CollectableCoin", numCoinsToDrop);
        }

        if (numHeartsToDrop > 0)
        {
            DropItems("CollectableHeart", numHeartsToDrop);
        }
    }

    private void DropItems(string prefabName, int num)
    {
        for (int i = 0; i < num; ++i)
        {
            GameObject newItem = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefabName));
            Vector3 sourcePosition = transform.position;
            Vector3 endPosition = transform.position + VectorHelper.RandomNormalizedXZVector3() * 0.3f;
            newItem.transform.position = sourcePosition;
            newItem.GetComponentInChildren<RevealWhenAvatarIsClose>().enabled = false;
            
            newItem.GetComponent<Collectable>().PlayDropAnimation(sourcePosition, endPosition, i != 0);
        }
    }

    private int NumCurrencyToDrop(DropData[] dropData)
    {
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
