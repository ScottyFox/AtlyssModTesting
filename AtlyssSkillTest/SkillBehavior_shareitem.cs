using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
public class SkillBehavior_shareitem : SkillObjectBehavior
{
    private StatusEntity _statusEntity;
    [Header("Share Item Skill Object")]
    [SerializeField]
    HashSet<string> allowedConsumables = new HashSet<string> {
        "Lesser Defense Potion",
        "Lesser Evasion Potion",
        "Lesser Regeneration Potion",
        "Lesser Mgk. Defense Potion",
        "Defense Potion",
        "Evasion Potion",
        "Mgk. Defense Potion",
        "Regeneration Potion",
        "Bunbag",
        "Bunjar",
        "Bunpot",
        "Carrot Cake",
        "Magiclove",
        "Magiflower",
        "Magileaf",
        "Stamstar",
        "Minchroom Juice"};
    HashSet<ConsumableBuffer> activeBuffers = new HashSet<ConsumableBuffer>();
    public override void Init_SkillBehavior()
    {
    }

    public override void End_SkillBehavior()
    {
    }

    public override void Update_SkillBehavior()
    {
        //Debug.Log("Updating Skill Behaviour");
        //if (!((bool)_skillObject._parentPlayer && _skillObject._parentPlayer.isServer))
        //    return;
        _statusEntity = _skillObject._parentPlayer.GetComponent<StatusEntity>();
        PlayerInventory _inventory = _skillObject._parentPlayer.GetComponent<PlayerInventory>();
        foreach (ConsumableBuffer consumableBuffer in _inventory._consumableBuffers)
        {
            if (!Is_Valid_ConsumableBuffer(consumableBuffer))
                continue;
            Share_ConsumableBuffer_With_Party(consumableBuffer);
        }
    }
    public void Share_ConsumableBuffer_With_Party(ConsumableBuffer consumableBuffer)
    {
        ScriptableConsumable consumable = consumableBuffer._scriptableConsumable;
        foreach (Player partyPeer in _skillObject._parentPlayer._partyObject._syncPartyPeers)
        {
            if (partyPeer == _skillObject._parentPlayer)
                continue;
            //THIS MAY HAPPEN INFINITELY IF OTHERS HAVE THIS BUFF, MAYBE MODIFY VALUES DIRECTLY AS HOST//
            PlayerInventory Peerinventory = partyPeer.GetComponent<PlayerInventory>();
            StatusEntity Peerstatus = partyPeer.GetComponent<StatusEntity>();
            consumable.Use_Consumable(Peerstatus);
        }
    }
    public bool Is_Valid_ConsumableBuffer(ConsumableBuffer consumableBuffer)
    {
        if (!allowedConsumables.Contains(consumableBuffer._scriptableConsumable._itemName))
            return false;
        if (activeBuffers.Contains(consumableBuffer))
            return false;
        if (consumableBuffer._bufferTimer <= 0)
            return false;
        activeBuffers.Add(consumableBuffer);
        return true;
    }
    public void CleanUpConsumableBuffers()
    {
        foreach(var consumableBuffer in activeBuffers)
        {
            if(consumableBuffer._bufferTimer <= 0)
            {
                activeBuffers.Remove(consumableBuffer);
            }
        }
        //Unsure if buffers ever properly get removed, might lead to a memory leak?
    }
}
