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
        [SerializeField, Required] private RectTransform _animationTarget;
        [SerializeField, Required] private RectTransform _hiddenPosition;
        [SerializeField, Required] private RectTransform _shownPosition;

        [Header("Animation Settings")]
        [SerializeField, Required] private float _blackoutDuration = 1f;
        [SerializeField, Required] private float _slideDuration = 2f;
        [SerializeField, Required] private float _blackoutAlpha = 0.4f;

        private LevelNode _currentNode;
        private List<LevelEdge> _edges = new();
        private Dictionary<(LevelNode, LevelNode), LevelEdge> _nodesToEdges = new(20);

        private LevelEdge _highlightedEdge;

        private LevelService _levelService;

        private MotionHandle _animationMotion;

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
            InitializeEdges();
            _currentNode = _rootNode;
            _currentNode.MarkVisited();
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

            StartCurrentLevel();
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

        private async Task StartCurrentLevel()
        {
            HideLevelGraph();

            await Awaitable.WaitForSecondsAsync(_blackoutDuration + _slideDuration, destroyCancellationToken);

            _levelService.StartLevel(_currentNode.Level);
        }

        private void OnLevelFinished(LevelBase level)
        {
            if (_currentNode.Connected == null || _currentNode.Connected.Length == 0)
                return;

            ShowLevelGraph();
        }

        private void ShowLevelGraph()
        {
            _animationTarget.gameObject.SetActive(false);
            _graphGroup.blocksRaycasts = true;
            AnimateGraph(0f, _blackoutAlpha, _hiddenPosition, _shownPosition);
        }

        private async void HideLevelGraph()
        {
            _animationTarget.gameObject.SetActive(true);
            AnimateGraph(_blackoutAlpha, 0f, _shownPosition, _hiddenPosition);

            await _animationMotion;

            _graphGroup.blocksRaycasts = false;
        }

        private void AnimateGraph(float blackoutFrom, float blackoutTo, RectTransform slideFrom, RectTransform slideTo)
        {
            if (_animationMotion.IsValid())
                _animationMotion.Cancel();

            var sequence = LSequence.Create();

            var blackout = LMotion.Create(blackoutFrom, blackoutTo, _blackoutDuration)
                .WithEase(Ease.OutCubic)
                .BindToAlpha(_graphGroup);
            var slide = LMotion.Create(slideFrom.anchoredPosition, slideTo.anchoredPosition, _slideDuration)
                .WithEase(Ease.OutCubic)
                .BindToAnchoredPosition(_animationTarget);

            sequence.Append(blackout).Append(slide);

            _animationMotion = sequence.Run();

            _animationMotion.AddTo(this);
        }
    }
}