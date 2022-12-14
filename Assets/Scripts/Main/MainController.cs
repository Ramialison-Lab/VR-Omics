using UnityEngine;
using UnityEngine.SceneManagement;
// Main Controller Application Script

public class MainController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var m_Scene = SceneManager.GetActiveScene();
            if (m_Scene.name == "Visualisation")
            {
                Destroy(GameObject.Find("ScriptHolder"));
                Destroy(GameObject.Find("ScriptHolderPipeline"));
                SceneManager.LoadScene(0);
            }
            else
            {
                Application.Quit();

            }
        }
    }
}
