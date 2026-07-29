using Dalamud.Bindings.ImGui;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Interface.Colors;
using FlyTextFilter.Model.FlyTextAdjustments;

namespace FlyTextFilter.GUI;

public class AdjustmentsTab
{
    public static void Draw()
    {
        var hasDamageTypeChanged = false;

        ImGui.Text("隱藏下列項目的傷害類型圖示（物理、魔法或特殊）：");

        Util.DrawHelp("若想在隱藏圖示後仍查看傷害類型，請使用 Damage Info 插件。");

        ImGui.Indent();

        hasDamageTypeChanged |= ImGui.Checkbox("自動攻擊", ref Service.Configuration.FlyTextAdjustments.ShouldHideDamageTypeIconAutoAttacks);
        hasDamageTypeChanged |= ImGui.Checkbox("狀態效果", ref Service.Configuration.FlyTextAdjustments.ShouldHideDamageTypeIconStatusEffects);
        hasDamageTypeChanged |= ImGui.Checkbox("其他傷害", ref Service.Configuration.FlyTextAdjustments.ShouldHideDamageTypeIconOthers);

        if (hasDamageTypeChanged)
        {
            Service.Configuration.Save();
        }

        ImGui.Unindent();

        ImGui.Separator();

        ImGui.Text("位置：");

        ImGui.Indent();
        ImGui.Text("玩家身上的浮動文字分為兩組：" +
                   "\n- 治療＝所有受到的治療，包括持續恢復效果。" +
                   "\n- 狀態－傷害＝取得／失去的狀態效果及受到的傷害。" +
                   "\n此分組由遊戲本身決定，目前無法修改。");

        var adjustmentsConfig = Service.Configuration.FlyTextAdjustments;
        var posConfig = adjustmentsConfig.FlyTextPositions;
        var defaultPosConfig = FlyTextHandler.GetDefaultPositions();

        var hasPosChanged = false;
        var tmp = posConfig.HealingGroupX ?? defaultPosConfig.HealingGroupX!.Value;
        if (ImGui.DragFloat("治療－水平##HealingGroupPosXSlider", ref tmp))
        {
            posConfig.HealingGroupX = tmp;
            hasPosChanged = true;
        }

        tmp = posConfig.HealingGroupY ?? defaultPosConfig.HealingGroupY!.Value;
        if (ImGui.DragFloat("治療－垂直##HealingGroupPosYSlider", ref tmp))
        {
            posConfig.HealingGroupY = tmp;
            hasPosChanged = true;
        }

        tmp = posConfig.StatusDamageGroupX ?? defaultPosConfig.StatusDamageGroupX!.Value;
        if (ImGui.DragFloat("狀態－傷害－水平##StatusDamageGroupPosXSlider", ref tmp))
        {
            posConfig.StatusDamageGroupX = tmp;
            hasPosChanged = true;
        }

        tmp = posConfig.StatusDamageGroupY ?? defaultPosConfig.StatusDamageGroupY!.Value;
        if (ImGui.DragFloat("狀態－傷害－垂直##StatusDamageGroupPosYSlider", ref tmp))
        {
            posConfig.StatusDamageGroupY = tmp;
            hasPosChanged = true;
        }

        if (ImGui.Button("測試##TestPositions"))
        {
            CreateFlyPopupText(true, false);
        }

        ImGui.SameLine();
        if (ImGui.Button("重設所有位置##ResetPositions"))
        {
            Service.FlyTextHandler.ResetPositions();
            Service.Configuration.FlyTextAdjustments.FlyTextPositions = new FlyTextPositions();
            hasPosChanged = true;
        }

        if (hasPosChanged)
        {
            Service.Configuration.Save();
            Service.FlyTextHandler.ApplyPositions();
            CreateFlyPopupText(true, false);
        }

        ImGui.Unindent();

        ImGui.Separator();

        ImGui.Text("縮放：");

        ImGui.Indent();

        ImGui.Text("浮動文字來自你或顯示於你身上；彈出文字則來自其他人或顯示於其他人身上。" +
                   "\n實際規則更複雜，但大多數情況可如此理解。" +
                   "\n此數值會覆寫角色設定中的大小。");

        var hasPopUpTextPosChanged = false;
        var (defaultFlyTextScale, defaultPopUpTextScale) = FlyTextHandler.GetDefaultScaling();
        tmp = adjustmentsConfig.FlyTextScale ?? defaultFlyTextScale;
        if (ImGui.DragFloat("浮動文字縮放##FlyTextScaleSlider", ref tmp, 0.005f))
        {
            adjustmentsConfig.FlyTextScale = tmp;
            hasPopUpTextPosChanged = true;
        }

        tmp = adjustmentsConfig.PopupTextScale ?? defaultPopUpTextScale;
        if (ImGui.DragFloat("彈出文字縮放##PopupTextScaleSlider", ref tmp, 0.005f))
        {
            adjustmentsConfig.PopupTextScale = tmp;
            hasPopUpTextPosChanged = true;
        }

        if (ImGui.Button("測試##TestScaling"))
        {
            CreateFlyPopupText(true, true);
        }

        ImGui.SameLine();
        if (ImGui.Button("重設縮放##ResetPositions"))
        {
            Service.FlyTextHandler.ResetScaling();
            Service.Configuration.FlyTextAdjustments.FlyTextScale = null;
            Service.Configuration.FlyTextAdjustments.PopupTextScale = null;
            hasPopUpTextPosChanged = true;
        }

        if (hasPopUpTextPosChanged)
        {
            Service.Configuration.Save();
            Service.FlyTextHandler.ApplyScaling();
            CreateFlyPopupText(true, true);
        }

        ImGui.Unindent();

        ImGui.Separator();

        if (Service.GameConfig.UiControl.TryGetBool("FlyTextDisp", out var flyTextEnabled) && !flyTextEnabled)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "遊戲設定中已停用浮動文字。");
        }

        if (Service.GameConfig.UiControl.TryGetBool("PopUpTextDisp", out var popUpTextEnabled) && !popUpTextEnabled)
        {
            ImGui.TextColored(ImGuiColors.DalamudRed, "遊戲設定中已停用彈出文字。");
        }

        ImGui.TextColored(ImGuiColors.DalamudGrey, "提示：按住 Ctrl 並點擊可直接編輯數值；所有插件皆可使用此操作。");
    }

    public static void CreateFlyPopupText(bool createFlyText, bool createPopUpText)
    {
        if (createFlyText)
        {
            Service.FlyTextHandler.CreateFlyText(FlyTextKind.Healing, 0, 0);
            Service.FlyTextHandler.CreateFlyText(FlyTextKind.Buff, 0, 0);
            Service.FlyTextHandler.CreateFlyText(FlyTextKind.Damage, 0, 0);
        }

        if (createPopUpText)
        {
            Service.FlyTextHandler.CreateFlyText(FlyTextKind.Damage, 3, 2);
            Service.FlyTextHandler.CreateFlyText(FlyTextKind.Damage, 2, 2);
        }
    }
}
