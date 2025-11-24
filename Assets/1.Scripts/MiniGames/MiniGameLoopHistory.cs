using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MiniGameLoopHistory : IEnumerable
{
    List<MiniGameLoopSnapshot> snapshots;
    public LoopRank RankOnFullCompletion;
    public int Count => snapshots != null ? snapshots.Count : 0;
    public MiniGameLoopHistory()
    {
        snapshots = new List<MiniGameLoopSnapshot>();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }
    public MiniGameLoopHistoryEnum GetEnumerator()
    {
        return new MiniGameLoopHistoryEnum(snapshots);
    }
    public void AddSnapshot(MiniGameLoop iTarget)
    {
        snapshots.Add(new MiniGameLoopSnapshot(iTarget));
    }
}

public class MiniGameLoopHistoryEnum : IEnumerator<MiniGameLoopSnapshot>
{
    public MiniGameLoopSnapshot Current { get { return currentSnapshot; } }
    object IEnumerator.Current { get { return Current; } }
    MiniGameLoopSnapshot currentSnapshot;
    public List<MiniGameLoopSnapshot> snapshots;
    public int index = -1;

    public MiniGameLoopHistoryEnum(List<MiniGameLoopSnapshot> iSnapshots)
    {
        snapshots = iSnapshots;
    }
    public void Reset()
    {
        index = -1;
    }

    public bool MoveNext()
    {
        if (++index >= snapshots.Count)
            return false;
        currentSnapshot = snapshots[index];
        return true;
    }
    void IDisposable.Dispose()
    {
        //snapshots.Clear();
    }


}
