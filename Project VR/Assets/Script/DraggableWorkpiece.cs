using UnityEngine;

public class DraggableWorkpiece : MonoBehaviour
{
    [Header("Settings")]
    public float attachDistance = 1.5f; // Радиус примагничивания

    private Transform snapPoint; // Точка на столе, куда заготовка должна примагнититься
    private Camera cam;
    private Vector3 screenPoint;
    private Vector3 offset;
    private Rigidbody rb;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) cam = FindObjectOfType<Camera>();
        
        rb = GetComponent<Rigidbody>();

        // Ищем точку примагничивания на столе
        FindSnapPoint();
    }

    void FindSnapPoint()
    {
        // Ищем объект по имени "TableSnapPoint" на сцене
        GameObject snapObj = GameObject.Find("TableSnapPoint"); 
        
        if (snapObj != null)
        {
            snapPoint = snapObj.transform;
        }
        else
        {
            // Запасной вариант: если точки нет, ищем сам стол
            GameObject tableObj = GameObject.Find("Table");
            if (tableObj != null)
            {
                snapPoint = tableObj.transform;
                Debug.LogWarning("Точка 'TableSnapPoint' не найдена, используется центр объекта 'Table'. Рекомендуется создать пустой объект на поверхности стола!");
            }
            else
            {
                Debug.LogWarning("Не удалось найти ни 'TableSnapPoint', ни 'Table' на сцене!");
            }
        }
    }

    void OnMouseDown()
    {
        if (cam == null) return;

        isDragging = true;
        transform.SetParent(null); // Отвязываем от стола при начале перемещения

        if (rb != null)
        {
            rb.isKinematic = true; // Отключаем физику во время перетаскивания
        }

        screenPoint = cam.WorldToScreenPoint(transform.position);
        offset = transform.position - cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        if (!isDragging || cam == null) return;

        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = cam.ScreenToWorldPoint(curScreenPoint) + offset;
        
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (rb != null)
        {
            rb.isKinematic = false; // Включаем физику обратно после отпускания
        }

        if (snapPoint == null)
        {
            FindSnapPoint();
        }

        if (snapPoint != null)
        {
            float distance = Vector3.Distance(transform.position, snapPoint.position);
            
            // Если заготовка достаточно близко к точке на столе — примагничиваем
            if (distance < attachDistance)
            {
                transform.position = snapPoint.position; // Выравниваем точно по точке
                transform.rotation = snapPoint.rotation; // Выравниваем поворот
                transform.SetParent(snapPoint);          // Привязываем к столу
                
                // Снова включаем kinematic, чтобы деталь мертво стояла на столе и не падала из-за физики
                if (rb != null) rb.isKinematic = true;

                Debug.Log("Заготовка успешно закреплена на столе!");
            }
        }
    }
}