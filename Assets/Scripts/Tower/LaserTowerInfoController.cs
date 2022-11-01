using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class LaserTowerInfoController : MonoBehaviour
{
    private List<Toggle> toggles;
    private List<Button> buttons;

    private LaserTower tower;
    private TMP_Text Count;

    // Start is called before the first frame update
    void Start()
    {
        tower = GetComponentInParent<LaserTower>();

        buttons = GetComponentsInChildren<Button>(true).ToList();
        buttons.ForEach(x => x.onClick.AddListener(delegate { OnButtonClicked(x); }));

        toggles = GetComponentsInChildren<Toggle>(true).ToList();
        toggles.ForEach(x => x.onValueChanged.AddListener(delegate { OnToggleClicked(x); }));

        Count = GetComponentsInChildren<TMP_Text>(true).First(x => x.name == "WorkerCount");

        GetComponentInChildren<Slider>().onValueChanged.AddListener(OnSliderChanged);
    }
    void OnButtonClicked(Button btn)
    {
        var val = int.Parse(toggles.FirstOrDefault(x => x.isOn).name.Replace("ToggleTimes", ""));
        switch (btn.name)
        {
            case "ButtonUp":
                UpdateWorkerCount(val);
                break;
            case "ButtonDown":
                UpdateWorkerCount(-val);
                break;
        }
    }

    void OnToggleClicked(Toggle toggle)
    {
        var toggleIsOn = toggle.isOn;
        toggles.ForEach(x => x.SetIsOnWithoutNotify(false));
        toggle.SetIsOnWithoutNotify(toggleIsOn);

        Debug.Log(toggleIsOn);

        bool anyIsOn = toggles.Any(x => x.isOn);

        buttons.ForEach(x => x.enabled = anyIsOn);
    }

    void UpdateWorkerCount(long value)
    {
        tower.ChangeWorkerCount(value);
        Count.text = $"Worker count: {tower.WorkerCount}";
    }
    void OnSliderChanged(float arg1)
    {
        tower.EnergyUse = arg1;
    }

    void Update()
    {
        UpdateWorkerCount(0);
    }
}
