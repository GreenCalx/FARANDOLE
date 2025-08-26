using UnityEngine;
using UnityEngine.UI;

public class UIStageClearAnimation : MonoBehaviour
{
    public const string StageClearTrigger = "StageClear";
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void Animate()
    {
        animator.SetTrigger(StageClearTrigger);
    }
}
