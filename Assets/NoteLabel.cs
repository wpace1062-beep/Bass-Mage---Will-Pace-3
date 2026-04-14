using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NoteLabel : MonoBehaviour
{
    public int noteIndex;
    public BassMage bassMage;

    void OnMouseDown()
    {
        bassMage.PlayNote(noteIndex);
    }
}
