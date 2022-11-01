using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Reference https://love2d.org/forums/viewtopic.php?t=85229
namespace TowerTypes
{
    public interface ITower
    {
    }
    public interface IDefenceTower : ITower
    {
    }
    public interface IWorkerTower : ITower
    {
    }
    #region Damaging Towers
    public interface ILaser
    {
        public void UpdateTemperature();
    }
    public interface IGunTower : IDefenceTower // Basic tower for shooting projectiles
    {
        void Shoot();
    }

    #endregion

    #region Non Damaging Towers

    #endregion


}