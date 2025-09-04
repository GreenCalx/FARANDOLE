using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
public interface IAnimationQueue
{
    public Queue<Func<UniTask>> GetAnimQueue(CancellationToken iCT);
}
