using UnityEngine;
using UnityEngine.UI;

public class ButtonHoldWithStages : MonoBehaviour
{
    public Sprite[] stageSprites; // Массив спрайтов для 5 стадий.
    public float totalHoldTime = 5f; // Общее время для достижения последней стадии.
    public Image buttonImage;

    public float currentHoldTime = 0f; // Текущее время удержания.
    public int currentStage = 0; // Текущая стадия.


    public PlayerMovement PM;

    void Start()
    {
        buttonImage = GetComponent<Image>();

        // Устанавливаем начальный спрайт.
        if (stageSprites.Length > 0)
        {
            buttonImage.sprite = stageSprites[0];
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E) && PM.ExpScore >= 1)
        {
            currentHoldTime += Time.deltaTime;
            UpdateStage();
        }
        else
        {
            ResetHold(); // Сбрасываем состояние кнопки.
        }
    }

    private void UpdateStage()
    {
        // Рассчитываем текущую стадию на основе времени удержания, с зацикливанием.
        int newStage = (int)(currentHoldTime / (totalHoldTime / stageSprites.Length)) % stageSprites.Length;

        if (newStage != currentStage)
        {
            currentStage = newStage;
            buttonImage.sprite = stageSprites[currentStage]; // Обновляем текущий спрайт.
        }
    }

    private void ResetHold()
    {
        currentHoldTime = 0f; // Сбрасываем время удержания.
        currentStage = 0; // Возвращаемся к начальному этапу.

        if (stageSprites.Length > 0)
        {
            buttonImage.sprite = stageSprites[0]; // Возвращаем первый спрайт.
        }
    }
}