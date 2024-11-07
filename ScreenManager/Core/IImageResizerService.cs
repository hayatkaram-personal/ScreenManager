using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenManager.Core
{
    public interface IImageResizerService
    {
        void ResizeIamge(string srcImgPath,string destImgPath, int width, int height);
    }
}
