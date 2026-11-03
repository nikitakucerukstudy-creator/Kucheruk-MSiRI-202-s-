using UnityEngine;

public class Second : MonoBehaviour
{
    public tutorial_1 t_1;
    void Start()
    {
      print ("Сообщение от Second");
      print (t_1.GetInfo());  
    }
    void Update()
    {

    }
}
