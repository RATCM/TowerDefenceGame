using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoneyTower : WorkerTower
{
    [SerializeField] public float MoneyPerWorkerPerRound = 1;
    [SerializeField] public float PopulationPerRoundMultiplier = 1.2f;
    [HideInInspector] public float PopulationPerRound
    {
        get
        {
            float rect = 0f;
            for(ulong i = 1; i <= WorkerCount; i++)
            {
                rect += PopulationPerRoundMultiplier / i; // basically a harmonic series
                // This is because the population was increesing too fast and making this tower too OP
            }
            return rect;
        }
    }
    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();
    [HideInInspector] private GameObject Generator;
    public override string TowerInfoDisplay =>
        $"Money generation: {(long)(MoneyPerWorkerPerRound * WorkerCount)}$ per round\n" +
        $"Population generation: {MathF.Round((PopulationPerRound),1)} per round\n" +
        $"Upkeep: 0$ per round\n" +
        $"Is Active: {(WorkerCount >= MinimumWorkerCount ? "Yes" : "No")}";


    void Start()
    {        
        Generator = GetComponentsInChildren<Transform>().First(x => x.name == "Generator").gameObject;
        DefaultGunColor = Generator.GetComponent<SpriteRenderer>().color;

        InstantiateUIPrefab(TowerUIPrefab.ShootTower);

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% Money Generation", this, 350, delegate { MoneyPerWorkerPerRound *= 1.25f; DefaultTowerColor = Color.green; this.MouseNotOver(); }),
            new TowerUpgrade("(2) +25% Money Generation", this, 750, delegate { MoneyPerWorkerPerRound *= 1.25f; DefaultTowerColor = Color.blue; this.MouseNotOver(); }),
            new TowerUpgrade("(3) +25% Money Generation", this, 1500, delegate { MoneyPerWorkerPerRound *= 1.25f; DefaultTowerColor = Color.red; this.MouseNotOver(); })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +5% Population Generation", this, 350, delegate { PopulationPerRoundMultiplier *= 1.05f; DefaultGunColor = Color.green; MouseNotOver(); }),
            new TowerUpgrade("(2) +5% Population Generation", this, 750, delegate { PopulationPerRoundMultiplier *= 1.05f; DefaultGunColor = Color.blue; MouseNotOver(); }),
            new TowerUpgrade("(3) +5% Population Generation", this, 1500, delegate { PopulationPerRoundMultiplier *= 1.05f; DefaultGunColor = Color.red; MouseNotOver(); })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +50% Maximum worker count", this, 750, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); Generator.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(2) +50% Maximum worker count", this, 1500, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); Generator.gameObject.transform.localScale *= 1.1f; }),
            new TowerUpgrade("(3) +100% Maximum worker count", this, 4000, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 2f); Generator.gameObject.transform.localScale *= 1.1f; })
            ));
    }

    protected override void MouseOver()
    {
        base.MouseOver();
        Generator.GetComponent<SpriteRenderer>().color = DefaultGunColor * 0.8f;
    }

    protected override void MouseNotOver()
    {
        base.MouseNotOver();
        Generator.GetComponent<SpriteRenderer>().color = DefaultGunColor;
    }

    // The money generation is handled in Global.EndRound()
}
