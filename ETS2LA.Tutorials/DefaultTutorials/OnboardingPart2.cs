using Hexa.NET.ImGui;
using ETS2LA.Overlay;
using ETS2LA.Controls;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using System.Numerics;
using ETS2LA.Backend.Events;

namespace ETS2LA.Tutorials.DefaultTutorials;

public class OnboardingPart2
{
    bool hasMoved = false;

    public Tutorial Create()
    {
        return new Tutorial("入门引导第二部分", "从目录插件安装完成后的引导。", "ETS2LA", new List<TutorialSection>
        {
            new TutorialSection
            {
                Title = "用户界面介绍",
                Actions = new List<TutorialAction>
                {
                    new ShowMessageAction
                    {
                        Message = "很好！\n接下来让我们去设置页面。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 220, position.Item2 + size.Item2 - 70);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.UI.SwitchedPage.Settings"
                    },
                    new ShowMessageAction
                    {
                        Message = "让我们检查控件设置。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 220, position.Item2 + 320);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.UI.SwitchedPage.Settings.Controls"
                    },
                    new ExecuteFunctionAction
                    {
                        Function = () =>
                        {
                            OverlayHandler.Current.OpenWindow("State Info");
                        }
                    },
                    new ShowMessageAction
                    {
                        Message = "您可以在这里看到 ETS2LA 的所有控件。我们还打开了「状态信息」窗口，方便您查看当前设置。\n尝试按下「SET」键来切换 ACC。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 442, position.Item2 + 3);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId="ETS2LA.State.AssistsPaused"
                    },
                    new ShowMessageAction
                    {
                        Message = "您可以在这里看到 ETS2LA 的所有控件。我们还打开了「状态信息」窗口，方便您查看当前设置。\n您也可以使用「ASSIST」键在转向模式间切换。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 442, position.Item2 + 3);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId="ETS2LA.State.SteeringLevel.None"
                    },
                    new ShowMessageAction
                    {
                        Message = "很好！\n让我们去插件管理页面。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 15, position.Item2 + 202);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.UI.SwitchedPage.Manager"
                    },
                    new ShowMessageAction
                    {
                        Message = "这里显示您已安装的所有插件。首先，请启用「车道辅助」和「自适应巡航控制」插件。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 230, position.Item2 + 1);
                        }
                    },
                    new WaitForEventAction
                    {
                        EventId = "ETS2LA.Backend.Enabled.tumppi066.adaptivecruisecontrol"
                    },
                    new ShowMessageWaitNextAction
                    {
                        Message = "引导到此结束。如果您还有疑问，请查看我们的 YouTube 频道和 Discord 了解更多详情。\n您可以在仪表盘页面找到所有链接。",
                        ScreenPositionCallback = () =>
                        {
                            var position = ETS2LAWindowLocation();
                            var size = ETS2LAWindowSize();
                            return (position.Item1 + 230, position.Item2 + 3);
                        }
                    },
                    new SendNotificationAction
                    {
                        Title = "教程已完成",
                        Message = "欢迎使用 ETS2LA！",
                        Level = Notifications.NotificationLevel.Success,
                        CloseAfter = 5f
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
        AlignForWidth(ImGui.CalcTextSize("Welcome to ETS2LA!").X);
        ImGui.Text("Welcome to ETS2LA!");
        ImGui.Spacing();
        ImGui.PopFont();

        AlignForWidth(ImGui.CalcTextSize("Let's start off by familiarizing you to our User Interface.").X);
        ImGui.Text("Let's start off by familiarizing you to our User Interface.");
        AlignForWidth(ImGui.CalcTextSize("The window you're seeing right now is an overlay.").X);
        ImGui.Text("The window you're seeing right now is an overlay.");
        ImGui.Spacing();
        ImGui.Spacing();

        AlignForWidth(ImGui.CalcTextSize("You can continue by holding down the overlay interaction key.").X);
        ImGui.Text("You can continue by holding down the overlay interaction key.");

        # if LINUX
        AlignForWidth(ImGui.CalcTextSize("Note: You're on Linux, make sure you are allowing X11 global hotkeys on keys you need.").X);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1f), "Note: You're on Linux, make sure you are allowing X11 global hotkeys on keys you need.");
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
            AlignForWidth(ImGui.CalcTextSize("Great!").X);
            ImGui.Text("Great!");
            ImGui.Spacing();
            ImGui.PopFont();

            AlignForWidth(ImGui.CalcTextSize("This overlay is used for many features in ETS2LA.").X);
            ImGui.Text("This overlay is used for many features in ETS2LA.");
            AlignForWidth(ImGui.CalcTextSize("If you don't like the keybind you can always change it in the settings later.").X);
            ImGui.Text("If you don't like the keybind you can always change it in the settings later.");
            ImGui.Spacing();
            ImGui.Spacing();

            AlignForWidth(ImGui.CalcTextSize("Now that we're in overlay mode, you can interact with windows.").X);
            ImGui.Text("Now that we're in overlay mode, you can interact with windows.");

            AlignForWidth(ImGui.CalcTextSize("Try to move this window around by dragging it!").X);
            ImGui.TextColored(new Vector4(0.5f, 1f, 0.5f, 1f), "Try to move this window around by dragging it!");
            ImGui.Spacing();
            ImGui.Spacing();
        }

        if (hasMoved)
        {
            ImGui.PushFont(OverlayHandler.Current.Fonts[FontStyle.Bold], 20);
            AlignForWidth(ImGui.CalcTextSize("Fantastic!").X);
            ImGui.Text("Fantastic!");
            ImGui.Spacing();
            ImGui.PopFont();
            
            AlignForWidth(ImGui.CalcTextSize("Remember, if you need to interact with overlay windows, enter interaction mode first!").X);
            ImGui.Text("Remember, if you need to interact with overlay windows, enter interaction mode first!");
            ImGui.Spacing();

            AlignForWidth(ImGui.CalcTextSize("Exit overlay interaction mode to continue.").X);
            ImGui.TextColored(new Vector4(0.5f, 1f, 0.5f, 1f), "Exit overlay interaction mode to continue.");
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