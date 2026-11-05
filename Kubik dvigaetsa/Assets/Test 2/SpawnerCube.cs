using UnityEngine;

public class SpawnerCube : MonoBehaviour
{
    
    private string[] shapeNames = { "Кубик_0", "Кубик_1","Кубик_2","Кубик_3" };

    void Start()
    {
       for (int i = 0; i < shapeNames.Length; i++) 
   {
     GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
     cube.name = shapeNames[i];

     Rigidbody rb = cube.AddComponent<Rigidbody>();
     rb.useGravity = false;

     cube.AddComponent<Collision>();

     float spacing = 3f; 
     cube.transform.position = new Vector3 (i * spacing, 0, 0);

     cube.GetComponent<Renderer>().material.color = Random.ColorHSV();
 
   }
   
    }
}
