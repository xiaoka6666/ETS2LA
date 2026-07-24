using ETS2LA.Networking.Users;
using ETS2LA.Networking.Settings;
using ETS2LA.Backend;
using ETS2LA.Backend.Events;
using ETS2LA.Backend.Plugins;
using ETS2LA.Notifications;
using ETS2LA.Logging;

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Reflection;

namespace ETS2LA.Networking.Plugins;

public class PluginApiClient
{
    public List<NetworkPlugin> AvailablePlugins { get; private set; } = new List<NetworkPlugin>();

    JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
    };

    private void Log(string message, NotificationLevel level = NotificationLevel.Information)
    {
        switch (level)
        {
            case NotificationLevel.Information:
                Logger.Info(message);
                break;
            case NotificationLevel.Warning:
                Logger.Warn(message);
                break;
            case NotificationLevel.Danger:
                Logger.Error(message);
                break;
            case NotificationLevel.Success:
                Logger.Success(message);
                break;
            default:
                Logger.Info(message);
                break;
        }

        NotificationHandler.Current.SendNotification(new Notification
        {
            Id = Guid.NewGuid().ToString(),
            Title = "插件安装器",
            Content = message,
            Level = level
        });
    }

    public async Task FetchAvailablePluginsAsync()
    {
        try
        {
            var apiServer = NetworkingSettings.Current.CurrentApiServer;
            if (apiServer == null)
            {
                throw new InvalidOperationException("CurrentApiServer is not set.");
            }

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{apiServer.Value.BaseUrl}/plugins");
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            AvailablePlugins = JsonSerializer.Deserialize<List<NetworkPlugin>>(jsonResponse, jsonOptions) ?? new List<NetworkPlugin>();

            Log($"从 {apiServer.Value.BaseUrl} 获取了 {AvailablePlugins.Count} 个插件");
        }
        catch
        {
            Log($"获取可用插件失败。请检查您的网络连接。", NotificationLevel.Danger);
        }
    }

    public bool PluginHasUpdateAvailable(string pluginId)
    {
        var plugin = AvailablePlugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin == null)
        {
            Log($"在可用插件中未找到 ID 为 {pluginId} 的插件。", NotificationLevel.Warning);
            return false;
        }

        InstalledPlugin? installedPlugin = InstalledPluginManifest.Current.InstalledPlugins.FirstOrDefault(p => p.Id == pluginId);
        if (!installedPlugin.HasValue || string.IsNullOrEmpty(installedPlugin.Value.Version))
        {
            return false;
        }

        var appVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0.0.0";
        OperatingSystem currentOS = Environment.OSVersion.Platform != PlatformID.Unix ? OperatingSystem.Windows : OperatingSystem.Linux;
        var latestVersion = plugin.GetLatestCompatibleVersion(appVersion, currentOS);

        if (latestVersion == null || string.IsNullOrEmpty(latestVersion.Version))
        {
            Log($"未找到 ID 为 {pluginId} 的插件的有效版本。", NotificationLevel.Warning);
            return false;
        }

        return new Version(latestVersion.Version) > new Version(installedPlugin.Value.Version);
    }

    public bool InstallPlugin(string pluginId)
    {
        var plugin = AvailablePlugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin == null)
        {
            Log($"未找到 ID 为 {pluginId} 的插件。", NotificationLevel.Warning);
            return false;   
        }

        var appVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString(3) ?? "0.0.0";
        OperatingSystem currentOS = Environment.OSVersion.Platform != PlatformID.Unix ? OperatingSystem.Windows : OperatingSystem.Linux;
        var latestVersion = plugin.GetLatestCompatibleVersion(appVersion, currentOS);
        if (latestVersion == null)
        {
            Log($"未找到 ID 为 {pluginId} 的插件的有效版本。", NotificationLevel.Warning);
            return false;
        }

        // Downloading is done from whatever region the user is in
        Region currentRegion = NetworkingSettings.Current.CurrentApiServer?.Name == "China" ? Region.China : Region.Global;
        string downloadUrl = latestVersion.DownloadUrl.FirstOrDefault(d => d.Key == Region.Global).Value;
        if (currentRegion == Region.China)
            downloadUrl = downloadUrl.Replace("ets2la.com", "ets2la.cn");

        if (string.IsNullOrEmpty(downloadUrl))
        {
            Log($"在 {currentRegion} 区域未找到 ID 为 {pluginId} 的插件的下载链接。", NotificationLevel.Warning);
            return false;
        }

        if (latestVersion.Dependencies.Count > 0)
        {
            bool allDependenciesInstalled = true;
            foreach (var dependencyId in latestVersion.Dependencies)
            {
                if (!InstalledPluginManifest.Current.InstalledPlugins.Any(p => p.Id == dependencyId))
                {
                    if (!InstallPlugin(dependencyId))
                    {
                        Log($"安装插件 {pluginId} 的依赖 {dependencyId} 失败。", NotificationLevel.Warning);
                        allDependenciesInstalled = false;
                    }
                }
            }
            if (!allDependenciesInstalled)
            {
                Log($"插件 {pluginId} 的依赖未全部安装。", NotificationLevel.Warning);
                return false;
            }
        }

        string tempFilePath = Path.GetTempFileName();
        using (var httpClient = new HttpClient())
        {
            var downloadTask = httpClient.GetAsync(downloadUrl);
            downloadTask.Wait();
            var downloadResponse = downloadTask.Result;
            if (!downloadResponse.IsSuccessStatusCode)
            {
                Log($"从 {downloadUrl} 下载 ID 为 {pluginId} 的插件失败。状态码：{downloadResponse.StatusCode}", NotificationLevel.Warning);
                return false;
            }
            using (var fs = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var copyTask = downloadResponse.Content.CopyToAsync(fs);
                copyTask.Wait();
            }
        }

        // And the output path is determined by the PluginBackend's PluginRootPath.
        // On windows that's set to none so it's in /Plugins or /Libraries.
        string location = PluginBackend.Current.PluginHandler?.PluginRootPath ?? string.Empty;

        string type = plugin.Tags.Contains(NetworkPluginTags.Plugin) ? "Plugin" : "Library";
        string folder = type == "Plugin" ? "Plugins" : "Libraries";
        string outputPath = Path.Combine(location, folder, plugin.Id);
        Directory.CreateDirectory(outputPath);

        System.IO.Compression.ZipFile.ExtractToDirectory(tempFilePath, outputPath, true);
        File.Delete(tempFilePath);

        // Finally we just have to register this plugin in the InstalledPluginManifest.
        InstalledPluginManifest.Current.InstalledPlugins.Add(new InstalledPlugin
        {
            Id = plugin.Id,
            Version = latestVersion.Version,
            Dependencies = latestVersion.Dependencies,
            DllPath = Path.Combine(outputPath, latestVersion.DllPath),
            Type = type == "Plugin" ? PluginType.Plugin : PluginType.Library
        });
        InstalledPluginManifest.Current.Save();

        Events.Current.Publish<string>("ETS2LA.Plugins.Installed", pluginId);
        Events.Current.Publish<EventArgs>($"ETS2LA.Plugins.Installed.{pluginId}", EventArgs.Empty);
        Log($"成功安装插件 {plugin.Name} ({plugin.Id}, {latestVersion.Version})", NotificationLevel.Success);
        return true;
    }

    public bool UpdatePlugin(string pluginId)
    {
        if (!PluginHasUpdateAvailable(pluginId))
        {
            Log($"ID 为 {pluginId} 的插件没有可用更新。", NotificationLevel.Information);
            return false;
        }

        // Uninstall the current version first.
        if (!UninstallPlugin(pluginId, overrideDependencyCheck: true))
        {
            Log($"卸载 ID 为 {pluginId} 的插件的当前版本失败。", NotificationLevel.Warning);
            return false;
        }

        // Then install the latest version.
        if (!InstallPlugin(pluginId))
        {
            Log($"安装 ID 为 {pluginId} 的插件的最新版本失败。", NotificationLevel.Warning);
            return false;
        }

        Events.Current.Publish<string>("ETS2LA.Plugins.Updated", pluginId);
        Events.Current.Publish<EventArgs>($"ETS2LA.Plugins.Updated.{pluginId}", EventArgs.Empty);
        Log($"成功更新 ID 为 {pluginId} 的插件。", NotificationLevel.Success);
        return true;
    }

    public bool UninstallPlugin(string pluginId, bool overrideDependencyCheck = false)
    {
        InstalledPlugin? installedPlugin = InstalledPluginManifest.Current.InstalledPlugins.FirstOrDefault(p => p.Id == pluginId);
        if (installedPlugin == null)
        {
            Log($"未找到已安装的 ID 为 {pluginId} 的插件。", NotificationLevel.Warning);
            return false;
        }

        if (!overrideDependencyCheck)
        {
            // Scan for other plugins that depend on this one.
            var dependentPlugins = InstalledPluginManifest.Current.InstalledPlugins
                .Where(p => p.Dependencies.Contains(installedPlugin.Value.Id));
            if (dependentPlugins.Any())
            {
                string dependentPluginIds = string.Join(", ", dependentPlugins.Select(p => p.Id));
                Log($"无法卸载 ID 为 {pluginId} 的插件，因为以下已安装的插件依赖于它：{dependentPluginIds}", NotificationLevel.Warning);
                return false;
            }
        }
        
        // Remove the plugin's files from the filesystem.
        string pluginPath = Path.Combine(
            PluginBackend.Current.PluginHandler?.PluginRootPath ?? string.Empty, 
            installedPlugin.Value.Type == PluginType.Plugin ? "Plugins" 
                                                            : "Libraries", 
            installedPlugin.Value.Id
        );

        if (Directory.Exists(pluginPath)) Directory.Delete(pluginPath, true);
        else
        {
            Log($"插件目录 {pluginPath} 不存在。", NotificationLevel.Warning);
            return false;
        }

        // And then we remove it from the InstalledPluginManifest.
        InstalledPluginManifest.Current.InstalledPlugins.Remove(installedPlugin.Value);
        InstalledPluginManifest.Current.Save();

        Events.Current.Publish<string>("ETS2LA.Plugins.Uninstalled", pluginId);
        Events.Current.Publish<EventArgs>($"ETS2LA.Plugins.Uninstalled.{pluginId}", EventArgs.Empty);
        Log($"成功卸载 ID 为 {pluginId} 的插件", NotificationLevel.Success);
        return true;
    }
}