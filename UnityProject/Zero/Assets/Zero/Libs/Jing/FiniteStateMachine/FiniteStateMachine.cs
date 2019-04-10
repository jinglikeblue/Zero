using System;
using System.Collections.Generic;

/* 示例代码
public class TestFSM
{
    public enum ET
    {
        A,
        B,
        C,
    }

    FiniteStateMachine<ET> fsm = new FiniteStateMachine<ET>();
    
    public TestFSM()
    {
        fsm.RegistState(ET.A, OnEnter, OnExit, OnUpdate, SwitchEnable);
        fsm.RegistState(ET.B, OnEnter, OnExit, OnUpdate, SwitchEnable);
        fsm.SwitchState(ET.A);
        fsm.Update();
        fsm.SwitchState(ET.B);
        fsm.Update();
        fsm.SwitchState(ET.C);
        fsm.Update();
    }

    public void OnEnter(ET from)
    {

    }

    public void OnExit(ET to)
    {

    }

    public void OnUpdate()
    {
        Debug.Log("state: " + fsm.curState);
    }

    public bool SwitchEnable(ET toState)
    {
        return true;
    }
}
*/

namespace Jing
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// 状态控制器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class StateController<T>
        {
            /// <summary>
            /// 进入状态的委托
            /// </summary>
            /// <param name="fromState"></param>
            public Action<T, object> onEnter;

            /// <summary>
            /// 退出状态的委托
            /// </summary>
            /// <param name="toState"></param>
            public Action<T> onExit;

            /// <summary>
            /// 更新状态的委托
            /// <param name="curState"></param>
            /// </summary>
            public Action<T> onUpdate;

            /// <summary>
            /// 切换状态检查的委托
            /// </summary>
            /// <param name="toState"></param>
            /// <returns></returns>
            public Func<T, bool> checkSwitchEnable;

            /// <summary>
            /// 状态
            /// </summary>
            public T state;

            /// <summary>
            /// 配置的能切换到的状态，null表示不限制
            /// </summary>
            public HashSet<T> roleSwitch = null;

            public StateController(T state)
            {
                this.state = state;
            }
        }

        /// <summary>
        /// 在当前状态下经过的时间(根据Update传入的dt值累计）
        /// </summary>
        public float StateStayTime { get; private set; }

        /// <summary>
        /// 当前状态
        /// </summary>
        public T CurState { get; private set; }

        /// <summary>
        /// 状态字典
        /// </summary>
        Dictionary<T, StateController<T>> _stateDic = new Dictionary<T, StateController<T>>();

        /// <summary>
        /// 状态机名称
        /// </summary>
        public string Name { get; private set; }

        public FiniteStateMachine(string name = null)
        {
            this.Name = name;
        }

        /// <summary>
        /// 注册一个状态，不适用的方法可以传递Null
        /// </summary>
        public void RegistState(T state, Action<T, object> onEnter = null, Action<T> onExit = null, Action<T> onUpdate = null, Func<T, bool> checkSwitchEnable = null)
        {
            StateController<T> sc = new StateController<T>(state);
            sc.onEnter = onEnter;
            sc.onExit = onExit;
            sc.onUpdate = onUpdate;
            sc.checkSwitchEnable = checkSwitchEnable;

            if (null == CurState)
            {
                //设置为第一个状态
                CurState = state;
            }

            _stateDic[state] = sc;
        }

        /// <summary>
        /// 注销一个状态
        /// </summary>
        public void UnregistState(T state)
        {
            if (_stateDic.ContainsKey(state))
            {
                _stateDic.Remove(state);
            }

            if (CurState.Equals(state))
            {
                CurState = default(T);
            }
        }

        /// <summary>
        /// 添加一个合法的状态转换规则
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        public void AddSwitchRole(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState))
            {
                return;
            }


            if (null == _stateDic[fromState].roleSwitch)
            {
                _stateDic[fromState].roleSwitch = new HashSet<T>();
            }

            _stateDic[fromState].roleSwitch.Add(toState);
        }

        /// <summary>
        /// 移除一个合法的状态转换规则
        /// </summary>
        /// <param name="fromState"></param>
        public void RemoveSwitchRole(T fromState, T toState)
        {
            if (false == _stateDic.ContainsKey(fromState) || null == _stateDic[fromState].roleSwitch)
            {
                return;
            }

            _stateDic[fromState].roleSwitch.Remove(toState);
        }

        /// <summary>
        /// 进入一个状态
        /// </summary>
        public bool SwitchState(T toState, object data = null)
        {
            if (false == _stateDic.ContainsKey(toState))
            {
                return false;
            }

            var oldSC = _stateDic[CurState];

            if (oldSC.roleSwitch != null && false == oldSC.roleSwitch.Contains(toState))
            {
                return false;
            }

            var newSC = _stateDic[toState];

            if (null != oldSC.checkSwitchEnable && false == oldSC.checkSwitchEnable.Invoke(toState))
            {
                return false;
            }

            if (null != oldSC.onExit)
            {
                oldSC.onExit.Invoke(toState);
            }
            CurState = toState;
            StateStayTime = 0;
            if (null != newSC.onEnter)
            {
                newSC.onEnter.Invoke(oldSC.state, data);
            }
            return true;
        }

        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="dt">距离上次状态更新的间隔，如果传入，可以统计状态持续的时间</param>
        public void Update(float dt = 0f)
        {
            StateStayTime += dt;
            var nowSC = _stateDic[CurState];
            if (null != nowSC.onUpdate)
            {
                nowSC.onUpdate.Invoke(CurState);
            }
        }
    }
}
