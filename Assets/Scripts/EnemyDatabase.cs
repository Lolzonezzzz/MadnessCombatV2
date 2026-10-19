using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyDatabase : MonoBehaviour
{
    public List<GameObject> allEnemies =  new List<GameObject>();

#if UNITY_EDITOR
    [ContextMenu("Find All Enemy"), Button("Find All Enemies")]
    void FindAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");

        
        foreach (string guid in guids)
        {

            string path = AssetDatabase.GUIDToAssetPath(guid);
            
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
  
                if (prefab.CompareTag("Enemy"))
                {
                    allEnemies.Add(prefab);
                    Debug.Log($"Successfully detected enemy prefab: '{prefab.name}' at path: {path}");
                }
            }
        }

        Debug.Log($"Detection complete. Found {allEnemies.Count} enemy type prefabs total.");
        
    }
#endif
}