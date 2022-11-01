using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTower : WorkerTower
{
    [SerializeField] public float MoneyPerWorkerPerRound = 1;
    [SerializeField] public float PopulationPerRoundMultiplier = 1.2f;
    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get; set; } = new List<TowerUpgradePath>();

    void Start()
    {
        InstantiateUIPrefab(TowerUIPrefab.ShootTower);

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("+25% Money Generation", this, 350, delegate { MoneyPerWorkerPerRound *= 1.25f; }),
            new TowerUpgrade("+25% Money Generation", this, 750, delegate { MoneyPerWorkerPerRound *= 1.25f; }),
            new TowerUpgrade("+25% Money Generation", this, 1500, delegate { MoneyPerWorkerPerRound *= 1.25f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("+5% Population Generation", this, 350, delegate { PopulationPerRoundMultiplier *= 1.05f; }),
            new TowerUpgrade("+5% Population Generation", this, 750, delegate { PopulationPerRoundMultiplier *= 1.05f; }),
            new TowerUpgrade("+5% Population Generation", this, 1500, delegate { PopulationPerRoundMultiplier *= 1.05f; })
            ));

        upgradePath.Add(new TowerUpgradePath(
            new TowerUpgrade("+50% Maximum worker count", this, 750, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); }),
            new TowerUpgrade("+50% Maximum worker count", this, 1500, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 1.5f); }),
            new TowerUpgrade("+100% Maximum worker count", this, 4000, delegate { MaximumWorkerCount = (ulong)(MaximumWorkerCount * 2f); })
            ));
    }

    // The money generation is handled in Global.EndRound()
}
