using System;

namespace Zero
{
    public class OnGUIEventListener : AEventListener<OnGUIEventListener>
    {
        public event Action onGUI;

        private void OnGUI()
        {
            onGUI?.Invoke();
        }
    }
}