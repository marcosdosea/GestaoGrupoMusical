﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Email.Template {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class EmailResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Email.Template.EmailResource", typeof(EmailResource).Assembly);
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
        ///   Looks up a localized string similar to &lt;/div&gt;
        ///        &lt;div style=&quot;margin: 0px auto;max-width: 600px;padding-bottom: 50px;&quot;&gt;
        ///            &lt;h2&gt;
        ///                Abraços,&lt;br&gt;
        ///                Equipe Batalá
        ///            &lt;/h2&gt;
        ///            &lt;p Align=&quot;justify&quot;&gt;
        ///                Por favor, pedimos que você não responda esse e-mail, pois se trata de uma mensagem 
        ///                automática e não é possível dar continuidade com seu atendimento por aqui.
        ///            &lt;/p&gt;
        ///        &lt;/div&gt;
        ///&lt;div style=&quot;display: flex;margin: 0px auto;max-width: 600px;justif [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string EmailPatternDown {
            get {
                return ResourceManager.GetString("EmailPatternDown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;!DOCTYPE html&gt;
        ///&lt;html lang=&quot;pt-BR&quot;&gt;
        ///&lt;head&gt;
        ///    &lt;title&gt;Mensagem de E-mail&lt;/title&gt;
        ///    &lt;link rel=&quot;preconnect&quot; href=&quot;https://fonts.googleapis.com&quot;&gt;
        ///    &lt;link rel=&quot;preconnect&quot; href=&quot;https://fonts.gstatic.com&quot; crossorigin&gt;
        ///    &lt;link href=&quot;https://fonts.googleapis.com/css2?family=Roboto:wght@100&amp;display=swap&quot; rel=&quot;stylesheet&quot;&gt;
        ///&lt;/head&gt;
        ///
        ///&lt;body style=&quot;font-family: &apos;Roboto&apos;, sans-serif;
        ///  font-weight: 900;&quot;&gt;
        ///    &lt;div style=&quot;background-color: #e5e5e5;&quot;&gt;
        ///        &lt;div style=&quot;margin: 0px auto;max-width: 600px [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string EmailPatternTop {
            get {
                return ResourceManager.GetString("EmailPatternTop", resourceCulture);
            }
        }
    }
}
