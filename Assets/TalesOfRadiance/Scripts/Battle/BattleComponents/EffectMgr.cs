using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace TalesOfRadiance.Scripts.Battle.BattleComponents
{
    public class EffectMgr : MonoBehaviour, ISingleton
    {

        public static EffectMgr Instance;

        [SerializeField] private LineRenderer _lineRenderer;

        private void Awake()
        {
            Instance = this;
        }

        public void RenderLineFromTo(Vector3 source, Vector3 target)
        {
            UniTask.Void((async () =>
            {
                var p0 = source;
                var p2 = target;
                var p1 = (source + target) / 2;
                p1.y += 5f;
                var o = Instantiate(_lineRenderer, this.transform);
                o.gameObject.SetActive(true);
                List<Vector3> bezierNodes = new List<Vector3>();
                float t = 0;
                while (t <= 1)
                {
                    Vector3 pt = (1-t) * (1-t) * source + 2*t*(1-t)*p1 + t*t*p2;
                    t += 0.1f;
                    bezierNodes.Add(pt);
                }
                bezierNodes.Add(target);

                o.positionCount = bezierNodes.Count;
                o.SetPositions(bezierNodes.ToArray());
                await UniTask.Delay(1000);
                Destroy(o.gameObject);
            }));
        }
    }
}