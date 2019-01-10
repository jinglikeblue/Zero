using System;

namespace Zero
{
    public class ColliderMouseDownEventListener : AEventListener<ColliderMouseDownEventListener>
    {
        public event Action onEvent;

        private void OnMouseDown()
        {
            onEvent?.Invoke();
        }
    }
}