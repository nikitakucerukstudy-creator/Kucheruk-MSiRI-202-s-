using UnityEngine;

public class WorkpieceSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject workpieceScript; // Префаб заготовки
    public Transform spawnPoint;       // Обязательная точка спавна (пустой объект на сцене)

    void Update()
    {
        // Спавн по клавише F (если нужно)
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnNewWorkpiece();
        }
    }

    public void SpawnNewWorkpiece()
    {
        if (workpieceScript == null)
        {
            Debug.LogWarning("Не назначен префаб заготовки!");
            return;
        }

        // Если точка спавна не задана, создаем объект прямо на позиции этого скрипта (а не камеры!)
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        // Создаем новый кубик
        Instantiate(workpieceScript, spawnPos, spawnRot);
        Debug.Log("Заготовка успешно спавнена через UI!");
    }
}