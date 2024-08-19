using BepInEx.Configuration;
using RiskOfOptions.Options;
using RiskOfOptions;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace SonicTheHedgehog.Modules.Forms
{
    // Thank you VarianceAPI for most of this stuff (conceptually)
    public static class FormCatalog
    {
        public static FormDef[] formsCatalog = Array.Empty<FormDef>();

        public static ResourceAvailability availability = default(ResourceAvailability);

        [SystemInitializer]
        private static void SystemInit()
        {
            Log.Message("FormCatalog initialized");
            InitializeFormConfigs();
            availability.MakeAvailable();
        }
        public static void AddFormDefs(FormDef[] forms)
        {
            if (availability.available)
            {
                Debug.LogErrorFormat("Forms {0} are trying to be added after the catalog is initialized", new object[] { string.Concat(forms.Select(x => x.ToString() + "\n")) });
                return;
            }

            Debug.LogFormat("Adding new FormDef(s) to catalog.\n {0}", new object[] { string.Concat(forms.Select(x => x.ToString() + "\n")) });
            int length = formsCatalog.Length;
            Array.Resize(ref formsCatalog, length + forms.Length);
            for (int i = 0; i < forms.Length; i++)
            {
                // Adding form to catalog
                formsCatalog[length + i] = forms[i];
                Debug.LogFormat("FormDef {0} added to catalog", new object[] { forms[i].name });
            }

            formsCatalog = formsCatalog.OrderBy(form => form.name).ToArray();

            Debug.LogFormat("FormDef(s) added to formCatalog. formCatalog now contains:\n {0}", new object[] { string.Concat(formsCatalog.Select(x => x.ToString() + "\n")) });
        }

        // I will make all modded forms require risk of options to sort out controls. Otherwise I'd have to put effort into some kind of form picker ui wheel and that would be mega complicated
        public static void InitializeFormConfigs()
        {
            KeyCode[] defaultKeys = new KeyCode[10]
            {
                KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
            };
            int count = 0;
            foreach (FormDef form in formsCatalog)
            {
                if (form == Forms.superSonicDef)
                {
                    form.keybind = SonicTheHedgehogPlugin.instance.Config.Bind<KeyboardShortcut>("Controls", form.ToString() + " Transform Key", new KeyboardShortcut(KeyCode.V), "The key you press to transform into the " + form.ToString() + " form. This config is automatically generated.");
                    continue;
                }
                form.keybind = SonicTheHedgehogPlugin.instance.Config.Bind<KeyboardShortcut>("Controls", form.ToString() + " Transform Key", new KeyboardShortcut(defaultKeys[count]), "The key you press to transform into the " + form.ToString() + " form. This config is automatically generated.");
                count = (count + 1) % defaultKeys.Length;
            }
            if (SonicTheHedgehogPlugin.riskOfOptionsLoaded)
            {
                InitializeFormConfigRiskOfOptions();
            }
        }

        public static void InitializeFormConfigRiskOfOptions()
        {
            foreach (FormDef form in formsCatalog)
            {
                ModSettingsManager.AddOption(new KeyBindOption(form.keybind));
            }
        }
    }
}
