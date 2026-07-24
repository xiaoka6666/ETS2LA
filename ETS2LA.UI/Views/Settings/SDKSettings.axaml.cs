using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;

using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;

using ETS2LA.Game;
using ETS2LA.Notifications;
using ETS2LA.Logging;

namespace ETS2LA.UI.Views.Settings;

public partial class SDKSettings : UserControl
{
    public ObservableCollection<GameItem> Games { get; } = new();

    public SDKSettings()
    {
        InitializeComponent();
        DataContext = this;
        UpdateGamesList();
    }

    private void UpdateGamesList()
    {
        Games.Clear();
        foreach (var installation in GameHandler.Current.Installations)
        {
            Games.Add(new GameItem(installation));
        }
    }

    private void OnTriggerChange(object? sender, PointerPressedEventArgs e)
    {
        if (!e.Properties.IsLeftButtonPressed)
            return;

        if (sender is Control { Tag: GameItem item })
        {
            Task.Run(() => item.TriggerChange());
        }
    }

    private void OnTriggerChangeKey(object? sender, KeyEventArgs e)
    {
        if (sender is Control { Tag: GameItem item })
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                Task.Run(() => item.TriggerChange());
            }
        }
    }

    private async void OnAddGameManually(object? sender, RoutedEventArgs e)
    {
        try
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择游戏的安装文件夹",
                AllowMultiple = false
            });

            if (folders.Count == 0)
                return;

            string? gamePath = folders[0].TryGetLocalPath();
            var installation = gamePath != null ? GameHandler.Current.AddManualInstallation(gamePath) : null;
            if (installation == null)
            {
                NotificationHandler.Current.SendNotification(new Notification
                {
                    Id = "ETS2LA.UI.SDKSettings.AddGameFailed",
                    Title = "无法添加游戏",
                    Content = "在所选文件夹中未找到 ETS2 或 ATS 的可执行文件。请选择游戏的安装文件夹，例如「.../steamapps/common/Euro Truck Simulator 2」。",
                    Level = NotificationLevel.Danger
                });
                return;
            }

            UpdateGamesList();
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to add game manually: {ex.Message}");
        }
    }

    private void OnRemoveGame(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { Tag: GameItem item })
        {
            GameHandler.Current.RemoveManualInstallation(item.Installation);
            UpdateGamesList();
        }
    }
}

public class GameItem : INotifyPropertyChanged
{
    public string Name => GetName();
    public string Version => installation.Version;
    public string UpdatedVersion { get; set; } = string.Empty;
    public string Path => installation.Path;
    public bool IsUnknownVersion => !Version.Contains(".");
    public bool IsSDKInstalled => installation.IsSDKInstalled(IsUnknownVersion ? UpdatedVersion : Version);
    public bool IsManuallyAdded => installation.IsManuallyAdded;

    public string AutomationName => GetAutomationName();

    public Installation Installation => installation;

    private Installation installation;

    public GameItem(Installation installation)
    {
        this.installation = installation;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void TriggerChange()
    {
        if (IsSDKInstalled)
        {
            if (installation.UninstallSDK(IsUnknownVersion ? UpdatedVersion : Version))
            {
                Logger.Info($"Uninstalled SDK for {Name} at {Path}");
                NotificationHandler.Current.SendNotification(new Notification
                {
                    Id = "ETS2LA.UI.SDKSettings.Uninstall",
                    Title = $"已卸载 {Name} 的 SDK",
                    Content = $"已在 {Path} 成功卸载 {Name} 的 SDK。",
                    Level = NotificationLevel.Success
                });
            }
            else
            {
                Logger.Error($"Failed to uninstall SDK for {Name} at {Path}");
                NotificationHandler.Current.SendNotification(new Notification
                {
                    Id = "ETS2LA.UI.SDKSettings.UninstallFailed",
                    Title = $"卸载 {Name} 的 SDK 失败",
                    Content = $"在 {Path} 卸载 {Name} 的 SDK 时发生错误。请查看日志了解详情。",
                    Level = NotificationLevel.Danger
                });
            }
        }
        else
        {
            if (installation.InstallSDK(IsUnknownVersion ? UpdatedVersion : Version))
            {
                Logger.Info($"Installed SDK for {Name} at {Path}");
                NotificationHandler.Current.SendNotification(new Notification
                {
                    Id = "ETS2LA.UI.SDKSettings.Install",
                    Title = $"已安装 {Name} 的 SDK",
                    Content = $"已在 {Path} 成功安装 {Name} 的 SDK。",
                    Level = NotificationLevel.Success
                });
            }
            else
            {
                Logger.Error($"Failed to install SDK for {Name} at {Path}");
                NotificationHandler.Current.SendNotification(new Notification
                {
                    Id = "ETS2LA.UI.SDKSettings.InstallFailed",
                    Title = $"安装 {Name} 的 SDK 失败",
                    Content = $"在 {Path} 安装 {Name} 的 SDK 时发生错误。请查看日志了解详情。",
                    Level = NotificationLevel.Danger
                });
            }
        }

        OnPropertyChanged(nameof(IsSDKInstalled));
    }

    private string GetName()
    {
        return installation.Type == GameType.EuroTruckSimulator2 ? "Euro Truck Simulator 2" : "American Truck Simulator";
    }

    private string GetAutomationName()
    {
        return $"{Name} {Version}, SDK is {(IsSDKInstalled ? "Installed" : "Not Installed")} at {Path}, button";
    }
}
