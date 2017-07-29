using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testimony.Data;

namespace Testimony.Extensions
{
    public static class MemberTargettingExtensions
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
                    return (lValue != 0) && (lValue & (lValue - 1)) == 0;
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
