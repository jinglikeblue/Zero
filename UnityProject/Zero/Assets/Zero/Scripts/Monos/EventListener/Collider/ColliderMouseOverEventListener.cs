using System;

namespace Zero
{
    public class ColliderMouseOverEventListener : AEventListener<ColliderMouseOverEventListener>
    {

        public event Action OnEvent;
        private void OnMouseOver()
        {
            OnEvent?.Invoke();
        }
    }
}