using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTempWizard
{
    public class RootWizardImpl : IWizard
    {
        private string safeprojectname;
        private static Dictionary<string, string> globalParameters = new Dictionary<string, string>();

        public static IEnumerable<KeyValuePair<string, string>> GlobalParameters
        {
            get { return globalParameters; }
        }

        #region IWizard Members

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem) { }

        public void ProjectFinishedGenerating(EnvDTE.Project project) { }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem) { }

        public void RunFinished() { }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            safeprojectname = replacementsDictionary["$safeprojectname$"];
            globalParameters["$safeprojectname$"] = safeprojectname;
        }

        public bool ShouldAddProjectItem(string filePath) { return true; }

        #endregion
    }
}
