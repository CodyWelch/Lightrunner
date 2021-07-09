using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler_Enemy : vp_DamageHandler
{
    private Animator anim;
    private Enemy enemy;

    protected override void Awake()
    {
        base.Awake();
        enemy = this.GetComponent<Enemy>();
        anim = GetComponent<Animator>();
    }
    public override void Damage(vp_DamageInfo damageInfo)
    {
        if (CurrentHealth > 0)
        {
            base.Damage(damageInfo);
            enemy.PlayerDetected();
            anim.Play("hit", 0, 0.25f);
        }
    }

    public override void Die()
    {
        if (!enabled || !vp_Utility.IsActive(gameObject))
        {
            return;
        }

        if (m_Audio != null)
        {
            m_Audio.pitch = Time.timeScale;
            m_Audio.PlayOneShot(DeathSound);
        }
        anim.Play("death", 0, 0);
        this.GetComponent<Enemy>().Die();
    }
}
