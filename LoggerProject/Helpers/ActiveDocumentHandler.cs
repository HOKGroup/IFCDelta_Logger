using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace Helpers
{
  public interface IDocumentValidator
  {
    bool IsValid(ActiveDocumentHandler documentHandler);
  }

  public class ActiveDocumentHandler
  {
    public delegate void DocumentChangedEventHandler(bool isFamilyDocument);
    public static event DocumentChangedEventHandler DocumentChangedEvent;

    private UIControlledApplication UIControlledApp = null;
    private Document mCurrentDocument = null;
    private Window mCurrentWindow = null;
    private string mPluginName = null;

    public static ActiveDocumentHandler Instance { get; set; }
    public UIApplication UIApp = null;
    public IDocumentValidator DocumentValidator = null;
    public Document ActiveDocument = null;

    public ActiveDocumentHandler(UIControlledApplication controlledApp, string pluginName)
    {
      UIControlledApp = controlledApp;
      mPluginName = pluginName;
      BindDocumentEvents();
    }

    public void BindWindowEvent(UIApplication uiApp, Window window, Document document)
    {
      UIApp = uiApp;
      mCurrentWindow = window;
      mCurrentDocument = document;
      ActiveDocument = document;
      window.Loaded += Window_Loaded;
      window.Closed += Window_Closed;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      DocumentChangedEvent += DocumentChanged;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      DocumentChangedEvent -= DocumentChanged;
    }

    private void BindDocumentEvents()
    {
      UIControlledApp.ControlledApplication.DocumentClosed += DocumentClosed;
      UIControlledApp.ControlledApplication.DocumentOpened += DocumentOpened;
      UIControlledApp.ViewActivated += OnViewActivated;
    }

    public void UnBindDocumentEvents()
    {
      UIControlledApp.ControlledApplication.DocumentClosed -= DocumentClosed;
      UIControlledApp.ControlledApplication.DocumentOpened -= DocumentOpened;
      UIControlledApp.ViewActivated -= OnViewActivated;
    }


    private void DocumentOpened(object sender, DocumentOpenedEventArgs args)
    {
      if (DocumentValidator != null && !DocumentValidator.IsValid(this))
      {
        return;
      }
      DocumentChangedEvent?.Invoke(false);
    }

    private void DocumentChanged(object sender, DocumentChangedEventArgs e)
    {
      // DocumentChangedEvent(false);
    }

    private void DocumentClosed(object sender, DocumentClosedEventArgs e)
    {
      if (DocumentValidator != null && !DocumentValidator.IsValid(this))
      {
        return;
      }
      if (DocumentChangedEvent != null)
      {
        try
        {
          if (UIApp.ActiveUIDocument == null || UIApp.ActiveUIDocument.Document.PathName != mCurrentDocument.PathName)
          {
            DocumentChangedEvent(false);
          }
        }
        catch (Exception ex )
        {
                    TaskDialog.Show("Error", "ActiveDocumentHandler File\n" + ex.Message);
                    DocumentChangedEvent(false);
        }
      }
    }

    private void OnViewActivated(object sender, ViewActivatedEventArgs e)
    {
      if (DocumentChangedEvent != null)
      {
        try
        {
          if (UIApp.ActiveUIDocument == null || UIApp.ActiveUIDocument.Document.PathName != mCurrentDocument.PathName)
          {
            DocumentChangedEvent(false);
          }
        }
        catch (Exception ex)
        {
                    TaskDialog.Show("Error", "ActiveDocumentHandler File\n" + ex.Message);
                    DocumentChangedEvent(false);
        }
      }
    }

    private void DocumentChanged(bool isFamilyDocument)
    {
      if (mCurrentDocument == null)
      {
        return;
      }

      mCurrentWindow.Focus();
      MessageBox.Show($"Active document has been changed, please click Close and open {mPluginName} again.", "Close");
      mCurrentWindow.Close();
      ActivateWindow();
    }

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    public static bool ActivateWindow()
    {
      IntPtr ptr = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

      if (ptr != IntPtr.Zero)
      {
        return SetForegroundWindow(ptr);
      }

      return false;
    }
  }
}