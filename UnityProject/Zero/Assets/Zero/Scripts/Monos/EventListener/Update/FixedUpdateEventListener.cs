using System;

namespace Zero
{
    public class FixedUpdateEventListener : AEventListener<UpdateEventListener>
    {
        public event Action onFixedUpdate;

        private void FixedUpdate()
        {
            if (null != onFixedUpdate)
            {
                onFixedUpdate.Invoke();
            }
        }                 
    }
}