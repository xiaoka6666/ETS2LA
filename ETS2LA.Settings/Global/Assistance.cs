namespace ETS2LA.Settings.Global;

using System.ComponentModel;

public enum AccelerationResponseOption
{
    [Description("慢速")]
    Slow,
    [Description("正常")]
    Normal,
    [Description("快速")]
    Fast
}

public enum SteeringSensitivityOption
{
    [Description("慢速")]
    Slow,
    [Description("正常")]
    Normal,
    [Description("快速")]
    Fast
}

public enum FollowingDistanceOption
{
    [Description("近")]
    Near,
    [Description("正常")]
    Normal,
    [Description("远")]
    Far
}

public enum SetSpeedBehaviour
{
    [Description("限速")]
    SpeedLimit,
    [Description("当前速度")]
    CurrentSpeed
}

public enum SpeedLimitWarning
{
    [Description("关闭")]
    Off,
    [Description("视觉提醒")]
    Visual,
    [Description("提示音")]
    Chime
}

public enum CollisionAvoidance
{
    [Description("关闭")]
    Off,
    [Description("较晚")]
    Late,
    [Description("中等")]
    Medium,
    [Description("较早")]
    Early
}

[Serializable]
public class AssistanceSettings
{
    [NonSerialized]
    private static readonly Lazy<AssistanceSettings> _instance = new(() => new AssistanceSettings(loadSettings: true));
    public static AssistanceSettings Current => _instance.Value;

    public bool SeparateCruiseAndSteering { get; set; } = false;
    public bool PauseWhenUnstable { get; set; } = true;
    public AccelerationResponseOption AccelerationResponse { get; set; } = AccelerationResponseOption.Normal;
    public SteeringSensitivityOption SteeringSensitivity { get; set; } = SteeringSensitivityOption.Normal;
    public FollowingDistanceOption FollowingDistance { get; set; } = FollowingDistanceOption.Normal;
    public SetSpeedBehaviour SetSpeedBehaviourOption { get; set; } = SetSpeedBehaviour.SpeedLimit;
    public SpeedLimitWarning SpeedLimitWarningOption { get; set; } = SpeedLimitWarning.Visual;
    public CollisionAvoidance CollisionAvoidanceOption { get; set; } = CollisionAvoidance.Early;
    public float MaximumSpeed { get; set; } = 0f; // 0 = no limit
    public bool IgnoreTrafficRules { get; set; } = false;

    [NonSerialized]
    private SettingsHandler? _settingsHandler;

    public AssistanceSettings(bool loadSettings = false)
    {
        if (loadSettings)
        {
            _settingsHandler = new SettingsHandler();
            var loadedSettings = _settingsHandler.Load<AssistanceSettings>("AssistanceSettings.json");
            if (loadedSettings != null)
            {
                SeparateCruiseAndSteering = loadedSettings.SeparateCruiseAndSteering;
                PauseWhenUnstable = loadedSettings.PauseWhenUnstable;
                AccelerationResponse = loadedSettings.AccelerationResponse;
                SteeringSensitivity = loadedSettings.SteeringSensitivity;
                FollowingDistance = loadedSettings.FollowingDistance;
                SetSpeedBehaviourOption = loadedSettings.SetSpeedBehaviourOption;
                SpeedLimitWarningOption = loadedSettings.SpeedLimitWarningOption;
                CollisionAvoidanceOption = loadedSettings.CollisionAvoidanceOption;
                MaximumSpeed = loadedSettings.MaximumSpeed;
                IgnoreTrafficRules = loadedSettings.IgnoreTrafficRules;
            }
            _settingsHandler.RegisterListener<AssistanceSettings>("AssistanceSettings.json", OnSettingsChanged);
        }
    }

    public AssistanceSettings() { }

    public void Save()
    {
        _settingsHandler?.Save<AssistanceSettings>("AssistanceSettings.json", this);
    }

    public void OnSettingsChanged(AssistanceSettings newSettings)
    {
        SeparateCruiseAndSteering = newSettings.SeparateCruiseAndSteering;
        PauseWhenUnstable = newSettings.PauseWhenUnstable;
        AccelerationResponse = newSettings.AccelerationResponse;
        SteeringSensitivity = newSettings.SteeringSensitivity;
        FollowingDistance = newSettings.FollowingDistance;
        SetSpeedBehaviourOption = newSettings.SetSpeedBehaviourOption;
        SpeedLimitWarningOption = newSettings.SpeedLimitWarningOption;
        CollisionAvoidanceOption = newSettings.CollisionAvoidanceOption;
        MaximumSpeed = newSettings.MaximumSpeed;
        IgnoreTrafficRules = newSettings.IgnoreTrafficRules;
    }
}