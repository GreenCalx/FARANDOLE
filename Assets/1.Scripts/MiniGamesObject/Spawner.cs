using UnityEngine;

public static class GOSpawner
{
    public static T[] SpawnAtRandom<T>(Transform parent, GameObject prefab, Bounds pgBounds, int count, string name = "", Bounds? avoidZone = null) where T : MonoBehaviour
    {
        Vector2 size = getSizeOfPrefab(prefab);
        Vector2 maxBound = new Vector2(pgBounds.max.x - size.x, pgBounds.max.y - size.y);
        Vector2 minBound = new Vector2(pgBounds.min.x + size.x, pgBounds.min.x + size.y);
        Vector2 pos;
        if (name == "")
        {
            name = "spawned";
        }
        T[] GOs = new T[count];
        
        for (int i = 0; i < count; i++)
            {
                do
                {
                    pos = new Vector2(Random.Range(maxBound.x, minBound.y), Random.Range(minBound.y, maxBound.y));
                } while (avoidZone != null && avoidZone.Value.Contains(pos));
                GOs[i] = GOBuilder.Create(prefab)
                .WithLocalPosition(new Vector2(Random.Range(maxBound.x, minBound.y), Random.Range(minBound.y, maxBound.y)))
                .WithParent(parent)
                .WithName(name + i)
                .BuildAs<T>();
            }
        return GOs;
    }

    public static T[] SpawnInGrid<T>(Transform parent, GameObject prefab, Bounds pgBounds, int col, int row, string name = "") where T : MonoBehaviour
    {
        Vector2 size = getSizeOfPrefab(prefab);
        Vector2 maxBound = new Vector2(pgBounds.max.x - size.x, pgBounds.max.y - size.y);
        Vector2 minBound = new Vector2(pgBounds.min.x + size.x, pgBounds.min.x + size.y);
        if (name == "") {
            name = "spawned";
        }
        T[] GOs = new T[col * row];

        float delta_x = maxBound.x - minBound.x;
        float delta_y = maxBound.y - minBound.y;
        for (int j = 0; j < row; j++)
        {
            for (int i = 0; j < col; j++)
            {
                
                GOs[j * row + i] = GOBuilder.Create(prefab)
                    .WithParent(parent)
                    .WithLocalPosition(new Vector2(pgBounds.min.x + (delta_x * (i + 1)), pgBounds.max.y - (delta_y * (j + 1))))
                    .WithName(name + i + "" + j)
                    .BuildAs<T>();
            }
        }
        return GOs;
    }

    public static T[] SpawnInCircle<T>(Transform parent, GameObject prefab, Bounds pgBounds, int count, float distFromCenter = 0, string name = "") where T : MonoBehaviour
    {
        Vector2 size = getSizeOfPrefab(prefab);
        Vector2 maxBound = new Vector2(pgBounds.max.x - size.x, pgBounds.max.y - size.y);
        Vector2 minBound = new Vector2(pgBounds.min.x + size.x, pgBounds.min.x + size.y);

        if (distFromCenter == 0)
        {
            distFromCenter = Mathf.Min((maxBound.x - minBound.x) / 2, maxBound.y - minBound.y) / 2;    
        }

        float stepAngle = Mathf.PI * 2f / count;
        float angle;

        if (name == "")
        {
            name = "spawned";
        }
        T[] GOs = new T[count];
        for (int i = 0; i < count; i++)
        {
            angle = i * stepAngle;
            GOs[i] = GOBuilder.Create(prefab)
                            .WithName("card" + i)
                            .WithParent(parent)
                            .WithLocalPosition(parent.position + distFromCenter * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)))
                            .BuildAs<T>();
        }
        return GOs;
    }

    private static Vector2 getSizeOfPrefab(GameObject prefab)
    {
        GameObject temp = GameObject.Instantiate(prefab);
        Bounds bounds;
        Renderer[] renderers = temp.GetComponentsInChildren<Renderer>();
        GameObject.Destroy(temp);
        if (renderers.Length == 0)
        {
            bounds = new Bounds(temp.transform.position, Vector3.zero);
            return new Vector2(bounds.size.x, bounds.size.y);
        }

        bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }
        return new Vector2(bounds.size.x, bounds.size.z);
    }
}
