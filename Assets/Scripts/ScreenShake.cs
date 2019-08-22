using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public GameObject player;
    public Camera cam;

    private Transform cameraLocation;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.2f;
    private float dampingSpeed = 1.0f;
    Vector3 initialPosition;

    void Start()
    {
        
    }

    private void Awake()
    {
        if (cameraLocation == null)
        {
            cameraLocation = cam.GetComponent(typeof(Transform)) as Transform;
        }
    }

    private void OnEnable()
    {
        initialPosition = cam.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (shakeDuration > 0)
        {
            cam.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            cam.transform.localPosition = initialPosition;
        }
        if (player.GetComponent<Rigidbody2D>().velocity.y <= -5.0f)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                player.GetComponent<PlayerController>().fallMod = 30;
                shakeDuration = 0.5f;
            }
            player.GetComponent<PlayerController>().fallMod = 2;
        }
    }
}
