using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectScene_controller : MonoBehaviour
{
   public void SceneLoad(string name)
    {
        SceneManager.LoadScene(name);
    }
}

