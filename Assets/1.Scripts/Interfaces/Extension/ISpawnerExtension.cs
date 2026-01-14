public interface ISpawnerExtension : IMiniGameExtension<int>
{
    int Get(SpawnableDef def, LoopRank rank);
    void Set(SpawnableDef def, LoopRank rank, int value);
    void Mutate(SpawnableDef def, LoopRank rank, int delta);
}
