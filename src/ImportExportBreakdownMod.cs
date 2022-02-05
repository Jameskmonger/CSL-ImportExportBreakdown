using ICities;
using ImportExportBreakdown.Redirection;
using UnityEngine;

namespace ImportExportBreakdown
{
    public class ImportExportBreakdownMod : IUserMod
    {
        public string Name => "Import Export Breakdown";

        public string Description => "Breaks down import/export information into raw/processed materials.";

        public void OnEnabled() {
            Debug.Log("Enabled!");

            Redirector.PerformRedirections();
        }
    }
}
