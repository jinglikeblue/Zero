using System;

public class ColliderMouseUpAsButtonEventListener : AEventListener<ColliderMouseUpAsButtonEventListener>
{
    public event Action OnEvent;

    private void OnMouseUpAsButton()
    {
        OnEvent?.Invoke();
    }
}
