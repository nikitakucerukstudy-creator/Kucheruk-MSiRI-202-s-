using UnityEngine;

public class SnapeToMachine : MonoBehaviour
{
    [Header("Настройки привязки")]
    public Transform snapPoint; // Ссылка на пустой объект (Snap Point) на станке
    public float snapDistance = 0.3f; // Дистанция (в метрах), на которой срабатывает привязка
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

        // Если есть Rigidbody, сбрасываем скорости и перемещаем через физику
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;
    rb.Sleep();

            rb.MovePosition(snapPoint.position);
            rb.MoveRotation(snapPoint.rotation);

            rb.isKinematic = true;
            rb.useGravity = true;
        }
        else
        {
            transform.position = snapPoint.position;
            transform.rotation = snapPoint.rotation;
        }

        Debug.Log("Деталь успешно привязана к станку!");
    }
}