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
            string formNamesListed = string.Concat(forms.Select(x => x.ToString() + "\n"));
            if (availability.available)
            {
                Log.Message("Forms "+formNamesListed+" are trying to be added after the catalog is initialized");
                return;
            }

            Log.Message("Adding new FormDef(s) to catalog.\n"+ formNamesListed);
            int length = formsCatalog.Length;
            Array.Resize(ref formsCatalog, length + forms.Length);
            for (int i = 0; i < forms.Length; i++)
            {
                // Adding form to catalog
                formsCatalog[length + i] = forms[i];
                Log.Message("FormDef "+ forms[i].name +" added to catalog");
            }

            formsCatalog = formsCatalog.OrderBy(form => form.name).ToArray();

            string allForms = string.Concat(formsCatalog.Select(x => x.ToString() + "\n"));
            Log.Message("FormDef(s) added to formCatalog. formCatalog now contains:\n"+allForms);
        }

        // I will make all modded forms require risk of options to sort out controls. Otherwise I'd have to put effort into some kind of form picker ui wheel and that would be mega complicated
        public static void InitializeFormConfigs()
        {
            KeyCode[] defaultKeys = new KeyCode[10]
            {
                KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0
            };
            List<KeyCode> usedKeys = new List<KeyCode>();
            int count = 0;
            foreach (FormDef form in formsCatalog)
            {
                if (form.defaultKeyBind != KeyCode.None)
                {
                    form.keybind = SonicTheHedgehogPlugin.instance.Config.Bind<KeyboardShortcut>("Controls", form.ToString() + " Transform Key", new KeyboardShortcut(form.defaultKeyBind), "The key you press to transform into the " + form.ToString() + " form. This config is automatically generated.");
                    if (usedKeys.Contains(form.defaultKeyBind))
                    {
                        Log.Warning("Multiple forms share the same default keybind of " + form.defaultKeyBind.ToString());
                    }
                    else
                    {
                        usedKeys.Add(form.defaultKeyBind);
                    }
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
