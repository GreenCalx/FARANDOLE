public interface IMiniGameExtension<T>
{
    T Get(object key, LoopRank rank);
    void Set(object key, LoopRank rank, T value);
    void Mutate(object key, LoopRank rank, T delta);
}
