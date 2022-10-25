using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LaserTowerInfoController : MonoBehaviour
{
    private LaserTower towerScript;
    // Start is called before the first frame update
    void Start()
    {
        towerScript = GetComponentInParent<LaserTower>();

        Button button = GetComponentInChildren<Button>();
        button.onClick.AddListener(IncreseWorkerCount);

        Slider slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(OnSliderChanged);
    }
    void OnSliderChanged(float arg1)
    {
        towerScript.EnergyUse = arg1;
    }
    void IncreseWorkerCount()
    {
        towerScript.ChangeWorkerCount(1);
        var text = GetComponentsInChildren<TMP_Text>().First(x => x.name == "WorkerCount");
        text.text = $"Worker count: {towerScript.WorkerCount}";
    }
}
