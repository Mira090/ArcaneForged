using Mirror;
using Mirror.RemoteCalls;
using SephiriaArcaneForged.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SephiriaArcaneForged.ArcaneWeapons
{
    public class ArcaneWeapon_DashAttack : ArcaneWeapon_Basic
    {
        public int dashAttackDamageBonus = 60;

        [Header("Auto Attack")]
        public Timer autoAttackCheckTimer = new Timer(0.03f);

        public float autoAttackRadius = 3f;

        private bool autoAttackChance;

        private bool isDashCostPaid;

        private static Collider2D[] hits = new Collider2D[15];

        public GameObject dashAttackFxPrefab => SephiriaPrefabs.DashAttackFxPrefab;

        public int dashAttackIdx;

        [ServerCallback]
        private void Update()
        {
            if (!IsEffectEnabled)
                return;
            if (NetworkServer.active && autoAttackChance && WeaponController.currentWeapon)
            {
                UnitAvatar unitAvatar = SearchTarget();
                if ((bool)unitAvatar)
                {
                    WeaponController.OnBeginDashAttackAnimation?.Invoke();
                    Vector3 normalized = (unitAvatar.transform.position - NetworkAvatar.transform.position).normalized;
                    WeaponController.currentWeapon.CreateDashAttackProjectile(dashAttackIdx, unitAvatar.transform.position, normalized.normalized, null, 0f, isDashCostPaid ? 1f : 0.5f, 0);
                    Vector3 originPos = (WeaponController.currentWeapon.FirePosition_DashAttack(dashAttackIdx) + unitAvatar.transform.position) / 2f;
                    float angleFromVector = HorayUtility.GetAngleFromVector(normalized);
                    RpcDashAttackFx(originPos, angleFromVector);
                    isDashCostPaid = false;
                    autoAttackCheckTimer.SetTimer(999f);
                }

                if (autoAttackCheckTimer.Update(Time.deltaTime))
                {
                    autoAttackChance = false;
                }
            }
        }

        [ClientRpc]
        private void RpcDashAttackFx(Vector3 originPos, float angle)
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            writer.WriteVector3(originPos);
            writer.WriteFloat(angle);
            var func = "System.Void ArcaneWeapon_DashAttack::RpcDashAttackFx(UnityEngine.Vector3,System.Single)";
            SendRPCInternal(func, func.ToFunctionHashCode(), writer, 0, includeOwner: true);
            NetworkWriterPool.Return(writer);
        }
        public override void OnEnabledEffect()
        {
            base.OnEnabledEffect();
            //NetworkAvatar.AddCustomStat(ECustomStat.DashAttackDamageBonus, dashAttackDamageBonus);
            NetworkAvatar.OnDashServerside += HandleDash;
        }
        public override void OnDisabledEffect()
        {
            base.OnDisabledEffect();
            //NetworkAvatar.AddCustomStat(ECustomStat.DashAttackDamageBonus, -dashAttackDamageBonus);
            NetworkAvatar.OnDashServerside -= HandleDash;
        }

        private void HandleDash(Vector2 motionTo, bool isDashCostPaid)
        {
            this.isDashCostPaid = isDashCostPaid;
            autoAttackChance = true;
        }

        protected UnitAvatar SearchTarget()
        {
            if (!NetworkAvatar || !WeaponController)
            {
                return null;
            }

            int num = HorayPhysics2D.OverlapCircle(NetworkAvatar.transform.position, autoAttackRadius, hits, CombatManager.Topdown1FLayerMask);
            UnitAvatar unitAvatar = null;
            for (int i = 0; i < num; i++)
            {
                if (hits[i] == null)
                {
                    continue;
                }

                Hitbox component = hits[i].GetComponent<Hitbox>();
                if (!component)
                {
                    continue;
                }

                CombatBehaviour combatBehaviour = component.GetCombatBehaviour(0);
                if (!combatBehaviour)
                {
                    continue;
                }

                UnitAvatar unitAvatar2 = combatBehaviour as UnitAvatar;
                if (!unitAvatar2 || unitAvatar2.IsDead || unitAvatar2 == NetworkAvatar || GetRelation(unitAvatar2) != 0 || unitAvatar2.IsInvulnerable)
                {
                    continue;
                }

                if (!unitAvatar)
                {
                    unitAvatar = unitAvatar2;
                    continue;
                }

                float num2 = Vector3.Distance(unitAvatar.transform.position, base.transform.position);
                if (Vector3.Distance(unitAvatar2.transform.position, base.transform.position) < num2)
                {
                    unitAvatar = unitAvatar2;
                }
            }

            return unitAvatar;
        }

        private ERelationBehaviour GetRelation(UnitAvatar target)
        {
            if (!target)
            {
                return ERelationBehaviour.Neutral;
            }

            if ((bool)NetworkAvatar.NetworkLeader)
            {
                return RuntimeFactionManager.Instance.GetRelationBehaviour(NetworkAvatar.NetworkLeader.faction, target.faction, NetworkAvatar.NetworkLeader.attackableTargetSelector);
            }

            return RuntimeFactionManager.Instance.GetRelationBehaviour(NetworkAvatar.faction, target.faction, NetworkAvatar.attackableTargetSelector);
        }

        static ArcaneWeapon_DashAttack()
        {
            hits = new Collider2D[15];
            RemoteProcedureCalls.RegisterRpc(typeof(ArcaneWeapon_DashAttack), "System.Void ArcaneWeapon_DashAttack::RpcDashAttackFx(UnityEngine.Vector3,System.Single)", InvokeUserCode_RpcDashAttackFx__Vector3__Single);
        }

        public override bool Weaved()
        {
            return true;
        }

        protected void UserCode_RpcDashAttackFx__Vector3__Single(Vector3 originPos, float angle)
        {
            SpriteFx.Pool.Spawn(dashAttackFxPrefab, originPos).SetRotation(new Vector3(0f, 0f, angle));
        }

        protected static void InvokeUserCode_RpcDashAttackFx__Vector3__Single(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
        {
            if (!NetworkClient.active)
            {
                Debug.LogError("RPC RpcDashAttackFx called on server.");
            }
            else
            {
                ((ArcaneWeapon_DashAttack)obj).UserCode_RpcDashAttackFx__Vector3__Single(reader.ReadVector3(), reader.ReadFloat());
            }
        }
    }
}
