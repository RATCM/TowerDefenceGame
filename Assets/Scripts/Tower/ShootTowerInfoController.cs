using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootTowerInfoController : MonoBehaviour
{
    private TowerObject tower;
    // Start is called before the first frame update
    void Start()
    {
        tower = GetComponentInParent<TowerObject>(true);
        var buttons = GetComponentsInChildren<Button>(true);
        buttons.ToList().ForEach(x => x.onClick.AddListener(delegate { OnButtonClicked(x); }));
    }

    void OnButtonClicked(Button btn)
    {
        switch (btn.name)
        {
            case "ButtonUp":
                UpdateWorkerCount(1);
                break;
            case "ButtonDown":
                UpdateWorkerCount(-1);
                break;
        }
    }

    void UpdateWorkerCount(long value)
    {
        tower.ChangeWorkerCount(value);
        var text = GetComponentsInChildren<TMP_Text>().First(x => x.name == "WorkerCount");
        text.text = $"Worker count: {tower.WorkerCount}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
