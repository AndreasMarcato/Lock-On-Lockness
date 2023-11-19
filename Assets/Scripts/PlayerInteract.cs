using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
        public LayerMask layerNPC;
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }
    private void Interact()
    {
        Ray ray = new Ray(transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Physics.Raycast(ray, out hit, Mathf.Infinity, layerNPC);
        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.red, 1f);

        if (hit.transform.tag == "NPC")
        {
            hit.transform.gameObject.GetComponent<Interactable>().Interact();
        }
    }
}
