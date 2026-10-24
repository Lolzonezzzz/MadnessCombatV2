using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScript : MonoBehaviour
{
    private TMPro.TextMeshProUGUI _text;

    void Start()
    {
        _text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        StartCoroutine(TextAnimation());
        

    }

    IEnumerator TextAnimation()
    {
        while (true)
        {
            _text.text = "Loading...";
            yield return new WaitForSeconds(1);
            _text.text = "Loading..";
            yield return new WaitForSeconds(1);
            _text.text = "Loading.";
            yield return new WaitForSeconds(1);
            _text.text = "Loading..";
            yield return new WaitForSeconds(1);
        }
    }
}
