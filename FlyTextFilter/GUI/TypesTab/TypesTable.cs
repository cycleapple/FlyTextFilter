using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Bindings.ImGui;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using FlyTextFilter.Model;

namespace FlyTextFilter.GUI.TypesTab;

public class TypesTable
{
    private readonly IOrderedEnumerable<KeyValuePair<FlyTextKind, string>> flyTextKindOrder = Enum.GetValues<FlyTextKind>().Distinct().ToDictionary(kind => kind, FlyTextKindData.GetAlias).OrderBy(pair => pair.Value);

    public void Draw()
    {
        if (ImGui.BeginTable(
                "##TableKinds",
                7,
                ImGuiTableFlags.BordersH | ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp,
                new Vector2(-1.0f, 600.0f * ImGuiHelpers.GlobalScale)))
        {
            ImGui.TableSetupScrollFreeze(0, 1);

            DrawHeader();

            foreach (var (flyTextKind, flyTextKindAlias) in this.flyTextKindOrder)
            {
                ImGui.PushID($"{flyTextKind}RowTypesType");

                var hasChanged = false;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.AlignTextToFramePadding();
                ImGui.Text($"{flyTextKindAlias}  ");

                ImGui.TableNextColumn();
                if (Util.CenterButtonIcon(FontAwesomeIcon.Eye))
                {
                    Service.FlyTextHandler.CreateFlyText(flyTextKind, 0, 3);
                    Service.FlyTextHandler.CreateFlyText(flyTextKind, 3, 0);
                    Service.FlyTextHandler.CreateFlyText(flyTextKind, 3, 2);
                    Service.FlyTextHandler.CreateFlyText(flyTextKind, 2, 2);
                }

                Util.SetHoverTooltip("在自己身上顯示此類型的浮動文字");

                Service.Configuration.FlyTextSettings.TryGetValue(flyTextKind, out var flyTextSetting);
                flyTextSetting ??= new FlyTextSetting();

                ImGui.TableNextColumn();
                Util.CenterCursor(22.0f * ImGuiHelpers.GlobalScale);
                hasChanged |= Util.CheckboxFlagAll("##AllCheckbox" + flyTextKind, ref flyTextSetting);

                ImGui.TableNextColumn();
                hasChanged |= DrawTargets("##DrawTargetsSourceYou", ref flyTextSetting.SourceYou);

                ImGui.TableNextColumn();
                hasChanged |= DrawTargets("##DrawTargetsSourceParty", ref flyTextSetting.SourceParty);

                ImGui.TableNextColumn();
                hasChanged |= DrawTargets("##DrawTargetsSourceOthers", ref flyTextSetting.SourceOthers);

                ImGui.TableNextColumn();
                if (Util.DrawButtonIcon(FontAwesomeIcon.Edit))
                {
                    ImGui.OpenPopup("##PopupEditKind");
                }

                Util.SetHoverTooltip("編輯");

                ImGui.SameLine();
                if (Util.DrawButtonIcon(FontAwesomeIcon.SignOutAlt))
                {
                    var flyTextSettingDic = new ConcurrentDictionary<FlyTextKind, FlyTextSetting>();
                    flyTextSettingDic.TryAdd(flyTextKind, flyTextSetting);
                    ImGui.SetClipboardText(ImportExport.ExportFlyTextSettings(flyTextSettingDic));
                    Service.ConfigWindow.UpdateMessage("已複製到剪貼簿。", ImGuiColors.DalamudWhite);
                }

                Util.SetHoverTooltip("匯出單一類型");

                ImGui.SameLine();
                if (Util.DrawButtonIcon(FontAwesomeIcon.SignInAlt))
                {
                    try
                    {
                        var importedFlyTextSettings = ImportExport.ImportFlyTextSettings(ImGui.GetClipboardText());
                        if (importedFlyTextSettings.Count > 1)
                        {
                            Service.ConfigWindow.UpdateMessage("匯入內容包含多組設定。", ImGuiColors.DalamudRed);
                        }
                        else
                        {
                            if (importedFlyTextSettings.IsEmpty)
                            {
                                flyTextSetting = new FlyTextSetting();
                            }
                            else
                            {
                                foreach (var (_, importedFlyTextSetting) in importedFlyTextSettings)
                                {
                                    flyTextSetting = importedFlyTextSetting;
                                }
                            }

                            hasChanged = true;
                            Service.ConfigWindow.UpdateMessage("匯入完成。", ImGuiColors.DalamudWhite);
                        }
                    }
                    catch (Exception ex)
                    {
                        Service.PluginLog.Error(ex, "Failed to import setting from clipboard.");
                        Service.ConfigWindow.UpdateMessage("無法從剪貼簿匯入設定。", ImGuiColors.DalamudRed);
                    }
                }

                Util.SetHoverTooltip("匯入單一類型");

                ImGui.SameLine();
                ImGui.Dummy(new Vector2(-5.0f));

                hasChanged |= EditPopup.Draw(flyTextKind, flyTextSetting);

                if (hasChanged)
                {
                    Service.Configuration.UpdateFlyTextSettings(flyTextKind, flyTextSetting);
                    Service.Configuration.Save();
                }

                ImGui.PopID();
            }

            ImGui.EndTable();
        }
    }

    private static bool DrawTargets(string id, ref FlyTextTargets targets)
    {
        var hasChanged = false;
        hasChanged |= Util.CheckboxFlag(id + "TargetYou", ref targets, FlyTextTargets.You);

        ImGui.SameLine();
        hasChanged |= Util.CheckboxFlag(id + "TargetParty", ref targets, FlyTextTargets.Party);

        ImGui.SameLine();
        hasChanged |= Util.CheckboxFlag(id + "TargetOthers", ref targets, FlyTextTargets.Others);

        return hasChanged;
    }

    private static void DrawHeader()
    {
        var headerColor = ImGui.ColorConvertFloat4ToU32(ImGui.GetStyle().Colors[(int)ImGuiCol.TableHeaderBg]);

        ImGui.TableNextColumn();
        Util.CenterText("類型", 2);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText(" 顯示（？）", 2);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);
        Util.SetHoverTooltip("若可行，會產生兩組浮動文字與兩組彈出文字，" +
                             "\n以顯示此類型的所有不同樣式。" +
                             "\n部分類型僅支援浮動文字，因此只會顯示兩組。");

        ImGui.TableNextColumn();
        Util.CenterText("   全部   ", 2);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        DrawTargetsHeader("自己");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        DrawTargetsHeader("小隊");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        DrawTargetsHeader("其他");
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);

        ImGui.TableNextColumn();
        Util.CenterText("其他", 2);
        ImGui.TableSetBgColor(ImGuiTableBgTarget.CellBg, headerColor);
    }

    private static void DrawTargetsHeader(string header)
    {
        Util.CenterText(header);
        if (ImGui.BeginTable("##TargetsHeader", 3))
        {
            ImGui.TableNextColumn();
            Util.CenterText("己");

            ImGui.TableNextColumn();
            Util.CenterText("隊");

            ImGui.TableNextColumn();
            Util.CenterText("他");

            ImGui.EndTable();
        }
    }
}
