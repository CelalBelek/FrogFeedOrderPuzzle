using System;
public class LevelEvents
{
    public static event Action NewModule;
    public static void TriggerNewModule() => NewModule?.Invoke();
}
