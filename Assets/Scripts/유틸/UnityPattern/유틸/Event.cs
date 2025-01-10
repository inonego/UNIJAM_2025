using System;

using UnityEngine;
using UnityEngine.Events;

namespace inonego.util
{
    public struct EventArgs { }

    public class Event<TSender, TEventArgs>
    {
        private event Action<TSender, TEventArgs> internal_event;

        [SerializeField]
        private UnityEvent<TSender, TEventArgs> internal_unity_event;

        /// <summary>
        /// 이벤트의 더티 플래그 상태를 나타냅니다.
        /// </summary>
        public bool IsDirty { get; private set; } = false;

        /// <summary>
        /// 이벤트의 더티 플래그를 활성화 시킵니다.
        /// </summary>
        public void SetDirty()
        {
            IsDirty = true;
        }

        /// <summary>
        /// 이벤트에 액션을 추가합니다. 
        /// </summary>
        /// <param name="target">이벤트</param>
        /// <param name="action">추가할 액션</param>
        /// <returns>이벤트</returns>
        public static Event<TSender, TEventArgs> operator +(Event<TSender, TEventArgs> target, Action<TSender, TEventArgs> action)
        {
            target.internal_event += action;

            return target;
        }

        /// <summary>
        /// 이벤트에서 액션을 제거합니다.
        /// </summary>
        /// <param name="target">이벤트</param>
        /// <param name="action">제거할 액션</param>
        /// <returns>이벤트</returns>
        public static Event<TSender, TEventArgs> operator -(Event<TSender, TEventArgs> target, Action<TSender, TEventArgs> action)
        {
            target.internal_event -= action;

            return target;
        }

        /// <summary>
        /// 이벤트를 즉시 호출합니다.
        /// </summary>
        /// <param name="sender">이벤트를 발생시킨 객체</param>
        /// <param name="eventArgs">이벤트 인자</param>
        public void InvokeHere(TSender sender, TEventArgs eventArgs)
        {
            internal_event?.Invoke(sender, eventArgs);
            internal_unity_event?.Invoke(sender, eventArgs);
                
            IsDirty = false;
        }

        /// <summary>
        /// 이벤트의 더티 플래그가 활성화 되어있는 경우에 이벤트를 호출합니다.
        /// </summary>
        /// <param name="sender">이벤트를 발생시킨 객체</param>
        /// <param name="eventArgs">이벤트 인자</param>
        public void InvokeIfDirty(TSender sender, TEventArgs eventArgs)
        {
            if (IsDirty)
            {
                InvokeHere(sender, eventArgs);
            }
        }
    }
}

