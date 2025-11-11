using StarterAssets;
using TMPro;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	// A câmera que representa a visão e o centro da mira
	public Camera playerCamera;
	// Referência do seu script de mira dinâmica
	public Crosshair crosshairController;

	// Lista de todas as armas disponíveis
	public Weapon[] availableWeapons;
	private int currentWeaponIndex = 0;

	public TMP_Text ammoDisplay;

	// Propriedade para controlar o aiming (mirar)
	public bool isAiming = false;
	public GameObject playerAimCamera;

	Animator animator;

	public StarterAssetsInputs playerInputs;

	void Start()
	{
		animator = GetComponent<Animator>();
		SwitchWeapon(currentWeaponIndex);
	}

	void Update()
	{
		//INPUT DE TIRO (Mouse 0 ou botão "Fire1")
		if (playerInputs.shoot)
		{
			// Pede para a arma ativa disparar
			FireCurrentWeapon();
			if (!availableWeapons[currentWeaponIndex].automatic)
			{
				playerInputs.shoot = false; // Evita disparos contínuos
			}
		}

		if(playerInputs.reload)
		{
			playerInputs.reload = false;
			Weapon currentWeapon = availableWeapons[currentWeaponIndex];
			if(currentWeapon == null || currentWeapon.IsReloading() || currentWeapon.GetCurrentAmmo() == currentWeapon.ammoClip)
				return;
			animator.SetTrigger("reload");
			availableWeapons[currentWeaponIndex].Reload();
			
		}
		

		//INPUT DE AIMING (Mirar - Ex: botão direito do mouse)
		isAiming = Input.GetButton("Fire2");
		animator.SetBool("aim", isAiming);
		playerAimCamera.SetActive(isAiming);

	}

	public void FireCurrentWeapon()
	{
		if (availableWeapons.Length == 0) return;

		// Pega a arma ativa
		Weapon activeWeapon = availableWeapons[currentWeaponIndex];

		//Cálculo da Direção do Raycast com Aleatoriedade (Spread)
		float currentSpread = isAiming ? 0f : activeWeapon.maxSpreadAngle;
		Vector3 shootDirection = GetSpreadDirection(currentSpread);

		// 2. Dispara o Raycast da Câmera
		Vector3 impactPoint;

		if (Physics.Raycast(playerCamera.transform.position, shootDirection, out RaycastHit hit, 100f))
		{
			// Acertou algo, o ponto de impacto é o hit.point
			impactPoint = hit.point;
		}
		else
		{
			// Não acertou nada, define um ponto distante na direção do Raycast
			impactPoint = playerCamera.transform.position + shootDirection * 100f;
		}

		//Chama a função de tiro da arma, passando TUDO o que ela precisa
		// (a direção do raycast, o ponto de impacto e a referência à mira)
		bool shot = activeWeapon.Shoot(shootDirection, impactPoint, crosshairController);
		if (shot)
		{
			animator.SetTrigger("shoot");		
		}
	}

	private Vector3 GetSpreadDirection(float spreadAngle)
	{
		// ******* (A mesma função que estava no WeaponController) *******
		Vector3 baseDirection = playerCamera.transform.forward;

		if (spreadAngle <= 0) return baseDirection;

		Quaternion randomRotation = Quaternion.Euler(
			Random.Range(-spreadAngle, spreadAngle),
			Random.Range(-spreadAngle, spreadAngle),
			0f
		);
		return randomRotation * baseDirection;
	}

	public void SwitchWeapon(int newIndex)
	{
		if (newIndex >= 0 && newIndex < availableWeapons.Length && newIndex != currentWeaponIndex)
		{
			availableWeapons[currentWeaponIndex].gameObject.SetActive(false);
			currentWeaponIndex = newIndex;
			availableWeapons[currentWeaponIndex].gameObject.SetActive(true);
			// Implementar animação de swap aqui
		}
	}

	public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
	{
		if (ammoDisplay != null)
		{
			ammoDisplay.text = $"{currentAmmo} / {maxAmmo}";
		}
	}
}