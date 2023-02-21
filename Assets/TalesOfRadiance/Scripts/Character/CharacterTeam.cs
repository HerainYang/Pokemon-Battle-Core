using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TalesOfRadiance.Scripts.Battle;
using TalesOfRadiance.Scripts.Battle.BattleComponents.RuntimeClass;
using TalesOfRadiance.Scripts.Battle.Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TalesOfRadiance.Scripts.Character
{
    public class CharacterTeam : MonoBehaviour
    {
        public BattleTeamInfo playerInfo;
        private readonly Dictionary<int, CharacterAnchor> _characterAnchors = new Dictionary<int, CharacterAnchor>();

        public List<RuntimeHero> Heroes = new List<RuntimeHero>();


        public void LoadBattlePlayables()
        {
            foreach (var hero in Heroes)
            {
                if(hero.Properties.IsAlive) 
                    hero.LoadBattleMoveBp();
            }
        }

        public RuntimeHero GetHeroByIndex(int index)
        {
            if (!_characterAnchors.ContainsKey(index))
                return null;
            return !_characterAnchors[index].Hero.Properties.IsAlive ? null : _characterAnchors[index].Hero;
        }

        public async UniTask SentHeroOnStage()
        {
            List<UniTask> tasks = new List<UniTask>();

            async UniTask GenerateAnchor(int position)
            {
                var handler = await Addressables.LoadAssetAsync<GameObject>("CharacterAnchor");
                var o = Instantiate(handler, transform);
                o.transform.localPosition = ConfigManager.Instance.GetAnchorPosition(position);
                var anchor = o.GetComponent<CharacterAnchor>();
                _characterAnchors.Add(position, anchor);
                BattleMgr.Instance.SentHeroOnStage(playerInfo.squadInfoByIndex[position], anchor, this);
            }
            
            for (int i = 0; i < 9; i++)
            {
                if(playerInfo.squadInfoByIndex[i] == -1)
                    continue;
                tasks.Add(GenerateAnchor(i));
            }
            
            await tasks;
        }

        public void UpdateHeroCooldown()
        {
            foreach (var skill in Heroes.SelectMany(hero => hero.RuntimeSkillList.Where(skill => skill.Cooldown > 0)))
            {
                skill.Cooldown--;
            }
        }

        public override bool Equals(object other)
        {
            if (other is not CharacterTeam team)
                return false;
            return team.playerInfo.RuntimeID == playerInfo.RuntimeID;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
