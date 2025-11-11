using UnityEngine;

public class Projectile : MonoBehaviour
{
	public GameObject bulletHoleEffect;
	public LayerMask wallLayer, groundLayer;
	public GameObject wallImpactEffect, groundImpactEffect;

	[Header("Dano")]
	[Tooltip("Dano que este projétil causa em inimigos.")]
	public float damage = 25f;

	private void OnCollisionEnter(Collision collision)
	{
		var enemy = collision.gameObject.GetComponentInParent<Enemy>();
		if (enemy != null)
		{
			enemy.TakeDamage(damage);
		}

		Vector3 contactPoint = collision.contacts[0].point;
		Vector3 contactNormal = collision.contacts[0].normal;

		GameObject newImpact = Instantiate(bulletHoleEffect, contactPoint, Quaternion.LookRotation(-contactNormal));
		newImpact.transform.Translate(Vector3.forward * -0.001f);
		Destroy(newImpact, 5);

		int hitLayer = collision.gameObject.layer;

		if (wallLayer == (wallLayer | (1 << hitLayer)))
		{
			GameObject wallImpact = Instantiate(wallImpactEffect, contactPoint, Quaternion.LookRotation(contactNormal));
		}
		else if(groundLayer == (groundLayer | (1 << hitLayer)))
		{
			GameObject groundImpact = Instantiate(groundImpactEffect, contactPoint, Quaternion.LookRotation(contactNormal));
		}

		Destroy(gameObject);
	}
}
