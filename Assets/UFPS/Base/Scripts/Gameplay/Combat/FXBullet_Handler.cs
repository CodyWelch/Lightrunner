using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class FXBullet_Handler : vp_FXBullet
{
    protected override void DoUFPSDamage()
    {
        Debug.Log("hit!");

        // Must bea headshot if the BoxCollider is hit
        if (m_Hit.collider is BoxCollider)
        {
            Debug.Log("headshot!");
            Damage = 100;
        }

        // Call base method
        base.DoUFPSDamage();
    }
}

