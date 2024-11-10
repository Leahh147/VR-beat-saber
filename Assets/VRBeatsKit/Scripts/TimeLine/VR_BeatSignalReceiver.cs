using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UserInTheBox;
using Random = UnityEngine.Random;

namespace VRBeats
{
    public class CubeInfo
    {
        public Direction direction;
        public Vector3 position;
        public ColorSide colorSide;
        
        public CubeInfo(Direction direction, Vector3 position, ColorSide colorSide)
        {
            this.direction = direction;
            this.position = position;
            this.colorSide = colorSide;
        }
    }

    public class VR_BeatSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        private EnviromentController enviromentController = null;

        private double lastTime = 0;
        private int frame = 0;
        private List<CubeInfo> cubeInfo = new List<CubeInfo>();
        private List<CubeInfo> mineInfo = new List<CubeInfo>();
        private PlayableDirector playableDirector;
        private bool rightHandOnly = false;

        private void Awake()
        {
            enviromentController = FindObjectOfType<EnviromentController>();
            playableDirector = GetComponent<PlayableDirector>();
            
            // Check if we should convert targets to right hand targets
            if (UitBUtils.GetOptionalArgument("right_hand_only"))
            {
                rightHandOnly = true;
            }
        }
        
        private void Start() 
        {   
            if (playableDirector.playableAsset.name == "new_track")
            {
                // Use only a subset of cubes
                cubeInfo.Add(new CubeInfo(Direction.LowerLeft, new Vector3(-0.6f, -0.2f, 1.2f), ColorSide.Left));
                cubeInfo.Add(new CubeInfo(Direction.UpperLeft, new Vector3(-0.6f, 1.0f, 1.2f), ColorSide.Left));
                cubeInfo.Add(new CubeInfo(Direction.LowerRight, new Vector3(0.6f, -0.2f, 1.2f), ColorSide.Right));
                cubeInfo.Add(new CubeInfo(Direction.UpperRight, new Vector3(0.6f, 1.0f, 1.2f), ColorSide.Right));

                // User a different subset for mines; ColorSide doesn't matter 
                mineInfo.Add(new CubeInfo(Direction.LowerLeft, new Vector3(0.0f, 1.0f, 1.2f), ColorSide.Left));
                mineInfo.Add(new CubeInfo(Direction.LowerLeft, new Vector3(-0.6f, 0.4f, 1.2f), ColorSide.Left));
                mineInfo.Add(new CubeInfo(Direction.LowerLeft, new Vector3(0.0f, -0.2f, 1.2f), ColorSide.Left));
                mineInfo.Add(new CubeInfo(Direction.LowerLeft, new Vector3(0.6f, 0.4f, 1.2f), ColorSide.Left));
            }
        }

        private void Update()
        {
            frame++;
        }

        public void OnNotify(Playable origin, INotification notification, object context)
        {
            if (notification is VR_BeatSpawnMarker spawnMarker)
            {
                if (playableDirector.playableAsset.name == "new_track")
                {
                    // Use the same speed for all markers
                    spawnMarker.spawInfo.speed = 6;

                    // Sample new cube info
                    CubeInfo newCube;
                    if (spawnMarker.spawneable.gameObject.GetComponent<VR_BeatCube>() != null)
                    {
                        newCube = cubeInfo[Random.Range(0, cubeInfo.Count)];
                    }
                    else if (spawnMarker.spawneable.gameObject.GetComponent<Mine>() != null)
                    {
                        newCube = mineInfo[Random.Range(0, mineInfo.Count)];
                    }
                    else
                    {
                        // Not a cube or a mine, just spawn it and return
                        VR_BeatManager.instance.Spawn(spawnMarker.spawneable, spawnMarker.spawInfo);
                        lastTime = spawnMarker.time;
                        return;
                    }

                    // Set handedness
                    spawnMarker.spawInfo.colorSide = rightHandOnly ? ColorSide.Right : newCube.colorSide;
                    //spawnMarker.spawInfo.colorSide = (ColorSide)Random.Range(0, 2);

                    // Set hit direction
                    spawnMarker.spawInfo.hitDirection = newCube.direction;
                    //spawnMarker.spawInfo.hitDirection = (Direction)Random.Range(0, 10);

                    // Set position
                    spawnMarker.spawInfo.position = newCube.position;
                    //spawnMarker.spawInfo.position.x = Random.Range(-0.6f, 0.6f);
                    //spawnMarker.spawInfo.position.y = Random.Range(-0.6f, 0.6f);
                    //spawnMarker.spawInfo.position.z = Random.Range(0.8f, 1.2f);

                    VR_BeatManager.instance.Spawn(spawnMarker.spawneable, spawnMarker.spawInfo);
                    lastTime = spawnMarker.time;
                }
                else
                {
                    if (rightHandOnly)
                    {
                        spawnMarker.spawInfo.colorSide = ColorSide.Right;
                    }
                    VR_BeatManager.instance.Spawn(spawnMarker.spawneable, spawnMarker.spawInfo);
                    lastTime = spawnMarker.time;
                }
            }
            else if (notification is VR_BeatEnvironmentColorMarker enviromentColorMarker)
            {
                enviromentController.FadeToColor(enviromentColorMarker.color, enviromentColorMarker.fadeTime, enviromentColorMarker.ease);
            }
            else if ( notification is VR_BeatEnvironmentEmissionMarker emmmisionMarker )
            {
                enviromentController.FadeEmmisiveValue(emmmisionMarker.targetEmissionValue, emmmisionMarker.fadeTime, emmmisionMarker.ease);
            }

            
        }
    }
}
