using System;
using UnityEngine;
using VRBeats;


namespace UserInTheBox
{
    public class RLEnv_VRBeat_BoxingStyle : RLEnv
    {
        // This class implements the RL environment for the BoxingStyle in VRBeat.
        public VR_BeatManager vrbeatManager;
        public ScoreManager scoreManager;

        private float _previousPoints, _initialPoints;

        private Transform _rightHand;

        protected override void CalculateReward()
        {
            // Get hit points
            int points = scoreManager.CurrentScore;
            _reward = (points - _previousPoints)*10;
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
            vrbeatManager.RestartLevel();
            
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
            // Update finished
            _isFinished = !vrbeatManager.getIsGameRunning();
        }

    }
}