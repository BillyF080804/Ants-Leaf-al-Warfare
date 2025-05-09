using UnityEngine;
using UnityEngine.Pool;

namespace WeaponSystem {
    public class BulletSpawner : MonoBehaviour {
        public ObjectPool<BulletScript> bulletPool;
        private GunScript gunScript;

        private void Start() {
            gunScript = GetComponent<GunScript>();

            int defaultCapacity = 500;

            if (gunScript.gunObject.useAmmoLimit == true) {
                defaultCapacity = Mathf.CeilToInt(gunScript.gunObject.maxClipSize * 1.5f); //Set default capacity
            }

            bulletPool = new ObjectPool<BulletScript>(CreateNewBullet, GetBullet, DisableBullet, DestroyBullet, true, defaultCapacity, defaultCapacity * 2); //Create bullet pool
        }

        //Add new object to bullet pool
        private BulletScript CreateNewBullet() {
            BulletScript bulletScript = Instantiate(gunScript.GetBulletPrefab().GetComponent<BulletScript>(), gunScript.GetReticleTransform().position, Quaternion.identity);
            bulletScript.SetPool(bulletPool);

            return bulletScript;
        }

        //Get bullet from pool
        private void GetBullet(BulletScript bulletScript) {
            bulletScript.transform.position = gunScript.GetReticleTransform().position; //Set bullets starting position
            bulletScript.gameObject.SetActive(true); //Enable bullet
        }

        //Return bullet to pool
        private void DisableBullet(BulletScript bulletScript) {
            bulletScript.gameObject.SetActive(false);
        }

        //Destory bullet
        private void DestroyBullet(BulletScript bulletScript) {
            Destroy(bulletScript.gameObject);
        }
    }
}