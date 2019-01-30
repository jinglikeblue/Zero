using System;

namespace Zero
{
    public class UpdateEventListener : AEventListener<UpdateEventListener>
    {
        public event Action onUpdate;

        private void Update()
        {
            if (null != onUpdate)
            {
                onUpdate.Invoke();
            }
        }
    }
}