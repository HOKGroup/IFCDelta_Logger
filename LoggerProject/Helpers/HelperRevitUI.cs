using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using adWin = Autodesk.Windows;

namespace Helpers
{
    public static class HelperRevitUI
    {
        private static readonly string PanelColor = "#FFF2CC";

        private static RibbonPanel _panel;

        /// <summary>Adds the ribbon tab.</summary>
        /// <param name="application">The application.</param>
        /// <param name="tabName">The name of the tab to be created.</param>
        /// <returns>
        ///  true if the RibbonTab created; otherwise, false.
        /// </returns>
        public static bool AddRibbonTab(UIControlledApplication application, string tabName)
        {
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch(Exception ex)
            {
                TaskDialog.Show("Error", "HelperRevitUI File\n" + ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Adds the ribbon panel.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="tabName">The name of the tab, on which the new panel will be created.</param>
        /// <param name="panelName">The name of the panel to be created.</param>
        /// <param name="addSeparator">if set to <c>true</c> [add separator]
        /// and panel exists with given name, separator will be added in the panel.</param>
        /// <returns></returns>
        public static RibbonPanel AddRibbonPanel(UIControlledApplication application, string tabName, string panelName, bool addSeparator)
        {
            List<RibbonPanel> panels = application.GetRibbonPanels(tabName);
            RibbonPanel panel = panels.FirstOrDefault(x => x.Name == panelName);

            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tabName, panelName);
            }
            else if (addSeparator)
            {
                panel.AddSeparator();
            }
               _panel = panel;
            return panel;
        }

        /// <summary>
        /// Sets panel title background.
        /// </summary>
        /// <param name="tabName">The name of the tab, on which the all panel title background will be changed.</param>
        public static void SetColor(string tabName)
        {
            adWin.RibbonControl ribbon = adWin.ComponentManager.Ribbon;
            SolidColorBrush gradientBrush = new SolidColorBrush();

            System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml(PanelColor);
            gradientBrush.Color = Color.FromArgb(color.A, color.R, color.G, color.B);

            foreach (adWin.RibbonTab tab in ribbon.Tabs)
            {
                if (tab.Name == tabName)
                {
                    foreach (adWin.RibbonPanel panelInternal in tab.Panels)
                    {
                        panelInternal.CustomPanelTitleBarBackground = gradientBrush;
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Adds the push button.
        /// </summary>
        /// <param name="panel">The name of the panel, on which the new button will be created.</param>
        /// <param name="name">The name of the button to be created.</param>
        /// <param name="title">The user visible text seen on the new button.</param>
        /// <param name="targetClass">The name of the class containing the implementation for the command.</param>
        /// <param name="path">The assembly path of the button.</param>
        /// <returns></returns>
        public static PushButton AddPushButton(RibbonPanel panel, string name, string title, Type targetClass, string path)
        {
            var button = panel.AddItem(new PushButtonData(name, title, path, targetClass.FullName)) as PushButton;

            return button;
        }
        /// <summary>
        /// Get a pushButton in cetain panel by name
        /// </summary>
        /// <param name="panel">The name of the panel, on which the new button will be created.</param>
        /// <param name="name">The name of the button to be created.</param>
        /// <returns></returns>
        public static PushButton GetPushButton( string name)
        {

            // var button = panel.AddItem(new PushButtonData(name, title, path, targetClass.FullName)) as PushButton;
            var panelItems = _panel.GetItems();
            PushButton button = null;
            foreach (RibbonItem item in panelItems)
            {
                if (item.ItemType == RibbonItemType.PushButton && item.Name == name )
                {
                    button = item as PushButton;
                }
            }

            return button;
        }




    }








}