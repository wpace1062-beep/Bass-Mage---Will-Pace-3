using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Note : MonoBehaviour
{
    private Vector3 startPosition;
    private bool isDragging = false;

    public int numColumns;
    public int numRows;

    public int gridX = -1;
    public int gridY = -1;

    public GameObject[,] slotsGrid;
    public float snapRange = 1.5f;

    public BassMage bassMage;

    [Header("Settings")]
    public float dragZ = 0f;
    public float placedZ = -5f;

    private Transform currentSlot;

    // Start is called once before the first execution of Update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePos2D = Input.mousePosition;
            mousePos2D.z = 0f;

            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

            Vector3 pos = transform.position;
            pos.x = mousePos3D.x;
            pos.y = mousePos3D.y;
            pos.z = dragZ;

            transform.position = pos;
        }

        // Mouse release
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;

            if (currentSlot != null)
            {
                int slotX = -1;
                int slotY = -1;

                // Find slot coordinates
                for (int x = 0; x < numColumns; x++)
                {
                    for (int y = 0; y < numRows; y++)
                    {
                        if (slotsGrid[x, y].transform == currentSlot)
                        {
                            slotX = x;
                            slotY = y;
                            break;
                        }
                    }
                }

                // Check if slot available
                if (slotX != -1 && bassMage.notesGrid[slotX, slotY] == null)
                {
                    Vector3 snapPos = currentSlot.position;
                    snapPos.z = placedZ;

                    transform.position = snapPos;
                    startPosition = snapPos;

                    gridX = slotX;
                    gridY = slotY;

                    bassMage.notesGrid[slotX, slotY] = this;

                    // Spawn new draggable note
                    bassMage.SpawnNewNote();
                }
                else
                {
                    // Invalid placement
                    transform.position = startPosition;
                }
            }
            else
            {
                transform.position = startPosition;
            }
        }
    }

    void OnMouseDown()
    {
        // If already placed, delete
        if (gridX != -1)
        {
            bassMage.notesGrid[gridX, gridY] = null;
            Destroy(gameObject);
            return;
        }

        isDragging = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.name + " Tag: " + other.tag);

        if (other.CompareTag("Slot"))
        {
            currentSlot = other.transform;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: " + other.name + " Tag: " + other.tag);

        if (other.CompareTag("Slot") && currentSlot == other.transform)
        {
            currentSlot = null;
        }
    }
}