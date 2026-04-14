using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public BassMage bassMage;

    public void OnPlayPressed()
    {
        bassMage.PlaySong();
    }
}
