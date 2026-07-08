using TMPro;
using UnityEngine;

public class WeaponIndex : MonoBehaviour
{
    public GameObject ContentHolder;
    public GameObject[] Elements;
    public GameObject SearchBar;
    
    [SerializeField]
    private GameObject ItemPrefab;
    
    private ItemDatabase _itemDatabase;
    public int totalElements;


    private void Start()
    {
        _itemDatabase = GetComponent<ItemDatabase>();
        for (int i = 0; i < _itemDatabase.allItems.Length; i++)
        {
            GameObject item = Instantiate(ItemPrefab, ContentHolder.transform);
            ItemUI itemUI = item.GetComponent<ItemUI>();
            itemUI.item = _itemDatabase.allItems[i];
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
