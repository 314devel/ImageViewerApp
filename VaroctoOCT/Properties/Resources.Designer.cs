﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VaroctoOCT.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("VaroctoOCT.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Cannot connect to the remote database. Please contact the system administrator..
        /// </summary>
        internal static string DBConnetError {
            get {
                return ResourceManager.GetString("DBConnetError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error was encountered when trying to capture from the pupil camera. Please contact the system administrator..
        /// </summary>
        internal static string PupilCamCaptureError {
            get {
                return ResourceManager.GetString("PupilCamCaptureError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pupil Camera Error.
        /// </summary>
        internal static string PupilCamErrorTitle {
            get {
                return ResourceManager.GetString("PupilCamErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error was encountered when trying to initialize the pupil camera. Please contact the system administrator..
        /// </summary>
        internal static string PupilCamInitError {
            get {
                return ResourceManager.GetString("PupilCamInitError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error was encountered when trying to allocate memory for the pupil camera. Please contact the system administrator..
        /// </summary>
        internal static string PupilCamMemAllocError {
            get {
                return ResourceManager.GetString("PupilCamMemAllocError", resourceCulture);
            }
        }
    }
}
