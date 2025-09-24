using Unity.VisualScripting;
using UnityEngine;

public class MoleInHoleBehaviour : StateMachineBehaviour
{
    private Mole mole;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (mole == null)
        {
            mole = animator.GetComponent<Mole>();
        }
        mole.MoleInHole();
    }
}
