using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Code reference: https://github.com/KIGAMESYTB/Upload-Image-From-Phone-Maui-Dotnet-App
**/

namespace ArbolesMAUI.Model
{
    /// <summary>
    /// Model for making an image file
    /// </summary>
    public class ImageFile
    {
        public string byteBase64 { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
