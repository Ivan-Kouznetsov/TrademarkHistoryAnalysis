﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrademarkHistoryAnalysis.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TrademarkHistoryAnalysis.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Extracted {0} case files.
        /// </summary>
        internal static string CaseFileCountMessage {
            get {
                return ResourceManager.GetString("CaseFileCountMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TrademarkHistoryAnalysis (C) 2019 Ivan Kouznetsov. Free software distributed under AGPL..
        /// </summary>
        internal static string Copyright {
            get {
                return ResourceManager.GetString("Copyright", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Database file already exists. Specify a new file name..
        /// </summary>
        internal static string DatabaseFileAlreadyExists {
            get {
                return ResourceManager.GetString("DatabaseFileAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Finished parsing.
        /// </summary>
        internal static string EndedParsing {
            get {
                return ResourceManager.GetString("EndedParsing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File Not Found.
        /// </summary>
        internal static string FileNotFound {
            get {
                return ResourceManager.GetString("FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TrademarkHistoryAnalysis\nUsage: TrademarkHistoryAnalysis[.exe] [Directory of USPTO annual case files as zip files] [name of sqlite file to create] \n Example: TrademarkHistoryAnalysis &quot;D:\Downloads\USPTOzipfiles&quot; mynewdatabase.db.
        /// </summary>
        internal static string Instructions {
            get {
                return ResourceManager.GetString("Instructions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Specify a file.
        /// </summary>
        internal static string NoFile {
            get {
                return ResourceManager.GetString("NoFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Started parsing annual USPTO file {0}.
        /// </summary>
        internal static string StartedParsing {
            get {
                return ResourceManager.GetString("StartedParsing", resourceCulture);
            }
        }
    }
}