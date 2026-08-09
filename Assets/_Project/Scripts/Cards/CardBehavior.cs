using Game.Player;
using Game.Services;
using System;
using System.Text;
using UnityEngine;

namespace Game.Cards
{
    [Serializable]
    public abstract class CardBehavior
    {
        public abstract void OnCardPushed(CardService cardService);

        public abstract string GetDescription();
    }

    [Serializable]
    public class GiveRandomCardBehaviour : CardBehavior
    {
        [SerializeField] private string _description = "Gives a random card";

        public override void OnCardPushed(CardService cardService)
        {
            var db = ServiceLocator.Get<CardDatabase>();

            var card = db.GetRandomCard();
            cardService.PushCard(card);
        }

        public override string GetDescription()
        {
            return _description;
        }
    }

    [Serializable]
    public class ShotgunValuesBehaviour : CardBehavior
    {
        public ShotgunValues Values;
        public Color _goodColor = Color.darkGreen;
        public Color _badColor = Color.darkRed;

        public override void OnCardPushed(CardService cardService)
        {
            cardService.Shotgun.PushModifier(Values);
        }

        public override string GetDescription()
        {
            StringBuilder sb = new StringBuilder();
            var v = Values;

            // Absolute Values (Skip if 0)
            // Explicitly define if positive/negative is good for each stat
            AppendAbsolute(sb, v.Damage, "Damage", true);          // +Damage = Good
            AppendAbsolute(sb, v.Ammo, "Ammo", true);              // +Ammo = Good
            AppendAbsolute(sb, v.ReloadTime, "Reload Time", false);// +ReloadTime = Bad
            AppendAbsolute(sb, v.ShootCooldown, "Weapon Cooldown", false); // +Cooldown = Bad

            // Multipliers (Skip if 1.0f)
            AppendMultiplier(sb, v.DamageMultiplier, "Damage", false);
            AppendMultiplier(sb, v.ShootCooldownMultiplier, "Weapon Cooldown", true);
            AppendMultiplier(sb, v.ReloadTimeMultiplier, "Reload Time", true);

            return sb.ToString().TrimEnd('\n');
        }

        /// <summary>
        /// Appends an absolute float value with color coding.
        /// </summary>
        /// <param name="isGood">True if this change is beneficial to the player.</param>
        private void AppendAbsolute(StringBuilder sb, float value, string label, bool isGood)
        {
            if (Mathf.Approximately(value, 0f)) return;

            string sign = value > 0 ? "+" : "";
            Color color = isGood ? _goodColor : _badColor;
            string hex = ColorUtility.ToHtmlStringRGB(color);

            sb.AppendLine($"<color=#{hex}>{sign}{value:F0}</color> {label}");
        }

        /// <summary>
        /// Appends an absolute int value with color coding.
        /// </summary>
        /// <param name="isGood">True if this change is beneficial to the player.</param>
        private void AppendAbsolute(StringBuilder sb, int value, string label, bool isGood)
        {
            if (value == 0) return;

            string sign = value > 0 ? "+" : "";
            Color color = isGood ? _goodColor : _badColor;
            string hex = ColorUtility.ToHtmlStringRGB(color);

            sb.AppendLine($"<color=#{hex}>{sign}{value}</color> {label}");
        }

        /// <summary>
        /// Appends a percentage multiplier with color coding.
        /// </summary>
        /// <param name="isInvertedStat">
        /// True for time-based stats where a multiplier &lt; 1.0 is beneficial.
        /// When true, the color goodness is flipped relative to the raw percent change.
        /// </param>
        private void AppendMultiplier(StringBuilder sb, float multiplier, string label, bool isInvertedStat)
        {
            if (Mathf.Approximately(multiplier, 1f)) return;

            float percentChange = (multiplier - 1f) * 100f;

            // For inverted stats (reload/cooldown), a negative % change is GOOD
            // For normal stats (damage), a positive % change is GOOD
            bool isGood = isInvertedStat ? (percentChange < 0) : (percentChange > 0);

            string sign = percentChange > 0 ? "+" : "";
            Color color = isGood ? _goodColor : _badColor;
            string hex = ColorUtility.ToHtmlStringRGB(color);

            sb.AppendLine($"<color=#{hex}>{sign}{percentChange:F0}%</color> {label}");
        }
    }
}