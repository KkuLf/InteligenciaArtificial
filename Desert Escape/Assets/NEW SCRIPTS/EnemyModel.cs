using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModel : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float fireCooldown = 1.5f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float projectileLifeTime = 3f;
    [SerializeField] private float muzzleHeightOffset = 0f; // fine-tune on top of the collider's real center
    [SerializeField] private float aimHeightOffset = 0f;
    Rigidbody _rb;
    Collider _collider;
    bool _canFire = true;

    public Node[] waypoints; // Array to hold the patrol waypoints
    public Node currentWayPoint;
    public int currentWaypointIndex = 0; // Index of the current waypoint
    public int index;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    // Real world-space center of the body, regardless of where the transform's own
    // pivot sits (head, feet, center - varies per imported model/rig).
    Vector3 BodyCenter => _collider != null ? _collider.bounds.center : transform.position;

    public void Move(Vector3 dir)
    {
        dir *= speed;
        dir.y = _rb.velocity.y;
        _rb.velocity = dir;
    }

    public void LookDir(Vector3 dir)
    {
        // Keep the enemy upright: only yaw to face the horizontal direction, don't let
        // vertical steering (obstacle avoidance, prediction) pitch or roll the body.
        // Pinning "up" to world-up via Quaternion.LookRotation corrects any existing
        // roll too, not just forward - a plain transform.forward assignment wouldn't.
        dir.y = 0;
        if (dir.x == 0 && dir.z == 0) return;
        transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Attack(Vector3 targetPosition)
    {
        if (!_canFire) return;
        StartCoroutine(FireAndCooldown(targetPosition));
    }

    IEnumerator FireAndCooldown(Vector3 targetPosition)
    {
        _canFire = false;
        FireProjectile(targetPosition);
        yield return new WaitForSeconds(fireCooldown);
        _canFire = true;
    }

    void FireProjectile(Vector3 targetPosition)
    {
        // Aim from the enemy's real body center (from its collider bounds, not its
        // possibly-offset transform pivot) toward the target - direction is computed
        // AFTER the spawn offset so it always points at the actual aim point.
        Vector3 spawnPos = BodyCenter + Vector3.up * muzzleHeightOffset;
        Vector3 aimPoint = targetPosition + Vector3.up * aimHeightOffset;
        Vector3 dir = (aimPoint - spawnPos).normalized;

        var projectileGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectileGO.name = "EnemyProjectile";
        projectileGO.transform.position = spawnPos + dir * 0.5f;
        projectileGO.transform.localScale = Vector3.one * 0.3f;

        var col = projectileGO.GetComponent<SphereCollider>();
        col.isTrigger = true;

        var rb = projectileGO.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        var projectile = projectileGO.AddComponent<Projectile>();
        projectile.Init(dir, projectileSpeed, projectileLifeTime);
    }

    // Visualize where shots actually spawn from, so muzzleHeightOffset can be
    // calibrated against the model's real collider bounds instead of guessing blindly.
    private void OnDrawGizmosSelected()
    {
        var col = _collider != null ? _collider : GetComponent<Collider>();
        Vector3 center = col != null ? col.bounds.center : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center + Vector3.up * muzzleHeightOffset, 0.15f);
    }

}
