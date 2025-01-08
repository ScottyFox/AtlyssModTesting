using System;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
namespace ScottyFoxArt.UnityBundler.Utilities
{
    public static class ReflectionUtils
    {
        public static HashSet<Type> Fetch_Subtypes(Type t)
        {
            var Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var t_assembly = t.Assembly;
            HashSet<Type> result = new HashSet<Type>();
            foreach (var assembly in Assemblies)
                if (assembly == t_assembly || assembly.GetReferencedAssemblies().Contains(t_assembly.GetName()))
                    foreach (var item in assembly.GetTypes())
                        if (item == t || item.IsSubclassOf(t))
                            result.Add(item);
            return result;
        }
        public static Type Fetch_Type_By_Name(string typeName)
        {
            //Check Locally First
            var type = Type.GetType(typeName, false, true);
            if (type != null)
                return type;
            //Check Each Assembly...
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false, true);
                if (type != null)
                    return type;
            }
            //Finally, Probably not neccesary, we can do the direct check.
            return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);
        }
        public static object Create_Instance(Type type, params object[] parameters)
        {
            object instance = null;
            try
            {
                // Handle ScriptableObject
                if (typeof(ScriptableObject).IsAssignableFrom(type))
                    instance = ScriptableObject.CreateInstance(type);
                // Everything Else
                else
                {
                    //Possibly Merge this into a single Activator CreateInstance
                    if (parameters.Length == 0)
                        instance = Activator.CreateInstance(type);
                    else
                        instance = Activator.CreateInstance(type, parameters);
                }
            }
            catch (Exception)
            {
                throw new NotSupportedException($"Failed to create an instance of type {type.FullName}");
            }
            return instance;
        }
        public static T Create_Instance<T>(params object[] parameters)
        {
            return (T)Create_Instance(typeof(T), parameters);
        }
        public static object Create_Instance_JSON(Type type, string json, JsonSerializerSettings settings = null)
        {
            object obj = null;
            try
            {
                obj = Create_Instance(type);
                Populate_From_JSON(obj, json, settings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);

            }
            return obj;
        }
        public static T Create_Instance_JSON<T>(string json, JsonSerializerSettings settings = null)
        {
            return (T)Create_Instance_JSON(typeof(T), json, settings);
        }
        public static void Populate_From_JSON(object obj, string json, JsonSerializerSettings settings = null)
        {
            JsonConvert.PopulateObject(json, obj, settings);
        }
    }
}
