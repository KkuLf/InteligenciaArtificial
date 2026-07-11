using UnityEngine;

// Simple straight-line projectile spawned procedurally by EnemyModel.Attack() -
// no prefab asset needed. Flies forward, ends the game on hitting the Player,
// and cleans itself up either way.
public class Projectile : MonoBehaviour
{
    Vector3 _dir;
    float _speed;

    public void Init(Vector3 dir, float speed, float lifeTime)
    {
        _dir = dir;
        _speed = speed;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += _dir * _speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameOver.Trigger();
        }
        Destroy(gameObject);
    }
}
