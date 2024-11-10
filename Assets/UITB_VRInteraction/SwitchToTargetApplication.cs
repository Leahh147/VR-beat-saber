using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRBeats.UI 
{
    public class SwitchToTargetApplication : MonoBehaviour
    {
        [SerializeField]
        private bool boxingStyle = false;

        // Start is called before the first frame update
        void Start()
        {
            string sceneName = boxingStyle ? "BoxingStyle" : "SaberStyle";
            int index = boxingStyle ? 3 : 0;
            PlayableManager.SetSelectedTrackIndex(index);

            Debug.LogWarning("Switching to the " + sceneName + "...");
            SceneManager.LoadScene( sceneName );
            //SceneManager.LoadScene("BoxingStyle", LoadSceneMode.Single);

        }

    }
}