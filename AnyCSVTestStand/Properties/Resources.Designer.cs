﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AnyCSVTestStand.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("AnyCSVTestStand.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {2}The specified output format, {0}, is invalid, and will be ignored.{2}    The default value, {1}, will be used.{2}.
        /// </summary>
        internal static string ERRMSG_INVALID_OUTPUT_FORMAT {
            get {
                return ResourceManager.GetString("ERRMSG_INVALID_OUTPUT_FORMAT", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The name of an input file is required..
        /// </summary>
        internal static string ERRMSG_NEED_INPUT_FILENAME {
            get {
                return ResourceManager.GetString("ERRMSG_NEED_INPUT_FILENAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A run-time exception has occurred..
        /// </summary>
        internal static string ERRMSG_RUNTIME {
            get {
                return ResourceManager.GetString("ERRMSG_RUNTIME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The task completed successfully..
        /// </summary>
        internal static string ERRMSG_SUCCESS {
            get {
                return ResourceManager.GetString("ERRMSG_SUCCESS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to                            Field {0,2} of {1,2} = {2}.
        /// </summary>
        internal static string MSG_CASE_DETAIL {
            get {
                return ResourceManager.GetString("MSG_CASE_DETAIL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {4}Caee {0,2} of {1,2}: Subfield Count = {2}{4}               Input String   = {3}.
        /// </summary>
        internal static string MSG_CASE_LABEL {
            get {
                return ResourceManager.GetString("MSG_CASE_LABEL", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {2}Working Directory Name = {0}{2}{2}Test Case File  Name   = {1}.
        /// </summary>
        internal static string MSG_INPUT_FILENAME {
            get {
                return ResourceManager.GetString("MSG_INPUT_FILENAME", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}               Scenario 1: Robust Parsing{0}.
        /// </summary>
        internal static string SCENARIO_1 {
            get {
                return ResourceManager.GetString("SCENARIO_1", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}               Scenario 2: Keep Guards{0}.
        /// </summary>
        internal static string SCENARIO_2 {
            get {
                return ResourceManager.GetString("SCENARIO_2", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0}               Scenario 3: Trim Leading{0}.
        /// </summary>
        internal static string SCENARIO_3 {
            get {
                return ResourceManager.GetString("SCENARIO_3", resourceCulture);
            }
        }
    }
}