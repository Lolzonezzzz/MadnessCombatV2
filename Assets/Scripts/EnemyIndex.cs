using TMPro;
using UnityEngine;

public class EnemyIndex : MonoBehaviour
{
    public GameObject ContentHolder;
    public GameObject[] Elements;
    public GameObject SearchBar;
    
    [SerializeField]
    private GameObject ItemPrefab;
    
    private EnemyDatabase _enemyDatabase;
    public int totalElements;


    private void Start()
    {
        _enemyDatabase = GetComponent<EnemyDatabase>();
        for (int i = 0; i < _enemyDatabase.allEnemies.Count; i++)
        {
            GameObject enemy = Instantiate(ItemPrefab, ContentHolder.transform);
            EnemiesUI enemyUI = enemy.GetComponent<EnemiesUI>();
            enemyUI.enemy = _enemyDatabase.allEnemies[i];
        }
        
        totalElements = ContentHolder.transform.childCount;
        
        Elements = new GameObject[totalElements];

        for (int i = 0; i < totalElements; i++)
        {
            Elements[i] = ContentHolder.transform.GetChild(i).gameObject;
        }
    }

    public void Search()
    {
        string searchText = SearchBar.GetComponent<TMP_InputField>().text.ToLower();

        foreach (GameObject ele in Elements)
        {
            string itemName = ele.transform.GetComponentInChildren<TextMeshProUGUI>().text.ToLower();

            if (string.IsNullOrEmpty(searchText) || itemName.Contains(searchText))
            {
                ele.SetActive(true);
            }
            else
            {
                ele.SetActive(false);
            }
        }
    }
}