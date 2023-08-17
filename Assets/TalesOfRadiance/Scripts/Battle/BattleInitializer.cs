using System;
using TalesOfRadiance.Scripts.Battle.Managers;
using TalesOfRadiance.Scripts.Character;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle
{
    public class BattleInitializer : MonoBehaviour
    {
        public BattleTeamInfo player;
        public BattleTeamInfo enemy;

        public CharacterTeam playerTeam;
        public CharacterTeam enemyTeam;

        public Canvas cameraSpaceCanvas;
        public Camera uiCamera;

        private void Start()
        {
            playerTeam.playerInfo = player;
            enemyTeam.playerInfo = enemy;
            BattleMgr.Instance.Init(uiCamera, cameraSpaceCanvas, playerTeam, enemyTeam);
        }

    }
}