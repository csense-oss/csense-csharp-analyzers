using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace csenseCsharpAnalyzers.bll {
    public static class DiagnosticAnalyzerLocalizable {
        public static LocalizableString GetLocalizableStringFromResources(
            string nameOfParameter) =>
            new LocalizableResourceString(nameOfParameter, Resources.ResourceManager, typeof(Resources));
    }
}
