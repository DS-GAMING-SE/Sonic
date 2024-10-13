using RoR2;
using R2API.Networking;
using R2API.Networking.Interfaces;
using UnityEngine.Networking;
using UnityEngine;
using SonicTheHedgehog.SkillStates;
using EntityStates;
using SonicTheHedgehog.Modules;
using SonicTheHedgehog.Modules.Forms;

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
            Log.Message("Parry Recieved Client");
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
            Log.Message("Scepter Boost Damage Received Server");
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

        FormIndex formIndex;

        public SuperSonicTransform()
        {

        }

        public SuperSonicTransform(NetworkInstanceId netId, FormIndex formIndex)
        {
            this.netId = netId;
            this.formIndex = formIndex;
        }

        public void OnReceived()
        {
            GameObject body = Util.FindNetworkObject(netId);
            if (!body) { return; }
            if (Forms.formToHandlerObject.TryGetValue(Forms.GetFormDef(formIndex), out GameObject handlerObject))
            {
                FormHandler handler = handlerObject.GetComponent(typeof(FormHandler)) as FormHandler;
                handler.OnTransform(body.GetComponent<SuperSonicComponent>());
            }

        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(netId);
            writer.Write(formIndex);
        }

        public void Deserialize(NetworkReader reader)
        {
            netId = reader.ReadNetworkId();
            formIndex = reader.ReadFormIndex();
        }
    }

    public static class Extensions
    {
        public static void Write(this NetworkWriter writer, FormIndex formIndex)
        {
            writer.WritePackedIndex32((int)formIndex);
        }

        public static FormIndex ReadFormIndex(this NetworkReader reader)
        {
            return (FormIndex)reader.ReadPackedIndex32();
        }
    }

}