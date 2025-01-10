using System;

using UnityEngine;

namespace inonego
{

using inonego.util;

public class Timer
{
    public class TimeDATA
    {
        public float Total      { get; private set; }
        public float Current    { get; private set; }

        public float LeftTime       => Current;
        public float ElapsedTime    => Total - Current;
        public float LeftTime01     => LeftTime / Total;
        public float ElapsedTime01  => ElapsedTime / Total;

        public void Set(float total, float current)
        {
            Total = total;
            Current = current;
        }
    }

#region Enumerations

    public enum State
    {
        Started, Paused, Stopped
    }

#endregion

#region Events

    #region EventArgs

        public struct StateChangedEventArgs
        {
            public State Previous;
            public State Current;
        }        
        
        public struct EndedEventArgs
        {
            // TODO
        }

    #endregion

    /// <summary>
    /// 상태가 변화되었을떼 호출되는 이벤트입니다.
    /// </summary>
    public event Action<Timer, StateChangedEventArgs> OnStateChanged { add => OnStateChangedEvent += value; remove => OnStateChangedEvent -= value; }
    private Event<Timer, StateChangedEventArgs> OnStateChangedEvent = new();

    /// <summary>
    /// 타이머가 종료되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event Action<Timer, EndedEventArgs> OnEnded { add => OnEndedEvent += value; remove => OnEndedEvent -= value; }
    private Event<Timer, EndedEventArgs> OnEndedEvent = new();

#endregion

    [field: SerializeField]
    public State Current { get; private set; } = State.Stopped;

    public bool IsWorking => Current == State.Started;

    public TimeDATA Time { get; private set; } = new();

    private State previous  = State.Stopped;

    private void Clear()
    {
        previous = Current;
    }
    
    /// <summary>
    /// 타이머를 업데이트합니다.
    /// </summary>
    public void Update()
    {
        if (IsWorking)
        {
            Time.Set(total: Time.Total, current: Time.Current - UnityEngine.Time.deltaTime);

            // 타이머의 시간이 0 이하가 되면 타이머를 종료합니다.
            if (Time.Current <= 0f)
            {
                Stop();
                
                OnEndedEvent.SetDirty();
            }
        }

        OnStateChangedEvent.InvokeIfDirty(this, new StateChangedEventArgs { Previous = previous, Current = Current });

        OnEndedEvent.InvokeIfDirty(this, new EndedEventArgs { });

        Clear();
    }

    private void SetState(State state)
    {
        Current = state;

        OnStateChangedEvent.SetDirty();
    }

    /// <summary>
    /// 타이머의 작동을 시작시킵니다.
    /// </summary>
    /// <param name="time">시간 값</param>
    public void Start(float time)
    {
        Time.Set(total: time, current: time);
        
        SetState(State.Started);
    }

    /// <summary>
    /// 타이머의 작동을 중지시킵니다.
    /// </summary>
    public void Stop()
    {
        if (Current != State.Stopped)
        {
            Time.Set(total: 0f, current: 0f);

            SetState(State.Stopped);
        }
    }

    /// <summary>
    /// 타이머의 작동을 일시정지시킵니다.
    /// </summary>
    public void Pause()
    {
        if (Current == State.Started)
        {
            SetState(State.Paused);
        }
    }

    /// <summary>
    /// 타이머의 작동을 재개시킵니다.
    /// </summary>
    public void Resume()
    {
        if (Current == State.Paused)
        {
            SetState(State.Started);
        }
    }
}

}