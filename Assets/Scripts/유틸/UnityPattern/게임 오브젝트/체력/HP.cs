using System;

using UnityEditor;
using UnityEngine;

namespace inonego
{

using inonego.util;

public class HP : MonoBehaviour
{   

#region Enumerations

    public enum State       { Alive, Dead, None }

    private enum ApplyType   { Heal, Damage }

#endregion

#region Events

    #region EventArgs

        public struct StateChangedEventArgs
        {
            public State Previous;
            public State Current;
        }

        public struct HPChangedEventArgs
        {
            public int PreviousValue;
            public int CurrentValue;
            public int Delta;
        }

        public struct AppliedEventArgs
        {
            public int PreviousValue;
            public int CurrentValue;
            public int Delta;

            public int? Heal;
            public int? Damage;
        }

    #endregion

    /// <summary>
    /// 상태가 변화되었을때 호출되는 이벤트입니다.
    /// </summary>
    public Event<HP, StateChangedEventArgs> OnStateChangedEvent = new();
    public event Action<HP, StateChangedEventArgs> OnStateChanged     { add => OnStateChangedEvent += value; remove => OnStateChangedEvent -= value; }

    /// <summary>
    /// 체력이 변경되었을때 호출되는 이벤트입니다.
    /// </summary>
    public Event<HP, HPChangedEventArgs> OnHPChangedEvent = new();
    public event Action<HP, HPChangedEventArgs> OnHPChanged           { add => OnHPChangedEvent += value; remove => OnHPChangedEvent -= value; }

    /// <summary>
    /// 힐이나 데미지가 적용되었을때 호출되는 이벤트입니다.
    /// </summary>
    public Event<HP, AppliedEventArgs> OnAppliedEvent = new();
    public event Action<HP, AppliedEventArgs> OnApplied               { add => OnAppliedEvent += value; remove => OnAppliedEvent -= value; }

#endregion

    /// <summary>
    /// 현재 상태
    /// </summary>
    [field: SerializeField] public State Current { get; private set; } = State.None;

    /// <summary>
    /// 살아있는 상태인지 여부
    /// </summary>
    public bool IsAlive => Current == State.Alive;

    /// <summary>
    /// 죽어있는 상태인지 여부
    /// </summary>
    public bool IsDead  => Current == State.Dead;

    [field: SerializeField] public bool AliveOnAwake  { get; set; } = true;
    [field: SerializeField] public bool DestroyOnDead { get; set; } = true;
    
    [field: SerializeField] public int Value       { get; private set; } = 0;
    [field: SerializeField] public int MaxValue    { get; private set; } = 0;

    private State previous  = State.None;
    private int previousValue  = 0;
    private int? heal       = null;
    private int? damage     = null;

    private void Clear()
    {
        previous   = Current;
        previousValue = Value;
        heal       = null;
        damage     = null;
    }

    private void Awake()
    {
        if (AliveOnAwake)
        {
            SetAlive();
        }
    }

    private void Update()
    {
        ProcessEvent();

        // 이전 상태를 재설정합니다.
        Clear();
    }

    protected virtual void Destroy() => Destroy(gameObject);

    /// <summary>
    /// 이벤트를 처리합니다.
    /// </summary>
    private void ProcessEvent()
    {
        // 체력이 회복되거나 데미지를 입었을때 체력을 설정하도록 합니다.
        if (heal is not null || damage is not null) SetValue(Value + (heal ?? 0) - (damage ?? 0));

        OnAppliedEvent.InvokeIfDirty(this, new AppliedEventArgs { PreviousValue = previousValue, CurrentValue = Value, Delta = Value - previousValue, Heal = heal, Damage = damage });

        OnHPChangedEvent.InvokeIfDirty(this, new HPChangedEventArgs { PreviousValue = previousValue, CurrentValue = Value, Delta = Value - previousValue });

        // 살아있는데
        if (IsAlive)
        {
            // HP가 0이면
            if (Value == 0)
            {
                // 죽은 상태로 설정합니다.
                SetDead();
            }
        }

        OnStateChangedEvent.InvokeIfDirty(this, new StateChangedEventArgs { Previous = previous, Current = Current });

        if (IsDead)
        {
            #if UNITY_EDITOR
                if (!EditorApplication.isPlaying) return;
            #endif
        
            if (DestroyOnDead)
            {
                Destroy();
            }
        }
    }

    /// <summary>
    /// 상태를 설정합니다.
    /// </summary>
    /// <param name="state">설정할 상태</param>
    private void SetState(State state)
    {
        Current = state;
        
        if (Current == State.Alive)
        {
            SetValue(MaxValue);
        }
        else
        //
        if (Current == State.Dead)
        {
            SetValue(0);
        }

        OnStateChangedEvent.SetDirty();
    }

    /// <summary>
    /// 현재 상태를 살아있는 상태로 설정합니다.
    /// </summary>
    public void SetAlive()
    {
        SetState(State.Alive);
    }

    /// <summary>
    /// 현재 상태를 죽어있는 상태로 설정합니다.
    /// </summary>
    public void SetDead()
    {
        SetState(State.Dead);
    }

    /// <summary>
    /// 체력을 설정합니다.
    /// </summary>
    /// <param name="hp">체력 값</param>
    public void SetValue(int hp)
    {
        Value = Mathf.Clamp(hp, 0, MaxValue);

        OnHPChangedEvent.SetDirty();
    }

    /// <summary>
    /// 최대 체력을 설정합니다.
    /// </summary>
    /// <param name="maxHP">최대 체력 값</param>
    public void SetMaxValue(int value)
    {
        if (value < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("최대 체력 값은 0보다 작을 수 없습니다.");
            #endif
    
            value = 0;
        }
        
        MaxValue = value;

        // 최대 체력이 변경됨에 따라 HP도 조정되도록 합니다.
        SetValue(Value);
    }
    
    /// <summary>
    /// 힐(체력 회복)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyHeal(int value)
    {
        Apply(ApplyType.Heal, value);
    }

    /// <summary>
    /// 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyDamage(int value)
    {
        Apply(ApplyType.Damage, value);
    }

    /// <summary>
    /// 힐(체력 회복) 또는 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="applyType">적용할 타입</param>
    /// <param name="value">적용할 값</param>
    private void Apply(ApplyType applyType, int value)
    {
        // 죽은 상태면 데미지나 힐을 받지 않습니다.
        if (IsDead) return;

        if (value < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("값이 0보다 작을 수 없습니다.");
            #endif
    
            value = 0;
        }
        
        if (applyType == ApplyType.Heal)
        {
            if (heal is null) heal = 0;

            heal += value;
        }
        else
        {
            if (damage is null) damage = 0;

            damage += value; 
        }
        
        OnAppliedEvent.SetDirty();
    }
}

}