using Cysharp.Threading.Tasks;
using PokemonDemo.Scripts.Managers;
using PokemonDemo.Scripts.PokemonLogic.PokemonData;

namespace PokemonDemo.Scripts.PokemonLogic
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

        public async UniTask RemoveAttribute(Pokemon self)
        {
            foreach (int buffKey in _attributeBuffKeyList)
            {
                await BuffMgr.Instance.RemoveBuffBySource(self, PokemonMgr.Instance.GetBuffTemplateByID(buffKey));
            }
        }
    }
}