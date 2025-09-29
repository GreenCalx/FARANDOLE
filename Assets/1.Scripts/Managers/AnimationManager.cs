using UnityEngine;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;

public class AnimationManager : MonoBehaviour, IManager
{
    internal class AnimationQueue
    {
        public Animator animator;
        public Queue<Func<UniTask>> queue;
        public CancellationToken cancellationToken;
        public bool animationDone = false;
        public AnimationQueue(Animator iAnimator, CancellationToken iCT)
        {
            animator = iAnimator;
            cancellationToken = iCT;
            queue = new Queue<Func<UniTask>>();
            animationDone = false;
        }

        public void AddTail()
        {
            Func<UniTask> tail = async () =>
            {
                animationDone = true;
                Debug.Log("Tail");
            };
            queue.Enqueue(tail);
        }
        public bool AnimationDone()
        {
            return animationDone;
        }

        public void OnCancel()
        {
            Debug.Log("Cancelled");
            animationDone = true;
            queue.Clear();
        }
    }
    List<AnimationQueue> trackedAnimators;
    GameManager GM;

    public void Init(GameManager iGameManager)
    {
        GM = iGameManager;
        trackedAnimators = new List<AnimationQueue>(0);
    }
    public bool IsReady()
    {
        return trackedAnimators != null;
    }

    // ------------------------
    AnimationQueue GetAnimQ(Animator iAnimator)
    {
        foreach (var q in trackedAnimators.Where(e => e.animator == iAnimator))
        {
            return q;
        }
        return null;
    }
    public void TrackAnimator(Animator iAnimator, CancellationToken iCT)
    {
        if (!trackedAnimators.Exists(e => e.animator == iAnimator))
            trackedAnimators.Add(new AnimationQueue(iAnimator, iCT));
    }
    public void StopTrackAnimator(Animator iAnimator)
    {
        AnimationQueue to_rm = GetAnimQ(iAnimator);
        if (to_rm != null)
        {
            trackedAnimators.Remove(to_rm);
            trackedAnimators = trackedAnimators.Where(e => e != null).ToList();
        }
    }

    public void QueueAnim(Animator iAnimator, Func<UniTask> iAnimStep)
    {
        AnimationQueue animQ = GetAnimQ(iAnimator);
        if (animQ == null)
            return;
        animQ.queue.Enqueue(iAnimStep);
    }
    public void QueueAnimRange(Animator iAnimator, Queue<Func<UniTask>> iAnimSteps)
    {
        AnimationQueue animQ = GetAnimQ(iAnimator);
        if (animQ == null)
            return;
        while (iAnimSteps.Count > 0)
        {
            animQ.queue.Enqueue(iAnimSteps.Dequeue());
        }
    }

    public async UniTask PlayAnim(Animator iAnimator)
    {
        AnimationQueue animQ = GetAnimQ(iAnimator);
        animQ.AddTail();
        CancellationTokenRegistration ctr = animQ.cancellationToken.Register(() => animQ.OnCancel());
        UniTask animStep;
        do
        {
            animStep = UniTask.Run(animQ.queue.Dequeue());
            await animStep;
        } while (!animQ.AnimationDone());
    }
}
