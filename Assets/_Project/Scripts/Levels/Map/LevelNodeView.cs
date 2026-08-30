using Alchemy.Inspector;
using LitMotion;
using LitMotion.Extensions;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Levels
{
    public class LevelNodeView : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField, Required] private LevelNode _node;
        [SerializeField, Required] private Graphic _graphic;

        [Header("Settings")]
        [SerializeField] private Color _baseColor = Color.brown;
        [SerializeField] private Color _visitedColor = Color.darkGreen;
        [SerializeField] private Color _highlightedColor = Color.orange;
        [SerializeField] private float _colorChangeDuration = 0.15f;
        [SerializeField] private float _sizeOnHover = 1.2f;
        [SerializeField] private float _sizeHoverAnimationDuration = 0.2f;

        private MotionHandle _sizeMotion;
        private MotionHandle _colorMotion;

        private bool _hovered = false;

        private void OnEnable()
        {
            _node.PointerEnter += OnPointerEnter;
            _node.PointerExit += OnPointerExit;
            _node.MarkedVisited += OnMarkedVisited;

            UpdateGraphicsImmediately();
        }

        private void OnDisable()
        {
            _node.PointerEnter -= OnPointerEnter;
            _node.PointerExit -= OnPointerExit;
            _node.MarkedVisited -= OnMarkedVisited;
        }

        private void OnMarkedVisited()
        {
            UpdateGraphics();
        }

        public void OnPointerEnter(LevelNode node, PointerEventData eventData)
        {
            _hovered = true;

            if (_sizeMotion.IsValid() && _sizeMotion.IsPlaying())
                _sizeMotion.Cancel();

            _sizeMotion = LMotion.Create(Vector3.one, Vector3.one * _sizeOnHover, _sizeHoverAnimationDuration)
                .BindToLocalScale(_graphic.rectTransform);

            UpdateGraphics();
        }

        public void OnPointerExit(LevelNode node, PointerEventData eventData)
        {
            _hovered = false;

            if (_sizeMotion.IsValid() && _sizeMotion.IsPlaying())
                _sizeMotion.Cancel();

            _sizeMotion = LMotion.Create(Vector3.one * _sizeOnHover, Vector3.one, _sizeHoverAnimationDuration)
                .BindToLocalScale(_graphic.rectTransform);

            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            var targetColor = GetTargetColor();
            if (_graphic.color != targetColor)
            {
                if (_colorMotion.IsValid() && _colorMotion.IsPlaying())
                    _colorMotion.Cancel();

                _colorMotion = LMotion.Create(_graphic.color, targetColor, _colorChangeDuration)
                    .BindToColor(_graphic);
            }
        }

        public void UpdateGraphicsImmediately()
        {
            var targetColor = GetTargetColor();
            _graphic.color = targetColor;
        }

        public Color GetTargetColor()
        {
            var targetColor = _hovered ? _highlightedColor : _baseColor;
            targetColor = _node.Visited ? _visitedColor : targetColor;
            return targetColor;
        }
    }
}