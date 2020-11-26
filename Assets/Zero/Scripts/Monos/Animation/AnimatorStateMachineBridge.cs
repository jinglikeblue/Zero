using UnityEngine;

namespace Zero
{
    /// <summary>
    /// 动画状态机事件桥接器
    /// </summary>    
    public class AnimatorStateMachineBridge : StateMachineBehaviour
    {
        public delegate void AnimatorStateDelegate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex);

        public event AnimatorStateDelegate onStateEnter;
        public event AnimatorStateDelegate onStateUpdate;
        public event AnimatorStateDelegate onStateExit;
        public event AnimatorStateDelegate onStateMove;
        public event AnimatorStateDelegate onStateIK;

        public void RemoveAllEvents()
        {
            onStateEnter = null;
            onStateUpdate = null;
            onStateExit = null;
            onStateMove = null;
            onStateIK = null;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log(Log.Zero2("OnStateEnter"));
            onStateEnter?.Invoke(animator, stateInfo, layerIndex);            
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log(Log.Zero2("OnStateUpdate {0}", stateInfo.normalizedTime));
            onStateUpdate?.Invoke(animator, stateInfo, layerIndex);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log(Log.Zero2("OnStateExit"));
            onStateExit?.Invoke(animator, stateInfo, layerIndex);
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that processes and affects root motion
            //Debug.Log(Log.Zero2("OnStateMove"));
            onStateMove?.Invoke(animator, stateInfo, layerIndex);
        }

        // OnStateIK is called right after Animator.OnAnimatorIK()
        override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Implement code that sets up animation IK (inverse kinematics)
            //Debug.Log(Log.Zero2("OnStateIK"));
            onStateIK?.Invoke(animator, stateInfo, layerIndex);
        }
    }
}