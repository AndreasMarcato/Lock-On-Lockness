using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;
    public float maxSelectDistance = 25f;
    Transform player;
    bool isInteracting = false;

    [Range(0f, 2f)]
    public int currentSelection;

    public GameObject[] panelToShow = new GameObject[3];

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        foreach (GameObject go in panelToShow) { go.SetActive(false); }
    }

    public virtual void Interact()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
      
        if (distance > maxSelectDistance)
        {
            foreach (GameObject go in panelToShow) { go.SetActive(false); }
        }

        if (isInteracting)
        {
            return;
        }

        if (distance <= maxSelectDistance && !isInteracting)
        {
            isInteracting = true;
            if(currentSelection == 0)
            {
                panelToShow[1].SetActive(true); 
            }
            else if(currentSelection == 1)
            {
                panelToShow[2].SetActive(true);
            }
            else if (currentSelection == 2)
            {
                panelToShow[3].SetActive(true);
            }
        }
        
        StartCoroutine(HidePanels());

        
    }

    IEnumerator HidePanels()
    {
        yield return new WaitForSeconds(5f);
        foreach (var panel in panelToShow) { panel.SetActive(false); };
        isInteracting = false;
    }

    [ExecuteAlways]
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxSelectDistance);
    }
}