using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{

    //Script attached to any bullet projectiles
    public Unit target;
	protected Transform targetT;
	protected Vector3 targetPosition;
    protected float damage = 1f;
	protected float velocity = 1f;

    protected virtual void FixedUpdate()
    {
        if (targetT == null)
        {
            Destroy(gameObject);
        }
        else
        {
			transform.LookAt (targetT.position);
			transform.Translate(Vector3.forward * velocity * Time.fixedDeltaTime);

            if (Vector3.Distance(targetT.transform.position, transform.position) < 1f)
            {
                target.Damage(damage);
                Destroy(gameObject);
            }
        }
    }

    public virtual void Setup(Unit tar, float dam, float vel)
    {
        target = tar;
        damage = dam;
        velocity = vel;
        targetT = target.transform;
		targetPosition = targetT.position;
    }
}
