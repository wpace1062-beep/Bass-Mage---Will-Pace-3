using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BassMage : MonoBehaviour {

    [Header("Note Labels")]

    public GameObject labelPrefab;
    public float labelX = -24f;

    public AudioClip[] noteClips;
    private AudioSource audioSource;

    private string[] noteNames = {"E1", "F1", "F#1", "G1", "G#1", "A1", "A#1", "B1", "C2", "C#2",
                                 "D2", "D#2", "E2"};


    [Header("Inscribed")]

    public GameObject slotPrefab;
    public GameObject notePrefab;

    public bool isPlaying = false;

    public float slotBottomY = -14f;
    public float slotSpacingY = 2.3f;
    public float slotStartingX = -21f;
    public float slotSpacingX = 3f;
    public int measureCount;
    public float gap;

    public GameObject[,] slotsGrid;
    public Note[,] notesGrid;

    void Start () {

        Debug.Log("BassMage Start is running");
        Vector3 pos;

        audioSource = gameObject.AddComponent<AudioSource>();

        slotsGrid = new GameObject[16,13];
        notesGrid = new Note[16,13];

        //Draw slots

        for (int j = 0; j < 16; j++) {
            for (int i = 0; i < 13; i++) {
            GameObject tSlotGO = Instantiate<GameObject>(slotPrefab);

            measureCount = j / 4;
            gap = measureCount * 1;
            
            pos.x = slotStartingX + (slotSpacingX * j) + gap;
            pos.y = slotBottomY + (slotSpacingY * i);
            pos.z = 0f;

            tSlotGO.transform.position = pos;

            slotsGrid[j, i] = tSlotGO;
            }
        }

        //Draw note labels

        for (int i = 0; i < 13; i++)
        {
            GameObject label = Instantiate(labelPrefab);
            
            pos.x = labelX;
            pos.y = slotBottomY + (slotSpacingY * i);
            pos.z = -6f;

            label.transform.position = pos;

            TextMeshPro text = label.GetComponent<TextMeshPro>();
            text.text = noteNames[i];
            
            NoteLabel labelScript = label.GetComponent<NoteLabel>();
            labelScript.noteIndex = i;
            labelScript.bassMage = this;
        }

        SpawnNewNote();
    }

    public void SpawnNewNote() {
        GameObject tNote = Instantiate(notePrefab);

        Note noteScript = tNote.GetComponent<Note>();

        noteScript.slotsGrid = slotsGrid;
        noteScript.numColumns = 16;
        noteScript.numRows = 13;
        noteScript.bassMage = this;

        tNote.transform.position = new Vector3(-27, 0, 0);
    }

    public void PlayNote (int index)
    {
        if (index >= 0 && index < noteClips.Length)
        {
            audioSource.PlayOneShot(noteClips[index]);
        }
    }

    //loop used for playback after song has been assembled

    IEnumerator PlaySongRoutine()
    {
        isPlaying = true;

        for (int column = 0; column < 16; column++)
        {
            for (int row = 0; row < 13; row++)
            {
                if (notesGrid[column, row] != null)
                {
                    PlayNote(row);
                }
            }

            yield return new WaitForSeconds(0.7f);
        }

        isPlaying = false;
    }

    public void PlaySong ()
    {

        if (CheckMary())    FindObjectOfType<AchievementManager>().UnlockAchievement("Hail Mary");

        if (!isPlaying)
        {
            StartCoroutine(PlaySongRoutine());
        }
    }

    public bool CheckMary()
    {
        // 2D array for Mary Had a Little Lamb
        // Rows = note index (0 = E1, 12 = E2)
        // Columns = sequencer columns
        // 1 = note present, null = empty column
        int?[,] maryPattern = new int?[13, 16]; // 13 rows x 16 columns, initialized to null

        // Fill in the notes based on sequence [8,6,4,6,8,8,8,null,6,6,6,null,8,11,11,null]

        maryPattern[8, 0]  = 1;
        maryPattern[6, 1]  = 1;
        maryPattern[4, 2]  = 1;
        maryPattern[6, 3]  = 1;
        maryPattern[8, 4]  = 1;
        maryPattern[8, 5]  = 1;
        maryPattern[8, 6]  = 1;
        // column 7 = null
        maryPattern[6, 8]  = 1;
        maryPattern[6, 9]  = 1;
        maryPattern[6,10]  = 1;
        // column 11 = null
        maryPattern[8,12]  = 1;
        maryPattern[11,13] = 1;
        maryPattern[11,14] = 1;
        // column 15 = null

        int rows = notesGrid.GetLength(1);
        int cols = notesGrid.GetLength(0);

        // Ensure pattern fits the grid
        if (maryPattern.GetLength(0) != rows || maryPattern.GetLength(1) != cols)
        {
            Debug.LogError("Mary pattern size does not match the grid size!");
            return false;
        }

        // Compare pattern to notesGrid
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Note note = notesGrid[x, y];

                if (maryPattern[y, x].HasValue)
                {
                    // Pattern expects a note
                    if (note == null)
                        return false; // missing note
                } else {
                    // Pattern expects empty
                    if (note != null)
                        return false; // rogue note
                }
            }
        }

        // Grid exactly matches Mary Had a Little Lamb
        return true;
    }
}