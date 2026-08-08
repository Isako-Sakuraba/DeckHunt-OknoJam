using System;

namespace Game.Levels
{
    [Serializable]
    public abstract class LevelBase
    {
        public abstract bool IsInProcess { get; }
        public abstract bool IsFinished { get; }

        public event Action Finished = delegate { };

        public abstract void Start();

        public abstract void Update();

        public void Finish()
        {
            Finished.Invoke();
        }

        protected virtual void LateFinish() { }
    }
}