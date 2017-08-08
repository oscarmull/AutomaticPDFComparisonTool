using NUnit.Framework;
using System;
using PDFReader;
namespace UnitTests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void DifferentFilesAreDifferent()
        {
			PDFComparer comparer = new PDFComparer("Sample_1.pdf", "Sample_3.pdf");
			var areTheSame = comparer.AreEquivalentDocuments();
            Assert.IsFalse(areTheSame);
        }

		[Test()]
		public void EqualFilesAreEqual()
		{
			PDFComparer comparer = new PDFComparer("Sample_1.pdf", "Sample_1.pdf");
			var areTheSame = comparer.AreEquivalentDocuments();
			Assert.IsTrue(areTheSame);
		}
    }
}
