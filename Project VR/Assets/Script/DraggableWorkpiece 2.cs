using UnityEngine;
using Valve.VR.InteractionSystem; // Подключаем пространство имен SteamVR

public class DraggableWorkpiece : MonoBehaviour
{
    [Header("Settings")]
    public float attachDistance = 1.5f; // Радиус примагничивания

    private Transform snapPoint; // Точка на столе, куда деталь должна примагнититься
    private Rigidbody rb;
    private Interactable interactable; // Компонент взаимодействия SteamVR

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();

        // Ищем точку примагничивания на сцене
        FindSnapPoint();
    }

    void FindSnapPoint()
    {
        GameObject snapObj = GameObject.Find("TableSnapPoint");
        if (snapObj != null)
        {
            snapPoint = snapObj.transform;
        }
        else
        {
            GameObject tableObj = GameObject.Find("Table");
            if (tableObj != null)
            {
                snapPoint = tableObj.transform;
                Debug.LogWarning("Точка 'TableSnapPoint' не найдена, используется центр объекта 'Table'. Рекомендуется создать пустой объект-точку.");
            }
            else
            {
                Debug.LogWarning("Не удалось найти ни 'TableSnapPoint', ни 'Table' на сцене!");
            }
        }
    }

    // Эти методы автоматически вызываются SteamVR, когда игрок берет или отпускает предмет
    private void OnHandHoverBegin(Hand hand) { }
    private void OnHandHoverEnd(Hand hand) { }

    private void HandAttachedUpdate(Hand hand)
    {
        // Пока объект в руке, можно отслеживать действия (если нужно)
    }

    private void OnAttachedToHand(Hand hand)
    {
        // Когда игрок взял предмет в VR
        if (rb != null)
        {
            rb.isKinematic = false; // Включаем физику во время переноса (или оставляем по настройкам Throwable)
        }
    }

    private void OnDetachedFromHand(Hand hand)
    {
        // Когда игрок отпустил предмет в VR
        CheckSnapToTable();
    }

    void CheckSnapToTable()
    {
        if (snapPoint == null) return;

        // Проверяем расстояние от объекта до точки на столе
        float distance = Vector3.Distance(transform.position, snapPoint.position);

        if (distance < attachDistance)
        {
            // Если деталь достаточно близко к точке на столе — примагничиваем
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
            
            // Если нужно, чтобы она привязалась к столу и замерла:
            // transform.SetParent(snapPoint);

            if (rb != null)
            {
                rb.isKinematic = true; // Замораживаем физику, чтобы деталь не падала со стола
            }

            Debug.Log("Заготовка успешно закреплена на столе!");
        }
    }
}