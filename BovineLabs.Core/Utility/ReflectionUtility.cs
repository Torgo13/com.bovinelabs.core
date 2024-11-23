﻿// <copyright file="ReflectionUtility.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Unity.Collections;
#if UNITY_EDITOR
    using UnityEditor.Compilation;
    using Assembly = UnityEditor.Compilation.Assembly;
    using AssemblyFlags = UnityEditor.Compilation.AssemblyFlags;
#endif

    /// <summary> Common reflection helpers. </summary>
    public static class ReflectionUtility
    {
        private static System.Reflection.Assembly[]? allAssemblies;
        private static Type[]? allTypes;
        private static Type[]? allTypesWithImplementation;
        private static Type[]? allTypesWithImplementationNoGeneric;
        private static MethodInfo[]? allMethods;

#if UNITY_EDITOR
        private static Dictionary<string, Assembly> AssembliesMap { get; }
            = CompilationPipeline.GetAssemblies().ToDictionary(r => r.name, r => r);
#endif

        private static System.Reflection.Assembly[] AllAssemblies => allAssemblies ??= AppDomain.CurrentDomain.GetAssemblies();

        private static Type[] AllTypes => allTypes ??= AllAssemblies.SelectMany(s => s.GetTypes()).ToArray();

        private static Type[] AllTypesWithImplementation => allTypesWithImplementation ??= AllTypes.Where(t => !t.IsAbstract && !t.IsInterface).ToArray();

        private static Type[] AllTypesWithImplementationNoGeneric => allTypesWithImplementationNoGeneric ??= AllTypesWithImplementation.Where(t => !t.ContainsGenericParameters).ToArray();

        private static MethodInfo[] AllPublicMethods => allMethods ??= AllTypes.SelectMany(s => s.GetMethods()).ToArray();

        /// <summary> Searches all assemblies to find all types that implement a type. </summary>
        /// <typeparam name="T"> The base type that is inherited from. </typeparam>
        /// <returns> All the types. </returns>
        public static IEnumerable<T> GetAllAssemblyAttributes<T>()
            where T : Attribute
        {
            return AllAssemblies
                .SelectMany(s => s.GetCustomAttributes(typeof(T), true))
                .Cast<T>();
        }

        /// <summary> Finds an implementation of an interface T in the all assemblies, falling back on TD if no others found. </summary>
        /// <typeparam name="T"> The interface to fall back on. </typeparam>
        /// <typeparam name="TD"> The type of the default implementation. </typeparam>
        /// <returns> The implementation found. null if default is not set and none were found. </returns>
        /// <exception cref="ArgumentException"> Type is valid. </exception>
        /// <exception cref="InvalidOperationException"> Implementation not found. </exception>
        public static T? GetCustomImplementation<T, TD>()
            where TD : T
        {
            return GetCustomImplementation<T>(typeof(TD));
        }

        /// <summary> Finds an implementation of an interface T in the all assemblies. </summary>
        /// <typeparam name="T"> The interface to fall back on. </typeparam>
        /// <returns> The implementation found. null if default is not set and none were found. </returns>
        /// <exception cref="ArgumentException"> Type is valid. </exception>
        /// <exception cref="InvalidOperationException"> Implementation not found. </exception>
        public static T? GetCustomImplementation<T>()
        {
            return GetCustomImplementation<T>(null);
        }

        /// <summary>
        /// Gets all types that inherit and have a generic definition of itself.
        /// class SettingsBase{T} : where T : SettingsBase{T} for example.
        /// </summary>
        /// <param name="type"> The generic type definitions. </param>
        /// <returns> An enumeration of the types. </returns>
        public static IEnumerable<Type> GetAllWithGenericDefinition(Type type)
        {
            var types = from t in AllTypesWithImplementation
                let i = t.BaseType
                where (i != null) && i.IsGenericType &&
                      (i.GetGenericTypeDefinition() == type) &&
                      (i.GetGenericArguments()[0] == t) // must equal itself
                select t;

            return types;
        }

        /// <summary> Searches all assemblies to find all types that implement a type. </summary>
        /// <param name="type"> The base type that is inherited from. </param>
        /// <returns> All the types. </returns>
        public static IEnumerable<Type> GetAllImplementations(Type type)
        {
            return AllTypesWithImplementation
                .Where(t => t != type)
                .Where(type.IsAssignableFrom);
        }

        /// <summary> Searches all assemblies to find all types that implement a type. </summary>
        /// <typeparam name="T"> The base type that is inherited from. </typeparam>
        /// <returns> All the types. </returns>
        public static IEnumerable<Type> GetAllImplementations<T>()
            where T : class
        {
            var type = typeof(T);

            return AllTypesWithImplementation
                .Where(t => t != type)
                .Where(type.IsAssignableFrom);
        }

        public static IEnumerable<Type> GetAllOpenGenericImplementations(Type type)
        {
            return AllTypesWithImplementation
                .Where(s =>
                {
                    var baseType = s.BaseType;
                    return (baseType is { IsGenericType: true } && type.IsAssignableFrom(baseType.GetGenericTypeDefinition())) ||
                           s.GetInterfaces().Any(z => z.IsGenericType && type.IsAssignableFrom(z.GetGenericTypeDefinition()));
                });
        }

        /// <summary> Searches all assemblies to find all types that have an attribute. </summary>
        /// <typeparam name="T"> The attribute to search for. </typeparam>
        /// <returns> All the types. </returns>
        public static IEnumerable<Type> GetAllWithAttribute<T>()
            where T : Attribute
        {
            return AllTypes.Where(t => t.GetCustomAttribute<T>() != null);
        }

        public static IEnumerable<MethodInfo> GetAllMethodsWithAttribute<T>()
            where T : Attribute
        {
            return AllPublicMethods.Where(t => t.GetCustomAttribute<T>() != null);
        }

        /// <summary> Searches all assemblies to find all types that implement both 2 types. </summary>
        /// <typeparam name="T1"> The first base type that is inherited from. </typeparam>
        /// <typeparam name="T2"> The second base type that is inherited from. </typeparam>
        /// <returns> All the types that inherit both types. </returns>
        public static IEnumerable<Type> GetAllImplementations<T1, T2>()
            where T1 : class
            where T2 : class
        {
            var type1 = typeof(T1);
            var type2 = typeof(T2);

            return AllTypesWithImplementationNoGeneric
                .Where(t => (t != type1) && (t != type2))
                .Where(t => type1.IsAssignableFrom(t) && type2.IsAssignableFrom(t));
        }

        /// <summary> Searches all assemblies to find all types that implement both 2 types but only keep top level inheritance. </summary>
        /// <typeparam name="T1"> The first base type that is inherited from. </typeparam>
        /// <typeparam name="T2"> The second base type that is inherited from. </typeparam>
        /// <returns> All the types that inherit both types. </returns>
        public static IEnumerable<Type> GetAllImplementationsRootOnly<T1, T2>()
            where T1 : class
            where T2 : class
        {
            var type1 = typeof(T1);
            var type2 = typeof(T2);

            var all = AllTypesWithImplementationNoGeneric
                .Where(t => (t != type1) && (t != type2))
                .Where(t => type1.IsAssignableFrom(t) && type2.IsAssignableFrom(t))
                .ToList();

            // Remove anything that has been been inherited from
            for (var i = all.Count - 1; i >= 0; i--)
            {
                var testing = all[i];
                if (all.Any(t => (t != testing) && testing.IsAssignableFrom(t)))
                {
                    all.RemoveAtSwapBack(i);
                }
            }

            return all;
        }

        /// <summary> Checks if an assembly is referencing another assembly. the name of all assemblies with a specific reference. </summary>
        /// <param name="assembly"> The assembly to check. </param>
        /// <param name="reference"> The reference to check if the assembly has. </param>
        /// <returns> True if referencing. </returns>
        public static bool IsAssemblyReferencingAssembly(this System.Reflection.Assembly assembly, System.Reflection.Assembly reference)
        {
            if (assembly == reference)
            {
                return true;
            }

            var referenceName = reference.GetName().FullName;
            return assembly.GetReferencedAssemblies().Any(referenced => referenced.FullName == referenceName);
        }

#if UNITY_EDITOR
        /// <summary> Gets the name of all assemblies with a specific reference. Ignores editor assemblies. </summary>
        /// <param name="reference"> The reference. </param>
        /// <returns> The name of all the assemblies. </returns>
        public static IEnumerable<string> GetAllAssemblyNamesWithReference(System.Reflection.Assembly reference)
        {
            return GetAllUnityAssembliesWithReference(reference).Select(a => a.name);
        }

        /// <summary> Checks if an assembly is referencing another assembly. </summary>
        /// <param name="assembly"> The assembly to check. </param>
        /// <param name="reference"> The reference to check if the assembly has. </param>
        /// <returns> True if referencing. </returns>
        public static bool IsAssemblyReferencingAssembly(this Assembly assembly, Assembly reference)
        {
            return (assembly == reference) || assembly.assemblyReferences.Any(referenced => referenced.name == reference.name);
        }

        /// <summary> Gets the name of all assemblies with a specific reference. Ignores editor assemblies. </summary>
        /// <param name="reference"> The reference. </param>
        /// <returns> The name of all the assemblies. </returns>
        public static IEnumerable<Assembly> GetAllUnityAssembliesWithReference(System.Reflection.Assembly reference)
        {
            var refName = reference.GetName().Name;
            var refAsm = AssembliesMap[refName];

            return GetAllUnityAssembliesWithReference(refAsm);
        }

        /// <summary> Gets the name of all assemblies with a specific reference. Ignores editor assemblies. </summary>
        /// <param name="reference"> The reference. </param>
        /// <returns> The name of all the assemblies. </returns>
        public static IEnumerable<Assembly> GetAllUnityAssembliesWithReference(Assembly reference)
        {
            return AssembliesMap.Values
                .Where(asm => (asm.flags & AssemblyFlags.EditorAssembly) == 0)
                .Where(asm => IsAssemblyReferencingAssembly(asm, reference));
        }

        /// <summary> Checks if an assembly is an editor only assembly. </summary>
        /// <param name="asm"> The assembly to check. </param>
        /// <returns> True if editor assembly. </returns>
        public static bool IsAssemblyEditorAssembly(this System.Reflection.Assembly asm)
        {
            if (!AssembliesMap.TryGetValue(asm.GetName().Name, out var uAssembly))
            {
                return false; // this happens in sub scene conversion process if a new assembly is added after loading unity...
            }

            return (uAssembly.flags & AssemblyFlags.EditorAssembly) != 0;
        }

        /// <summary> Checks if an assembly is an editor only assembly. </summary>
        /// <param name="asm"> The assembly to check. </param>
        /// <returns> True if editor assembly. </returns>
        public static bool IsTestEditorAssembly(this System.Reflection.Assembly asm)
        {
            if (!AssembliesMap.TryGetValue(asm.GetName().Name, out var uAssembly))
            {
                return false; // this happens in sub scene conversion process if a new assembly is added after loading unity...
            }

            return uAssembly.assemblyReferences.Any(c => c.name == "UnityEngine.TestRunner");
        }
#else
        /// <summary> Gets the name of all assemblies with a specific reference. </summary>
        /// <param name="reference"> The reference. </param>
        /// <returns> The name of all the assemblies. </returns>
        public static IEnumerable<string> GetAllAssemblyNamesWithReference(System.Reflection.Assembly reference)
        {
            return GetAllAssemblyWithReference(reference).Select(a => a.GetName().Name);
        }
#endif

        /// <summary> Gets the name of all assemblies with a specific reference. </summary>
        /// <param name="reference"> The reference. </param>
        /// <returns> The name of all the assemblies. </returns>
        public static IEnumerable<System.Reflection.Assembly> GetAllAssemblyWithReference(System.Reflection.Assembly reference)
        {
            return AllAssemblies.Where(a => IsAssemblyReferencingAssembly(a, reference));
        }

        public static FieldInfo? GetFieldInBase(this Type? type, string name)
        {
            while (true)
            {
                if (type == null)
                {
                    return null;
                }

                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Static | BindingFlags.Instance |
                                           BindingFlags.DeclaredOnly;

                var field = type.GetField(name, flags);
                if (field != null)
                {
                    return field;
                }

                type = type.BaseType;
            }
        }

        private static T? GetCustomImplementation<T>(Type? defaultImplementation)
        {
            var type = typeof(T);
            if (!type.IsInterface)
            {
                throw new ArgumentException("T should be an interface.", nameof(T));
            }

            var types = AllTypesWithImplementation
                .Where(p => type.IsAssignableFrom(p))
                .ToList();

            switch (types.Count)
            {
                case 0:
                    if (defaultImplementation != null)
                    {
                        throw new InvalidOperationException("Could not find default implementation");
                    }

                    // No implementation was found and default wasn't set
                    return default;
                case 1:
                    return (T)Activator.CreateInstance(types[0]);
                case 2:
                {
                    if (defaultImplementation != null)
                    {
                        if (types.Remove(defaultImplementation))
                        {
                            return (T)Activator.CreateInstance(types[0]);
                        }
                    }

                    break;
                }
            }

            throw new InvalidOperationException($"More than 1 implementation of {type} found");
        }
    }
}
