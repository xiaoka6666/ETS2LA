using Hexa.NET.ImGui;
using ETS2LA.Overlay;
using ETS2LA.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Numerics;
using ETS2LA.Backend.Events;

namespace ETS2LA.Tutorials.DefaultTutorials;

public class OnboardingPart1
{
    bool hasMoved = false;

    public Tutorial Create()
    {
        return new Tutorial("入门引导第一部分", "引导用户直到目录插件安装完成。", "ETS2LA", new List<TutorialSection>
        {
            new TutorialSection
            {
                Title = "叠加层介绍",
                Actions = new List<TutorialAction>
                {
                    new ShowImguiWindowAction
                    {
                        ImGuiCallback = WelcomePage,
                        ScreenPositionCallback = ETS2LAWindowLocation,
                        SizeCallback = ETS2LAWindowSize,
                        ImGuiWindowFlags = ImGuiWindowFlags.NoDecoration
                    },
                    new WaitForInputAction
                    {
                        ControlId = OverlayHandler.Current.Interact.Id
                    },
                    new ShowImguiWindowAction
                    {
                        ImGuiCallback = OverlayInteractionPage,
                        ScreenPositionCallback = ETS2LAWindowLocation,
                        SizeCallback = ETS2LAWindowSize,
                        ImGuiWindowFlags = ImGuiWindowFlags.NoDecoration
                    },
                    new WaitForEventAction
                    {
                        EventId = "Onboarding.MovedWindow"
                    },
                    new ShowImguiWindowAction
                    {
                        ImGuiCallback = OverlayInteractionPage,
                        ScreenPositionCallback = ETS2LAWindowLocation,
                        SizeCallback = ETS2LAWindowSize,
                        ImGuiWindowFlags = ImGuiWindowFlags.NoDecoration
                    },
                    new WaitForInputAction
                    {
                        ControlId = OverlayHandler.Current.Interact.Id
                    },
                }
            },
            new TutorialSection
            {
                Title = "用户界面介绍",
                Actions = new List<TutorialAction>
                {
                    new ShowMessageAction
                    {
                        Message = "侧边栏应有尽有。\n让我们先去目录页面开始吧。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 15, position.Item2 + 230);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.UI.SwitchedPage.Catalogue"
                    },
                    new ShowMessageAction
                    {
                        Message = "您需要安装「车道辅助」和「自适应巡航控制」插件。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 230, position.Item2 + 1);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.Plugins.Installed.tumppi066.adaptivecruisecontrol"
                    },
                    new ShowMessageAction
                    {
                        Message = "您可能已经注意到我们自动安装了依赖项。\n每次安装/卸载插件或库时，您都需要\n重启 ETS2LA。在某些系统上，您可能需要手动重启 ETS2LA。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 140, position.Item2 + 224);
                        }
                    },
                }
            }
        });
    }

    private void AlignForWidth(float width, float alignment = 0.5f)
    {
        float avail = ImGui.GetContentRegionAvail().X;
        float off = (avail - width) * alignment;
        if (off > 0.0f)
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + off);
    }

    private void WelcomePage()
    {
        // Pad top
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 100);

        ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 20);
        AlignForWidth(ImGui.CalcTextSize("欢迎使用 ETS2LA！").X);
        ImGui.Text("欢迎使用 ETS2LA！");
        ImGui.Spacing();
        ImGui.PopFont();

        AlignForWidth(ImGui.CalcTextSize("让我们先熟悉一下用户界面。").X);
        ImGui.Text("让我们先熟悉一下用户界面。");
        AlignForWidth(ImGui.CalcTextSize("您现在看到的这个窗口就是叠加层。").X);
        ImGui.Text("您现在看到的这个窗口就是叠加层。");
        ImGui.Spacing();
        ImGui.Spacing();

        AlignForWidth(ImGui.CalcTextSize("按住叠加层交互键即可继续。").X);
        ImGui.Text("按住叠加层交互键即可继续。");

        # if LINUX
        AlignForWidth(ImGui.CalcTextSize("注意：您正在使用 Linux，请确保已允许您需要的按键使用 X11 全局快捷键。").X);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), "注意：您正在使用 Linux，请确保已允许您需要的按键使用 X11 全局快捷键。");
        ImGui.Spacing();
        ImGui.Spacing();
        # endif

        var controls = ControlsBackend.Current.GetRegisteredControls();        
        var interactKey = controls.FirstOrDefault(c => c.Definition.Id == OverlayHandler.Current.Interact.Id);
        var text = interactKey != null ? interactKey.ControlId.ToString() : "UNBOUND";

        AlignForWidth(ImGui.CalcTextSize(text).X);
        ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 18f);
        ImGui.TextColored(new Vector4(1f, 0.5f, 0.5f, 1f), text);
        ImGui.PopFont();
    }

    private void OverlayInteractionPage()
    {
        if (!hasMoved)
        {
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                Events.Current.Publish("Onboarding.MovedWindow", new EventArgs());
                hasMoved = true;
            }
        }

        // Pad top
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 100);

        if (!hasMoved)
        {
            ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 20);
            AlignForWidth(ImGui.CalcTextSize("很好！").X);
            ImGui.Text("很好！");
            ImGui.Spacing();
            ImGui.PopFont();

            AlignForWidth(ImGui.CalcTextSize("这个叠加层用于 ETS2LA 的许多功能。").X);
            ImGui.Text("这个叠加层用于 ETS2LA 的许多功能。");
            AlignForWidth(ImGui.CalcTextSize("如果您不喜欢这个按键绑定，可以稍后在设置中更改。").X);
            ImGui.Text("如果您不喜欢这个按键绑定，可以稍后在设置中更改。");
            ImGui.Spacing();
            ImGui.Spacing();

            AlignForWidth(ImGui.CalcTextSize("现在我们已经进入叠加层模式，您可以与窗口交互了。").X);
            ImGui.Text("现在我们已经进入叠加层模式，您可以与窗口交互了。");

            AlignForWidth(ImGui.CalcTextSize("尝试拖拽移动这个窗口！").X);
            ImGui.TextColored(new Vector4(0.5f, 1f, 0.5f, 1f), "尝试拖拽移动这个窗口！");
            ImGui.Spacing();
            ImGui.Spacing();
        }

        if (hasMoved)
        {
            ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 20);
            AlignForWidth(ImGui.CalcTextSize("太棒了！").X);
            ImGui.Text("太棒了！");
            ImGui.Spacing();
            ImGui.PopFont();
            
            AlignForWidth(ImGui.CalcTextSize("记住，如果您需要与叠加层窗口交互，请先进入交互模式！").X);
            ImGui.Text("记住，如果您需要与叠加层窗口交互，请先进入交互模式！");
            ImGui.Spacing();

            AlignForWidth(ImGui.CalcTextSize("退出叠加层交互模式以继续。").X);
            ImGui.TextColored(new Vector4(0.5f, 1f, 0.5f, 1f), "退出叠加层交互模式以继续。");
        }
    }

    private (int, int) ETS2LAWindowLocation()
    {
        if (Application.Current == null || Application.Current.ApplicationLifetime == null)
            return (0, 0);

        var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
        if (window == null)
            return (0, 0);
        
        return (window.Position.X, window.Position.Y);
    }

    private (int, int) ETS2LAWindowSize()
    {
        if (Application.Current == null || Application.Current.ApplicationLifetime == null)
            return (0, 0);

        var window = ((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow;
        if (window == null || window.FrameSize == null)
            return (0, 0);
        
        var size = ((int)window.FrameSize.Value.Width, (int)window.FrameSize.Value.Height);
        return size;
    }
}