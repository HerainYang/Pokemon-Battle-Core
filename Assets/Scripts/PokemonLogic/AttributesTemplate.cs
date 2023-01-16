using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enum;
using Managers.BattleMgrComponents.BattlePlayables.Skills;

namespace Managers.BattleMgrComponents.PokemonLogic
{
    public class AttributesTemplate
    {
        public readonly string Name;
        private readonly int[] _attributeBuffKeyList;

        public AttributesTemplate(string name, int[] buffKeys)
        {
            this.Name = name;

            _attributeBuffKeyList = buffKeys;
        }
        
        public async UniTask InitAttribute(Pokemon self)
        {
            foreach (int buffKey in _attributeBuffKeyList)
            {
                await BuffMgr.Instance.AddBuff(self, self, buffKey, true);
            }
        }

        public void RemoveAttribute(Pokemon self)
        {
            foreach (int buffKey in _attributeBuffKeyList)
            {
                BuffMgr.Instance.RemoveBuffBySource(self, buffKey);
            }
        }
    }
}