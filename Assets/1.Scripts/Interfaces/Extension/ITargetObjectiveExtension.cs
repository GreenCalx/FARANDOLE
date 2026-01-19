public interface ITargetObjectiveExtension : IMiniGameExtension<int>
{
    int Get(string key, LoopRank rank);
    void Set(string key, LoopRank rank, int value);
    void Mutate(string key, LoopRank rank, int delta);
}
