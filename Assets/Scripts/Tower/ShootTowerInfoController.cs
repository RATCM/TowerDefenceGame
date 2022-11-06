using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShootTowerInfoController : TowerInfoController
{
    private TowerObject tower;

    private List<Toggle> toggles;
    private List<Button> buttons;

    private TMP_Text Count;
    private TMP_Text Info;

    // Start is called before the first frame update
    void Start()
    {
        tower = GetComponentInParent<TowerObject>(true);
        buttons = GetComponentsInChildren<Button>(true).ToList();
        buttons.ForEach(x => x.onClick.AddListener(delegate { OnButtonClicked(x); }));

        var b = buttons.Where(x => x.name.Contains("ButtonUpgrade")).OrderByDescending(x => x.GetComponent<RectTransform>().position.y).ToList();
        b.ForEach(x => UpdateButtonText(x, b.IndexOf(x)));

        toggles = GetComponentsInChildren<Toggle>(true).ToList();
        toggles.ForEach(x => x.onValueChanged.AddListener(delegate { OnToggleClicked(x); }));

        Count = GetComponentsInChildren<TMP_Text>(true).First(x => x.name == "WorkerCount");

        Info = GetComponentsInChildren<TMP_Text>(true).First(x => x.name == "TowerInfo");
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
        Debug.Log("Button clicked");
        var val = int.Parse(toggles.FirstOrDefault(x => x.isOn).name.Replace("ToggleTimes",""));

        var b = buttons.Where(x => x.name.Contains("ButtonUpgrade")).OrderByDescending(x => x.GetComponent<RectTransform>().position.y).ToList();

        switch (btn.name)
        {
            case "ButtonUp":
                UpdateWorkerCount(val);
                break;
            case "ButtonDown":
                UpdateWorkerCount(-val);
                break;
            case "ButtonSell":
                tower.Sell();
                break;
            case "ButtonUpgrade1":
                tower.upgradePath[0].ApplyUpgrade();
                goto default;
            case "ButtonUpgrade2":
                tower.upgradePath[1].ApplyUpgrade();
                goto default;
            case "ButtonUpgrade3":
                tower.upgradePath[2].ApplyUpgrade();
                goto default;
            default:
                var btnIndex = b.IndexOf(btn);
                if (tower.upgradePath[btnIndex].GetNext() == null)
                {
                    btn.enabled = false;
                    btn.GetComponent<Image>().color = new Color(0.7f,0.7f,0.7f);
                }
                UpdateButtonText(btn, btnIndex);
                break;
        }
    }

    void UpdateButtonText(Button btn, int index)
    {
        var txt = btn.GetComponentInChildren<TMP_Text>();
        txt.text = tower.upgradePath[index].GetNext()?.UpgradeName ?? "Maxed out";

        if (txt.text != "Maxed out")
            txt.text += $"\n{tower.upgradePath[index].GetNext().UpgradePrice}$";
    }

    void UpdateWorkerCount(long value)
    {
        if (!Global.RoundInProgress)
        {
            tower.ChangeWorkerCount(value);
        }
        Count.text = $"Worker count: {tower.WorkerCount}";
        Info.text =
            $"Tower Info:\n" +
            tower.TowerInfoDisplay;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWorkerCount(0);
    }
}
