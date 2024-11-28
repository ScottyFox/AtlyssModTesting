namespace AtlyssHelperUtils
{
    public class StatUtils
    {
        public static StatLogics StatLogics { get { return GameManager._current._statLogics; } }
        public static int Creep_CurrencyDrop(ScriptableCreep creep)
        {
            return (int)(StatLogics._currencyDropCurve.Evaluate((float)(creep._creepLevel - 1)) + creep._currencyDropBonus);
        }
        public static int Creep_Experience(ScriptableCreep creep,int player_count = 1, StatStruct stats = default)
        {
            float percentage = 1;
            switch (player_count)
            {
                case 2:
                    percentage = 0.85f;
                    break;
                case 3:
                    percentage = 0.65f;
                    break;
                case 4:
                    percentage = 0.45f;
                    break;
                case 5:
                    percentage = 0.35f;
                    break;
            }
            if(object.Equals(stats, default(StatStruct)))
            {
                stats = creep._creepStatStruct;
            }
            return (int)((GameManager._current._statLogics._experienceGainCurve.Evaluate((float)creep._creepLevel) + stats._experience) * percentage);
        }

        public static StatStruct Creep_Calculate_StatStruct(ScriptableCreep creep,int player_count = 1)
        {
            StatLogics statLogics = GameManager._current._statLogics;
            StatStruct statStruct = default(StatStruct);
            float StatModifierPercentage = 0f;
            float experiencePercentage = 0f;
            float MaxHealthBoostPercentage = 0f;
            int MaxHealth = (int)statLogics._maxHealthCurve.Evaluate((float)(creep._creepLevel - 1)) + creep._creepStatStruct._maxHealth;
            int MaxMana = (int)statLogics._maxManaCurve.Evaluate((float)(creep._creepLevel - 1)) + creep._creepStatStruct._maxMana;
            int MaxStamina = (int)statLogics._maxStaminaCurve.Evaluate((float)(creep._creepLevel - 1)) + creep._creepStatStruct._maxStamina;
            int Experience = creep._creepStatStruct._experience;
            /*if (this._scriptStatModifier)
            {
                statStruct = this._scriptStatModifier._modifierStatStruct;
                StatModifierPercentage = (float)this._creepLevel * this._scriptStatModifier._creepStatModifierPercentage;
                experiencePercentage = (float)this._creepLevel * this._scriptStatModifier._experiencePercentage;
                MaxHealthBoostPercentage = (float)this._creepLevel * this._scriptStatModifier._creepMaxHealthBoostPercentage;
                this.Network_statModifierTag = this._scriptStatModifier._modifierTag;
                if (this._scriptStatModifier._overrideDamageType)
                {
                    this._creepDamageType = this._scriptStatModifier._damageType;
                }
                else
                {
                    this._creepDamageType = creep._creepDamageType;
                }
            }
            else
            {
                this._creepDamageType = creep._creepDamageType;
                this.Network_statModifierTag = string.Empty;
            }*/
            int PlayersWithinSpawnerRadius = player_count;
            /*if (this._parentCreepSpawner)
            {
                PlayersWithinSpawnerRadius = this._parentCreepSpawner._playersWithinSpawnerRadius.Count;
            }
            if (this._parentCreepSpawner._creepArena && this._parentCreepSpawner._creepArena._patternInstanceManager && this._parentCreepSpawner._creepArena._patternInstanceManager.Network_instanceParty)
            {
                PlayersWithinSpawnerRadius = this._parentCreepSpawner._creepArena._patternInstanceManager.Network_instanceParty._syncPartyPeers.Count;
            }*/
            if (PlayersWithinSpawnerRadius > 1)
            {
                int BonusHealth = (int)((float)MaxHealth * GameManager._current._statLogics._creepMaxHealthModifier) * PlayersWithinSpawnerRadius;
                MaxHealth += BonusHealth;
                int BonusExperience = (int)((float)Experience * GameManager._current._statLogics._creepExperienceModifier) * PlayersWithinSpawnerRadius;
                Experience += BonusExperience;
            }
            if (MaxHealth <= 0)
            {
                MaxHealth = 1;
            }
            if (MaxMana <= 0)
            {
                MaxMana = 1;
            }
            if (MaxStamina <= 0)
            {
                MaxStamina = 1;
            }
            StatStruct network_statStruct = new StatStruct
            {
                _maxHealth = MaxHealth + (int)((float)statStruct._maxHealth * StatModifierPercentage) + (int)((float)MaxHealth * MaxHealthBoostPercentage),
                _maxMana = MaxMana + (int)((float)statStruct._maxMana * StatModifierPercentage),
                _maxStamina = MaxStamina + (int)((float)statStruct._maxStamina * StatModifierPercentage),
                _experience = Experience + (int)((float)statStruct._experience * experiencePercentage),
                _attackPower = creep._creepStatStruct._attackPower + (int)((float)statStruct._attackPower * StatModifierPercentage),
                _defense = creep._creepStatStruct._defense + (int)((float)statStruct._defense * StatModifierPercentage),
                _criticalRate = creep._creepStatStruct._criticalRate + statStruct._criticalRate * StatModifierPercentage,
                _magicPower = creep._creepStatStruct._magicPower + (int)((float)statStruct._magicPower * StatModifierPercentage),
                _dexPower = creep._creepStatStruct._dexPower + (int)((float)statStruct._dexPower * StatModifierPercentage),
                _magicDefense = creep._creepStatStruct._magicDefense + (int)((float)statStruct._magicDefense * StatModifierPercentage),
                _magicCriticalRate = creep._creepStatStruct._magicCriticalRate + statStruct._magicCriticalRate * StatModifierPercentage,
                _evasion = statLogics._baseEvasion + creep._creepStatStruct._evasion + statStruct._evasion * StatModifierPercentage
            };
            //this.Network_statStruct = network_statStruct;
            //this._combatElement = creep._combatElement;
            //this._statusEntity.Network_currentHealth = this._statStruct._maxHealth;
            //this._statusEntity.Network_currentMana = this._statStruct._maxMana;
            return network_statStruct;
        }
    }
}