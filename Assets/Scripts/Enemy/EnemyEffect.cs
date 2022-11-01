using System;
using System.Linq;
using System.Timers;
using UnityEngine;

public abstract class EnemyEffect
{
    protected abstract EnemyScript Enemy { get; set; }
    public abstract void ApplyEffect();
    public abstract void UpdateEffects(EnemyEffect effect);
}

public abstract class FreezeEffect : EnemyEffect
{
    /// <summary>
    /// The value to decreese the speed by, where 1 is no change and 0 is full stop
    /// </summary>
    public abstract float Value { get; set; }
}

public class TempFreeze : FreezeEffect
{
    public override float Value { get; set; }
    public float Duration;
    protected override EnemyScript Enemy { get; set; }
    private Timer timer;
    public TempFreeze(float value, float duration, GameObject enemy)
    {
        Value = value;
        Duration = duration;
        Enemy = enemy.GetComponent<EnemyScript>();
    }

    private void onTimer(object source, ElapsedEventArgs e)
    {
        Enemy.CurrentEffects.Remove(this);

        Enemy.CurrentSpeed = Enemy.DefaultSpeed;

        Debug.Log("Effect reset...");
    }

    private void InitTimer()
    {
        timer = new Timer();
        timer.Elapsed += new ElapsedEventHandler(onTimer);
        timer.AutoReset = false;
        timer.Interval = Duration * 1000;
        timer.Start();
    }

    public override void ApplyEffect()
    {
        if (timer == null)
            InitTimer();

        Enemy.CurrentSpeed = Value * Enemy.DefaultSpeed;
    }

    public override void UpdateEffects(EnemyEffect effect)
    {
        // Reset Timer
        timer.Stop();
        timer.Interval = ((TempFreeze)effect).Duration * 1000;
        timer.Start();

        Value = ((TempFreeze)effect).Value;
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

        if(!Enemy.CurrentEffects.Any(x => x.GetType() == typeof(PermaFreeze) && ((PermaFreeze)x).Value < Value))
            ApplyEffect();
    }

    public override void ApplyEffect()
    {
        Enemy.CurrentEffects.TryRemoveEffect(x => x.GetType() == typeof(PermaFreeze));

        Enemy.CurrentEffects.Add(this);

        Enemy.DefaultSpeed = (1 - Value) * Enemy.InitSpeed;
        Enemy.CurrentSpeed = Enemy.DefaultSpeed;
    }

    public override void UpdateEffects(EnemyEffect effect)
    {
        throw new NotImplementedException();
    }
}