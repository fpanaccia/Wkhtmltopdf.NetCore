﻿using Wkhtmltopdf.NetCore.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Wkhtmltopdf.NetCore
{
    public class ConvertOptions
    {
        public ConvertOptions()
        {
            this.PageMargins = new Margins();
        }

        protected void SetOptions(ConvertOptions options)
        {
            this.PageMargins = new Margins();
            this.PageSize = options.PageSize;
            this.PageWidth = options.PageWidth;
            this.PageHeight = options.PageHeight;
            this.PageOrientation = options.PageOrientation;
            this.PageMargins = options.PageMargins;
            this.IsLowQuality = options.IsLowQuality;
            this.Copies = options.Copies;
            this.IsGrayScale = options.IsGrayScale;
            this.HeaderHtml = options.HeaderHtml;
            this.HeaderSpacing = options.HeaderSpacing;
            this.FooterHtml = options.FooterHtml;
            this.FooterSpacing = options.FooterSpacing;
            this.Replacements = options.Replacements;
        }

        /// <summary>
        /// Sets the page size.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize { get; set; }

        /// <summary>
        /// Sets the page width in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.</remarks>
        [OptionFlag("--page-width")]
        public double? PageWidth { get; set; }

        /// <summary>
        /// Sets the page height in mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--page-height")]
        public double? PageHeight { get; set; }

        /// <summary>
        /// Sets the page orientation.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation { get; set; }

        /// <summary>
        /// Sets the page margins.
        /// </summary>
        public Margins PageMargins { get; set; }

        protected string GetContentType()
        {
            return "application/pdf";
        }

        /// <summary>
        /// Indicates whether the PDF should be generated in lower quality.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality { get; set; }

        /// <summary>
        /// Number of copies to print into the PDF file.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// Indicates whether the PDF should be generated in grayscale.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale { get; set; }

        /// <summary>
        /// Path to header HTML file.
        /// </summary>
        [OptionFlag("--header-html")]
        public string HeaderHtml { get; set; }

        /// <summary>
        /// Sets the header spacing.
        /// </summary>
        [OptionFlag("--header-spacing")]
        public int? HeaderSpacing { get; set; }

        /// <summary>
        /// Path to footer HTML file.
        /// </summary>
        [OptionFlag("--footer-html")]
        public string FooterHtml { get; set; }

        /// <summary>
        /// Sets the footer spacing.
        /// </summary>
        [OptionFlag("--footer-spacing")]
        public int? FooterSpacing { get; set; }

        /// <summary>
        /// Sets the variables to replace in the header and footer html
        /// </summary>
        /// <remarks>Replaces [name] with value in header and footer (repeatable).</remarks>
        [OptionFlag("--replace")]
        public Dictionary<string, string> Replacements { get; set; }

        protected string GetConvertOptions()
        {
            var result = new StringBuilder();

            if (this.PageMargins != null)
                result.Append(this.PageMargins.ToString());

            result.Append(" ");
            result.Append(GetConvertBaseOptions());

            return result.ToString().Trim();
        }

        protected string GetConvertBaseOptions()
        {
            var result = new StringBuilder();

            var fields = this.GetType().GetProperties();
            foreach (var fi in fields)
            {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(this, null);
                if (value == null)
                    continue;

                if (fi.PropertyType == typeof(Dictionary<string, string>))
                {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary)
                    {
                        result.AppendFormat(" {0} \"{1}\" \"{2}\"", of.Name, d.Key, d.Value);
                    }
                }
                else if (fi.PropertyType == typeof(bool))
                {
                    if ((bool)value)
                        result.AppendFormat(CultureInfo.InvariantCulture, " {0}", of.Name);
                }
                else
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }
    }
}