using Dalamud.Bindings.ImGui;
using Dalamud.Game.Gui.FlyText;
using FlyTextFilter.Model;

namespace FlyTextFilter.GUI;

public class EditPopup
{
    public static bool Draw(FlyTextKind flyTextKind, FlyTextSetting flyTextSetting)
    {
        var hasChanged = false;
        if (ImGui.BeginPopup("##PopupEditKind"))
        {
            ImGui.Text($"正在編輯 {FlyTextKindData.GetAlias(flyTextKind)}");
            ImGui.Text("勾選＝過濾。");
            Util.DrawHelp("每種來源（你／小隊／其他人）皆有三種可能的目標（你／小隊／其他人）。" +
                          "\n若要過濾從某來源傳至某目標的浮動文字，請勾選對應項目。" +
                          "\n例如：勾選「你」至「小隊」，將過濾所有從你送往小隊成員的浮動文字。");

            hasChanged |= Util.CheckboxFlagAll("全部過濾##CheckboxEdit", ref flyTextSetting);

            if (ImGui.BeginTable("PopupEditTable", 2, ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.BordersInnerH | ImGuiTableFlags.SizingStretchProp))
            {
                ImGui.TableNextColumn();
                ImGui.Text("（來自）來源");
                ImGui.TableNextColumn();
                ImGui.Text("（傳至）目標");

                hasChanged |= DrawPopupSetting("你", "##DrawPopupSettingSourceYou", ref flyTextSetting.SourceYou);
                hasChanged |= DrawPopupSetting("小隊", "##DrawPopupSettingSourceParty", ref flyTextSetting.SourceParty);
                hasChanged |= DrawPopupSetting("其他人", "##DrawPopupSettingSourceOthers", ref flyTextSetting.SourceOthers);

                ImGui.EndTable();
            }

            ImGui.EndPopup();
        }

        return hasChanged;
    }

    private static bool DrawPopupSetting(string source, string id, ref FlyTextTargets flyTextTargets)
    {
        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.Text(source);
        ImGui.TableNextColumn();

        var hasChanged = false;
        hasChanged |= Util.CheckboxFlag("你" + id, ref flyTextTargets, FlyTextTargets.You);
        hasChanged |= Util.CheckboxFlag("小隊" + id, ref flyTextTargets, FlyTextTargets.Party);
        hasChanged |= Util.CheckboxFlag("其他人" + id, ref flyTextTargets, FlyTextTargets.Others);

        return hasChanged;
    }
}
