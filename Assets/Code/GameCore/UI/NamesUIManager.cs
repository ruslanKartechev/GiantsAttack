using System.Collections.Generic;
using RaftsWar.Boats;
using UnityEngine;
using UnityEngine.UI;

namespace RaftsWar.UI
{
    public class NamesUIManager : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private float _widthPerElement;
        [SerializeField] private float _widthOffset;
        [SerializeField] private Color _deadColor;
        [SerializeField] private List<NameUI> _uiBlocks;
        private Dictionary<ITeamPlayer, NameUI> _map = new Dictionary<ITeamPlayer, NameUI>();
        private int _ind = 0;
        private int _count = 0;
        
        public void AddPlayer(ITeamPlayer player)
        {
            if (_ind >= _uiBlocks.Count)
            {
                Debug.LogError($"NOT ENOUGH BLOCKS");
                return;
            }
            var ui = _uiBlocks[_ind];
            _ind++;
            ui.DeadColor = _deadColor;
            _map.Add(player, ui);
            _count++;
            UpdateBackground();
            player.SetTeamUnitUI(ui);
        }

        private void UpdateBackground()
        {
            var width = _count * _widthPerElement + _widthOffset;
            var size = _background.rectTransform.sizeDelta;
            size.y = width;
            _background.rectTransform.sizeDelta = size;
        }

        private void OnDied(ITeamPlayer obj)
        {
            var ui = _map[obj];
            ui.Die();
        }
    }
}