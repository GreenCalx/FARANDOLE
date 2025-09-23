using UnityEngine;

public class RandomizeSpriteOnAwake : MonoBehaviour
{

    public Sprite[] sprites;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

}
