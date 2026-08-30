using Alchemy.Inspector;
using Game.Services;
using System;
using UnityEngine;

namespace Game.Levels
{
    public class LevelService : MonoBehaviour
    {
        private LevelBase _currentLevel;
        
        public LevelBase CurrentLevel => _currentLevel;

        public event Action<LevelBase> LevelStarted = delegate { };
        public event Action<LevelBase> LevelFinished = delegate { };

        [SerializeReference]
        private LevelBase _testLevel;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void StartLevel(LevelBase level)
        {
            _currentLevel?.Finish();
            _currentLevel = level;
            _currentLevel.Start();
            LevelStarted.Invoke(_currentLevel);
        }

        private void Update()
        {
            if (_currentLevel == null)
                return;

            _currentLevel.Update();

            if (_currentLevel.IsFinished)
            {
                _currentLevel.Finish();
                LevelFinished.Invoke(_currentLevel);
                _currentLevel = null;
            }
        }

        [Button]
        private void StartTestLevel()
        {
            StartLevel(_testLevel);
        }
    }
}