using UnityEngine;

namespace WeaponSystem {
    public class SimpleHealthScript : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private int health;
        [SerializeField] private AudioSource audioSource;

        //Remove health
        public void RemoveHealth(int damage) {
            health -= damage;

            if (health <= 0) {
                OnDeath();
            }
        }

        //Destroy object
        private void OnDeath() {
            if (audioSource != null) {
                audioSource.Play();

                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;

                FindFirstObjectByType<EnemySpawner>().RemoveEnemy(gameObject);
                Destroy(gameObject, audioSource.clip.length);
            }
            else {
                FindFirstObjectByType<EnemySpawner>().RemoveEnemy(gameObject);
                Destroy(gameObject);
            }
        }
    }
}