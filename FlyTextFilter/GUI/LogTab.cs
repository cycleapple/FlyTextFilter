using System.Linq;
using System.Numerics;

using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using FlyTextFilter.Model;

namespace FlyTextFilter.GUI;

public class LogTab
{
    public static void Draw()
    {
        if (!Service.ConfigWindow.HasPassedTests)
        {
            ImGui.Text("測試失敗；為保護設定，已停用此分頁。" +
                       "\n詳情請查看「類型」分頁。");
            return;
        }

        ImGui.Checkbox("記錄浮動文字", ref Service.FlyTextHandler.ShouldLog);

        Util.DrawHelp("在下表記錄所有浮動文字，包括已過濾的項目。" +
                      "\n若有浮動文字未被記錄，請附上重現步驟回報。" +
                      "\n重新啟動時會關閉記錄，以避免不必要的處理。");

        ImGui.SameLine();
        ImGui.SetNextItemWidth(34.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.InputInt("最大紀錄筆數", ref Service.Configuration.NbOfLogs, 0))
        {
            if (Service.Configuration.NbOfLogs < 1)
            {
                Service.Configuration.NbOfLogs = 1;
            }
            else if (Service.Configuration.NbOfLogs > 500)
            {
                Service.Configuration.NbOfLogs = 500;
            }

            Service.Configuration.Save();
        }

        ImGui.SameLine();
        if (ImGui.Button("清除"))
        {
            Service.FlyTextHandler.Logs.Clear();
        }

        if (ImGui.BeginTable(
                "##LogTable",
                7,
                ImGuiTableFlags.BordersH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp,
                new Vector2(-1.0f, 600.0f * ImGuiHelpers.GlobalScale)))
        {
            ImGui.TableSetupScrollFreeze(0, 1);

            DrawHeader();

            for (var i = Service.FlyTextHandler.Logs.Count - 1; i >= 0; i--)
            {
                var flyTextLog = Service.FlyTextHandler.Logs.ElementAt(i);
                if (flyTextLog.IsPartial)
                {
                    continue;
                }

                ImGui.PushID($"{i}RowLog");

                var hasChanged = false;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.AlignTextToFramePadding();
                ImGui.Text($"{FlyTextKindData.GetAlias(flyTextLog.FlyTextCreation.FlyTextKind)}  ");

                ImGui.TableNextColumn();
                if (Util.CenterButtonIcon(FontAwesomeIcon.Eye))
                {
                    Service.FlyTextHandler.CreateFlyText(flyTextLog);
                }

                Util.SetHoverTooltip("在你身上建立此浮動文字");

                ImGui.TableNextColumn();
                if (Util.CenterButtonIcon(FontAwesomeIcon.Edit))
                {
                    ImGui.OpenPopup("##PopupEditKind");
                }

                Util.SetHoverTooltip("編輯");

                ImGui.TableNextColumn();
                Util.CenterText(flyTextLog.WasFiltered ? "是" : "否");

                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                Util.CenterText(flyTextLog.SourceCategory + (flyTextLog.HasSourceBeenAdjusted ? " (?)" : string.Empty));
                if (flyTextLog.HasSourceBeenAdjusted)
                {
                    Util.SetHoverTooltip("此浮動文字的原始來源已調整；詳情請查看「其他」分頁。");
                }

                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();
                Util.CenterText(flyTextLog.TargetCategory.ToString());

                ImGui.TableNextColumn();
                ImGui.AlignTextToFramePadding();

                var value = flyTextLog.FlyTextCreation.Val1 == 0 ? flyTextLog.FlyTextCreation.Val2 : flyTextLog.FlyTextCreation.Val1;

                Util.CenterText(value + " ");

                Service.Configuration.FlyTextSettings.TryGetValue(flyTextLog.FlyTextCreation.FlyTextKind, out var flyTextSetting);
                flyTextSetting ??= new FlyTextSetting();
                hasChanged |= EditPopup.Draw(flyTextLog.FlyTextCreation.FlyTextKind, flyTextSetting);

                if (hasChanged)
                {
                    Service.Configuration.UpdateFlyTextSettings(flyTextLog.FlyTextCreation.FlyTextKind, flyTextSetting);
                    Service.Configuration.Save();
                }

                ImGui.PopID();
            }

            ImGui.EndTable();
        }
    }

    public static void DrawHeader()
    {
        var headerColor = ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int)ImGuiCol.TableHeaderBg]);

        ImGui.TableNextColumn();
        Util.CenterText(" 類型 ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 顯示（？）");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);
        Util.SetHoverTooltip("若使用 Damage Info，建立的浮動文字顏色可能與原本不同。");

        ImGui.TableNextColumn();
        Util.CenterText(" 編輯類型 ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 已過濾？ ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 來源 ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 目標 ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 數值（？） ");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);
        Util.SetHoverTooltip("通常是畫面顯示的數值；若沒有顯示數值，則代表 ID，可忽略。");
    }
}
