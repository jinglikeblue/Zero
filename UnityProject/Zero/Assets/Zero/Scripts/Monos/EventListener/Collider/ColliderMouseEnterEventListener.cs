using System;
using Zero;

namespace Zero
{
    public class ColliderMouseEnterEventListener : AEventListener<ColliderMouseEnterEventListener>
    {
        public event Action onEvent;

        private void OnMouseEnter()
        {
            if (null != onEvent)
            {
                onEvent.Invoke();
            }
        }
    }
}