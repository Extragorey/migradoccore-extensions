# MigraDocCore.Extensions

Extensions for [MigraDocCore/PDFSharpCore](https://github.com/ststeiger/PdfSharpCore). This project was ported from the [hivvetech/migradoc-extensions](https://github.com/hivvetech/migradoc-extensions) project. It also adds support for HTML and Markdown in header or footer sections.

## Quick Start

The biggest feature provided by this library is the ability to convert from HTML and Markdown to PDF, via MigraDocCore's Document Object Model.

MigraDocCore.Extensions makes use of [Markdig](https://github.com/xoofx/markdig) (replacing MarkdownSharp in the original library) to convert from Markdown to HTML and the [Html Agility Pack](https://html-agility-pack.net/) to convert from HTML to PDF.

Since the MigraDoc DOM is pretty basic, much of the conversion involves setting the `Style` of generated MigraDoc `Paragraph` instances. You can then configure these styles however you like. See the [example project](https://github.com/Extragorey/migradoccore-extensions/blob/master/MigraDocCore.Extensions.Examples/Program.cs#L61) for more details.

#### Converting from Markdown to PDF

Import the `MigraDocCore.Extensions.Markdown` namespace and call `AddMarkdown` on a MigraDoc `Section`, `HeaderFooter`, `Cell`, or `Paragraph` instance:


	var markdown = @"
		# This is a heading

		This is some **bold** ass text with a [link](https://www.example.com).

		- List Item 1
		- List Item 2
		- List Item 3

		Pretty cool huh?
	";

	section.AddMarkdown(markdown);


#### Converting from HTML to PDF

Import the `MigraDocCore.Extensions.Html` namespace and call `AddHtml` on a MigraDoc `Section`, `HeaderFooter`, `Cell`, or `Paragraph` instance:


	var html = @"
		<h1>This is a heading</h1>

		<p>This is some **bold** ass text with a <a href='https://www.example.com'>link</a>.<p>

		<ul>
			<li>List Item 1</li>
			<li>List Item 2</li>
			<li>List Item 3</li>
		</ul>

		<p>Pretty cool huh?</p>
	";

	section.AddHtml(html);
	
#### What is supported?

The HTML converter currently supports the following:

- Headings (H1 -> H6) - Sets a "HeadingX" style on the generated paragraph
- Paragraphs
- Hyperlinks containing plain text or supported inline elements
- Lists - Adds a paragraph with style "ListStart" before the list and one with style "ListEnd" after the list.
  - Unordered Lists - Each list item has the style "UnorderedList"
  - Ordered Lists - Each list item has the style "Ordered List"
- Line breaks 
- Inline elements `<strong>`, `<em>`, `<i>`, `<u>`
- Horizontal Rules - Adds a paragraph with style "HorizontalRule"

For more details, check out the [specs](https://github.com/benfoster/MigraDoc.Extensions/blob/master/src/specs/MigraDoc.Extensions.Html.Specs/converting_tags.cs).


#### Extending the HTML converter

To add a custom handler, create a new instance of `HtmlConverter` and add to its `NodeHandlers` dictionary. The key is the HTML element you wish to handle and the value is a `Func<HtmlNode, DocumentObject, DocumentObject>`.

The `DocumentObject` instance passed to the handler is the parent object in the MigraDoc DOM, usually a `Section` or `Paragraph` (you may need to cater for both). The return value should be the `DocumentObject` that was created. This will be passed as the parent for any child elements. 

Here is the handler for processing a `<strong>` element:

    nodeHandlers.Add("strong", (node, parent) => {
        var format = TextFormat.Bold;
        
        var formattedText = parent as FormattedText;
        if (formattedText != null)
        {
            return formattedText.Format(format);
        }

        // otherwise parent is paragraph or section
        return GetParagraph(parent).AddFormattedText(format);
    });

In the above handler, we need to cater for nested format tags (e.g. `<strong><em>some text</em></strong>`) so we first attempt to cast the parent as `FormattedText`, otherwise fall back to adding formatted text to a `Paragraph`. Unfortunately such type checks are fairly frequent due to the limited relationships between objects in the MigraDoc DOM.

To use a custom converter instance use the `Section.Add(string content, IConverter converter)` extension in the `MigraDocCore.Extensions` namespace.

Note that an element handler should not process any inner HTML. For example the handler for a `<h1>` tag only adds a paragraph with a the style "Heading1", it does not add the text (there is a separate handler for processing text nodes).


#### Known issues

As of PdfSharpCore v1.3.62 there appears to be a bug where multiple paragraphs in footer sections are written in the same position, overlapping each other. This extension works around the bug by using a single paragraph in footer sections, adding newlines as needed. In practice, this means some paragraph-level formatting may not be fully supported in footers (e.g. headings, lists).

I have [opened an issue](https://github.com/ststeiger/PdfSharpCore/issues/414) in the PdfSharpCore repo to track the progress of this bug.


## License

Licensed under the MIT License.