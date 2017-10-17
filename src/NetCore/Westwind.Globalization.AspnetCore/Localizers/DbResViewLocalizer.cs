using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Westwind.Globalization.AspnetCore
{

    /// <summary>
    /// Patched ViewLocalizer to work around bug with RazorPage injection
    /// </summary>
    public class DbResViewLocalizer : IViewContextAware, IViewLocalizer
    {
        private readonly IHtmlLocalizerFactory _localizerFactory;
        private readonly string _applicationName;
        private IHtmlLocalizer _localizer;

        /// <summary>
        /// Creates a new <see cref="Microsoft.AspNetCore.Mvc.Localization.ViewLocalizer"/>.
        /// </summary>
        /// <param name="localizerFactory">The <see cref="IHtmlLocalizerFactory"/>.</param>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        public DbResViewLocalizer(IHtmlLocalizerFactory localizerFactory, IHostingEnvironment hostingEnvironment)
        {
            if (localizerFactory == null)
                throw new ArgumentNullException(nameof(localizerFactory));

            if (hostingEnvironment == null)
                throw new ArgumentNullException(nameof(hostingEnvironment));

            _applicationName = hostingEnvironment.ApplicationName;
            _localizerFactory = localizerFactory;
        }

        /// <inheritdoc />
        public virtual LocalizedHtmlString this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _localizer[key];
            }
        }

        /// <inheritdoc />
        public virtual LocalizedHtmlString this[string key, params object[] arguments]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return _localizer[key, arguments];
            }
        }

        /// <inheritdoc />
        public LocalizedString GetString(string name) => _localizer.GetString(name);

        /// <inheritdoc />
        public LocalizedString GetString(string name, params object[] values) => _localizer.GetString(name, values);

        /// <inheritdoc />
        public IHtmlLocalizer WithCulture(CultureInfo culture) => _localizer.WithCulture(culture);

        /// <inheritdoc />
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            _localizer.GetAllStrings(includeParentCultures);

        /// <summary>
        /// Apply the specified <see cref="ViewContext"/>.
        /// </summary>
        /// <param name="viewContext">The <see cref="ViewContext"/>.</param>
        public void Contextualize(ViewContext viewContext)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            // Given a view path "/Views/Home/Index.cshtml" we want a baseName like "MyApplication.Views.Home.Index"
            var path = viewContext.ExecutingFilePath;

            if (string.IsNullOrEmpty(path))
            {
                path = viewContext.View.Path;
            }

            // PATCH: To support Razor Pages
            if (string.IsNullOrEmpty(path) && viewContext.ActionDescriptor is PageActionDescriptor pageAD)
            {
                path = pageAD.RelativePath;
            }

            Debug.Assert(!string.IsNullOrEmpty(path), "Couldn't determine a path for the view");

            var baseName = BuildBaseName(path);

            _localizer = _localizerFactory.Create(baseName, _applicationName);
        }

        private string BuildBaseName(string path)
        {
            var extension = Path.GetExtension(path);
            var startIndex = path[0] == '/' || path[0] == '\\' ? 1 : 0;
            var length = path.Length - startIndex - extension.Length;
            var capacity = length + _applicationName.Length + 1;
            var builder = new StringBuilder(path, startIndex, length, capacity);

            builder.Replace('/', '.').Replace('\\', '.');

            // Prepend the application name
            //builder.Insert(0, '.');
            //builder.Insert(0, _applicationName);

            return builder.ToString();
        }
    }
}
