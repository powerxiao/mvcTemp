using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCTempWizard
{
    public class ChildWizardImpl : IWizard
    {
        #region IWizard Members

        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem) { }

        public void ProjectFinishedGenerating(EnvDTE.Project project) { }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem) { }

        public void RunFinished() { }

        public void RunStarted(object automationObject,
            Dictionary<string, string> replacementsDictionary,
            WizardRunKind runKind, object[] customParams)
        {
            string safeprojectname = RootWizardImpl.GlobalParameters.Where(p => p.Key == "$safeprojectname$").First().Value;
            replacementsDictionary["$safeprojectname$"] = safeprojectname;
        }

        public bool ShouldAddProjectItem(string filePath) { return true; }

        #endregion
    }
}
