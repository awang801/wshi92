using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBullet : Bullet
{

	public GameObject explosionParticle;

    protected override void FixedUpdate()
    {
        
		if (targetT == null)
		{
			Destroy(gameObject);
		}
		else
		{
			Vector3 relativePos = targetT.position - transform.position;
			Quaternion toRotation = Quaternion.LookRotation(relativePos);
				
			transform.rotation = Quaternion.Lerp (transform.rotation, toRotation, 0.1f);
			transform.Translate(Vector3.forward * velocity * Time.fixedDeltaTime);

			if (Vector3.Distance(targetT.transform.position, transform.position) < 1f)
			{
				Instantiate (explosionParticle, targetT.position, explosionParticle.transform.rotation);
				target.Damage(damage);
				Destroy(gameObject);
			}
		}
        
    }



}
