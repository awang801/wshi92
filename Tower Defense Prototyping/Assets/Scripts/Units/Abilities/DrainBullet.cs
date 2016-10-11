using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrainBullet : Bullet
{

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
				
			transform.rotation = Quaternion.Lerp (transform.rotation, toRotation, 0.5f);
			transform.Translate(Vector3.forward * velocity * Time.deltaTime);

			if (Vector3.Distance(targetT.transform.position, transform.position) < 1f)
			{
				target.Damage(damage);
				Destroy(gameObject);
			}
		}
        
    }



}
