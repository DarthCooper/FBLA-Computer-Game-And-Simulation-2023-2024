using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    PlayerAttack attack;

    // Start is called before the first frame update
    void OnEnable()
    {
       attack = GetComponentInParent<PlayerAttack>(); 
    }

    public void StartAttack()
    {
    }

    public void EndAttack()
    {
        attack.changeSwing(true);
        Invoke(nameof(resetAnim), 0.2f);
    }

    public void resetAnim()
    {
        attack.resetAnimValues();
        attack.changeSwing(false);
    }

    public void resetAllAnims()
    {
        attack.closeAllAnims();
    }
}
