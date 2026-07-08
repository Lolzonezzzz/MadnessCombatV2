using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ItemDatabase : MonoBehaviour
{
    public ItemData[] allItems;

#if UNITY_EDITOR
    [ContextMenu("Find All Items")]
    void FindAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemData");
        allItems = new ItemData[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            allItems[i] = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
    }
#endif
}