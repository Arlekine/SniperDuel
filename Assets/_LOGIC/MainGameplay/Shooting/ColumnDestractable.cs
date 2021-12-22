using System;

public class ColumnDestractable : DestractablePart
{
    public Action<ColumnDestractable> onDestroyed;
    
    public override void Destroy()
    {
        base.Destroy();
        onDestroyed?.Invoke(this);
    }
}