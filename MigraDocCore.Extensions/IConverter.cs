using System;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;

namespace MigraDocCore.Extensions {
    public interface IConverter {
        Action<Section> Convert(string contents);

        Action<HeaderFooter> ConvertHeaderFooter(string contents);

        Action<Cell> ConvertCell(string contents);

        Action<Paragraph> ConvertParagraph(string contents);
    }
}
