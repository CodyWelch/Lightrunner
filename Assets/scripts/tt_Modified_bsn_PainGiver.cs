// Inspired in its entirity by VisionPunks HitscanBullet and VenomUnity's
// initial script.

// tt has made some modifications to this version, but it can still function exactly as the original
// tt changed the class and file name so as not to overwrite the original work of bsn!
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
[RequireComponent(typeof(AudioSource))]

public class tt_Modified_bsn_PainGiver : MonoBehaviour

{
    public float DamageHigh = 20.0f;    // the max possible damage to send
    public float DamageLow = 0.001f;    // the min possible damage to send			
    private float Damage = 1f;
    public string DamageMethodName = "Damage";  // name of damage method on target	
    public bool VaryDamage = false;
    protected AudioSource m_Audio = null;
    public List<AudioClip> m_DamageSounds = new List<AudioClip>();  // sounds to randomly play,fire,steam,screams,etc.
    public Vector2 SoundDamagePitch = new Vector2(1.0f, 1.5f);  // random pitch for sounds to add variety
    public LayerMask ignoreMask;

    // tt solving for an issue where a player continues to receive damage at respawn.
    // It appears that there is a "race" condition, where the "dead" player is still inside
    // the trigger when respawn occurs, and therefore, damage continues to be dealt to the player.
    // My solution is to use a combination of damage values and time delays to overcome the issue.

    // tt added these vars

    // set this to true to use timing feature for special cases 

    public bool useTiming = false;

    // a timer for use in OnTriggerStay
    protected vp_Timer.Handle m_Timer = new vp_Timer.Handle();

    // if intending to kill the subject immediately, set a very high timer value
    // so that the trigger has a chance to clear properly
    public float PainInterval = 5f;

    // triggered to true when subject enters the paingiver and false upon exit
    private bool inPainZone = false;


	// Distance to player
	private GameObject player;

	[SerializeField]
	Animator anim;
    void Start()
    {
		anim = this.gameObject.GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag ("Player");

        m_Audio = GetComponent<AudioSource>();

    }

    void OnTriggerEnter(Collider other)
    {
		if (enabled) {
			if (Vector3.Distance (this.gameObject.transform.position, other.gameObject.transform.position) < 5) {
				inPainZone = true;
			}

			// if it's not on the mask, hurt it!
			if (!IsInLayerMask (other.gameObject, ignoreMask))
				SendThePain (other);
		}
    }

    void OnTriggerStay(Collider other)
    {
		if (enabled) {
			// tt added this IF and the timing process
			if (useTiming)
			{
				if (!IsInLayerMask(other.gameObject, ignoreMask))
				{
					vp_Timer.In(PainInterval, delegate ()
						{ SendThePain(other); });
				}
			}
			else
				if (!IsInLayerMask(other.gameObject, ignoreMask)) SendThePain(other);
			
		}
     }

    // tt added this exit
    void OnTriggerExit(Collider other)
    {
        inPainZone = false;
        m_Audio.Stop();
    }


    void SendThePain(Collider other)
    {
		Debug.Log (anim);
		//anim.SetTrigger ("attack");
		anim = this.gameObject.GetComponent<Animator>();
		Debug.Log (other.tag);
        // tt added this IF
		if (inPainZone && ready_to_attack)
        {
			anim.SetTrigger ("attack");
            // vary the damage or default to highest damage
			if (VaryDamage) {
				Damage = Random.Range (DamageLow, DamageHigh);
			} else { 
				Damage = DamageHigh;
				Debug.Log ("Damage High: " + DamageHigh);
			}

            if (m_DamageSounds.Count > 0)
            {
                m_Audio.pitch = Random.Range(SoundDamagePitch.x, SoundDamagePitch.y) * Time.timeScale;
                m_Audio.PlayOneShot(m_DamageSounds[(int)Random.Range(0, (m_DamageSounds.Count))]);
            }
			if (other.transform.gameObject.activeSelf == true) {
				// do damage on the target
				Debug.Log ("damage player for: " + Damage);
				other.SendMessage (DamageMethodName, Damage, SendMessageOptions.DontRequireReceiver);
				StartCoroutine (AttackCooldownTimer ());
			}
        }
    }

	int attackCooldown = 1;
	bool ready_to_attack = true;

	IEnumerator AttackCooldownTimer (){
		ready_to_attack = false;
		yield return new WaitForSeconds (attackCooldown);
		ready_to_attack = true;
	}

    // Convert the object's layer to a bitfield for comparison
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = (1 << obj.layer);
        if ((layerMask.value & objLayerMask) > 0) return true;
        else return false;
    }

}