using UnityEngine;

public class MouseDragObject : MonoBehaviour
{
    private Camera cam;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody rb;
    private bool isDragging = false;

    void Start()
    {
        // Находим главную камеру на сцене
        cam = FindFirstObjectByType<Camera>();
        
        // Получаем компонент Rigidbody, если он есть
        rb = GetComponent<Rigidbody>();
    }

    void OnMouseDown()
    {
        if (cam == null) return;

        isDragging = true;

        // Если у объекта есть физика (Rigidbody), отключаем её на время переноса,
        // чтобы объект не падал, не улетал и не сопротивлялся мышке.
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Запоминаем глубину (Z) объекта относительно камеры
        screenPoint = cam.WorldToScreenPoint(transform.position);

        // Вычисляем смещение между позицией мыши и центром объекта,
        // чтобы предмет не "прыгал" центром к курсору при клике.
        offset = transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!isDragging || cam == null) return;

        // Получаем текущие координаты мыши на экране и переводим их в мир 3D
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint) + offset;

        // Двигаем объект за мышкой
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;

        // Когда отпустили мышь, включаем физику обратно (если она нужна)
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}