using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpray : MonoBehaviour
{
    [SerializeField] private int FlameDamage;
    private CameraSystem cameraSystem;
    AntScript antScript;

    // Start is called before the first frame update
    private void Awake()
    {
        cameraSystem = FindFirstObjectByType<CameraSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {

        antScript = other.GetComponent<AntScript>();
        if( antScript != null )
        {
            BurnAnt();
        }
    }

    private void OnDestroy()
    {
        if (cameraSystem.CameraTarget == transform)
        {
            cameraSystem.SetCameraTarget(null);
        }
    }

    private void BurnAnt()
    {
        antScript.TakeDamage(FlameDamage);
        antScript = null;
    }
}
