using System;
using System.Collections;
using System.Globalization;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class MachineClient : MonoBehaviour
{
    [Header("Network Settings")]
    public string getUrl = "https://cnc.kovalev.team/get/3";
    public string postUrl = "https://cnc.kovalev.team/post";
    public int channel = 1;
    public float interval = 1f;

    [Header("UI References (TextMeshPro)")]
    public TextMeshProUGUI stateText;   // Верхняя строка (Состояние: Работает/Остановлен)
    public TextMeshProUGUI axesText;    // Средняя строка (Координаты X, Y, Z)
    public TextMeshProUGUI messageText; // Нижняя строка (Сообщения и ошибки)

    [Header("Machine 3D Nodes")]
    public Transform nodeX;
    public Transform nodeY;
    public Transform nodeZ;
    
    private Vector3 startX;
    private Vector3 startY;
    private Vector3 startZ;

    private IEnumerator Start()
    {
        if (nodeX == null || nodeY == null || nodeZ == null)
        {
            Debug.LogError("Назначьте три узла станка (nodeX, nodeY, nodeZ) в Inspector.");
        }
        else
        {
            startX = nodeX.localPosition;
            startY = nodeY.localPosition;
            startZ = nodeZ.localPosition;
        }

        // Цикл опроса сервера
        while (true)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(getUrl))
            {
                request.timeout = 2;
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {
                        ProcessData(request.downloadHandler.text);
                        ShowMessage(""); // Очищаем ошибки при успехе
                    }
                    catch (Exception error)
                    {
                        Debug.LogWarning("Ошибка чтения JSON: " + error.Message);
                        ShowMessage("Ошибка формата данных");
                    }
                }
                else
                {
                    Debug.LogWarning("Ошибка запроса: " + request.error);
                    ShowMessage("Нет связи со станком");
                }
            }

            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, interval));
        }
    }

    private void ProcessData(string json)
    {
        JObject data = JObject.Parse(json);
        float x = GetCoordinate(data, "X");
        float y = GetCoordinate(data, "Y");
        float z = GetCoordinate(data, "Z");

        // Движение 3D моделей в сцене (если узлы назначены)
        if (nodeX != null && nodeY != null && nodeZ != null)
        {
            nodeX.localPosition = startX + Vector3.right * (x * 0.001f);
            nodeY.localPosition = startY + Vector3.forward * (y * 0.001f);
            nodeZ.localPosition = startZ + Vector3.up * (z * 0.001f);
        }

        // Вывод координат на панель
        if (axesText != null)
        {
            axesText.text = $"X: {x:F1} мм\nY: {y:F1} мм\nZ: {z:F1} мм";
        }
    }

    private float GetCoordinate(JObject data, string axis)
    {
        foreach (JToken section in data["data"])
        {
            if ((string)section[0] != "Оси Канала " + channel)
                continue;

            foreach (JToken row in section[1])
            {
                if ((string)row[0] == "Перемещение по оси " + axis)
                {
                    string text = ((string)row[1]).Replace("мм", "").Trim();
                    text = text.Replace(',', '.');
                    float value = float.Parse(text, CultureInfo.InvariantCulture);

                    if (float.IsNaN(value) || float.IsInfinity(value))
                        throw new FormatException("Некорректная координата " + axis);

                    return value;
                }
            }
        }
        throw new FormatException("Не найдена координата " + axis);
    }

    // --- МЕТОДЫ ДЛЯ КНОПОК (Пуск / Стоп) ---

    public void StartMachine()
    {
        StartCoroutine(SendCommand("start"));
        if (stateText != null) stateText.text = "Состояние: Запущено";
        Debug.Log("Нажата кнопка: Пуск");
    }

    // Перегрузка для совместимости со SteamVR UIElement
    public void StartMachine(Valve.VR.InteractionSystem.Hand hand)
    {
        StartMachine();
    }

    public void StopMachine()
    {
        StartCoroutine(SendCommand("stop"));
        if (stateText != null) stateText.text = "Состояние: Остановлено";
        Debug.Log("Нажата кнопка: Стоп");
    }

    // Перегрузка для совместимости со SteamVR UIElement
    public void StopMachine(Valve.VR.InteractionSystem.Hand hand)
    {
        StopMachine();
    }

    private IEnumerator SendCommand(string command)
    {
        ShowMessage("Отправка команды...");

        WWWForm form = new WWWForm();
        form.AddField("command", command);

        using (UnityWebRequest req = UnityWebRequest.Post(postUrl, form))
        {
            req.timeout = 2;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                ShowMessage("Ошибка команды: " + req.error);
                yield break;
            }

            ShowMessage("Команда принята");
        }
    }

    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }
    }
}