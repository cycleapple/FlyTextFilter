using Dalamud.Game.Gui.FlyText;

namespace FlyTextFilter;

public class FlyTextKindData
{
    public static string GetAlias(FlyTextKind flyTextKind)
    {
        return flyTextKind switch
        {
            FlyTextKind.AutoAttackOrDot => "自動攻擊／持續傷害",
            FlyTextKind.AutoAttackOrDotDh => "自動攻擊／持續傷害－直擊",
            FlyTextKind.AutoAttackOrDotCrit => "自動攻擊／持續傷害－暴擊",
            FlyTextKind.AutoAttackOrDotCritDh => "自動攻擊／持續傷害－暴擊直擊",
            FlyTextKind.BuffFading => "增益狀態－消失",
            FlyTextKind.DebuffFading => "減益狀態－消失",
            FlyTextKind.DebuffResisted => "減益狀態－抵抗",
            FlyTextKind.DebuffNoEffect => "減益狀態－無效",
            FlyTextKind.DebuffInvulnerable => "減益狀態－無敵",
            FlyTextKind.LootedItem => "取得物品",
            FlyTextKind.HealingCrit => "治療－暴擊",
            FlyTextKind.DamageCrit => "傷害－暴擊",
            FlyTextKind.DamageDh => "傷害－直擊",
            FlyTextKind.DamageCritDh => "傷害－暴擊直擊",
            FlyTextKind.CraftingQuality => "製作品質",
            FlyTextKind.CraftingQualityCrit => "製作品質－暴擊",
            FlyTextKind.CraftingQualityDh => "製作品質－直擊",
            FlyTextKind.CraftingQualityCritDh => "製作品質－暴擊直擊",
            FlyTextKind.CraftingProgress => "製作進展",
            FlyTextKind.CpRegen => "恢復－製作力",
            FlyTextKind.EpRegen => "恢復－能量",
            FlyTextKind.GpRegen => "恢復－採集力",
            FlyTextKind.MpRegen => "恢復－魔力",
            FlyTextKind.AutoAttackNoText3 => "未使用－自動攻擊 3",
            FlyTextKind.Collectability => "收藏價值",
            FlyTextKind.CollectabilityCrit => "收藏價值－暴擊",
            FlyTextKind.CriticalHit4 => "未使用－暴擊 4",
            FlyTextKind.HpDrain => "吸收－體力",
            FlyTextKind.MpDrain => "吸收－魔力",
            FlyTextKind.NamedCriticalHitWithMp => "未使用－具名暴擊 MP",
            FlyTextKind.NamedCriticalHitWithTp => "未使用－具名暴擊 TP",
            FlyTextKind.NamedDodge => "閃避－具名",
            FlyTextKind.FullyResisted => "完全抵抗",
            FlyTextKind.HasNoEffect => "沒有效果",
            FlyTextKind.NamedMiss => "未命中－具名",
            FlyTextKind.NamedMp3 => "未使用－具名 MP3",
            FlyTextKind.NamedTp => "未使用－具名 TP",
            FlyTextKind.NamedTp2 => "未使用－具名 TP2",
            FlyTextKind.NamedTp3 => "未使用－具名 TP3",
            FlyTextKind.IslandExp => "經驗值－無人島",
            FlyTextKind.None => "未使用－無",
            FlyTextKind.PhantomExp => "經驗值－幻影",
            _ => flyTextKind.ToString(),
        };
    }
}
