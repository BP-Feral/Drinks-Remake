using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector]
    public Transform parentAfterDrag;
    private Vector3 dragOffset;

    private LevelManager levelManager;
    private AudioManager audioManager;

    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        audioManager = FindObjectOfType<AudioManager>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Begin drag");

        dragOffset = transform.position - Input.mousePosition;

        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        if (audioManager != null)
        {
            audioManager.PlaySelectSound();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition + dragOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        
        if (audioManager != null)
        {
            audioManager.PlayDropSound();
        }

        if (levelManager != null)
        {
            levelManager.CheckForMatches();
        }
        else
        {
            Debug.LogWarning("LevelManager not found!");
        }
    }
}
