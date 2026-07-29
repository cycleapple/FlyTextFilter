using Dalamud.Bindings.ImGui;

namespace FlyTextFilter.GUI.TypesTab;

public class DisabledTab
{
    public static void Draw()
    {
        ImGui.Text("測試失敗；為保護設定，已停用此分頁。" +
                   "\n部分設定可能無法如預期運作；手動編輯設定檔可能會在插件更新後再次失效，" +
                   "\n甚至造成設定檔損毀。");

        ImGui.AlignTextToFramePadding();
        ImGui.Text("請前往 GitHub 回報此問題：");
        ImGui.SameLine();
        if (ImGui.Button("開啟 GitHub 儲存庫"))
        {
            Util.OpenLink("https://github.com/Aireil/FlyTextFilter");
        }

        ImGui.AlignTextToFramePadding();
        ImGui.Text("若仍要編輯設定：");
        ImGui.SameLine();
        if (ImGui.Button("強制覆寫"))
        {
            ImGui.OpenPopup("##OverwriteTestsResults");
        }

        if (ImGui.BeginPopup("##OverwriteTestsResults", ImGuiWindowFlags.NoMove))
        {
            ImGui.Text("若設定檔損毀，將無法提供支援。\n確定要繼續嗎？");
            ImGui.Separator();
            if (ImGui.Button("是##OverwriteTestsResults"))
            {
                Service.ConfigWindow.HasPassedTests = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
            if (ImGui.Button("否##OverwriteTestsResults"))
            {
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
}
