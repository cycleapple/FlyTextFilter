using System.Numerics;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Windowing;

namespace FlyTextFilter.GUI;

internal class ConfigWindow : Window
{
    public bool HasPassedTests;
    private readonly TypesTab.TypesTab typesTab = new();
    private readonly BlacklistTab blacklistTab = new();
    private string message = string.Empty;
    private Vector4 messageColor = ImGuiColors.DalamudWhite;

    public ConfigWindow()
        : base("FlyTextFilter")
    {
        this.RespectCloseHotkey = true;
        this.Flags = ImGuiWindowFlags.AlwaysAutoResize;

        FlyTextKindTests.RunTests(out this.HasPassedTests);
    }

    public override void OnOpen()
    {
        this.UpdateMessage(string.Empty, ImGuiColors.DalamudWhite);
        base.OnOpen();
    }

    public override void Draw()
    {
        if (Service.FlyTextHandler.HasLoadingFailed)
        {
            ImGui.Text("插件無法正常載入；若遊戲剛更新，這可能是預期狀況。\n你可以前往 GitHub 回報此問題：");
            if (ImGui.Button("開啟 GitHub 儲存庫"))
            {
                Util.OpenLink("https://github.com/Aireil/FlyTextFilter");
            }

            return;
        }

        if (ImGui.BeginTabBar("##ConfigTab"))
        {
            if (ImGui.BeginTabItem("類型"))
            {
                this.typesTab.Draw();

                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("紀錄"))
            {
                LogTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("文字黑名單"))
            {
                this.blacklistTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("調整"))
            {
                AdjustmentsTab.Draw();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("其他"))
            {
                MiscTab.Draw();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }
    }

    public void UpdateMessage(string msg, Vector4 color)
    {
        this.message = msg;
        this.messageColor = color;
    }

    public void DrawMessage()
    {
        if (this.message != string.Empty)
        {
            ImGui.SameLine();
            ImGui.TextColored(this.messageColor, this.message);
        }
    }
}
