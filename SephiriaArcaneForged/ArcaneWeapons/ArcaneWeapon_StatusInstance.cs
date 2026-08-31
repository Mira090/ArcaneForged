using System;
using System.Collections.Generic;
using System.Text;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_StatusInstance : ArcaneWeapon_Basic
    {
        public string[] stats;

        public StatusInstance[] instances;

        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            if (stats == null)
                return;

            if (instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }

            instances = new StatusInstance[stats.Length];
            for (int j = 0; j < instances.Length; j++)
            {
                instances[j] = StatusDatabase.CreateStatusEntity(stats[j]);
                instances[j].SetTarget(base.NetworkAvatar);
                instances[j].ApplyStatus(fromRuntime: true);
            }
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();

            if(instances != null)
            {
                for (int i = 0; i < instances.Length; i++)
                {
                    instances[i].RemoveStatus();
                    instances[i].ClearTarget();
                }

                instances = null;
            }
        }
        public override Loc.KeywordValue[] BuildKeywords()
        {
            var list = new List<Loc.KeywordValue>();
            int index = 0;
            foreach(var stat in stats)
            {
                var instance = StatusDatabase.CreateStatusEntity(stat);
                if (instance == null)
                    continue;
                list.Add(new Loc.KeywordValue("VAL" + index, instance.ValueToString(false, false).Replace("-", "")));

                index++;
            }
            return list.ToArray();
        }
    }
}
