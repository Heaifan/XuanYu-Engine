namespace XuanYu.WarCore.State;

/// <summary>
/// 士兵状态：身体状态、体力、士气、压制，统一 0–100 范围。
/// R1 中作为外部输入，不代表最终全部由玩家直接控制。
/// 构造时校验范围，越界抛出明确错误。
/// </summary>
public readonly record struct SoldierState
{
    public const int MinValue = 0;

    public const int MaxValue = 100;

    public SoldierState(int bodyCondition, int stamina, int morale, int suppression)
    {
        BodyCondition = RequireRange(bodyCondition, nameof(bodyCondition));
        Stamina = RequireRange(stamina, nameof(stamina));
        Morale = RequireRange(morale, nameof(morale));
        Suppression = RequireRange(suppression, nameof(suppression));
    }

    public int BodyCondition { get; }

    public int Stamina { get; }

    public int Morale { get; }

    public int Suppression { get; }

    private static int RequireRange(int value, string paramName)
    {
        if (value is < MinValue or > MaxValue)
        {
            throw new ArgumentOutOfRangeException(
                paramName, value, $"状态值必须在 {MinValue} 到 {MaxValue} 之间。");
        }

        return value;
    }
}
