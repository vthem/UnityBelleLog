using UnityEngine;

public class ZobLoggerTester : MonoBehaviour
{
    private Zob.Logger logger = new Zob.Logger("ZobLoggerTester");

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            logger.Trace();
        }
    }
}
