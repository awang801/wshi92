using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public Unit target;
	Transform targetT;

	float damage = 1f;
	float velocity = 1f;

	void Start () {
		
	}

	void FixedUpdate()
	{
		if (targetT == null) {
			Destroy (gameObject);
		} else {
			Vector3 direction = (targetT.transform.position - transform.position).normalized;
			transform.Translate (direction * velocity * Time.deltaTime);

			if (Vector3.Distance (targetT.transform.position, transform.position) < 1f) {
				target.Damage (damage);
				Destroy (gameObject);
			}
		}
	}

	public void setup(Unit tar, float dam, float vel)
	{
		target = tar;
		damage = dam;
		velocity = vel;
		targetT = target.transform;
	}

}
