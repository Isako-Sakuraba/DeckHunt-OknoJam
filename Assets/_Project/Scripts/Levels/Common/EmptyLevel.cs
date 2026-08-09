using System;

namespace Game.Levels
{
    [Serializable]
    public class EmptyLevel : LevelBase
    {
        public override bool IsInProcess => false;
        public override bool IsFinished => true;

        public override void Start() { }
        public override void Update() { }
    }
}