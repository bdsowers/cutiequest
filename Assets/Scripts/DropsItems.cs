using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;
using ArrayExtensions;

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
    public bool ignoreGoldDigger;
    public bool scatter;
    public float delay;

    public void Drop()
    {
        if (delay < 0.01f)
            DropInternal();
        else
            Invoke("DropInternal", delay);
    }

    public void DropInternal()
    {
        int numCoinsToDrop = NumCurrencyToDrop(coinDropData);
        int numHeartsToDrop = NumCurrencyToDrop(heartDropData);
        
        if (GoldDiggerQuirk.quirkEnabled && !ignoreGoldDigger)
        {
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
            int value = Random.Range(0, 100) + Game.instance.playerStats.ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject);
            if (value < arbitraryDrops[i].rate)
            {
                DropItems(arbitraryDrops[i].itemName, arbitraryDrops[i].amount);
            }
        }
    }

    private void DropItems(string prefabName, int num)
    {
        List<Vector2Int> emptySurroundingPositions = null;

        for (int i = 0; i < num; ++i)
        {
            GameObject newItem = GameObject.Instantiate(PrefabManager.instance.PrefabByName(prefabName));
            Vector3 sourcePosition = transform.position;
            Vector3 endPosition = transform.position + VectorHelper.RandomNormalizedXZVector3() * Random.Range(0.1f, 0.3f);
            
            if (scatter)
            {
                if (emptySurroundingPositions == null)
                {
                    Vector2Int mapCoords = MapCoordinateHelper.WorldToMapCoords(sourcePosition);
                    emptySurroundingPositions = Game.instance.levelGenerator.collisionMap.EmptyOffsetsNearPosition(mapCoords, 1);
                }

                Vector2Int offsetMapPos = new Vector2Int(0, 0);
                if (emptySurroundingPositions.Count > 0)
                    offsetMapPos = emptySurroundingPositions.Sample();

                Vector3 offsetWorldPos = MapCoordinateHelper.MapToWorldCoords(offsetMapPos, 0f);
                endPosition += offsetWorldPos;
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

        // note bdsowers - do a subtraction here based on luck, the rates go lowest to highest
        int val = Random.Range(0, 100) - Game.instance.playerStats.ModifiedStatValue(CharacterStatType.Luck, Game.instance.avatar.gameObject); ;
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
