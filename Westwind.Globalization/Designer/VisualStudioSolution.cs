// Copyright, 2005 Oak Leaf Enterprises Solution Design, Inc.
// No part of this software or its associated documentation 
// may be stored in a retrieval system, copied, transmitted, 
// distributed, transcribed or reproduced in any other way or 
// disclosed to any third parties without the written permission 
// of Oak Leaf Enterprises Solution Design, Inc.

using EnvDTE;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VSLangProj;
using System.IO;


namespace Westwind.Globalization.Design
{
    /// <summary>
    /// Provides automation access to the current solution in design mode
    /// </summary>
    public class VisualStudioSolution
    {
        /// <summary>
        /// Gets the running object table
        /// </summary>
        /// <param name="res"></param>
        /// <param name="ROT"></param>
        /// <returns></returns>
        [DllImport("ole32.dll", EntryPoint = "GetRunningObjectTable")]
        public static extern uint GetRunningObjectTable(uint res, out
				UCOMIRunningObjectTable ROT);

        /// <summary>
        /// Creates Bind Ctx
        /// </summary>
        /// <param name="res"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport("ole32.dll", EntryPoint = "CreateBindCtx")]
        public static extern uint CreateBindCtx(uint res, out UCOMIBindCtx ctx);

        const uint S_OK = 0;


        private static EnvDTE._DTE DTE = null;

        /// <summary>
        /// Returns a reference to the Development Tools Extensibility (DTE) object
        /// </summary>
        /// <returns>DTE object</returns>
        public static EnvDTE._DTE GetDTE()
        {

            if (DTE != null)
                return DTE;

            // Get the DTE

            // The following method works fine IF you only have one instance of VS .NET open...
            // It returns the first created instance of VS .NET...not necessarily the current instance!
            //			return (EnvDTE._DTE)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");

            // This code was adapted from Girish Hegde's on-line article
            // "Adding or Changing Code using FileCodeModel in Visual Studio .NET
            EnvDTE.DTE dte = null;

            // Get the ROT
            UCOMIRunningObjectTable rot;
            uint uret = GetRunningObjectTable(0, out rot);
            if (uret == S_OK)
            {
                // Get an enumerator to access the registered objects
                UCOMIEnumMoniker EnumMon;
                rot.EnumRunning(out EnumMon);
                if (EnumMon != null)
                {
                    // Just grab a bunch of them at once, 100 should be
                    // plenty for a test
                    const int NumMons = 100;
                    UCOMIMoniker[] aMons = new UCOMIMoniker[NumMons];
                    int Fetched = 0;
                    EnumMon.Next(NumMons, aMons, out Fetched);

                    // Set up a binding context so we can access the monikers
                    string Name;
                    UCOMIBindCtx ctx;
                    uret = CreateBindCtx(0, out ctx);

                    // Create the ROT name of the _DTE object using the process id
                    System.Diagnostics.Process currentProcess =
                        System.Diagnostics.Process.GetCurrentProcess();

                    string dteName = "VisualStudio.DTE.";
                    string processID = ":" + currentProcess.Id.ToString();

                    // for each moniker retrieved
                    for (int i = 0; i < Fetched; i++)
                    {
                        // Get the display string
                        aMons[i].GetDisplayName(ctx, null, out Name);

                        // If this is the one we are interested in...
                        if (Name.IndexOf(dteName) != -1 && Name.IndexOf(processID) != -1)
                        {
                            object temp;
                            rot.GetObject(aMons[i], out temp);

                            dte = (EnvDTE.DTE)temp;
                            break;
                        }
                    }
                }
            }

            // *** Cache the reference if found
            DTE = dte;

            return dte;
        }


        /// <summary>
        /// Returns the name of the current solution
        /// </summary>
        /// <returns>Solution name</returns>
        public static string GetSolutionName()
        {
            Solution SolutionObj = GetSolution();
            return SolutionObj.FullName;
        }

        /// <summary>
        /// Returns a reference to the currrent solution
        /// </summary>
        /// <returns>Solution object</returns>
        public static Solution GetSolution()
        {
            // Get the DTE
            EnvDTE._DTE dte = GetDTE();

            // Get the current solution
            return dte.Solution;
        }

        /// <summary>
        /// Retuns an instance of the Active VS.NET Project COM object
        /// </summary>
        /// <returns></returns>
        public static Project GetActiveProject()
        {
            EnvDTE._DTE dte = GetDTE();
            if (dte == null)
                return null;

            if (dte.ActiveDocument == null)
                return null;

            Project proj = dte.ActiveDocument.ProjectItem.ContainingProject;
            if (proj == null)
                return null;

            return proj;
        }

        /// <summary>
        /// Returns the path of the current project
        /// </summary>
        /// <returns></returns>
        public static string GetActiveProjectPath()
        {
            Project proj = GetActiveProject();
            if (proj == null)
                return null;

            FileInfo fi = new FileInfo(proj.FullName);
            return fi.DirectoryName + "\\";
        }

        /// <summary>
        /// Returns a file path to web.config
        /// </summary>
        /// <returns></returns>
        public static string GetWebConfig()
        {
            Project proj = GetActiveProject();
            if (proj == null)
                return null;

            foreach (ProjectItem Item in proj.ProjectItems)
            {
                if (Item.Name.ToLower() == "web.config")
                {
                    FileInfo fi = new FileInfo(proj.FullName);
                    return fi.DirectoryName + "\\" + Item.Name;
                }
            }

            return null;
        }


        /// <summary>
        /// Returns a path to the active document
        /// </summary>
        /// <returns></returns>
        public static string GetActiveDocumentVirtualPath()
        {
            Project proj = GetActiveProject();
            if (proj == null)
                return null;

            FileInfo fi = new FileInfo(proj.FullName);
            string ProjectPath = fi.DirectoryName + "\\";

            return DTE.ActiveDocument.FullName.ToLower().Replace(ProjectPath.ToLower(), "/").Replace("\\", "/").TrimStart('/');
        }

        public Document GetActiveDocument()
        {
            return null;
        }

        /// <summary>
        /// Returns an ArrayList containing a list of unique assemblies referenced
        /// by all projects in the current solution
        /// </summary>
        /// <returns>ArrayList containing assemblies</returns>
        public static ArrayList GetReferencedAssemblies()
        {
            ArrayList assemblies = new ArrayList();

            // Get a reference to the current solution
            Solution SolutionObj = GetSolution();

            VSProject vsProj;

            // Iterate through all projects in the solution
            foreach (Project proj in SolutionObj.Projects)
            {
                // Check for either a C# or VB .NET project
                if (proj.Kind == PrjKind.prjKindCSharpProject ||
                    proj.Kind == PrjKind.prjKindVBProject)
                {
                    // Get the project object
                    vsProj = (VSProject)proj.Object;

                    // Iterate through all assembly references in the project
                    foreach (Reference refItem in vsProj.References)
                    {
                        // See if the assembly is already in the ArrayList
                        if (!assemblies.Contains(refItem.Type))
                        {
                            // Add the reference to the ArrayList
                            assemblies.Add(refItem.Path);
                        }
                    }
                }
            }
            return assemblies;
        }
    }
}
