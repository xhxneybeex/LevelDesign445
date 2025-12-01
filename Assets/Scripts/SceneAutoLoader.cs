using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneAutoLoader : MonoBehaviour
{
    [SerializeField] private float delay = 15f;

    private void Start()
    {
        Debug.Log("SceneAutoLoader started! Will load scene in " + delay + " seconds");
        StartCoroutine(LoadSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("MainScene_forDEMO");
    }
}
