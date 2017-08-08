# AutomaticPDFComparisonTool
This tool uses integration between C# and Java (using IKVM) to compare PDF documents.

By using this repo you can compare PDF documents at the following levels:

1. Are equivalent documents (have the exact same number of pages, text, fonts and images on a page basis)
2. Have the same number of pages
3. Have the exact same text (without considering page of the text)
4. Have the same text on each page
5. Have the same fonts on each page
6. Have the images on each page. Since it is not possible to compare image pixel by pixel in a performant way using this (we could use imageMagick), it checks each image on each page against same image on same page and verifies Width, Height, Extension and Decode

References for WebDriver are included but at the point of writing this, didn't have time to implement that portion in this particular repo.
