using UnityEngine.Splines;
public interface IPathExtension : IMiniGameExtension<SplineContainer>
{
    SplineContainer Get(PathDef def, LoopRank rank);
    void Set(PathDef def, LoopRank rank, SplineContainer value);
    void Mutate(PathDef def, LoopRank rank, SplineContainer delta);
}
