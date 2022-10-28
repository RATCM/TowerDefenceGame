using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootTowerInfoController : MonoBehaviour
{
    private TowerObject tower;

    private List<Toggle> toggles;
    private List<Button> buttons;
    // Start is called before the first frame update
    void Start()
    {
        tower = GetComponentInParent<TowerObject>(true);
        buttons = GetComponentsInChildren<Button>(true).ToList();
        buttons.ForEach(x => x.onClick.AddListener(delegate { OnButtonClicked(x); }));

        toggles = GetComponentsInChildren<Toggle>(true).ToList();
        toggles.ForEach(x => x.onValueChanged.AddListener(delegate { OnToggleClicked(x); }));
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

    void OnButtonClicked(Button btn)
    {
        var val = int.Parse(toggles.FirstOrDefault(x => x.isOn).name.Replace("ToggleTimes",""));
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
