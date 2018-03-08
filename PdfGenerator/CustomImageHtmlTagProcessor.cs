using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;

namespace PdfGenerator
{
    public class CustomImageHTMLTagProcessor : IHTMLTagProcessor
    {
        public void EndElement(HTMLWorker worker, string tag)
        {
            throw new NotImplementedException();
        }

        public void StartElement(HTMLWorker worker, string tag, IDictionary<string, string> attrs)
        {
            Image image;
            var src = attrs["src"];

            if (src.StartsWith("data:image/"))
            {
                // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                var base64Data = src.Substring(src.IndexOf(",") + 1);
                var imagedata = Convert.FromBase64String(base64Data);
                image = Image.GetInstance(imagedata);
            }
            else
            {
                image = Image.GetInstance(src);
            }

            worker.UpdateChain(tag, attrs);
            worker.ProcessImage(image, attrs);
            worker.UpdateChain(tag);
        }
    }
}
