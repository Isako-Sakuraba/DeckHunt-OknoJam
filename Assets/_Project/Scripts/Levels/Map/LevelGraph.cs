using Alchemy.Inspector;
using Game.Services;
using LitMotion;
using LitMotion.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Levels
{
    public class LevelGraph : MonoBehaviour
    {
        [Header("Core Dependencies")]
        [SerializeField, Required] private RectTransform _edgesParent;
        [SerializeField, Required] private LevelNode _rootNode;
        [SerializeField, Required] private LevelEdge _edgePrefab;

        [Header("Animation Dependencies")]
        [SerializeField, Required] private CanvasGroup _graphGroup;
        [SerializeField, Required] private CanvasGroup _inputBlockerGroup;
        [SerializeField, Required] private RectTransform _animationTarget;
        [SerializeField, Required] private RectTransform _hiddenPosition;
        [SerializeField, Required] private RectTransform _shownPosition;

        [Header("Animation Settings")]
        [SerializeField, Required] private float _blackoutDuration = 1f;
        [SerializeField, Required] private float _slideDuration = 2f;
        [SerializeField, Required] private float _blackoutAlpha = 0.4f;
        [SerializeField, Required] private float _showDelay = 1f;
        [SerializeField, Required] private float _hideDelay = 1f;

        private LevelNode _currentNode;
        private List<LevelEdge> _edges = new();
        private Dictionary<(LevelNode, LevelNode), LevelEdge> _nodesToEdges = new(20);

        private LevelEdge _highlightedEdge;

        private LevelService _levelService;

        private MotionHandle _animationMotion;

        private bool _levelInProgress = false;

        public bool LevelInProgress => _levelInProgress;

        private void Awake()
        {
            ServiceLocator.Register(this);

            _levelService = ServiceLocator.Get<LevelService>(); // Risky, but we need it in OnEnable
        }

        private void OnEnable()
        {
            SubscribeToLevelServiceEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromLevelServiceEvents();
        }

        private void Start()
        {
            _currentNode = _rootNode;
            _currentNode.MarkVisited();
            InitializeEdges();
            ShowLevelGraph();
        }

        private void SubscribeToLevelServiceEvents()
        {
            _levelService.LevelFinished += OnLevelFinished;
        }
        
        private void UnsubscribeFromLevelServiceEvents()
        {
            _levelService.LevelFinished -= OnLevelFinished;
        }

        private void InitializeEdges()
        {
            if (_rootNode == null)
                return;

            var visitedNodes = new HashSet<LevelNode>();
            // Directed edge key: (From, To) — order matters
            var visitedEdges = new HashSet<(LevelNode From, LevelNode To)>();

            var stack = new Stack<LevelNode>();
            visitedNodes.Add(_rootNode);
            stack.Push(_rootNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                current.PointerEnter += OnNodeHoverStart;
                current.PointerExit += OnNodeHoverEnd;

                foreach (var target in current.Connected)
                {
                    if (target == null)
                        continue;

                    // Register every outgoing edge exactly once
                    if (visitedEdges.Add((current, target)))
                    {
                        CreateEdge(current, target);
                    }

                    // Visit each node at most once
                    if (visitedNodes.Add(target))
                    {
                        stack.Push(target);
                    }
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

            try
            {
                _nodesToEdges.Add((current, neigbour), edge);
                _edges.Add(edge);
            } 
            catch (Exception ex)
            {
                Debug.LogError($"Sus activities for {current.name} and {neigbour.name}");
            }
        }

        private void OnNodeClicked(LevelNode node, PointerEventData data)
        {
            if (!_currentNode.Contains(node))
                return;

            _currentNode = node;
            _currentNode.MarkVisited();
            UpdateEdgeVisuals(); // There is no need to redraw all, maybe fix later

            StartCurrentLevel();
        }

        private void OnNodeHoverEnd(LevelNode node, PointerEventData data)
        {
            _highlightedEdge?.SetHighlight(false);
            _highlightedEdge = null;
        }

        private void OnNodeHoverStart(LevelNode node, PointerEventData data)
        {
            if (_currentNode == null)
            {
                Debug.LogError($"{nameof(_currentNode)} is null!", this);
                return;
            }

            if (_currentNode.Contains(node) 
                && _nodesToEdges.TryGetValue((_currentNode, node), out var edge))
            {
                _highlightedEdge?.SetHighlight(false);
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

        private async void StartCurrentLevel()
        {


            HideLevelGraph();

            await Awaitable.WaitForSecondsAsync(_blackoutDuration + _slideDuration, destroyCancellationToken);

            _levelService.StartLevel(_currentNode.Level);
        }

        private async void OnLevelFinished(LevelBase level)
        {
            if (_currentNode.Connected == null || _currentNode.Connected.Length == 0)
                return;

            await Awaitable.WaitForSecondsAsync(level.EndDelay + _showDelay, destroyCancellationToken);

            ShowLevelGraph();

            _levelInProgress = false;
        }

        private void ShowLevelGraph()
        {
            _animationTarget.gameObject.SetActive(true);
            _graphGroup.blocksRaycasts = true;
            AnimateGraph(0f, _blackoutAlpha, _hiddenPosition, _shownPosition);
        }

        private async void HideLevelGraph()
        {
            _inputBlockerGroup.blocksRaycasts = true;

            await Awaitable.WaitForSecondsAsync(_hideDelay, destroyCancellationToken);

            AnimateGraph(_blackoutAlpha, 0f, _shownPosition, _hiddenPosition, false);

            await _animationMotion;

            _animationTarget.gameObject.SetActive(false);
            _graphGroup.blocksRaycasts = false;
        }

        private async void AnimateGraph(float blackoutFrom, float blackoutTo, RectTransform slideFrom, RectTransform slideTo, bool blackoutFirst = true)
        {
            if (_animationMotion.IsValid())
                _animationMotion.Cancel();

            _inputBlockerGroup.blocksRaycasts = true;

            var sequence = LSequence.Create();

            var blackout = LMotion.Create(blackoutFrom, blackoutTo, _blackoutDuration)
                .BindToAlpha(_graphGroup);
            var slide = LMotion.Create(slideFrom.anchoredPosition, slideTo.anchoredPosition, _slideDuration)
                .BindToAnchoredPosition(_animationTarget);

            if (blackoutFirst)
                sequence.Append(blackout).Append(slide);
            else
                sequence.Append(slide).Append(blackout);

            _animationMotion = sequence.Run();

            _animationMotion.AddTo(this);

            await _animationMotion;

            _inputBlockerGroup.blocksRaycasts = false;
        }
    }
}