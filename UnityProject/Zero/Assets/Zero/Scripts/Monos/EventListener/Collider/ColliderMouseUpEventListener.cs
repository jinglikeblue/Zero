using System;

public class ColliderMouseUpEventListener : AEventListener<ColliderMouseUpEventListener>
{
    public event Action OnEvent;

    private void OnMouseUp()
    {
        OnEvent?.Invoke();
    }

}
