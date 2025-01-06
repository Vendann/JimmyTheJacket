using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
    public void SceneLoad(int index) {
        SceneManager.LoadScene(index);
    }

    public void Exit() {
        Application.Quit();
    }
}
