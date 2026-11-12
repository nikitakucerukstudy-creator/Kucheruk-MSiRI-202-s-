using System.Collections;
using UnityEngine;
using TMPro; // Обязательно для TextMeshPro

public class SpindleController : MonoBehaviour
{
    [Header("Settings")]
    public float plungeDepth = 0.05f; 
    public float speed = 2f;           
    public float waitAtBottom = 0.3f;  
    public float waitAtTop = 0.5f;     

    [Header("UI Status Display")]
    public TextMeshProUGUI statusText; // Ссылка на текст статуса на панели

    private Vector3 startLocalPos;
    private Coroutine cycleCoroutine;
    private bool isRunning = false;

    void Start()
    {
        startLocalPos = transform.localPosition;
        UpdateStatusUI(); // Устанавливаем начальный текст при запуске
    }

    public void StartCycling()
    {
        if (!isRunning)
        {
            isRunning = true;
            cycleCoroutine = StartCoroutine(CycleRoutine());
            UpdateStatusUI();
        }
    }

    public void StopCycling()
    {
        if (isRunning)
        {
            isRunning = false;
            if (cycleCoroutine != null)
            {
                StopCoroutine(cycleCoroutine);
            }
            StartCoroutine(MoveToPosition(startLocalPos));
            UpdateStatusUI();
        }
    }

    void UpdateStatusUI()
    {
        if (statusText != null)
        {
            if (isRunning)
            {
                statusText.text = "Статус: Работает";
                statusText.color = Color.green; // Зеленый цвет при работе
            }
            else
            {
                statusText.text = "Статус: Остановлен";
                statusText.color = Color.red; // Красный цвет при остановке
            }
        }
    }

    IEnumerator CycleRoutine()
    {
        Vector3 targetPos = startLocalPos - new Vector3(0, plungeDepth, 0);

        while (isRunning)
        {
            yield return StartCoroutine(MoveToPosition(targetPos));
            yield return new WaitForSeconds(waitAtBottom);
            yield return StartCoroutine(MoveToPosition(startLocalPos));
            yield return new WaitForSeconds(waitAtTop);
        }
    }

    IEnumerator MoveToPosition(Vector3 target)
    {
        float elapsed = 0f;
        float duration = 1f / speed;
        Vector3 initialPos = transform.localPosition;

        while (elapsed < duration)
        {
            if (!isRunning && target == startLocalPos) yield break;

            transform.localPosition = Vector3.Lerp(initialPos, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = target;
    }
}