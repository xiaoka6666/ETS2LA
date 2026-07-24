namespace ETS2LA.Controls.Defaults;

public static class DefaultControls
{
    public static ControlDefinition Assist { get; } = new ControlDefinition
    {
        Id = "ETS2LA.Controls.Assist",
        Name = "辅助",
        Description = "将开启或关闭 ETS2LA 的辅助功能。不会更新速度，如需更新请使用 SET 键。您可以在辅助驾驶设置中更改此键（和 SET 键）的行为方式。",
        DefaultKeybind = "N",
        Type = ControlType.Boolean
    };

    public static ControlDefinition SET { get; } = new ControlDefinition
    {
        Id = "ETS2LA.Controls.SET",
        Name = "SET/确认",
        Description = "功能类似于辅助键，但会按照您在辅助驾驶设置中选择的方式运作。此键还会用于确认操作。",
        DefaultKeybind = "Left",
        Type = ControlType.Boolean
    };

    public static ControlDefinition Next { get; } = new ControlDefinition
    {
        Id = "ETS2LA.Controls.Next",
        Name = "下一步/取消",
        Description = "此键将向前导航任何 ETS2LA 菜单，同时也用作任何确认操作的取消键。",
        DefaultKeybind = "Right",
        Type = ControlType.Boolean
    };

    public static ControlDefinition Increase { get; } = new ControlDefinition
    {
        Id = "ETS2LA.Controls.Increase",
        Name = "增加",
        Description = "将当前值（例如目标速度）增加一步。如果 UI 中未显示任何视觉修饰符，则会将目标速度增加 1 km/h。",
        DefaultKeybind = "Up",
        Type = ControlType.Boolean
    };

    public static ControlDefinition Decrease { get; } = new ControlDefinition
    {
        Id = "ETS2LA.Controls.Decrease",
        Name = "减少",
        Description = "将当前值（例如目标速度）减少一步。如果 UI 中未显示任何视觉修饰符，则会将目标速度减少 1 km/h。",
        DefaultKeybind = "Down",
        Type = ControlType.Boolean
    };

}