using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyTower : WorkerTower
{
    [SerializeField] public ulong MoneyPerWorkerPerSec = 1;

    [HideInInspector] private float MoneyGainedThisRound = 0;
    [HideInInspector] private int CurrentRound = PlayerInfo.CurrentRound;

    [HideInInspector] public override List<TowerUpgradePath> upgradePath { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // Start is called before the first frame update
    void Awake()
    {
        InstantiateUIPrefab("MoneyTowerInfoPopup");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Global.RoundInProgress)
            MoneyGainedThisRound += ((float)MoneyPerWorkerPerSec)/60f;
        else if(CurrentRound == PlayerInfo.CurrentRound)
            MoneyGainedThisRound = 0;

        if(CurrentRound != PlayerInfo.CurrentRound)
        {
            CurrentRound = PlayerInfo.CurrentRound;
            PlayerInfo.Money += (long)MoneyGainedThisRound;
        }
    }
}
