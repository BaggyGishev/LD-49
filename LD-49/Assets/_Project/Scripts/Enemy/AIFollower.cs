using UnityEngine;
using Random = UnityEngine.Random;

namespace Gisha.LD49.Enemy
{
    public class AIFollower : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float maxSpeed = 240;
        [SerializeField] private float minSpeed = 100;
        [SerializeField] private float rotationSpeed;

        [Header("Raycast")] [SerializeField] private float raycastRadius = 2f;
        [SerializeField] private float raycastDistance = 3f;

        private float _speed;
        private bool _isFollowing = false;
        private Transform _target;

        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _speed = Random.Range(minSpeed, maxSpeed);
        }

        private void Update()
        {
            if (_isFollowing || _target == null)
                return;

            if (CheckForPlayerInFront())
                _isFollowing = true;
        }

        private void FixedUpdate()
        {
            if (!_isFollowing || _target == null)
            {
                _isFollowing = false;
                return;
            }

            Vector2 dir = (_target.position - transform.position).normalized;
            Quaternion newRotation = GetRotationFromDirection(dir);
            _rb.velocity = transform.up * _speed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }

        private bool CheckForPlayerInFront()
        {
            var hits = Physics2D.CircleCastAll(transform.position, raycastRadius, transform.up, raycastDistance);

            if (hits.Length > 0)
                foreach (var hitInfo in hits)
                    if (hitInfo.collider.CompareTag("Player"))
                        return true;
            return false;
        }

        private Quaternion GetRotationFromDirection(Vector2 direction)
        {
            float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, 0, rotZ - 90);
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.collider.CompareTag("Player") && !_isFollowing)
                _isFollowing = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, raycastRadius);
            Gizmos.DrawRay(transform.position, transform.up * raycastDistance);
        }
    }
}