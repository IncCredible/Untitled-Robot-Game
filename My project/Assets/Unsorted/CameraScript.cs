using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    public float layer;

    void Update()
    {
        transform.position = new Vector3(player.transform.position.x,
            player.transform.position.y, layer);
    }
}
