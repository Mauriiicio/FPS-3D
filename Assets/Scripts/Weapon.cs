using UnityEngine;

public class Weapon : MonoBehaviour
{
	// Referências necessárias
	public Transform muzzlePoint;
	public GameObject projectilePrefab;

	[Header("Configurações do Tiro")]
	public float maxSpreadAngle = 5f;
	public float projectileForce = 50f;
	public float fireRateRPS = 10f;
	public bool automatic = false;

	public float bulletDestroyTime = 5f;

	public int ammoClip = 6;
	public float reloadTime = 2f;
	int currentAmmo;
	bool isReloading = false;
	public GameObject shootFx;
	// Variável de controle do tempo
	private float nextTimeToFire = 0f;
	AudioSource audioSource;
	WeaponManager weaponManager;
	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		currentAmmo = ammoClip;
		weaponManager = GetComponentInParent<WeaponManager>();
	}

	// FUNÇÃO PÚBLICA DE TIRO (Chamada pelo WeaponManager)
	public bool Shoot(Vector3 raycastDirection, Vector3 raycastImpactPoint, Crosshair crosshairController)
	{
		//VERIFICAÇÃO DE COOLDOWN (Responsabilidade da Arma)
		if (Time.time < nextTimeToFire || currentAmmo <= 0 || isReloading)
		{
			return false; // Impede o tiro
		}

		shootFx.SetActive(false);
		shootFx.SetActive(true);
		currentAmmo--;
		weaponManager.UpdateAmmoDisplay(currentAmmo, ammoClip);
		//ATUALIZAÇÃO DO COOLDOWN (Responsabilidade da Arma)
		nextTimeToFire = Time.time + 1f / fireRateRPS;


		// CÁLCULO DA TRAJETÓRIA DO PROJÉTIL (Corrigida e Perfeita)
		// Calcula a direção do muzzlePoint até o ponto de impacto (real ou distante)
		Vector3 projectileTargetDirection = (raycastImpactPoint - muzzlePoint.position).normalized;

		//Lança o Projétil
		LaunchProjectile(projectileTargetDirection);

		//Aplica Feedback Visual
		if (crosshairController != null)
		{
			crosshairController.ApplySpread();
		}

		audioSource.PlayOneShot(audioSource.clip);

		return true; // Tiro realizado com sucesso
					 
	}


	private void LaunchProjectile(Vector3 direction)
	{
		GameObject projectileGO = Instantiate(projectilePrefab, muzzlePoint.position, Quaternion.identity);
		Projectile projectileScript = projectileGO.GetComponent<Projectile>();

		if (projectileScript != null)
		{
			// Note que não precisamos mais passar o targetImpactPoint para o projétil,
			// apenas a direção e força, pois a correção de alinhamento já foi feita
			// ao calcular o 'projectileTargetDirection'.
			Rigidbody rb = projectileGO.GetComponent<Rigidbody>();
			if (rb != null)
			{
				rb.linearVelocity = direction * projectileForce;
			}

			Destroy(projectileGO, bulletDestroyTime);
		}
	}

	public async void Reload()
	{
		if (isReloading || currentAmmo == ammoClip) return;
		isReloading = true;
		await Awaitable.WaitForSecondsAsync(reloadTime);
		currentAmmo = ammoClip;
		weaponManager.UpdateAmmoDisplay(currentAmmo, ammoClip);
		isReloading = false;
	}

	public int GetCurrentAmmo()
	{
		return currentAmmo;
	}

	public bool IsReloading()
	{
		return isReloading;
	}
}