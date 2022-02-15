using R2API;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RAIC.Items
{
    public class MurasTriage
    {
        public static ItemDef itemDef;

        internal static void Init()
        {
            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = "MurasTriage";
            itemDef.AutoPopulateTokens(); //Remove this once tokens in place
            //TODO: Name, Lore, Icon, Model, All that Good Stuff
            itemDef.tier = ItemTier.Lunar;

            CustomItem customItem = new CustomItem(itemDef, new ItemDisplayRuleDict());
            ItemAPI.Add(customItem);

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active) return; // Bad client, go to jail

            if (self.body?.master?.minionOwnership?.ownerMaster is CharacterMaster minionMaster &&
                minionMaster.GetBody() is CharacterBody minionMasterBody &&
                minionMasterBody.inventory?.GetItemCount(MurasTriage.itemDef) is int itemCount && itemCount > 0)
            {
                DamageInfo newDamageInfo = damageInfo;
                // Take damage for the minion, +50% per additional stack
                newDamageInfo.damage = damageInfo.damage * (1f + (itemCount - 1f) * 0.5f);
                orig(minionMasterBody.healthComponent, damageInfo);
            }
            else
            {
                orig(self, damageInfo);
            }
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory?.GetItemCount(MurasTriage.itemDef) is int itemCount && itemCount > 0)
            {
                float totalMinionHealth = 0f, totalMinionShield = 0f, totalMinionRegen = 0f;
                IEnumerable<CharacterMaster> senderMinions = CharacterMaster.readOnlyInstancesList.Where(x => x.minionOwnership?.ownerMaster == sender.master); // Thanks KEB for this example way back when
                foreach(CharacterMaster minion in senderMinions)
                {
                    HealthComponent minionHC = minion.GetBody()?.healthComponent;
                    totalMinionHealth += minionHC.fullHealth;
                    totalMinionShield += minionHC.fullShield;
                    totalMinionRegen += minion.GetBody().regen;
                }

                // Gain 50% of the minions' health and shield per stack
                args.baseHealthAdd = totalMinionHealth * (itemCount * 0.5f);
                args.baseShieldAdd = totalMinionShield * (itemCount * 0.5f);
                args.baseRegenAdd = totalMinionRegen * (itemCount * 0.5f);
            }
        }
    }
}
