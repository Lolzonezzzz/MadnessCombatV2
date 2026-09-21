using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu]
public class LootTable : ScriptableObject
{
    [Serializable]
    public class LootTableData
    {
        public ItemData item;
        public int weight;
    }
    
    public List<LootTableData> table;
    
    [NonSerialized]
    int _totalWeight = -1;
    
    public int TotalWeight
    {
        get
        {
            if (_totalWeight == -1)
            {
                CalculateTotalWeight();
            }
            return _totalWeight;
        }
    }

    void CalculateTotalWeight()
    {
        _totalWeight = 0;
        for (int i = 0; i <= table.Count; i++)
        {
            _totalWeight += table[i].weight;
        }
    }

    /// <summary>
    /// the higher the number the more likely to drop the item
    /// if Item A is 50
    /// and Item B is 12
    /// item A will be more likely to drop more than item B
    /// </summary>
    /// <returns></returns>
    public ItemData GetDrop()
    {
        int roll = Random.Range(0, TotalWeight);

        for (int i = 0; i < table.Count; i++)
        {
            roll -= table[i].weight;
            if (roll <= 0)
            {
                return table[i].item;
            }
        }
        
        return table[0].item;
    }
    
}
