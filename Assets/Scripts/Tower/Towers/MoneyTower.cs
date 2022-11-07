using System;
using System.Collections;
using System.Collections.Generic;
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

    public override string TowerInfoDisplay =>
        $"Money generation: {(long)(MoneyPerWorkerPerRound * WorkerCount)}$ per round\n" +
        $"Population generation: {MathF.Round((PopulationPerRound),1)} per round\n" +
        $"Upkeep: 0$ per round\n" +
        $"Is Active: {(WorkerCount >= MinimumWorkerCount ? "Yes" : "No")}";


    void Start()
    {
        InstantiateUIPrefab(TowerUIPrefab.ShootTower);

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +25% Money Generation", this, 350, delegate { MoneyPerWorkerPerRound *= 1.25f; }),
            new TowerUpgrade("(2) +25% Money Generation", this, 750, delegate { MoneyPerWorkerPerRound *= 1.25f; }),
            new TowerUpgrade("(3) +25% Money Generation", this, 1500, delegate { MoneyPerWorkerPerRound *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +5% Population Generation", this, 350, delegate { PopulationPerRoundMultiplier *= 1.05f; }),
            new TowerUpgrade("(2) +5% Population Generation", this, 750, delegate { PopulationPerRoundMultiplier *= 1.05f; }),
            new TowerUpgrade("(3) +5% Population Generation", this, 1500, delegate { PopulationPerRoundMultiplier *= 1.05f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("(1) +50% Maximum worker count", this, 750, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); }),
            new TowerUpgrade("(2) +50% Maximum worker count", this, 1500, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); }),
            new TowerUpgrade("(3) +100% Maximum worker count", this, 4000, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 2f); })
            ));
    }

    // The money generation is handled in Global.EndRound()
}
