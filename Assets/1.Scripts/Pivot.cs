using UnityEngine;

public class Pivot : MonoBehaviour
{
    public float rotSpeed = 0f;


    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0f, 0f, rotSpeed * Time.deltaTime);

        foreach (Transform child in transform)
        {
            Vector3 euler = child.rotation.eulerAngles;
            child.rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }
}
