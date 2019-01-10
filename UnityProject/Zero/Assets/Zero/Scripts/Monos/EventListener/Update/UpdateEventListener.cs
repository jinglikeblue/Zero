using System;

namespace Zero
{
    public class UpdateEventListener : AEventListener<UpdateEventListener>
    {
        public event Action onUpdate;

        private void Update()
        {
            onUpdate?.Invoke();
        }
    }
}