using Alchemy.Inspector;
using System;
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

        private void OnEnable()
        {
            _node.PointerEnter += OnPointerEnter;
            _node.PointerExit += OnPointerExit;
            _node.MarkedVisited += OnMarkedVisited;
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
            _graphic.color = Color.yellow;
        }

        public void OnPointerExit(LevelNode node, PointerEventData eventData)
        {
            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            _graphic.color = _node.Visited ? Color.green : Color.white;
        }
    }
}