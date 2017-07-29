using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testimony
{
    /// <summary>
    /// Provides a slightly restricted set of options for targeting 
    /// members of an object via reflection.
    /// </summary>
    [Flags]
    public enum MemberTargetType
    {
        /// <summary>
        /// All members except indexers and operators which are treated separately
        /// </summary>
        Methods = 1 << 0,

        /// <summary>
        /// Anything that can be considered a property. Getters and setters are NOT 
        /// treated any differently, however.
        /// </summary>
        Properties = 1 << 1,

        /// <summary>
        /// All fields within the object.
        /// </summary>
        Fields = 1 << 2,

        /// <summary>
        /// All indexers within the object
        /// </summary>
        Indexers = 1 << 3,

        /// <summary>
        /// Targets operators within the object
        /// </summary>
        Operators = 1 << 4,

        /// <summary>
        /// Targets events within the object
        /// </summary>
        Events = 1 << 5,

        /// <summary>
        /// Methods/fields that are used to support properties.
        /// Reserved for internal use; here be dragons.
        /// </summary>
        PropertySupport = 1 << 6,

        /// <summary>
        /// Targets constructors within the object
        /// </summary>
        Constructors = 1 << 7,

        /// <summary>
        /// Methods/fields that are used to support events.
        /// Reserved for internal use; here be dragons.
        /// </summary>
        EventSupport = 1 << 8,

        /// <summary>
        /// Shorthand for all MethodInfo items the object could return
        /// </summary>
        AllMethods = Methods | Indexers | Operators,

        /// <summary>
        /// Shorthand for anything that isn't a field
        /// </summary>
        NonFields = AllMethods | Properties | Events,

        /// <summary>
        /// Shorthand for anything
        /// </summary>
        All = AllMethods | NonFields | Fields,
        
    }

    /// <summary>
    /// Provides an indication only of accesibility of an item.
    /// </summary>
    [Flags]
    public enum MemberTargetAccessibility
    {
        Public = 1 << 0,
        NonPublic = 1 << 1,
        Inherited = 1 << 2,
        InheritedExceptObject = 1 << 3
    }

    public static class MemberTargetting
    {
        private static readonly MemberTargetType[] MemberTargetTypeFlags =
            Enum.GetValues(typeof(MemberTargetType)).Cast<MemberTargetType>().Where(bf => bf.IsFlagValue()).ToArray();

        private static readonly MemberTargetAccessibility[] MemberTargetAccessibilityFlags =
            Enum.GetValues(typeof(MemberTargetAccessibility)).Cast<MemberTargetAccessibility>().Where(bf => bf.IsFlagValue()).ToArray();

        /// <summary>
        /// Returns whether the provided value is a power of 2 - and therefore usable as a flag
        /// or whether it is a shorthand combination of several powers of two...
        /// 
        /// Returns false when the value is 0, since it is impossible to distinguish a 0-flag
        /// from any other value.
        /// </summary>
        /// <param name="value">An integral enum value</param>
        /// <returns></returns>
        public static bool IsFlagValue(this Enum value)
        {
            var underlying = Enum.GetUnderlyingType(value.GetType());
            switch (underlying.Name)
            {
                case nameof(Int16):
                case nameof(Int32):
                case nameof(Int64):
                case nameof(UInt16):
                case nameof(UInt32):
                case nameof(UInt64):
                    var lValue = (long)Convert.ChangeType(value, typeof(long));
                    return (lValue != 0) && (lValue & (lValue - 1)) == 0 ;
                case nameof(Boolean):
                    return true;
                default:
                    return false;
            }
        }

        public static IEnumerable<MemberTargetType> GetFlags(this MemberTargetType value)
        {
            foreach (var flag in MemberTargetTypeFlags)
                if (value.HasFlag(flag))
                    yield return flag;
        }

        public static IEnumerable<MemberTargetAccessibility> GetFlags(this MemberTargetAccessibility value)
        {
            foreach (var flag in MemberTargetAccessibilityFlags)
                if (value.HasFlag(flag))
                    yield return flag;
        }
    }
}
