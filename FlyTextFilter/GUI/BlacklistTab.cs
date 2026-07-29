using Dalamud.Bindings.ImGui;

namespace FlyTextFilter.GUI;

public class BlacklistTab
{
    private string addToBlacklist = string.Empty;

    public void Draw()
    {
        ImGui.Text("完整符合任一項目時即會過濾浮動文字（例如技能名稱、狀態效果名稱等）。");

        ImGui.InputText("##addToBlacklist", ref this.addToBlacklist, 100);
        if (ImGui.Button("加入黑名單"))
        {
            Service.Configuration.Blacklist.Add(this.addToBlacklist);
            Service.Configuration.Save();
        }

        if (Service.Configuration.Blacklist.Count != 0)
        {
            ImGui.Separator();
        }

        foreach (var blItem in Service.Configuration.Blacklist)
        {
            if (ImGui.Button($"移除##{blItem}"))
            {
                Service.Configuration.Blacklist.Remove(blItem);
                Service.Configuration.Save();
            }

            ImGui.SameLine();
            ImGui.Text(blItem);
        }
    }
}
