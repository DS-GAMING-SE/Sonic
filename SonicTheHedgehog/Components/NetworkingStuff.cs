using RoR2;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;
using UnityEngine;
using SonicTheHedgehog.SkillStates;
using EntityStates;
using SonicTheHedgehog.Modules;

namespace SonicTheHedgehog.Components
{
    public class SonicParryHit : INetMessage
    {
        NetworkInstanceId netId;
        DamageInfo damageInfo;

        public SonicParryHit()
        {

        }

        public SonicParryHit(NetworkInstanceId netId, DamageInfo damageInfo)
        {
            this.netId = netId;
            this.damageInfo = damageInfo;
        }
        
        public void OnReceived()
        {
            if (NetworkServer.active) return;
            Debug.Log("Parry Recieved Client");
            GameObject body = Util.FindNetworkObject(netId);
            if (body)
            {
                EntityState state = EntityStateMachine.FindByCustomName(body, "Body").state;
                ((Parry)state).OnTakeDamage(damageInfo);
            }

        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(damageInfo);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            damageInfo = reader.ReadDamageInfo();
        }
    }

    public class ScepterBoostDamage : INetMessage
    {
        HurtBox hurtbox;
        DamageInfo damageInfo;

        public ScepterBoostDamage()
        {

        }

        public ScepterBoostDamage(HurtBox hurtbox, DamageInfo damageInfo)
        {
            this.hurtbox = hurtbox;
            this.damageInfo = damageInfo;
        }

        public void OnReceived()
        {
            if (!NetworkServer.active) return;
            Debug.Log("Scepter Boost Damage Received Server");
            ScepterBoost.DealDamage(this.hurtbox, this.damageInfo);
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(HurtBoxReference.FromHurtBox(hurtbox));
            writer.Write(damageInfo);
        }

        public void Deserialize(NetworkReader reader)
        {
            hurtbox = reader.ReadHurtBoxReference().ResolveHurtBox();
            damageInfo = reader.ReadDamageInfo();
        }
    }

    public class SuperSonicTransform : INetMessage
    {
        NetworkInstanceId netId;

        public SuperSonicTransform()
        {

        }

        public SuperSonicTransform(NetworkInstanceId netId)
        {
            this.netId = netId;
        }

        public void OnReceived()
        {
            if (SuperSonicHandler.instance)
            {
                SuperSonicHandler.instance.OnTransform();
            }

        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
        }
    }

}