using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels
{
    public class LevelGraph : MonoBehaviour
    {
        [SerializeField] private RectTransform _edgesParent;
        [SerializeField] private LevelNode _rootNode;
        [SerializeField] private LevelEdge _edgePrefab;

        private LevelNode _currentNode;

        private List<LevelEdge> _edges = new();

        private Dictionary<(LevelNode, LevelNode), LevelEdge> _nodesToEdges = new(20);

        private LevelEdge _highlightedEdge;

        private void Start()
        {
            InitializeEdges();
            _currentNode = _rootNode;
            _currentNode.MarkVisited();
        }

        private void InitializeEdges()
        {
            LevelNode current = _rootNode;
            current.PointerEnter += OnNodeHoverStart;
            current.PointerExit += OnNodeHoverEnd;

            Stack<LevelNode> stack = new(8);

            foreach(var neigbour in current.Connected)
            {
                stack.Push(neigbour);
                CreateEdge(current, neigbour);
            }

            while (stack.Count > 0)
            {
                current = stack.Pop();
                foreach (var neigbour in current.Connected)
                {
                    stack.Push(neigbour);
                    CreateEdge(current, neigbour);
                }
            }
        }

        private void CreateEdge(LevelNode current, LevelNode neigbour)
        {
            neigbour.PointerEnter += OnNodeHoverStart;
            neigbour.PointerExit += OnNodeHoverEnd;
            neigbour.PointerClick += OnNodeClicked;
            LevelEdge edge = Instantiate(_edgePrefab, _edgesParent);
            edge.Initialize(current, neigbour);
            _nodesToEdges.Add((current, neigbour), edge);
            _edges.Add(edge);
        }

        private void OnNodeClicked(LevelNode node, PointerEventData data)
        {
            if (!_currentNode.Contains(node))
                return;

            _currentNode = node;
            _currentNode.MarkVisited();
            UpdateEdgeVisuals(); // There is no need to redraw all, maybe fix later
        }

        private void OnNodeHoverEnd(LevelNode node, PointerEventData data)
        {
            _highlightedEdge?.SetHighlight(false);
        }

        private void OnNodeHoverStart(LevelNode node, PointerEventData data)
        {
            if (_currentNode.Contains(node) 
                && _nodesToEdges.TryGetValue((_currentNode, node), out var edge))
            {
                _highlightedEdge = edge;
                edge.SetHighlight(true);
            }
        }

        public void UpdateEdgeVisuals()
        {
            foreach (var edge in _edges)
            {
                edge.UpdateState();
            }
        }
    }
}