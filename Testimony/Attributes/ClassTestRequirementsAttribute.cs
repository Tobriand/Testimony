using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Testimony.Data;
using Testimony.Extensions;

namespace Testimony.Attributes
{
    

    /// <summary>
    /// Indicates that every defined member of a class should be treated as though it is decorated with 
    /// a bare <see cref="TestRequirementAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ClassTestRequirementsAttribute : Attribute
    {
        public ClassTestRequirementsAttribute()
            : this(null, null, new string[] { null } )
        {
        }

        public ClassTestRequirementsAttribute(params string[] globalTests)
            : this(null, null, globalTests)
        {
        }

        public ClassTestRequirementsAttribute(MemberTargetType targets, params string[] globalTests)
            : this(targets, null, globalTests)
        {
        }

        public ClassTestRequirementsAttribute(MemberTargetAccessibility accessibility, params string[] globalTests)
            : this(null, accessibility, globalTests)
        {
        }

        public ClassTestRequirementsAttribute(MemberTargetType targets, MemberTargetAccessibility accessibility) :
            this(targets, accessibility, new string[] { null })
        { }

        public ClassTestRequirementsAttribute(MemberTargetType? targets, MemberTargetAccessibility? accessibility, params string[] globalTests)
        {
            this.GlobalTests = globalTests;
            this.Targets = targets ?? this.Targets;
            this.Accessibility = accessibility ?? this.Accessibility;
        }

        public string[] GlobalTests { get; private set; }

        public MemberTargetType Targets { get; private set; } 
            = MemberTargetType.Properties | MemberTargetType.AllMethods;

        public MemberTargetAccessibility Accessibility { get; private set; } 
            = MemberTargetAccessibility.Public;

        public IEnumerable<MemberInfo> GetRelevantMembers(Type target)
        {
            BindingFlags bflags;
            bool suppressObjectMembers;
            GetRelevantMembers_GetBindingInfo(out bflags, out suppressObjectMembers);

            // We now have the flags we need to get relevant members...
            // Get them!
            var inScopeMembers = target.GetMembers(bflags)
                .Where(m => !suppressObjectMembers || m.DeclaringType != typeof(object));
            var inScopeInterfaceMembers = target
                .GetInterfaces()
                .SelectMany(i => i.GetMembers(bflags));
            inScopeMembers = inScopeMembers.Union(inScopeInterfaceMembers);

            // Now we need to do some filtering...
            var grouped = inScopeMembers
                .GroupBy(mi => GetRelevantMembers_MemberSorter(mi))
                .ToDictionary(mig => mig.Key, mig => (IEnumerable<MemberInfo>)mig);

            // ... now grab the flags set for which types we want ...
            var desiredFlags = this.Targets.GetFlags();
            foreach (var flag in desiredFlags)
            {
                IEnumerable<MemberInfo> group;
                if (grouped.TryGetValue(flag, out group))
                    foreach (var info in group)
                        yield return info;
            }
        }

        private void GetRelevantMembers_GetBindingInfo(out BindingFlags bflags, out bool suppressObjectMembers)
        {
            bflags = BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var flagMembers = this.Accessibility.GetFlags().ToList();
            suppressObjectMembers = false;
            foreach (var flag in flagMembers)
            {
                switch (flag)
                {
                    case MemberTargetAccessibility.Public:
                        bflags = bflags | BindingFlags.Public;
                        break;

                    case MemberTargetAccessibility.NonPublic:
                        bflags = bflags | BindingFlags.NonPublic;
                        break;
                    case MemberTargetAccessibility.InheritedExceptObject:
                    case MemberTargetAccessibility.Inherited:
                        // mask the default DeclaredOnly...
                        uint doMask = uint.MaxValue ^ (uint)BindingFlags.DeclaredOnly;
                        bflags = (BindingFlags)(((uint)bflags) & doMask);

                        if (flag == MemberTargetAccessibility.InheritedExceptObject)
                            suppressObjectMembers = true;
                        break;
                    default:
                        throw new NotImplementedException($"An unexpected flag was provided to GetRelevantMembers: {flag}");
                }
            }
        }


        private MemberTargetType GetRelevantMembers_MemberSorter(MemberInfo info)
        {
            if (info == null)
                throw new ArgumentNullException($"The passed {nameof(info)} argument must have a value");
            if (info is PropertyInfo)
            {
                if (info.Name == "Item" && ((PropertyInfo)info).GetIndexParameters().Length == 0)
                    return MemberTargetType.Indexers;
                return MemberTargetType.Properties;
            }
            if (info is FieldInfo)
                if (info.Name.StartsWith("<"))
                    // This may produce false positives with event backing delegate fields.
                    // We *probably* don't care, though; the crucial thing isn't that such elements
                    // are correctly grouped, it's that they are excluded from normal use.
                    return MemberTargetType.PropertySupport;
                else
                    return MemberTargetType.Fields;
            if (info is MethodInfo)
            {
                if ((info as MethodBase)?.IsSpecialName ?? false)
                {
                    var methodBase = info as MethodBase;
                    if (methodBase.IsConstructor)
                        return MemberTargetType.Constructors;
                    var hasPropertyMethodStart = info.Name.StartsWith("get_") || info.Name.StartsWith("set_");
                    if (hasPropertyMethodStart)
                        return MemberTargetType.PropertySupport;
                    // This might be wrong, but it's probably right enough for now...
                    var hasEventMethodStart = info.Name.StartsWith("add_") || info.Name.StartsWith("remove_");
                    if (hasEventMethodStart)
                        return MemberTargetType.EventSupport;
                    return MemberTargetType.Operators;
                }
                return MemberTargetType.Methods;
            }
            if (info is EventInfo)
                return MemberTargetType.Events;
            throw new NotImplementedException($"Encountered an unexpected MemberInfo instance of type {info?.GetType().Name}");
        }
    }
}
