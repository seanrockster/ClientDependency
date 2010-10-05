﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Linq;
using ClientDependency.Core.Config;

namespace ClientDependency.Core.FileRegistration.Providers
{
    public class PageHeaderProvider : WebFormsFileRegistrationProvider
	{		

		public const string DefaultName = "PageHeaderProvider";
		

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{			
			// Assign the provider a default name if it doesn't have one
			if (string.IsNullOrEmpty(name))
				name = DefaultName;

			base.Initialize(name, config);
		}

		protected override string RenderJsDependencies(List<IClientDependencyFile> jsDependencies)
		{
			if (jsDependencies.Count == 0)
				return string.Empty;

            StringBuilder sb = new StringBuilder();

            if (ConfigurationHelper.IsCompilationDebug || !EnableCompositeFiles)
			{
				foreach (IClientDependencyFile dependency in jsDependencies)
				{
                    sb.Append(RenderSingleJsFile(dependency.FilePath));
				}
			}
			else
			{
                var comp = ProcessCompositeList(jsDependencies, ClientDependencyType.Javascript);
                foreach (var s in comp)
                {
                    sb.Append(RenderSingleJsFile(s));
                }    
			}

            return sb.ToString();
		}

		protected override string RenderSingleJsFile(string js)
		{
            return string.Format(HtmlEmbedContants.ScriptEmbedWithSource, js);
		}

		protected override string RenderCssDependencies(List<IClientDependencyFile> cssDependencies)
		{
            if (cssDependencies.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            if (ConfigurationHelper.IsCompilationDebug || !EnableCompositeFiles)
			{
				foreach (IClientDependencyFile dependency in cssDependencies)
				{
                    sb.Append(RenderSingleCssFile(dependency.FilePath));
				}
			}
			else
			{
                var comp = ProcessCompositeList(cssDependencies, ClientDependencyType.Css);
                foreach (var s in comp)
                {
                    sb.Append(RenderSingleCssFile(s));
                }    
			}

            return sb.ToString();
		}

		protected override string RenderSingleCssFile(string css)
		{
            return string.Format(HtmlEmbedContants.CssEmbedWithSource, css);
		}
        
        /// <summary>
        /// Registers the dependencies in the page header
        /// </summary>
        /// <param name="dependantControl"></param>
        /// <param name="js"></param>
        /// <param name="css"></param>
        /// <remarks>
        /// For some reason ampersands that aren't html escaped are not compliant to HTML standards when they exist in 'link' or 'script' tags in URLs,
        /// we need to replace the ampersands with &amp; . This is only required for this one w3c compliancy, the URL itself is a valid URL.
        /// 
        /// </remarks>
        protected override void RegisterDependencies(Control dependantControl, string js, string css)
        {
            if (dependantControl.Page.Header == null)
                throw new NullReferenceException("PageHeaderProvider requires a runat='server' tag in the page's header tag");

            LiteralControl jsScriptBlock = new LiteralControl(js.Replace("&", "&amp;"));
            LiteralControl cssStyleBlock = new LiteralControl(css.Replace("&", "&amp;"));
            dependantControl.Page.Header.Controls.Add(cssStyleBlock);
            dependantControl.Page.Header.Controls.Add(jsScriptBlock);

        }

		
	}
}
