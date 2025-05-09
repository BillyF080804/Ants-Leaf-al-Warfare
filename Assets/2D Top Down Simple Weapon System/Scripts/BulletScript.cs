using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace WeaponSystem {
    [RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
    public class BulletScript : MonoBehaviour {
        private int damage;
        private float originalRotation;
        private LayerMask layerMask;
        private Vector2 originalVelocity;
        private GunScript gunScript;
        private Rigidbody2D rb;
        private TrailRenderer trailRenderer;
        private ObjectPool<BulletScript> bulletPool;
        private BulletCurrentPenetrationLevel currentPenetrationLevel;

        private enum BulletCurrentPenetrationLevel {
            VeryLow,
            Low,
            Medium,
            High,
            VeryHigh
        }
        
        private void Awake() {
            //Set values for rigidbody
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            gameObject.layer = LayerMask.NameToLayer("Bullet");
            layerMask = LayerMask.GetMask("Bullet");

            //Display error to the user if bullet layerMask is not set up correctly
            if (layerMask.value == 0) {
                Debug.LogError("ERROR: " + GetType().Name + " requires the layer mask 'Bullet'. Please create this.");
            }
            else {
                GetComponent<Collider2D>().excludeLayers = layerMask;
            }

            //Get required components
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            gunScript = FindFirstObjectByType<GunScript>();
        }

        private void OnEnable() {
            //Calculate velocity & rotation
            Vector3 bulletDirection = gunScript.GetReticleTransform().position - gunScript.GetPlayerTransform().position;
            Vector3 bulletRotation = gunScript.GetPlayerTransform().position - gunScript.GetReticleTransform().position;

            //Set velocity
            Vector2 bulletVelocity = new Vector2(bulletDirection.x, bulletDirection.y).normalized * gunScript.gunObject.bulletSettings.bulletSpeed;
            originalVelocity = bulletVelocity;
            rb.velocity = bulletVelocity;

            //Set rotation
            originalRotation = Mathf.Atan2(bulletRotation.y, bulletRotation.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, originalRotation);

            //Enable trailer renderer
            trailRenderer.Clear();
            trailRenderer.enabled = true;

            //Set initial values
            damage = gunScript.gunObject.bulletSettings.bulletDamage;
            currentPenetrationLevel = (BulletCurrentPenetrationLevel)gunScript.gunObject.bulletSettings.bulletPenetrationLevel;

            if (gunScript.gunObject.bulletSettings.explosiveBullets == false) {
                StartCoroutine(RemoveBulletAfterTime(gunScript.gunObject.bulletSettings.bulletLife));
            }
            else {
                ExplodeBullet(gunScript.gunObject.bulletSettings.bulletLife);
            }
        }

        //Handles functionality after collisions
        private void OnCollisionEnter2D(Collision2D collision) {
            if (collision.gameObject.TryGetComponent(out ObjectPenetration objectPenetration)) {
                if (gunScript.gunObject.bulletSettings.bulletPenetration == true && (int)currentPenetrationLevel >= (int)objectPenetration.penetrationLevel) {
                    Physics2D.IgnoreCollision(collision.collider, collision.otherCollider, true);
                    currentPenetrationLevel--;

                    //Play audio
                    if (objectPenetration.useAudio == true && objectPenetration.audioSource != null) {
                        objectPenetration.audioSource.Play();
                    }

                    Vector3 predictedPos = new Vector2(transform.position.x, transform.position.y) + originalVelocity * (Time.deltaTime * 10);
                    predictedPos += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));

                    //Calculate velocity & rotation
                    Vector3 bulletDirection = predictedPos - transform.position;
                    Vector3 bulletRotation = transform.position - predictedPos;

                    //Set velocity
                    Vector2 bulletVelocity = new Vector2(bulletDirection.x, bulletDirection.y).normalized * gunScript.gunObject.bulletSettings.bulletSpeed;
                    originalVelocity = bulletVelocity;
                    rb.velocity = bulletVelocity * 0.95f;

                    //Set rotation
                    originalRotation = Mathf.Atan2(bulletRotation.y, bulletRotation.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, originalRotation);
                }
                else {
                    ObjectCollision(collision.gameObject, collision.contacts[0].point);
                }
            }
            else {
                ObjectCollision(collision.gameObject, collision.contacts[0].point);
            }
        }

        //Called when an object collides with an object
        private void ObjectCollision(GameObject collision, Vector2 contactPosition) {
            if (gunScript.gunObject.bulletSettings.explosiveBullets == true) {
                ExplodeBullet();
            }
            else {
                if (collision.TryGetComponent(out SimpleHealthScript healthScript)) {
                    healthScript.RemoveHealth(damage); //Remove health if needed
                }

                //Create impact particle effect
                if (gunScript.gunObject.bulletSettings.impactParticlePrefab != null && gunScript.gunObject.bulletSettings.explosiveBullets == false) {
                    Instantiate(gunScript.gunObject.bulletSettings.impactParticlePrefab, contactPosition, Quaternion.identity);
                }
            }

            DisableBullet();
        }

        //Explodes the bullet instantly
        private void ExplodeBullet() {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f); //Get all colliders in explosion area

            foreach (Collider2D col in hitColliders) {
                if (col.gameObject.TryGetComponent(out SimpleHealthScript healthScript)) {
                    healthScript.RemoveHealth(damage); //Remove health if needed
                }
            }

            //Create explosion particle effect
            if (gunScript.gunObject.bulletSettings.explosionParticlePrefab != null) {
                PlayerMovementScript playerMovementScript = FindFirstObjectByType<PlayerMovementScript>();

                if (playerMovementScript != null && playerMovementScript.cameraFollowScript != null && playerMovementScript.cameraFollowScript.useCameraShake == true && playerMovementScript.cameraFollowScript.explosionCameraShake == true) {
                    playerMovementScript.cameraFollowScript.StartCameraShake();
                }

                Instantiate(gunScript.gunObject.bulletSettings.explosionParticlePrefab, transform.position, Quaternion.identity);
            }
        }

        //Explodes the bullet after a set delay
        private void ExplodeBullet(float timeDelay) {
            StartCoroutine(ExplodeBulletDelay(timeDelay));
        }

        //Coroutine for handling wait functionality
        private IEnumerator ExplodeBulletDelay(float timeDelay) {
            yield return new WaitForSeconds(timeDelay);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 2); //Get all colliders in explosion area

            foreach (Collider2D col in hitColliders) {
                if (col.gameObject.TryGetComponent(out SimpleHealthScript healthScript)) {
                    healthScript.RemoveHealth(damage); //Remove health if needed
                }
            }

            //Create explosion particle effect
            if (gunScript.gunObject.bulletSettings.explosionParticlePrefab != null) {
                PlayerMovementScript playerMovementScript = FindFirstObjectByType<PlayerMovementScript>();

                if (playerMovementScript != null && playerMovementScript.cameraFollowScript != null && playerMovementScript.cameraFollowScript.useCameraShake == true && playerMovementScript.cameraFollowScript.explosionCameraShake == true) {
                    playerMovementScript.cameraFollowScript.StartCameraShake();
                }

                Instantiate(gunScript.gunObject.bulletSettings.explosionParticlePrefab, transform.position, Quaternion.identity);
            }

            DisableBullet();
        }

        //Remove bullet after set time
        public IEnumerator RemoveBulletAfterTime(float time) {
            yield return new WaitForSeconds(time);
            DisableBullet();
        }

        //Set bullets pool
        public void SetPool(ObjectPool<BulletScript> pool) {
            bulletPool = pool;
        }

        //Disables bullet
        private void DisableBullet() {
            rb.velocity = Vector2.zero;
            trailRenderer.enabled = false;
            bulletPool.Release(this);
        }
    }
}