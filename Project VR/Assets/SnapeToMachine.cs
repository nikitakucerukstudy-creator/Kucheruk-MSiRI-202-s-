using UnityEngine;

public class SnapToMachine : MonoBehaviour
{
    [Header("Настройки привязки")]
    public Transform snapPoint; // Ссылка на пустой объект (Snap Point) на станке
    public float snapDistance = 1.0f; // Дистанция (в метрах), на которой срабатывает привязка
    public float snapAngle = 45f; // Допустимый угол отклонения в градусах

    private bool isSnapped = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Если деталь уже привязана или snapPoint не задан, ничего не проверяем
        if (isSnapped || snapPoint == null) return;

        float distance = Vector3.Distance(transform.position, snapPoint.position);
        float angle = Quaternion.Angle(transform.rotation, snapPoint.rotation);

        if (distance <= snapDistance && angle <= snapAngle)
        {
            SnapObject();
        }
    }

    void SnapObject()
    {
        isSnapped = true;

        // Мгновенно выставляем позицию и поворот ровно в точку привязки
        transform.position = snapPoint.position;
        transform.rotation = snapPoint.rotation;

        // Отключаем физику, чтобы деталь зафиксировалась
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Debug.Log("Деталь успешно привязана к станку!");
    }
}