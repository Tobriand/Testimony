using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Testimony.Data
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

    
}
