using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretTile : MonoBehaviour
{
    [SerializeField]
    private Material secretTileMat;

    [SerializeField]
    private GameObject secretTileLight;

    public void SetSecretTileLight(GameObject secretLight)
    {
        secretTileLight = secretLight;
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.name + " has collided with " + other.gameObject.name);
        if (other.transform.tag == "Player")
        {
            Debug.Log("Player on secret tile.");
            GameController.instance.SecretTileFound();
            this.GetComponent<Renderer>().material = secretTileMat;
            secretTileLight.GetComponent<Light>().color = new Color32(254, 9, 0, 1);

        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(gameObject.name + " has collided with " + collision.gameObject.name);
        if (collision.transform.tag == "Player")
           { 
            Debug.Log("Player on secret tile.");
            GameController.instance.SecretTileFound();
            this.GetComponent<Renderer>().material = secretTileMat;
        }
    }
}
