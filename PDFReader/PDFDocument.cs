using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using org.apache.pdfbox.pdmodel.font;
using org.apache.pdfbox.pdmodel.graphics.xobject;
using IKVM.NativeCode.java.util;
using IKVM.NativeCode.java.awt.image;
using System.IO;
using ikvm.io;



namespace PDFReader
{
    /// <summary>
    /// Class to handle documents using PDFBox as base library for implementation. Author: Oscar Mullin
    /// The constructor the class already loads all required data from the pdf file
    /// </summary>
    public class PDFDocument
    {

        /// <summary>
        /// The main document. Logic representation of the PDF file
        /// </summary>
        public PDDocument document { get; set; }
        /// <summary>
        /// Page contents. The key represents the page and the value the whole text of the page
        /// </summary>
        public Dictionary<int, string> pageContent { get; set; }
        /// <summary>
        /// Number of pages in the document
        /// </summary>
        public int numberOfPages { get; set; }
        /// <summary>
        /// Page's fonts. The key represents the page and the value a list of all the fonts in the page of the page
        /// </summary>
        public Dictionary<int, List<string>> fonts { get; set; }
        /// <summary>
        /// Page's images. The key represents the page and the value the list of images on the page as a List of PDXObjectImage
        /// </summary>
        public Dictionary<int, List<PDXObjectImage>> Images { get; set; }
        /// <summary>
        /// List of Pages in the PDPage format
        /// </summary>
        public List<PDPage> Pages { get; set; }

        public PDFDocument(string path)
        {
            this.document = PDDocument.load(path);
            this.LoadBasics();
        }

        public PDFDocument(MemoryStream mstream) { 
            var inputStream = new InputStreamWrapper(mstream);
            this.document = PDDocument.load(inputStream);
            this.LoadBasics();
        }

        private void LoadBasics(){
            this.Images = new Dictionary<int, List<PDXObjectImage>>();
            this.Pages = new List<PDPage>();
            this.pageContent = new Dictionary<int, string>();
            this.numberOfPages = this.document.getNumberOfPages();
            this.fonts = new Dictionary<int, List<string>>();
            this.LoadPagesText();
            this.LoadFonts();
            this.LoadPages();
            this.ReadImages();
        }

        /// <summary>
        /// Use this method to dispose PDF document after usage. Author: Oscar Mullin
        /// </summary>
        public void Close()
        {
            this.document.close();
        }

        /// <summary>
        /// Use this method to get a string with all the PDF text. Author: Oscar Mullin
        /// </summary>
        /// <returns>
        /// Full text of the PDF doucment
        /// </returns>
        public string ReadPDFText()
        {
            string pdfText = "";
            PDFTextStripper stripper = new PDFTextStripper();
            pdfText = stripper.getText(this.document);
            return pdfText;
        }

        /// <summary>
        /// Use this method to get whole text of a single PDF page. Author: Oscar Mullin
        /// </summary>
        /// <param name="page"></param>
        /// <returns>
        /// Whole page text if page index is correct, else null
        /// </returns>
        public string ReadPDFPage(int page)
        {
            if (page > this.numberOfPages)
            {
                return null;
            }
            else {
                return this.pageContent[page];
            }
        }

     
        private void LoadPagesText()
        {
            int numberOfPages = this.document.getNumberOfPages();
            PDFTextStripper stripper = new PDFTextStripper();
            for (int i = 1; i <= numberOfPages; i++)
            {
                stripper.setStartPage(i);
                stripper.setEndPage(i);
                this.pageContent.Add(i, stripper.getText(this.document));
            }
        }


        private void LoadFonts()
        {

            PDFTextStripper stripper = new PDFTextStripper();
            for (int i = 1; i <= this.document.getNumberOfPages(); i++)
            {
                PDPage page = (PDPage)this.document.getDocumentCatalog().getAllPages().get(i - 1);
                java.util.Map fonts = page.getResources().getFonts();
                java.util.Iterator it = fonts.entrySet().iterator();
                while (it.hasNext())
                {
                    java.util.Map.Entry e = (java.util.Map.Entry)it.next();
                    PDFont font = (PDFont)e.getValue();
                    string fontname = font.getFontDescriptor().getFontName();
                    if (this.fonts.ContainsKey(i))
                    {
                        this.fonts[i].Add(fontname);
                    }
                    else
                    {
                        List<string> aux = new List<string>();
                        aux.Add(fontname);
                        this.fonts.Add(i, aux);
                    }
                }
            }
        }

        /// <summary>
        /// This method returns a whole page of the PDF document in the PDPageFormat. Author: Oscar Mullin
        /// </summary>
        /// <param name="page"></param>
        /// <returns>
        /// PDPage if index is correct, else null
        /// </returns>

        public PDPage GetPageInPDFFormat(int page)
        {
            if (page > this.numberOfPages)
            {
                return null;
            }
            else {
                return (PDPage)this.document.getDocumentCatalog().getAllPages().get(page);
            }
            
        }

        private void LoadPages() {
            for (int i = 1; i <= this.document.getNumberOfPages(); i++)
            {
                PDPage page = (PDPage)this.document.getDocumentCatalog().getAllPages().get(i - 1);
                this.Pages.Add(page);
            }
        }

        private void ReadImages()
        {
            List<PDXObjectImage> listOfImages = new List<PDXObjectImage>();
            int i = 1;
            foreach (var page in this.Pages)
            {
                PDResources pageResources = page.getResources();
                var pageImages = pageResources.getXObjects();
                if (pageImages != null) {
                    java.util.Iterator iterForImage = pageImages.keySet().iterator();
                    while (iterForImage.hasNext())
                    {
                        string key = (string)iterForImage.next();
                        PDXObject pdxObj = (PDXObject)pageImages.get(key);
                        PDXObjectImage image = (PDXObjectImage)pdxObj;
                        listOfImages.Add(image);
                    }
                }
                this.Images.Add(i, listOfImages);
                listOfImages = new List<PDXObjectImage>();
                i++;
            }
        }

    

        public bool FindTextInPage(string search, int page)
        {
            string text = this.ReadPDFPage(page);
            if (text.Contains(search))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
