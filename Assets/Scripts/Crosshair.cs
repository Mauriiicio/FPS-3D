using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public RectTransform[] crosshairParts; // Array to hold the four parts of the crosshair
    public float spread = 10f; // Spread distance for the crosshair parts
	public float spreadSpeed = 5f; // Speed at which the crosshair spreads

    float currentSpread = 0f; // Current spread value
	Vector2[] originalPositions; // Array to hold the original positions of the crosshair parts

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
        originalPositions = new Vector2[crosshairParts.Length];
		for (int i = 0; i < crosshairParts.Length; i++)
        {
            originalPositions[i] = crosshairParts[i].anchoredPosition;
		}
	}

    // Update is called once per frame
    void Update()
    {
		currentSpread = Mathf.Lerp(currentSpread, 0f, Time.deltaTime * spreadSpeed);
		UpdateCrosshairPosition();
	}

	private void UpdateCrosshairPosition()
	{
		// Calcula o deslocamento atual
		float totalMovement = currentSpread * spread;

		// Move a barra superior
		crosshairParts[0].anchoredPosition = originalPositions[0] + new Vector2(0, totalMovement);

		// Move a barra inferior
		crosshairParts[1].anchoredPosition = originalPositions[1] + new Vector2(0, -totalMovement);

		// Move a barra direita
		crosshairParts[2].anchoredPosition = originalPositions[2] + new Vector2(totalMovement, 0);

		// Move a barra esquerda
		crosshairParts[3].anchoredPosition = originalPositions[3] + new Vector2(-totalMovement, 0);
	}

	public void ApplySpread()
	{
		float spreadIncrease = 1;

		// Garante que o spread não ultrapasse o limite (ex: 1.0f ou 100%)
		currentSpread = Mathf.Clamp01(currentSpread + spreadIncrease);
	}
}
