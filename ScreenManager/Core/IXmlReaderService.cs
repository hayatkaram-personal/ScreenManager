﻿using ScreenManager.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ScreenManager.Core
{
    public interface IXmlReaderService
    {
        ThemeLayout XmlParser(string filePath);
    }
}
