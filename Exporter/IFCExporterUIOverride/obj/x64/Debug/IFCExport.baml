   M S B A M L   `   `   `  ????U  QLoggerIFCExportUIOverride, Version=23.1.0.0, Culture=neutral, PublicKeyToken=nullK*clr-namespace:BIM.IFC.Export.UI.PropertiesBIM.IFC.Export.UI.Properties  5      N JAutodesk.UI.Windows, Version=3.0.0.0, Culture=neutral, PublicKeyToken=nullV>clr-namespace:Autodesk.UI.Windows;assembly=Autodesk.UI.WindowsAutodesk.UI.Windows 5      O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5      !    BIM.IFC.Export.UI.IFCExport   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      8p*clr-namespace:BIM.IFC.Export.UI.Properties     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Oauiw>clr-namespace:Autodesk.UI.Windows;assembly=Autodesk.UI.Windows     5      9System&clr-namespace:System;assembly=mscorlib     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5	      -   5        ,   &BIM.IFC.Export.UI.Properties.Resources    	IFCExport ? Title# Z  5       ? WindowStyle$ SingleBorderWindow=?5      %  Autodesk.UI.Windows.ChildWindow   SaveWindowSettings . $	??438??5      $	??730??5      $	??550??6   $	??300??6.   .??5      ? $
??10,0q?6
    J? RenderTransformOrigin$	 0,0&?6   .+?5      ?? -   5         ??Name$ label_FileName??5   e      FileName#??Z 6   $
??Left=?68   $??0,12,0,0q?6S   $??28??6}   $	??Top=?6?   $	??202??6?   $	??Top=?6?   $ 0.462,0.478&?6?   ?? 5      -   5        $??textBoxSetupFileName??5   2   $??23??6   $??210,12,125,0q?6   $	??Top=?6N   {?.  ?? TextWrapping$ NoWrap=?6x   $??Center=?6?   ?? 5      -   5        $??buttonBrowse??5   V      Browse#??Z 6   $??23??67   $??	0,12,11,0q?6C   $	??Top=?6j   $??Right=?6?   $	??107??6?   ?? 5      -   5        $??label1??5   q   	   CurrentSelectedSetup#??Z	 6   $
??Left=?6D   $??0,45,0,0q?6_   $??28??6   $	??Top=?6?   $	??202??6?   ?? 5      -   5        $??currentSelectedSetup??5   3   $??23??6   $??210,48,202,0q?6   $	??Top=?6O   ??6?   ?? 
 ? DataContext
 6?   ?? ?? 5      -   5        $??buttonEditSetup??5   [      ModifySetup#??Z 6   $??23??6<   $??	0,48,11,0q?6H   $	??Top=?6r   $??Right=?6?   $	??184??6?   ?? 5      -   5        $ label1_Copy1??5   o      IFCVersionSelected#??Z 6   $
??Left=?6B   $??0,78,0,0q?6]   $??28??6?   $	??Top=?6?   $	??202??6?   $	??Top=?6?   ?? 5      -	   5        $??textBoxSetupDescription??5      $??28??5      $??210,78,10,0q?5      $	??Top=?5      {?.  ?? 
IsReadOnly . $ WrapWithOverflow=?5       $??0q?5!   B   $??Center=?5"      
?????? 5#      ?? 5$      -
   5        $ label1_Copy??5$   m      ProjectToExport#??Z 6   $
??Left=?6?   $??	0,171,0,0q?6Z   $??28??6?   $	??Top=?6?   $	??666??6?   ?? 5%      -   5         ??Name$ listViewDocuments??5%   V   $??0,204,11,50q?6   $
??Auto=?6&   6V   ?? 5&      $??Bottom=?6   $??0,0,0,10q?6.   $??23??6@   $
??Left=?6L   $	??515??6g   ?? 5'      -   5        .?5'   )   .?5(      ??    Help#?Z 6   LearnMoreHyperlink??65   5)      5*   	   ?? 5+      -   5        $??
buttonNext??5+   J      Export#??Z 6   $??	0,0,93,10q?67   $??23??6w   $??Bottom=?6?   $??Right=?6?   $??77??6?   ?? 5,      -   5        $??buttonCancel??5,   J      Cancel#??Z 6   $??	0,0,11,10q?67   $??23??6y   $??Bottom=?6?   $??Right=?6?   $??77??6?   ?? 5-      -   5        $ label1_Copy2??5-   k      ProjectOrigin#??Z 6   $
??Left=?6=   $??	0,111,0,0q?6X   $??28??6?   $	??Top=?6?   $	??202??6?   $	??Top=?6?   ?? 5.      -   5         ??Name$  textBoxSetupCoordinateBase??50   
   $??28??5.      $??210,111,10,0q?5/   
   $	??Top=?51   
   {?.  . $ WrapWithOverflow=?54   
   $??0q?56   
   $??Center=?57   
   
?????? 58   
   ?? 59      -   5        $ label_ProjectLocation??59   i      ProjectSite#??Z 6   $
??Left=?6;   $??	0,141,0,0q?6V   $??28??6?   $	??Top=?6?   $	??202??6?   $	??Top=?6?   ?? 5:      -   5        $! textBoxSetupProjectLocation??5<   
   $??28??5:      $??210,141,10,0q?5;   
   $	??Top=?5=   
   {?.  . $ WrapWithOverflow=?5@   
   $??0q?5A   
   $??Center=?5B   
   
?????? 5C   
   5D       ? 	Resources 5F         ifcExportWindowTitle  currentSelectedSetup  next  saveSetupAndClose 
 cancel??%U  &	        &	 8     &	 {     &	 ?     &	 ?     ??5G      $ {x:Static p:Resources.IFCExport}??6U   ??5H      /+{x:Static p:Resources.CurrentSelectedSetup}??6`   ??5I      {x:Static p:Resources.Next}??6@   ??5J      ,({x:Static p:Resources.SaveSetupAndClose}??6Z   ??5K      !{x:Static p:Resources.Cancel}??6D   5L      5M      