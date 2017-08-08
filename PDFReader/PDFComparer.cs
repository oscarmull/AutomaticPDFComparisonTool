using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PDFReader
{
    public class PDFComparer
    {
        /// <summary>
        /// Base for comparison. If not comparing against a set base, is just one of the PDFs
        /// </summary>
        public PDFDocument BaseDocument { get; set; }
        /// <summary>
        /// PDF to compare against a base. If not comparing against a set base, is just one of the PDFs.
        /// </summary>
        public PDFDocument ComparisonDocument { get; set; }

        /// <summary>
        /// Build comparer passing file path to the PDF files
        /// </summary>
        /// <param name="baseDocumentPath">Path file to base document</param>
        /// <param name="comparisonDocumentPath">Path file to comparison document</param>
        public PDFComparer(string baseDocumentPath, string comparisonDocumentPath) {
            this.BaseDocument = new PDFDocument(baseDocumentPath);
            this.ComparisonDocument = new PDFDocument(comparisonDocumentPath);
        }

        /// <summary>
        /// Build comparer passing PDFDocument objects. These objects can be creating by doing var document = new PDFDocument(path);
        /// </summary>
        /// <param name="baseDocument">PDFDocument representing base PDF</param>
        /// <param name="comparisonDocument">PDFDocument representing comparison PDF</param>
        public PDFComparer(PDFDocument baseDocument, PDFDocument comparisonDocument) {
            this.BaseDocument = baseDocument;
            this.ComparisonDocument = comparisonDocument;
        }

        /// <summary>
        /// Build comparer passing memory streams instead of disk based documents.
        /// </summary>
        /// <param name="baseDocument">MemoryStream for the base document</param>
        /// <param name="comparisonDocument">MemoryStream for the comparison document</param>
        public PDFComparer(MemoryStream baseDocument, MemoryStream comparisonDocument)
        {
            this.BaseDocument = new PDFDocument(baseDocument);
            this.ComparisonDocument = new PDFDocument(comparisonDocument);
        }

        /// <summary>
        /// Method to verify if two PDF documents have the exact same number of pages, text, fonts and images on a page basis
        /// </summary>
        /// <returns>True if both documents are equal, else false</returns>
        public bool AreEquivalentDocuments() {
            return this.HaveSameNumberOfPages() && this.HaveSameFonts() && this.HaveSameImages() && this.HaveSameTextPerPage();
        }

        /// <summary>
        /// Method to verify if two PDF documents have the exact same number of pages. This is called internally on each comparison method
        /// </summary>
        /// <returns>True if have the same number of pages, else false</returns>
        public bool HaveSameNumberOfPages() {
            return this.BaseDocument.numberOfPages == this.ComparisonDocument.numberOfPages;
        }

        /// <summary>
        /// Method to verify if two PDF documents have the exact same text (without considering page of the text)
        /// </summary>
        /// <returns>True if have the same text, else false</returns>
        public bool HaveFullSameText() {
            return this.BaseDocument.ReadPDFText().Equals(this.ComparisonDocument.ReadPDFText());
        }


        /// <summary>
        /// Method to verify if two PDF documents have the same text in each page.
        /// </summary>
        /// <returns>True if have the same text in each page, else false</returns>
        public bool HaveSameTextPerPage() {
            if (this.HaveSameNumberOfPages()) {
                foreach (var pageText in this.BaseDocument.pageContent)
                {
                    var basePageContent = this.BaseDocument.pageContent[pageText.Key];
                    var comparisonPageContent = this.ComparisonDocument.pageContent[pageText.Key];
                    if (!basePageContent.Equals(comparisonPageContent)) {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method to verify if two PDF documents have the same fonts in each page.
        /// </summary>
        /// <returns>True if have the same font in each page, else false</returns>
        public bool HaveSameFonts() {
            if (this.HaveSameNumberOfPages())
            {
                foreach (var pageFonts in this.BaseDocument.fonts)
                {
                    if (this.BaseDocument.fonts[pageFonts.Key].Count == this.ComparisonDocument.fonts[pageFonts.Key].Count)
                    {
                        foreach (var font in this.BaseDocument.fonts[pageFonts.Key])
                        {
                            var fontsInBaseNotInComparison = this.BaseDocument.fonts[pageFonts.Key].Except(this.ComparisonDocument.fonts[pageFonts.Key]);
                            if (fontsInBaseNotInComparison.Count() != 0)
                            {
                                return false;
                            }
                            var fontsInComparisonNotInBase = this.ComparisonDocument.fonts[pageFonts.Key].Except(this.BaseDocument.fonts[pageFonts.Key]);
                            if (fontsInComparisonNotInBase.Count() != 0) {
                                return false;
                            }
                        }
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Method to verify if two PDF documents have the images in each page. Since it is not possible to compare image pixel by pixel in a performant way using this, it check each image in each page against same image on same page and verifies Width, Height, Extension and Decode
        /// </summary>
        /// <returns>True if have the same image in each page, else false</returns>
        public bool HaveSameImages() {
            if (this.HaveSameNumberOfPages())
            {
                foreach (var pageImage in this.BaseDocument.Images)
                {
                    var originalImages = this.BaseDocument.Images[pageImage.Key];
                    var comparisonImages = this.ComparisonDocument.Images[pageImage.Key];
                    if (originalImages.Count == comparisonImages.Count)
                    {

                        for (int i = 0; i < originalImages.Count; i++)
                        {
                            if (!(originalImages[i].getWidth() == comparisonImages[i].getWidth() && originalImages[i].getHeight() == comparisonImages[i].getHeight() && originalImages[i].getSuffix() == comparisonImages[i].getSuffix() && originalImages[i].getDecode() == comparisonImages[i].getDecode()))
                            {
                                //Images are different
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Different number of images
                        return false;
                    }
                }
            }
            else {
                //Different Number of pages
                return false;
            }
            return true;
        }

    }
}
