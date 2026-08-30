using System;
using UnityEngine;

namespace Game.Levels
{
    public class LevelEdge : MonoBehaviour
    {
        private LevelNode _from;
        private LevelNode _to;

        public LevelNode From => _from;
        public LevelNode To => _to;

        public event Action<EdgeState> StateChanged = delegate { };
        public event Action<bool> HighlightChanged = delegate { };
        public event Action<LevelNode, LevelNode> Initialized = delegate { };

        public void Initialize(LevelNode from, LevelNode to)
        {
            _from = from;
            _to = to;
            Initialized.Invoke(from, to);
        }

        public enum EdgeState
        {
            None,
            Completed
        }

        private EdgeState _state = EdgeState.None;

        public EdgeState State => _state;

        public void UpdateState()
        {
            var previous = _state;

            if (_from.Visited && _to.Visited)
            {
                _state = EdgeState.Completed;
            }
            else
            {
                _state = EdgeState.None;
            }

            if (previous != _state)
                StateChanged.Invoke(_state);
        }

        public void SetHighlight(bool highlight)
        {
            HighlightChanged.Invoke(highlight);
        }
    }
}