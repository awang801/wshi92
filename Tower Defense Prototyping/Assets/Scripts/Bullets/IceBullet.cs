using UnityEngine;
using System.Collections;

public class IceBullet : MonoBehaviour {

	public ParticleSystem particles;
	public ParticleSystem particles2;
	ParticleCollisionEvent[] collisionEvents;
	public float damage;
	public float coldness;

	public void Setup(float dmg, float cold)
	{
		damage = dmg;
		coldness = cold;
		ToggleOff ();

	}

	void Awake()
	{
		particles = GetComponent<ParticleSystem> ();
		particles2 = GetComponentInParent<ParticleSystem> ();
		collisionEvents = new ParticleCollisionEvent[128];

	}

	public void ToggleOn()
	{
		if (!particles.isPlaying) {
			particles.Play ();
			particles2.Play ();
		}

	}

	public void ToggleOff()
	{
		if (particles.isPlaying) {
			particles.Stop ();
			particles2.Stop ();
		}
	}

	void OnParticleCollision(GameObject other)
	{
		int safeLength = particles.GetSafeCollisionEventSize ();
		if (collisionEvents.Length < safeLength) {
			collisionEvents = new ParticleCollisionEvent[safeLength];
		}

		int numCollisionEvents = particles.GetCollisionEvents (other, collisionEvents);

		Unit unit = other.GetComponent<Unit> ();

		int i = 0;
		while (i < numCollisionEvents) {

			unit.Damage (damage);
			unit.addTemperature (-coldness);

			i++;
		}
	}

}
