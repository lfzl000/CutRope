using UnityEngine;

public class ShowFpsInfo : MonoBehaviour
{
    public float showTime = 1f;
    public float fps;

    private int count = 0;
    private float deltaTime = 0f;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        count++;
        deltaTime += Time.deltaTime;
        if (deltaTime >= showTime)
        {
            fps = count / deltaTime;
            float milliSecond = deltaTime * 1000 / count;
            string strFpsInfo = string.Format(" 当前每帧渲染间隔：{0:0.0} ms ({1:0.} 帧每秒)", milliSecond, fps);

            count = 0;
            deltaTime = 0f;
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("FPS: _" + fps);

        if (GUILayout.Button("RESET"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}