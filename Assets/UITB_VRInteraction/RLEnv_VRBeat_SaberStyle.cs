using System;
using UnityEngine;
using UnityEngine.Playables;
using VRBeats;
using VRBeats.ScriptableEvents;
using VRBeats.UI;
using Random = System.Random;


namespace UserInTheBox
{
    public class RLEnv_VRBeat_SaberStyle : RLEnv
    {
        // This class implements the RL environment for the BoxingStyle in VRBeat.
        public VR_BeatManager vrbeatManager  = null;
        public ScoreManager scoreManager  = null;
        public TriggerRestartLevelButton triggerButton = null;
        [SerializeField] private GameEvent onGameOverEvent = null;
        public PlayableDirector playableDirector = null;

        private float _previousPoints, _initialPoints;

        private Transform _rightHand = null;

        protected override void CalculateReward()
        {
            // Get hit points
            int points = scoreManager.CurrentScore;
            _reward = (points - _previousPoints);
            _previousPoints = points;
            
            // Also calculate distance component
            /*foreach (var target in vrbeatManager.targetArea.GetComponentsInChildren<Target>())
            {
                if (target.stateMachine.currentState == TargetState.Alive)
                {
                    var dist = Vector3.Distance(target.transform.position, _marker.position);
                    _reward += (float)(Math.Exp(-10*dist)-1) / 10;
                }
            }*/
        }
        
        public override void Reset()
        {
            // Set play level
            //vrbeatManager.RestartLevel();
            
            onGameOverEvent.Invoke();
            
            foreach (var obj in GameObject.FindGameObjectsWithTag("Destroyable"))
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
            
            triggerButton.TriggerRestartEvent();
            
            // Reset points
            InitialiseReward();
        }

        public override void InitialiseGame()
        {
            // Do nothing
        }

        public override float GetTimeFeature()
        {
            // DO nothing
            return 0.0f;
        }

        public override void InitialiseReward()
        {
            _initialPoints = scoreManager.CurrentScore;
            _previousPoints = _initialPoints;
            _rightHand = simulatedUser.rightHandController;
        }

        public override void UpdateIsFinished()
        {
            // Add additional logs
            _logDict["AccumulatedErrors"] = scoreManager.AcumulateErrors;
            _logDict["AccumulatedCorrectSlices"] = scoreManager.AcumulateCorrectSlices;
            
            // Update finished
            _isFinished = !vrbeatManager.getIsGameRunning();
        }

    }
}