using Game.Player;
using System.Text;
using UnityEngine;

namespace Game.Cards
{
    using System.Text;
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewCardConfig", menuName = "Scriptable Objects/CardConfig")]
    public class CardConfig : ScriptableObject
    {
        public string Title = "EmptyCard";
        public Sprite Icon = null;
        public int Price = 20;
        [SerializeReference] public CardBehavior CustomBehaviour = null;
    }
}