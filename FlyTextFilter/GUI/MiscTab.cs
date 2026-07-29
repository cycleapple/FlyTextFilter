using Dalamud.Bindings.ImGui;

namespace FlyTextFilter.GUI;

public class MiscTab
{
    public static void Draw()
    {
        var hasChanged = false;
        hasChanged |= ImGui.Checkbox("強制將你設為持續傷害的來源", ref Service.Configuration.ShouldAdjustDotSource);

        Util.DrawHelp("持續傷害會合併顯示，且來源固定為受到影響的目標，" +
                      "\n因此無法區分自己與其他玩家施加的持續傷害。" +
                      "\n此設定會強制將所有持續傷害的來源視為自己，" +
                      "\n讓你篩除「其他」來源時，仍可保留可能由自己造成的持續傷害。");

        hasChanged |= ImGui.Checkbox("強制將你設為寵物技能的來源", ref Service.Configuration.ShouldAdjustPetSource);

        Util.DrawHelp("寵物技能預設以「其他」作為來源；此設定會改為將自己視為來源。");

        hasChanged |= ImGui.Checkbox("強制將你設為陸行鳥技能的來源", ref Service.Configuration.ShouldAdjustChocoboSource);

        Util.DrawHelp("陸行鳥技能預設以「其他」作為來源；此設定會改為將自己視為來源。");

        ImGui.Separator();

        hasChanged |= ImGui.Checkbox("協助尋找未知類型", ref Service.Configuration.IsExplorer);

        Util.DrawHelp("部分類型的來源仍未知。啟用後，遇到未知類型時會在聊天視窗中輸出資訊。" +
                      "\n每次遊戲工作階段中，每種類型只會輸出一次。");

        if (hasChanged)
        {
            Service.Configuration.Save();
        }
    }
}
