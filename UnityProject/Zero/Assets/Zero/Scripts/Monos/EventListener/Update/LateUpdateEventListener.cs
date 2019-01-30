using System;

namespace Zero
{
    public class LateUpdateEventListener :AEventListener<LateUpdateEventListener>
    {
        public event Action onLateUpdate;

        private void LateUpdate()
        {
            if (null != onLateUpdate)
            {
                onLateUpdate.Invoke();
            }
        }
    }
}