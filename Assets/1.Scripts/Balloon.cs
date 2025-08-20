using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Balloon : ObjectTarget
{
    public float initPushStr = 2f;
    public Rigidbody2D self_attachedPhysxBody;
    public Animator animator;
    public SpriteRenderer face;
    public SpriteRenderer body;
    public List<Sprite> availableFaces;

    public Color colorRangeMin, colorRangeMax;

    void Start()
    {
        if ((availableFaces != null) && (availableFaces.Count > 0))
        {
            face.sprite = availableFaces[UnityEngine.Random.Range(0, availableFaces.Count)];
        }
        Color c = Color.Lerp(colorRangeMin, colorRangeMax, UnityEngine.Random.Range(0f, 1f));
        face.color = c;
        body.color = c;
        if (self_attachedPhysxBody == null)
        {
            Debug.LogError("Missing rigidbody2d on " + gameObject.name);
        }
    }

    public void InitPhysxPosition(Vector3 iPos)
    {
        self_attachedPhysxBody.transform.position = iPos;
        UpdatePosition();
        Vector3 rand = Random.insideUnitSphere;
        Vector2 dir = new Vector2(rand.x, rand.y);
        self_attachedPhysxBody.AddForce(dir * initPushStr);
    }

    public void InitSortOrder(LayerManager2D iLM2D)
    {
        // face.transform.position = new Vector3(face.transform.position.x, face.transform.position.y, zDepth);
        // body.transform.position = new Vector3(body.transform.position.x, body.transform.position.y, zDepth + zDepth/2f);
        //body.sortingOrder = iSortOrder;
        //face.sortingOrder = iSortOrder+1;
        iLM2D.PlaceObject(body);
        iLM2D.PlaceObject(face);
    }

    public void ExplodeAnim()
    {
        face.enabled = false;
        animator.SetTrigger("Explode");
    }

    void UpdatePosition()
    {
        transform.position = new Vector3(
            self_attachedPhysxBody.transform.position.x,
            self_attachedPhysxBody.transform.position.y,
            0f);
    }

    void Update()
    {
        UpdatePosition();
    }
}
