using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSourceTag))]
public class Enemy : MonoBehaviour
{

	protected NavMeshAgent pathfinder;
	private Transform target;
	private bool bPlayerDetected;
	private Animator anim;
	private bool bIsDead;

	private GameController gameController;
	private DamageHandler_Enemy damageHandler;

	[SerializeField]
	private int speed;
	[SerializeField]
	private int health;
	[SerializeField]
	private int damage;
	[SerializeField]
	private int experienceValue;

	public int Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
		}
	}

	public int Speed
	{
		get 
		{
			return speed;
		}
		set 
		{
			speed = value;
			pathfinder.speed = value;
		}
	}

    public int ExperienceValue
	{
		get
		{
			return experienceValue;
		}
        set
        {
			experienceValue = value;
        }

	}

	public int Health
    {
		get
		{
			return health;
        }
		set
        {
			health = value;
        }
    }

	void Awake()
    {
		damageHandler = this.GetComponent<DamageHandler_Enemy>();

	}
	void Start()
	{
		bIsDead = false;
		gameController = GameController.instance;
		experienceValue = 1;
		anim = this.GetComponent<Animator>();
		bPlayerDetected = false;
		pathfinder = GetComponent<NavMeshAgent>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		StartCoroutine(UpdatePath());
		pathfinder.height = 0.5f;
		pathfinder.baseOffset = 0;
	}

    IEnumerator UpdatePath()
	{

		float refreshRate = 0.25f;

		damageHandler.MaxHealth = health;

		if (target == null)
		{
			target = GameObject.FindGameObjectWithTag("Player").transform;
		}

		while (target != null)
		{

			if (bPlayerDetected && !bIsDead)
			{
				pathfinder.SetDestination(target.transform.position);
				anim.SetBool("moving", true);
			}
			else
			{
				if (Vector3.Distance(transform.position, target.transform.position) < 5)
				{
					PlayerDetected();
				}
			}
			yield return new WaitForSeconds(refreshRate);
		}
	}

	public void PlayerDetected()
	{
		pathfinder.enabled = true;
		bPlayerDetected = true;
	}

	public void Die()
	{
		gameController.EnemyDied(this.gameObject);
		bIsDead = true;
		this.gameObject.GetComponent<tt_Modified_bsn_PainGiver>().enabled = false;
		pathfinder.enabled = false;
	}
}
