using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(IncreseWorkerCount);
    }

    void IncreseWorkerCount()
    {
        var TowerScript = GetComponentInParent<TowerObject>();

        TowerScript.WorkerCount++;
        var text = GetComponentsInChildren<Text>().First(x => x.text.Contains("Worker count"));
        Debug.Log(text.text);
        text.text = $"Worker count: {TowerScript.WorkerCount}";
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
