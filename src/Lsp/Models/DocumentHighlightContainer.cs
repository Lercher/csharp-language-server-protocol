﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Lsp.Models
{
    public class DocumentHighlightContainer : Container<DocumentHighlight>
    {
        public DocumentHighlightContainer() : this(Enumerable.Empty<DocumentHighlight>())
        {
        }

        public DocumentHighlightContainer(IEnumerable<DocumentHighlight> items) : base(items)
        {
        }

        public DocumentHighlightContainer(params DocumentHighlight[] items) : base(items)
        {
        }

        public static implicit operator DocumentHighlightContainer(DocumentHighlight[] items)
        {
            return new DocumentHighlightContainer(items);
        }

        public static implicit operator DocumentHighlightContainer(Collection<DocumentHighlight> items)
        {
            return new DocumentHighlightContainer(items);
        }

        public static implicit operator DocumentHighlightContainer(List<DocumentHighlight> items)
        {
            return new DocumentHighlightContainer(items);
        }
    }
}