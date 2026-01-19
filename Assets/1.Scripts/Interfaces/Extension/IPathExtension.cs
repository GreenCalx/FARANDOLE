using UnityEngine.Splines;
public interface IPathExtension : IMiniGameExtension<SplineContainer>
{
    int Get(PathDef def, LoopRank rank);
    void Set(PathDef def, LoopRank rank, int value);
    void Mutate(PathDef def, LoopRank rank, int delta);
}
