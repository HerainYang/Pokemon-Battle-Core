using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;
using Managers.BattleMgrComponents.PokemonLogic;
using PokemonLogic;
using UnityEngine;

namespace Managers.BattleMgrComponents.BattlePlayer
{
    public class PlayerAI : ABattlePlayer
    {
        private bool _turn = false;
        protected override async void SendCommandRequest(Pokemon pokemon)
        {
            int skillID = !_turn ? 1 : 2;
            CommonSkillTemplate skill = PokemonMgr.Instance.GetSkillTemplateByID(pokemon.GetSkills()[1]);
            int[] targets;
            if (skill.TargetType == SkillTargetType.OneEnemy)
            {
                targets = await BattleMgr.Instance.TryAutoGetTarget(pokemon, SkillTargetType.FirstAvailableEnemy);
            } else if (skill.TargetType == SkillTargetType.OneTeammate)
            {
                targets = await BattleMgr.Instance.TryAutoGetTarget(pokemon, SkillTargetType.FirstAvailableTeammate);
            }
            else
            {
                targets = await BattleMgr.Instance.TryAutoGetTarget(pokemon, skill.TargetType);
            }
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestLoadPokemonSkill, pokemon, !_turn ? 1 : 2, targets);

            _turn = !_turn;
        }

        protected override void SendPokemonForceAddRequest(int onStagePosition)
        {
            //replace as first available
            EventMgr.Instance.Dispatch(Constant.EventKey.RequestSentPokemonOnStage, GetFirstPokemonCanSent(), onStagePosition);
            // EventMgr.Instance.Dispatch(Constant.EventKey.BattlePokemonForceChangeCommandSent, this);
        }

        public override void TestHeartBeat()
        {
            EventMgr.Instance.Dispatch(Constant.EventKey.HeartBeatSent, this);
        }

        public PlayerAI(BasicPlayerInfo info) : base(info)
        {
        }
    }
}