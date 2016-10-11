using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Shield : MonoBehaviour {

	Vector3 maxSize = new Vector3(200f, 200f, 200f);
	Vector3 lerpSize = new Vector3(20f, 20f, 20f);

	float maxHealth = 50f;
	float health;

	public bool isBlocking;

	public Reinhardt myReinhardt;

	public GameObject blockedParticle;

	public Renderer myRenderer;
	public Material[] myShieldMaterial;

	public LayerMask shieldLayerMask;

	public NetworkIdentity myUnitId;

	void Start()
	{
		myShieldMaterial = myRenderer.materials;
	}

	public void TakeDamage(float dmg)
	{
		
		health -= dmg;

		float newRed = ((maxHealth - health) / maxHealth);

		myShieldMaterial [1].SetColor ("_EmissionColor", new Color (newRed, 0.4f, 0.5f));

		if (health <= 0) {

			AnimateOff ();

		}

	}

	public void ResetShield()
	{
		health = maxHealth;
		myShieldMaterial[1].SetColor ("_EmissionColor", new Color (0f, 0.4f, 0.5f));
	}

	public void AnimateOn()
	{

		if (!isBlocking) {
			transform.localScale = Vector3.zero;
			health = maxHealth;
			StartCoroutine (AnimateShieldOn ());
		}

	}

	public void AnimateOff()
	{

		if (isBlocking) {
			transform.localScale = Vector3.zero;
			StartCoroutine (AnimateShieldOff ());
		}

	}

	void OnTriggerEnter(Collider other)
	{
		
		if (isBlocking) {
			GameObject collision = other.gameObject;

			if (collision.CompareTag ("Bullet")) {

				collision.gameObject.SetActive (false);

				RaycastHit hit;

				if (Physics.Raycast (collision.transform.position, collision.transform.forward, out hit, shieldLayerMask)) {

					Instantiate (blockedParticle, hit.point, transform.root.rotation);

				}

				Bullet tempBullet = collision.GetComponent<Bullet> ();

				myReinhardt.ShieldTakeDamage (tempBullet.damage);

				Destroy (collision);

			}
		}

	}

	IEnumerator AnimateShieldOn()
	{

		while (transform.localScale.x < maxSize.x) {

			transform.localScale = Vector3.Lerp (transform.localScale, maxSize + lerpSize, 5f * Time.deltaTime);

			yield return new WaitForSeconds (Time.deltaTime);

		}

		transform.localScale = maxSize;
		ResetShield ();
		isBlocking = true;

	}

	IEnumerator AnimateShieldOff()
	{

		while (transform.localScale.x > 0) {

			transform.localScale = Vector3.Lerp (transform.localScale, Vector3.zero - lerpSize, 5f * Time.deltaTime);

			yield return new WaitForSeconds (Time.deltaTime);

		}

		transform.localScale = Vector3.zero;
		isBlocking = false;
		myReinhardt.ShieldOff ();

	}


}
