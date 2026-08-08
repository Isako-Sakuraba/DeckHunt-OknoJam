using Alchemy.Inspector;
using Game.Services;
using UnityEngine;

namespace Game.Levels
{
    public class LevelService : MonoBehaviour
    {
        private LevelBase _currentLevel;
        
        public LevelBase CurrentLevel => _currentLevel;

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
        }

        private void Update()
        {
            if (_currentLevel == null)
                return;

            _currentLevel.Update();

            if (_currentLevel.IsFinished)
            {
                _currentLevel.Finish();
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