using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bucket_MiniGame : MiniGame
{
    public int n_catchables = 3;
    public GameObject prefab_ObjectsToCatch;
    public Bucket prefab_bucket;

    List<Catchable> inst_ObjectsToCatch;
    Bucket inst_Bucket;
    public override void Init()
    {

        // init mini game stuff
        inst_ObjectsToCatch = new List<Catchable>(n_catchables);
        for (int i = 0; i < n_catchables; i++)
        {
            GameObject newGO = Instantiate(prefab_ObjectsToCatch);
            Catchable as_c = newGO.GetComponent<Catchable>();
            inst_ObjectsToCatch.Add(as_c);
            inst_ObjectsToCatch[i] = as_c;

            Vector2 randPos = new Vector2(
                UnityEngine.Random.Range(PG.bounds.min.x, PG.bounds.max.x),
                UnityEngine.Random.Range(PG.bounds.min.y, PG.bounds.max.y)
            );
            as_c.transform.position = randPos;
            as_c.GetComponent<Rigidbody>();
        }

        inst_Bucket = Instantiate(prefab_bucket);
        PC.AddPositionTracker(inst_Bucket);
    }
    public override void Play()
    {

    }
    public override void Stop()
    {

    }
    public override void Win()
    {

    }
    public override void Lose()
    {
        
    }
    public override bool SuccessCheck()
    {
        return false;
    }
}
