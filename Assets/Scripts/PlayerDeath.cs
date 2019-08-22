using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public GameObject player;
    public string scenePath;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnBecameInvisible()
    {
        //Death by falling/out of bounds.
        Debug.Log("Fallen.");
        SceneManager.LoadScene(scenePath);
    }
}
