using Newtonsoft.Json;
using ScottyFoxArt.UnityBundler.Utilities;
using System.Collections.Generic;
using UnityEngine;
namespace ScottyFoxArt.UnityBundler.Json
{
    public class ComponentJsonSerializer : MonoBehaviour
    {
        [HideInInspector]
        public string serializedMode;
        [Header("Settings")]
        public JsonBehaviourSerializer_Mode selectedMode = JsonBehaviourSerializer_Mode.SerializeAll;
        [Header("Options")]
        [SerializeField]
        public Component target;
        [SerializeField]
        public string targetType;
        public List<JsonFieldModification> Modifications;
        [SerializeField, HideInInspector]
        private string targetFieldInfo;
        [SerializeField, HideInInspector]
        private string modificationsJson;
        public void DeserializeJsonInfo()
        {
            Debug.Log("Attempting To Deserialize");
            if (target != null)
            {
                if (target.GetType().Name != targetType)
                {
                    Object.DestroyImmediate(target);
                    target = null;
                }
            }
            if (target == null)
            {
                var type = ReflectionUtils.Fetch_Type_By_Name(targetType);
                if (type == null)
                    return;
                target = gameObject.AddComponent(type);
            }
            if (target == null)
                return;
            if (selectedMode == JsonBehaviourSerializer_Mode.SerializeAll)
            {
                if (!string.IsNullOrEmpty(targetFieldInfo))
                {
                    JsonConvert.PopulateObject(targetFieldInfo, target, UnityBundler.JsonSettings);
                }
                return;
            }
            if (!string.IsNullOrEmpty(modificationsJson))
            {
                JsonConvert.PopulateObject(modificationsJson, target, UnityBundler.JsonSettings);
            }
            Component.DestroyImmediate(this);
        }
    }
    public enum JsonBehaviourSerializer_Mode
    {
        SerializeAll,
        ModificationsOnly
    }
    [System.Serializable]
    public class JsonFieldModification
    {
        public string field;
        public string value;
    }
}