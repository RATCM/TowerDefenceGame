using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using UnityEngine;

public abstract class EnemyEffect
{
    protected abstract EnemyScript Enemy { get; set; }
    protected abstract void ApplyEffect();
}

public abstract class FreezeEffect : EnemyEffect
{
    /// <summary>
    /// The value to decreese the speed by, where 0 is no change and 1 is full stop
    /// </summary>
    public abstract float Value { get; set; }
}

public class TempFreeze : FreezeEffect
{
    public override float Value { get; set; }
    public float Duration;
    protected override EnemyScript Enemy { get; set; }

    public TempFreeze(float value, float duration, GameObject enemy)
    {
        Value = value;
        Duration = duration;
        Enemy = enemy.GetComponent<EnemyScript>();

        if (!Enemy.CurrentEffects.Any(x => x is TempFreeze && ((TempFreeze)x).Value < Value))
        {
            ApplyEffect();
            Timer t = new Timer();
            t.Elapsed += new ElapsedEventHandler(onTimer);
            t.AutoReset = false;
            t.Interval = Duration * 1000;
            t.Start();
        }
    }

    private void onTimer(object source, ElapsedEventArgs e)
    {
        Enemy.CurrentEffects.Remove(this);

        Enemy.CurrentSpeed = Enemy.DefaultSpeed;
    }

    protected override void ApplyEffect()
    {
        Enemy.CurrentEffects.RemoveAll(x => x is TempFreeze);

        Enemy.CurrentSpeed = (1-Value) * Enemy.DefaultSpeed;
    }
}

public class PermaFreeze : FreezeEffect
{
    public override float Value { get; set; }
    protected override EnemyScript Enemy { get; set; }

    public PermaFreeze(float value, GameObject enemy)
    {
        Value = value;
        Enemy = enemy.GetComponent<EnemyScript>();

        if(!Enemy.CurrentEffects.Any(x => x is PermaFreeze && ((PermaFreeze)x).Value < Value))
            ApplyEffect();
    }

    protected override void ApplyEffect()
    {
        Enemy.CurrentEffects.TryRemoveEffect(x => x is PermaFreeze);

        Enemy.CurrentEffects.Add(this);

        Enemy.DefaultSpeed = (1 - Value) * Enemy.InitSpeed;
        Enemy.CurrentSpeed = Enemy.DefaultSpeed;
    }
}