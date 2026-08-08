using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels
{
    public class LevelNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private LevelBase _level;
        [SerializeField] private LevelNode[] _connected;
        private bool _visited = false;
        private HashSet<LevelNode> _connectedCache = new();

        public event Action MarkedVisited = delegate { };

        private void Awake()
        {
            if (_connected == null)
                return;

            foreach (LevelNode node in _connected)
            {
                if (node != null)
                    _connectedCache.Add(node);
            }
        }

        public LevelBase Level => _level;
        public LevelNode[] Connected => _connected;
        public bool Visited => _visited;

        public event Action<LevelNode, PointerEventData> PointerEnter = delegate { };
        public event Action<LevelNode, PointerEventData> PointerExit = delegate { };
        public event Action<LevelNode, PointerEventData> PointerClick = delegate { };

        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEnter.Invoke(this, eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            PointerExit.Invoke(this, eventData);
        }

        public bool Contains(LevelNode node) => _connectedCache.Contains(node);

        private void OnDrawGizmos()
        {
            if (_connected == null)
                return;

            Gizmos.color = Color.magenta;

            foreach(var node in _connected)
            {
                if (node != null)
                    Gizmos.DrawLine(transform.position, node.transform.position);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick.Invoke(this, eventData);
        }

        public void MarkVisited()
        {
            _visited = true;
            MarkedVisited.Invoke();
        }
    }
}