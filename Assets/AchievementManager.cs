using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementManager : MonoBehaviour
{
    public GameObject achievementPrefab;
    public Transform canvasTransform;

    public void UnlockAchievement(string title)
    {
        if (achievementPrefab == null)
        {
            Debug.LogError("achievementPrefab is NULL");
            return;
        }

        GameObject panel = Instantiate(achievementPrefab, canvasTransform);

        if (panel == null)
        {
            Debug.LogError("panel failed to instantiate");
            return;
        }

        Transform textTransform = panel.transform.Find("AchievementText");

        if (textTransform == null)
        {
            Debug.LogError("AchievementText not found inside prefab");
            return;
        }

        TextMeshProUGUI text = textTransform.GetComponent<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("TextMeshPro component missing");
            return;
        }

        text.text = "Achievement Unlocked:\n" + title;

        StartCoroutine(Slide(panel));
    }

    IEnumerator Slide(GameObject panel)
    {
        RectTransform rt = panel.GetComponent<RectTransform>();

        // Start above screen (top right)
        Vector2 start = new Vector2(300, 150);

        // End visible
        Vector2 end = new Vector2(-150, -40);

        rt.anchoredPosition = start;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            rt.anchoredPosition = Vector2.Lerp(start, end, t);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 4f;
            rt.anchoredPosition = Vector2.Lerp(end, start, t);
            yield return null;
        }

        Destroy(panel);
    }
}