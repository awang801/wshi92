using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MagicBullet : Bullet
{

	public GameObject explosionParticle;

    protected override void Update()
    {
        
		if (targetT == null)
		{
			Destroy(gameObject);
		}
		else
		{
			Vector3 relativePos = targetT.position - transform.position;
			Quaternion toRotation = Quaternion.LookRotation(relativePos);
				
			transform.rotation = Quaternion.Lerp (transform.rotation, toRotation, 0.2f);
			transform.Translate(Vector3.forward * velocity * Time.deltaTime);

			if (Vector3.Distance(targetT.transform.position, transform.position) < 1f)
			{
				Instantiate (explosionParticle, targetT.position, explosionParticle.transform.rotation);
				target.Damage(damage);
				Destroy(gameObject);
			}
		}
        
    }



}
