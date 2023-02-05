using CoreScripts.BattlePlayables;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Types = TalesOfRadiance.Scripts.Battle.Constant.Types;

namespace TalesOfRadiance.Scripts.Battle.BattlePlayables
{
    public class BpDebut : ABattlePlayable
    {
        private int _heroId;
        private CharacterAnchor _anchor;
        private CharacterTeam _team;
        public BpDebut(int heroId, CharacterAnchor anchor, CharacterTeam team) : base((int)Types.PlayablePriority.Debut)
        {
            _heroId = heroId;
            _anchor = anchor;
            _team = team;
        }

        public override async void Execute()
        {
            await _anchor.Init(_heroId, _team);
            OnDestroy();
        }

        protected override void OnDestroy()
        {
            BattleMgr.Instance.BattlePlayableEnd();
        }
    }
}