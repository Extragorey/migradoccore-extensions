using System;
using System.Text.RegularExpressions;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.MigraDoc.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Shapes;
using MigraDocCore.DocumentObjectModel.Tables;

namespace MigraDocCore.Extensions {
    public static class SectionExtensions {
        public static Section Add(this Section section, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.Convert(contents);
            addAction(section);
            return section;
        }

        public static HeaderFooter Add(this HeaderFooter headerFooter, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertHeaderFooter(contents);
            addAction(headerFooter);
            return headerFooter;
        }

        public static Cell Add(this Cell cell, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertCell(contents);
            addAction(cell);
            return cell;
        }

        public static Paragraph Add(this Paragraph paragraph, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertParagraph(contents);
            addAction(paragraph);
            return paragraph;
        }

        /// <summary>
        /// Adds a new Image object.
        /// The <paramref name="fileName"/> parameter can either be the path to an image
        /// or a base-64 string encoding of an image, prefixed by "<c>base64:</c>".
        /// </summary>
        /// <param name="section">The current document section.</param>
        /// <param name="fileName">The full file path or base-64 string encoded byte array.</param>
        /// <param name="quality">The compression quality of the image when rendered.</param>
        /// <returns>The created Image object.</returns>
        public static Image AddImage(this Section section, string fileName, int? quality = 75) {
            var source = GetImageSource(fileName, quality);
            return section.AddImage(source);
        }

        /// <summary>
        /// Adds a new Image object.
        /// The <paramref name="fileName"/> parameter can either be the path to an image
        /// or a base-64 string encoding of an image, prefixed by "<c>base64:</c>".
        /// </summary>
        /// <param name="cell">The current table cell.</param>
        /// <param name="fileName">The full file path or base-64 string encoded byte array.</param>
        /// <param name="quality">The compression quality of the image when rendered.</param>
        /// <returns>The created Image object.</returns>
        public static Image AddImage(this Cell cell, string fileName, int? quality = 75) {
            var source = GetImageSource(fileName, quality);
            return cell.AddImage(source);
        }

        /// <summary>
        /// Adds a new Image object.
        /// The <paramref name="fileName"/> parameter can either be the path to an image
        /// or a base-64 string encoding of an image, prefixed by "<c>base64:</c>".
        /// </summary>
        /// <param name="paragraph">The current paragraph.</param>
        /// <param name="fileName">The full file path or base-64 string encoded byte array.</param>
        /// <param name="quality">The compression quality of the image when rendered.</param>
        /// <returns>The created Image object.</returns>
        public static Image AddImage(this Paragraph paragraph, string fileName, int? quality = 100) {
            var source = GetImageSource(fileName, quality);
            return paragraph.AddImage(source);
        }



        /// <summary>
        /// Gets the appropriate IImageSource for the given <paramref name="fileName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="fileName"/> is null or empty.</exception>
        private static ImageSource.IImageSource GetImageSource(string fileName, int? quality) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.StartsWith("base64:", StringComparison.OrdinalIgnoreCase)) {
                string base64String = Regex.Replace(fileName, "^(base64):", "", RegexOptions.IgnoreCase);
                return ImageSource.FromBinary("Image", () => Convert.FromBase64String(base64String), quality);
            } else {
                return ImageSource.FromFile(fileName, quality);
            }
        }
    }
}
