using AtlyssHelperUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using ScottyFoxArt.UnityBundler;
using ScottyFoxArt.UnityBundler.Utilities;
namespace AtlyssBundler
{
    public static class Registry
    {
        private static HashSet<UnityEngine.Object> Cached_Objects = new HashSet<UnityEngine.Object>();
        public static void Clear_Cache()
        { Cached_Objects.Clear(); }
        //
        public const string ATLYSS_ITEMS = "_ITEM/";
        public const string ATLYSS_CREEP = "_PREFAB/_ENTITY/_CREEP/";
        public const string ATLYSS_QUEST = "";
        public const string ATLYSS_STATMODIFIER = "_ITEM/01_EQUIPMENT/00_MODIFIER";
        public const string ATLYSS_CONDITION = "";
        public const string ATLYSS_PLAYERRACE = "_RACE/";
        public const string ATLYSS_COMBATELEMENT = "_COMBATELEMENT/";
        public const string ATLYSS_PLAYERBASECLASS = "_CLASS/";
        public const string ATLYSS_SKILL = "_SKILL/";
        public const string ATLYSS_DIALOG = "";
        public static HashSet<Type> ATLYSS_ITEM_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableItem));
        public static HashSet<Type> ATLYSS_EQUIPMENT_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableEquipment));
        public static HashSet<Type> ATLYSS_CREEP_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableCreep));
        public static HashSet<Type> ATLYSS_QUEST_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableQuest));
        public static HashSet<Type> ATLYSS_STATMODIFIER_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableStatModifier));
        public static HashSet<Type> ATLYSS_CONDITION_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableCondition));
        public static HashSet<Type> ATLYSS_PLAYERRACE_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptablePlayerRace));
        public static HashSet<Type> ATLYSS_COMBATELEMENT_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableCombatElement));
        public static HashSet<Type> ATLYSS_PLAYERBASECLASS_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptablePlayerBaseClass));
        public static HashSet<Type> ATLYSS_SKILL_TYPES = ReflectionUtils.Fetch_Subtypes(typeof(ScriptableSkill));
        public static Dictionary<string, Func<string, string>> Atlyss_ID_Getters = new()
        {
            {"ScriptableItem", Get_Item_ID},
            {"ScriptableCreep", Get_Creep_ID},
            {"ScriptableQuest", Get_Quest_ID},
            {"ScriptableStatModifier", Get_StatModifier_ID},
            {"ScriptableCondition", Get_Condition_ID},
            {"ScriptablePlayerRace", Get_Race_ID},
            {"ScriptableCombatElement", Get_CombatElement_ID},
            {"ScriptablePlayerBaseClass", Get_PlayerClass_ID},
            {"ScriptableSkill", Get_Skill_ID}
        };
        public static Dictionary<string, Func<string, UnityEngine.Object>> Atlyss_Getters = new()
        {
            {"ScriptableItem", Get_Item},
            {"ScriptableCreep", Get_Creep},
            {"ScriptableQuest", Get_Quest},
            {"ScriptableStatModifier", Get_StatModifier},
            {"ScriptableCondition", Get_Condition},
            {"ScriptablePlayerRace", Get_Race},
            {"ScriptableCombatElement", Get_CombatElement},
            {"ScriptablePlayerBaseClass", Get_PlayerClass},
            {"ScriptableSkill", Get_Skill},
            {"AudioMixerGroup", Get_AudioMixerGroup}
        };
        public static Dictionary<string, Action<UnityEngine.Object>> Atlyss_Registerers = new()
        {
            {"ScriptableItem", Register_Item},
            {"ScriptableCreep", Register_Creep},
            {"ScriptableQuest", Register_Quest},
            {"ScriptableStatModifier", Register_StatModifier},
            {"ScriptableCondition", Register_Condition},
            {"ScriptablePlayerRace", Register_Race},
            {"ScriptableCombatElement", Register_CombatElement},
            {"ScriptablePlayerBaseClass", Register_PlayerClass},
            {"ScriptableSkill", Register_Skill}
        };
        //TODO Might Not be neccesary, Probably Do a Base-Class or IsAssignableFrom Check instead.
        //If IsAssignableFrom is used, switch from String name To Types instead.
        public static bool Auto_Register_Types()
        {
            try
            {
                foreach (var type in ATLYSS_ITEM_TYPES)
                    Bulk_Register_Type("ScriptableItem", type.Name);
                foreach (var type in ATLYSS_EQUIPMENT_TYPES)
                    Bulk_Register_Type("ScriptableEquipment", type.Name);
                foreach (var type in ATLYSS_CREEP_TYPES)
                    Bulk_Register_Type("ScriptableCreep", type.Name);
                foreach (var type in ATLYSS_QUEST_TYPES)
                    Bulk_Register_Type("ScriptableQuest", type.Name);
                foreach (var type in ATLYSS_STATMODIFIER_TYPES)
                    Bulk_Register_Type("ScriptableStatModifier", type.Name);
                foreach (var type in ATLYSS_CONDITION_TYPES)
                    Bulk_Register_Type("ScriptableCondition", type.Name);
                foreach (var type in ATLYSS_PLAYERRACE_TYPES)
                    Bulk_Register_Type("ScriptablePlayerRace", type.Name);
                foreach (var type in ATLYSS_COMBATELEMENT_TYPES)
                    Bulk_Register_Type("ScriptableCombatElement", type.Name);
                foreach (var type in ATLYSS_PLAYERBASECLASS_TYPES)
                    Bulk_Register_Type("ScriptablePlayerBaseClass", type.Name);
                foreach (var type in ATLYSS_SKILL_TYPES)
                    Bulk_Register_Type("ScriptableSkill", type.Name);
            }
            catch
            {
                Debug.LogError("Error Auto Registering Atlyss Types");
                return false;
            }
            return true;
        }
        private static void Bulk_Register_Type(string basetype, string type)
        {
            Debug.Log($"Registering \"{type}\" as \"{basetype}\"");
            Atlyss_ID_Getters[type] = Atlyss_ID_Getters[basetype];
            Atlyss_Getters[type] = Atlyss_Getters[basetype];
            Atlyss_Registerers[type] = Atlyss_Registerers[basetype];
        }
        public static void Register_With_UnityBundler()
        {
            BundleRegistry.Register_TypeRegisterer_Dict(Atlyss_Registerers);
            BundleRegistry.Register_Reference_Fetcher_Dict(Atlyss_Getters);
        }
        public static T Fetch_Resource<T>(string id, string path = "", bool cache = true) where T : UnityEngine.Object
        {
            var resources = Resources.LoadAll(path, typeof(T));
            var lowerId = id.ToLower();
            foreach (var resource in resources)
            {
                var name = resource.name.ToLower();
                if (name.EndsWith(lowerId) || name.StartsWith(lowerId))
                {
                    if (cache && resource)
                        Cached_Objects.Add(resource);
                    return resource as T;
                }
            }
            return null;
        }
        //TODO Modify this to use Resources Probably?
        private static AudioMixerGroup Get_AudioMixerGroup(string mixerGroup)
        {
            Debug.Log($"AudioMixerGroup -> {mixerGroup}");
            AudioMixer mixerMain = SettingsManager._current._masterMixer;
            return mixerMain?.FindMatchingGroups(mixerGroup).FirstOrDefault();
        }
        //TODO Modify this to use User-Defined Scenes
        public static string Get_SceneName(string sceneid)
        {
            var numScenes = SceneManager.sceneCountInBuildSettings;
            var lowerId = sceneid.ToLower();
            for (int i = 0; i < numScenes; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i).ToLower();
                if (scenePath.EndsWith(lowerId) || scenePath.StartsWith(lowerId))
                    return scenePath;
            }
            return null;
        }
        //For Loose Dictionary Searching
        public static string Fetch_Loose_ID<T>(string id, Dictionary<string, T> dict)
        {
            var lowerId = id.ToLower();
            foreach (var key in dict.Keys)
            {
                var lowerKey = key.ToLower();
                if (lowerKey.EndsWith(lowerId) || lowerKey.StartsWith(lowerId))
                    return key;
            }
            return null;
        }
        public static string Get_CombatElement_ID(string id)
        {
            var combatElements = AtlyssUtils.GetCombatElements(GameManager._current);
            return Fetch_Loose_ID(id, combatElements);
        }
        public static ScriptableCombatElement Get_CombatElement(string id)
        {
            var combatElements = AtlyssUtils.GetCombatElements(GameManager._current);
            combatElements.TryGetValue(id, out var combatElement);
            return combatElement;
        }
        public static void Register_CombatElement(object obj)
        {
            if (obj is not ScriptableCombatElement combatElement)
                return;
            var combatElements = AtlyssUtils.GetCombatElements(GameManager._current);
            combatElements[combatElement._elementName] = combatElement;
        }
        public static string Get_Condition_ID(string id)
        {
            var conditions = AtlyssUtils.GetConditions(GameManager._current);
            return Fetch_Loose_ID(id, conditions);
        }
        public static ScriptableCondition Get_Condition(string id)
        {
            var conditions = AtlyssUtils.GetConditions(GameManager._current);
            conditions.TryGetValue(id, out var condition);
            return condition;
        }
        public static void Register_Condition(object obj)
        {
            if (obj is not ScriptableCondition condition)
                return;
            var conditions = AtlyssUtils.GetConditions(GameManager._current);
            conditions[$"{condition._conditionName}_{condition._conditionRank}"] = condition;
        }
        public static string Get_Creep_ID(string id)
        {
            var Creeps = AtlyssUtils.GetCreeps(GameManager._current);
            return Fetch_Loose_ID(id, Creeps);
        }
        public static ScriptableCreep Get_Creep(string id)
        {
            var creeps = AtlyssUtils.GetCreeps(GameManager._current);
            creeps.TryGetValue(id, out var creep);
            return creep;
        }
        public static void Register_Creep(object obj)
        {
            if (obj is not ScriptableCreep creep)
                return;
            var creeps = AtlyssUtils.GetCreeps(GameManager._current);
            creeps[creep._creepName] = creep;
        }
        public static string Get_Item_ID(string id)
        {
            var Items = AtlyssUtils.GetItems(GameManager._current);
            return Fetch_Loose_ID(id, Items);
        }
        public static ScriptableItem Get_Item(string id)
        {
            var items = AtlyssUtils.GetItems(GameManager._current);
            items.TryGetValue(id, out var item);
            return item;
        }
        public static void Register_Item(object obj)
        {
            if (obj is not ScriptableItem item)
                return;
            var items = AtlyssUtils.GetItems(GameManager._current);
            items[item._itemName] = item;
        }
        public static string Get_PlayerClass_ID(string id)
        {
            var playerClasses = AtlyssUtils.GetPlayerClasses(GameManager._current);
            return Fetch_Loose_ID(id, playerClasses);
        }
        public static ScriptablePlayerBaseClass Get_PlayerClass(string id)
        {
            var playerClasses = AtlyssUtils.GetPlayerClasses(GameManager._current);
            playerClasses.TryGetValue(id, out var playerClass);
            return playerClass;
        }
        public static void Register_PlayerClass(object obj)
        {
            if (obj is not ScriptablePlayerBaseClass playerClass)
                return;
            var playerClasses = AtlyssUtils.GetPlayerClasses(GameManager._current);
            playerClasses[playerClass._className] = playerClass;
        }
        public static string Get_Quest_ID(string id)
        {
            var quests = AtlyssUtils.GetQuests(GameManager._current);
            return Fetch_Loose_ID(id, quests);
        }
        public static ScriptableQuest Get_Quest(string id)
        {
            var quests = AtlyssUtils.GetQuests(GameManager._current);
            quests.TryGetValue(id, out var quest);
            return quest;
        }
        public static void Register_Quest(object obj)
        {
            if (obj is not ScriptableQuest quest)
                return;
            var quests = AtlyssUtils.GetQuests(GameManager._current);
            quests[quest._questName] = quest;
        }
        public static string Get_Race_ID(string id)
        {
            var races = AtlyssUtils.GetRaces(GameManager._current);
            return Fetch_Loose_ID(id, races);
        }
        public static ScriptablePlayerRace Get_Race(string id)
        {
            var races = AtlyssUtils.GetRaces(GameManager._current);
            races.TryGetValue(id, out var race);
            return race;
        }
        public static void Register_Race(object obj)
        {
            if (!(obj is ScriptablePlayerRace race))
                return;
            var races = AtlyssUtils.GetRaces(GameManager._current);
            races[race._miscName] = race;
        }
        public static string Get_Skill_ID(string id)
        {
            var skills = AtlyssUtils.GetSkills(GameManager._current);
            return Fetch_Loose_ID(id, skills);
        }
        public static ScriptableSkill Get_Skill(string id)
        {
            var skills = AtlyssUtils.GetSkills(GameManager._current);
            skills.TryGetValue(id, out var skill);
            return skill;
        }
        public static void Register_Skill(object obj)
        {
            if (!(obj is ScriptableSkill skill))
                return;
            var skills = AtlyssUtils.GetSkills(GameManager._current);
            skills[skill._skillName] = skill;
        }
        public static string Get_StatModifier_ID(string id)
        {
            var statModifiers = AtlyssUtils.GetStatModifiers(GameManager._current);
            return Fetch_Loose_ID(id, statModifiers);
        }
        public static ScriptableStatModifier Get_StatModifier(string id)
        {
            var statModifiers = AtlyssUtils.GetStatModifiers(GameManager._current);
            statModifiers.TryGetValue(id, out var statModifier);
            return statModifier;
        }
        public static void Register_StatModifier(object obj)
        {
            if (!(obj is ScriptableStatModifier statModifier))
                return;
            var statModifiers = AtlyssUtils.GetStatModifiers(GameManager._current);
            statModifiers[statModifier._modifierTag] = statModifier;
        }
        //
        public static ScriptableDialogData Fetch_DialogData(string id, bool cache = true)
        {
            return Fetch_Resource<ScriptableDialogData>(id, ATLYSS_DIALOG, cache);
        }
        //TODO, Probably Remove GameObject checks, May not be Neccesary.
        public static void Auto_Register_GameObject(GameObject gameObject)
        {
            var type = Verify_GameObject_Type(gameObject);
            switch (type)
            {
                default:
                    break;
            }
        }
        public static T ComponentDeepCheck<T>(GameObject gameObject)
        {
            var component = gameObject.GetComponent<T>();
            if (component == null)
                component = gameObject.GetComponentInChildren<T>();
            return component;
        }
        public static string Verify_GameObject_Type(GameObject gameObject)
        {
            //SKILL OBJECTS//
            if (ComponentDeepCheck<ConditionObject>(gameObject))
                return "ConditionObject";
            if (ComponentDeepCheck<ProjectileObject>(gameObject))
                return "ProjectileObject";
            if (ComponentDeepCheck<SkillObject>(gameObject))
                return "SkillObject";
            //
            return "GameObject";
        }
    }
}