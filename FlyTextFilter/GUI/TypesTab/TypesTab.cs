using System;
using System.Collections.Concurrent;

using Dalamud.Bindings.ImGui;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using FlyTextFilter.Model;

namespace FlyTextFilter.GUI.TypesTab;

public class TypesTab
{
    private readonly TypesTable typesTable = new();
    private ConcurrentDictionary<FlyTextKind, FlyTextSetting>? importedFlyTextSettings;

    public void Draw()
    {
        if (!Service.ConfigWindow.HasPassedTests)
        {
            DisabledTab.Draw();
            return;
        }

        if (Util.DrawButtonIcon(FontAwesomeIcon.Trash))
        {
            ImGui.OpenPopup("##ConfirmationDeleteAll");
        }

        Util.SetHoverTooltip("刪除所有類型設定");

        DrawConfirmationDeleteAllPopup();

        ImGui.SameLine();
        if (Util.DrawButtonIcon(FontAwesomeIcon.SignOutAlt))
        {
            ImGui.SetClipboardText(ImportExport.ExportFlyTextSettings(Service.Configuration.FlyTextSettings));
            Service.ConfigWindow.UpdateMessage("已複製到剪貼簿。", ImGuiColors.DalamudWhite);
        }

        Util.SetHoverTooltip("匯出所有類型");

        ImGui.SameLine();
        if (Util.DrawButtonIcon(FontAwesomeIcon.SignInAlt))
        {
            try
            {
                this.importedFlyTextSettings = ImportExport.ImportFlyTextSettings(ImGui.GetClipboardText());
                ImGui.OpenPopup("##ConfirmationImportAll");
            }
            catch (Exception ex)
            {
                Service.PluginLog.Error(ex, "Failed to import settings from clipboard.");
                Service.ConfigWindow.UpdateMessage("無法從剪貼簿匯入設定。", ImGuiColors.DalamudRed);
            }
        }

        Util.SetHoverTooltip("匯入所有類型");

        this.DrawConfirmationImportAllPopup();

        ImGui.SameLine();
        if (Util.DrawButtonIcon(FontAwesomeIcon.InfoCircle))
        {
            ImGui.OpenPopup("##TypeHelp");
        }

        Util.SetHoverTooltip("說明");

        DrawTypeHelpPopup();

        ImGui.AlignTextToFramePadding();
        Service.ConfigWindow.DrawMessage();

        this.typesTable.Draw();
    }

    private static void DrawTypeHelpPopup()
    {
        if (ImGui.BeginPopup("##TypeHelp"))
        {
            ImGui.Text("過濾依來源與目標判定，分為三組：");
            ImGui.Text("你（Y）－僅來自你或傳至你。");
            ImGui.Text("小隊（P）－僅來自小隊成員或傳至小隊成員，不包含你。");
            ImGui.Text("其他人（O）－來自或傳至你與小隊成員以外的任何人。");
            ImGui.Text("主表格一開始可能較難理解。上方（你－小隊－其他人）代表來源，" +
                       "\n其下各字母代表目標。");
            ImGui.Text("例如：");
            ImGui.Text("   - 「你」下方的 Y 欄代表從你傳至你的浮動文字。");
            ImGui.Text("   - 「你」下方的 P 欄代表從你傳至小隊成員的浮動文字。");
            ImGui.Text("   - 「小隊」下方的 Y 欄代表從小隊成員傳至你的浮動文字。");

            ImGui.AlignTextToFramePadding();
            ImGui.Text("若仍不清楚，可使用表格中的此按鈕：");

            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            ImGui.Text(FontAwesomeIcon.Edit.ToIconString());
            ImGui.PopFont();

            ImGui.EndPopup();
        }
    }

    private static void DrawConfirmationDeleteAllPopup()
    {
        if (ImGui.BeginPopup("##ConfirmationDeleteAll", ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("這會重設下表中的所有類型設定。\n確定要繼續嗎？");
            ImGui.Separator();
            if (ImGui.Button("是##ConfirmationDeleteAll"))
            {
                Service.Configuration.FlyTextSettings = new ConcurrentDictionary<FlyTextKind, FlyTextSetting>();
                Service.Configuration.Save();
                Service.ConfigWindow.UpdateMessage("重設完成。", ImGuiColors.DalamudWhite);
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            if (ImGui.Button("否##ConfirmationDeleteAll"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }

    private void DrawConfirmationImportAllPopup()
    {
        if (ImGui.BeginPopup("##ConfirmationImportAll", ImGuiWindowFlags.NoMove))
        {
            ImGui.Text($"這會取代所有類型設定；匯入資料包含 {this.importedFlyTextSettings!.Count} 種類型。\n確定要繼續嗎？");
            ImGui.Separator();
            if (ImGui.Button("是##ConfirmationImportAll"))
            {
                Service.Configuration.FlyTextSettings = this.importedFlyTextSettings;
                Service.Configuration.Save();
                this.importedFlyTextSettings = null;
                Service.ConfigWindow.UpdateMessage("匯入完成。", ImGuiColors.DalamudWhite);
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            if (ImGui.Button("否##ConfirmationImportAll"))
            {
                this.importedFlyTextSettings = null;
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
}
