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
    /// 状态控制器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class StateController<T>
    {
        public FiniteStateMachine<T>.OnEnter onEnter;
        public FiniteStateMachine<T>.OnExit onExit;
        public FiniteStateMachine<T>.OnUpdate onUpdate;
        public FiniteStateMachine<T>.CheckSwitchEnable checkSwitchEnable;

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
    /// 有限状态机
    /// </summary>
    public class FiniteStateMachine<T>
    {
        /// <summary>
        /// 进入状态的委托
        /// </summary>
        /// <param name="fromState"></param>
        public delegate void OnEnter(T fromState);

        /// <summary>
        /// 退出状态的委托
        /// </summary>
        /// <param name="toState"></param>
        public delegate void OnExit(T toState);

        /// <summary>
        /// 更新状态的委托
        /// </summary>
        public delegate void OnUpdate(T curState);

        /// <summary>
        /// 切换状态检查的委托
        /// </summary>
        /// <param name="fromState"></param>
        /// <param name="toState"></param>
        /// <returns></returns>
        public delegate bool CheckSwitchEnable(T toState);

        float _stateTime;

        /// <summary>
        /// 在当前状态下经过的时间
        /// </summary>
        public float stateTime
        {
            get { return _stateTime; }
        }


        T _curState;

        /// <summary>
        /// 当前状态
        /// </summary>
        public T curState {
            get { return _curState; }
        }


        /// <summary>
        /// 状态字典
        /// </summary>
        Dictionary<T, StateController<T>> _stateDic = new Dictionary<T, StateController<T>>();

        string _name;

        public FiniteStateMachine(string name = null)
        {
            _name = name;
        }

        void DefaultEnter(T fromState)
        {

        }

        void DefaultExit(T toState)
        {

        }

        void DefaultUpdate(T curState)
        {

        }

        bool DefaultCheckSwitchEnable(T toState)
        {
            return true;
        }

        /// <summary>
        /// 注册一个状态，不适用的方法可以传递Null
        /// </summary>
        public void RegistState(T state, OnEnter onEnter = null, OnExit onExit = null, OnUpdate onUpdate = null, CheckSwitchEnable checkSwitchEnable = null)
        {
            if(null == onEnter)
            {
                onEnter = DefaultEnter;
            }

            if (null == onExit)
            {
                onExit = DefaultExit;
            }

            if (null == onUpdate)
            {
                onUpdate = DefaultUpdate;
            }

            if (null == checkSwitchEnable)
            {
                checkSwitchEnable = DefaultCheckSwitchEnable;
            }


            StateController<T> sc = new StateController<T>(state);
            sc.onEnter = onEnter;
            sc.onExit = onExit;
            sc.onUpdate = onUpdate;
            sc.checkSwitchEnable = checkSwitchEnable;

            if(null == _curState)
            {
                //设置为第一个状态
                _curState = state;
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

            if(_curState.Equals(state))
            {
                _curState = default(T);    
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
        public bool SwitchState(T toState)
        {
            if (false == _stateDic.ContainsKey(toState))
            {
                return false;
            }

            var oldSC = _stateDic[_curState];

            if (oldSC.roleSwitch != null && false == oldSC.roleSwitch.Contains(toState))
            {
                return false;
            }

            var newSC = _stateDic[toState];

            if(false == oldSC.checkSwitchEnable(toState))
            {
                return false;
            }

            oldSC.onExit(toState);
            _curState = toState;
            _stateTime = 0;
            newSC.onEnter(oldSC.state);
            return true;
        }

        public void Update(float dt = 0f)
        {
            _stateTime += dt;
            var nowSC = _stateDic[_curState];
            nowSC.onUpdate(_curState);
        }
    }
}
