using UnityEngine;
using UnityEngine.Pool;

public class ToxicPuddlePool : MonoBehaviour
{
    [SerializeField]
    private ToxicPuddle toxicPuddlePrefab;

    [SerializeField]
    private int prewarmCount = 9; // roughly your typical max simultaneous puddles

    private ObjectPool<ToxicPuddle> pool;

    private void Awake()
    {
        pool = new ObjectPool<ToxicPuddle>(
            createFunc: CreatePuddle,
            actionOnGet: OnGetFromPool,
            actionOnRelease: OnReleaseToPool,
            actionOnDestroy: OnDestroyPuddle,
            collectionCheck: false,
            defaultCapacity: prewarmCount,
            maxSize: 64
        );

        Prewarm();
    }

    private void Prewarm()
    {
        // Pre-creates instances (and forces their materials/shaders to warm up)
        // during a load screen or scene start, instead of mid-combat.
        var warm = new ToxicPuddle[prewarmCount];
        for (int i = 0; i < prewarmCount; i++)
            warm[i] = pool.Get();
        for (int i = 0; i < prewarmCount; i++)
            pool.Release(warm[i]);
    }

    public ToxicPuddle Spawn(Vector3 worldPosition, Quaternion rotation)
    {
        ToxicPuddle puddle = pool.Get();
        puddle.transform.SetPositionAndRotation(worldPosition, rotation);
        return puddle;
    }

    private ToxicPuddle CreatePuddle()
    {
        ToxicPuddle puddle = Instantiate(toxicPuddlePrefab);
        puddle.SetPoolReleaseCallback(ReturnToPool);
        return puddle;
    }

    private void ReturnToPool(ToxicPuddle puddle) => pool.Release(puddle);

    private void OnGetFromPool(ToxicPuddle puddle) => puddle.gameObject.SetActive(true);

    private void OnReleaseToPool(ToxicPuddle puddle) => puddle.gameObject.SetActive(false);

    private void OnDestroyPuddle(ToxicPuddle puddle)
    {
        if (puddle != null)
            Destroy(puddle.gameObject);
    }
}
