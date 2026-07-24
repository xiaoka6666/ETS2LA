using Hexa.NET.ImGui;
using ETS2LA.Controls;
using System.Numerics;

namespace ETS2LA.Overlay.Window;

class OverlayInfoWindow : InternalWindow
{
    public OverlayInfoWindow()
    {
        Definition = new WindowDefinition
        {
            Title = "叠加层信息",
            Flags = ImGuiWindowFlags.AlwaysAutoResize,
        };

        IsWindowOpen = false;

        Render = () =>
        {
            ImGui.Text("*震惊* 这里居然有个新窗口 O_O");
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "这是最终将在游戏上方渲染信息的叠加层。对于 C#，我们实际上让它比之前强大得多！");
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "插件开发者现在拥有完全访问权限... 我是说对 ImGui 的*完全*访问权限用于渲染，希望我们会看到一些有趣的成果！");
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "目前我们只实现了基础功能，遥测插件在渲染大量数据时会展示出不错的性能。");
            ImGui.Separator();
            ImGui.Text("您可以按住以下按键与叠加层交互：");
            ImGui.SameLine();
            var controls = ControlsBackend.Current.GetRegisteredControls();        
            var interactKey = controls.FirstOrDefault(c => c.Definition.Id == OverlayHandler.Current.Interact.Id);

            ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 18f);
            if (interactKey != null)
                ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), interactKey.ControlId.ToString());
            else 
                ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), "UNBOUND");
            ImGui.PopFont();
            
            ImGui.SameLine();
            ImGui.Text("（可以在设置中更改！）");
            ImGui.Text("叠加层基本上是一个完整的窗口系统，应该不会有崩溃... 但愿如此... 但如果发生了，请报告给我们！");
        };
    }
}