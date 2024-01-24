using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Form = System.Windows.Forms.Form;

namespace UnitsConverter
{
    public partial class ConverterForm : Form
    {
        private readonly ExternalCommandData cmdData;
        private readonly ArrayList fileTypes = new ArrayList();
        private StreamWriter writer;
        private readonly IList<FileInfo> files = new List<FileInfo>();
        private readonly IList<string> failures = new List<string>();
        private int success;
        private int failed;

        // Variable to store if user cancels the process

        private bool cancelled;
        private bool addInfo;

        // Container for previous opened document

        private UIDocument previousDocument;

        public ConverterForm(ExternalCommandData commandData)
        {
            InitializeComponent();

            // Keep a local copy of the command data

            cmdData = commandData;

            previousDocument = null;
        }

        // Handler for Source folder browse button
        private void btnSource_Click(object sender, EventArgs e)
        {
            // Open the folder browser dialog

            var dlg = new FolderBrowserDialog();

            // Disable New Folder button since it is source location

            dlg.ShowNewFolderButton = false;

            // Provide description 

            dlg.Description = "Select the Source folder :";

            // Show the folder browse dialog

            dlg.ShowDialog();

            // Populate the source path text box

            txtSrcPath.Text = dlg.SelectedPath;
        }

        // Handler for the Destination folder browse button
        private void btnDestination_Click(object sender, EventArgs e)
        {
            // Open the folder browser dialog

            var dlgDest = new FolderBrowserDialog();

            // Enable the New folder button since users should have
            // ability to create destination folder incase it did 
            // not pre-exist

            dlgDest.ShowNewFolderButton = true;

            // Provide description

            dlgDest.Description = "Select the Destination folder : ";

            // Show the folder browse dialog

            dlgDest.ShowDialog();

            // Populate the destination path text box

            txtDestPath.Text = dlgDest.SelectedPath;
        }

        // Handler for the Cancel button
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Set the cancelled variable to true

            cancelled = true;
            Close();
        }

        public void TraverseAll(DirectoryInfo source,
            DirectoryInfo target)
        {
            try
            {
                // Check for user input events

                Application.DoEvents();

                // If destination directory does not exist, 
                // create new directory

                if (!Directory.Exists(target.FullName)) Directory.CreateDirectory(target.FullName);

                foreach (var fi in source.GetFiles())
                {
                    // Check for user input events

                    Application.DoEvents();
                    if (!cancelled)
                    {
                        var sec =
                            fi.GetAccessControl();
                        if (!sec.AreAccessRulesProtected)
                        {
                            // Proceed only if it is not a back up file

                            if (IsNotBackupFile(fi))
                            {
                                // Check if the file already exists, if not proceed

                                if (!AlreadyExists(target, fi))
                                {
                                    // The method contains the code to upgrade the file

                                    Upgrade(fi, target.FullName);
                                }
                                else
                                {
                                    // Print that the file already exists

                                    var msg = " already exists!";
                                    writer.WriteLine("------------------------------");
                                    writer.WriteLine("Error: "
                                                     + target.FullName + "\\" + fi.Name + " " + msg);
                                    writer.WriteLine("------------------------------");
                                    writer.Flush();

                                    lstBxUpdates.Items.Add(
                                        "-------------------------------");
                                    lstBxUpdates.Items.Add("Error: "
                                                           + target.FullName + "\\" + fi.Name + " " + msg);
                                    lstBxUpdates.Items.Add(
                                        "-------------------------------");
                                    lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                                }
                            }
                        }
                        else
                        {
                            var msg = " is not accessible or read-only!";
                            writer.WriteLine("-------------------------------");
                            writer.WriteLine("Error: " + fi.FullName + msg);
                            writer.WriteLine("-------------------------------");
                            writer.Flush();

                            lstBxUpdates.Items.Add(
                                "------------------------------");
                            lstBxUpdates.Items.Add("Error: " + fi.FullName + msg);
                            lstBxUpdates.Items.Add(
                                "------------------------------");
                            lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                        }
                    }
                }

                // Check for user input events

                Application.DoEvents();

                // RFT resave creates backup files 
                // Delete these backup files created
                foreach (var backupFile in target.GetFiles())
                    if (!IsNotBackupFile(backupFile))
                        File.Delete(backupFile.FullName);

                // Using recursion to work with sub-directories

                foreach (var sourceSubDir in
                         source.GetDirectories())
                {
                    var nextTargetSubDir =
                        target.CreateSubdirectory(sourceSubDir.Name);
                    TraverseAll(sourceSubDir, nextTargetSubDir);

                    // Delete the empty folders - this is created when
                    // none of the files in them meet our upgrade criteria

                    if (nextTargetSubDir.GetFiles().Count() == 0 &&
                        nextTargetSubDir.GetDirectories().Count() == 0)
                        Directory.Delete(nextTargetSubDir.FullName);
                }
            }
            catch
            {
            }
        }

        // Helper method to check if file already exists in target folder
        private bool AlreadyExists(DirectoryInfo target, FileInfo file)
        {
            foreach (var infoTarget in target.GetFiles())
                if (infoTarget.Name.Equals(file.Name))
                    return true;
            return false;
        }

        // Helps determine if the source file is back up file or not
        // Backup files are determined by the format : 
        // <project_name>.<nnnn>.rvt
        // This utility ignores backup files
        private bool IsNotBackupFile(FileInfo rootFile)
        {
            // Check if the file is a backup file

            if (rootFile.Name.Length < 9) return true;

            if (rootFile.Name.Substring(rootFile.Name.Length - 9)
                    .Length > 0)
            {
                var backUpFileName = rootFile.Name.Substring(
                    rootFile.Name.Length - 9);
                long result = 0;

                // Check each char in the file name if it follows 
                // the back up file naming convention

                if (
                    backUpFileName[0].ToString().Equals(".")
                    && long.TryParse(backUpFileName[1].ToString(), out result)
                    && long.TryParse(backUpFileName[2].ToString(), out result)
                    && long.TryParse(backUpFileName[3].ToString(), out result)
                    && long.TryParse(backUpFileName[4].ToString(), out result)
                )
                    return false;
            }

            return true;
        }

        // Searches the directory and creates an internal list of files
        // to be upgraded
        private void SearchDir(DirectoryInfo sDir, bool first)
        {
            try
            {
                // If at root level, true for first call to this method

                if (first)
                    foreach (var rootFile in sDir.GetFiles())
                        // Create internal list of files to be upgraded
                        // This will help with Progress bar
                        // Proceed only if it is not a back up file
                        if (IsNotBackupFile(rootFile))
                            // Keep adding files to the internal list of files
                            if (fileTypes.Contains(rootFile.Extension)
                                || rootFile.Extension.Equals(".txt"))
                            {
                                if (rootFile.Extension.Equals(".txt"))
                                {
                                    if (fileTypes.Contains(".rfa"))
                                        foreach (var rft in sDir.GetFiles())
                                            if (
                                                rft.Name.Remove(rft.Name.Length - 4, 4)
                                                    .Equals(
                                                        rootFile.Name.Remove(
                                                            rootFile.Name.Length - 4, 4)
                                                    ) &&
                                                !rft.Extension.Equals(rootFile.Extension)
                                            )
                                            {
                                                files.Add(rootFile);
                                                break;
                                            }
                                }
                                else
                                {
                                    files.Add(rootFile);
                                }
                            }

                // Get access to each sub-directory in the root directory

                foreach (var direct in sDir.GetDirectories())
                {
                    var sec =
                        direct.GetAccessControl();
                    if (!sec.AreAccessRulesProtected)
                    {
                        foreach (var fInfo in direct.GetFiles())
                            // Proceed only if it is not a back up file
                            if (IsNotBackupFile(fInfo))
                                // Keep adding files to the internal list of files
                                if (fileTypes.Contains(fInfo.Extension)
                                    || fInfo.Extension.Equals(".txt"))
                                {
                                    if (fInfo.Extension.Equals(".txt"))
                                    {
                                        if (fileTypes.Contains(".rfa"))
                                            foreach (var rft in direct.GetFiles())
                                                if (
                                                    rft.Name.Remove(
                                                        rft.Name.Length - 4, 4).Equals(
                                                        fInfo.Name.Remove(fInfo.Name.Length - 4, 4)
                                                    )
                                                    && !rft.Extension.Equals(fInfo.Extension)
                                                )
                                                {
                                                    files.Add(fInfo);
                                                    break;
                                                }
                                    }
                                    else
                                    {
                                        files.Add(fInfo);
                                    }
                                }

                        // Use recursion to drill down further into 
                        // directory structure

                        SearchDir(direct, false);
                    }
                    else
                    {
                        var msg = " is not accessible or read-only!";
                        writer.WriteLine("------------------------------------");
                        writer.WriteLine("Error: " + direct.FullName + msg);
                        writer.WriteLine("------------------------------------");
                        writer.Flush();

                        lstBxUpdates.Items.Add("------------------------------");
                        lstBxUpdates.Items.Add("Error: " + direct.FullName + msg);
                        lstBxUpdates.Items.Add("------------------------------");
                        lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                    }
                }
            }
            catch (Exception excpt)
            {
                writer.WriteLine("-------------------------------------");
                writer.WriteLine("Error :" + excpt.Message);
                writer.WriteLine("-------------------------------------");
                writer.Flush();
            }
        }

        // Handler code for the Upgrade button click event
        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            // Initialize the count for success and failed files

            success = 0;
            failed = 0;
            fileTypes.Clear();


            // add rfa files only

            fileTypes.Add(".rfa");

            // Error handling with file types

            if (fileTypes.Count == 0)
            {
                TaskDialog.Show("No File Types",
                    "Please select at least one file type!");
                return;
            }

            // Ensure all path information is filled in 

            if (txtSrcPath.Text.Length > 0
                && txtDestPath.Text.Length > 0)
            {
                // Perform checks to see if all the paths are valid

                var dir = new DirectoryInfo(txtSrcPath.Text);
                var dirDest = new DirectoryInfo(txtDestPath.Text);

                if (!dir.Exists)
                {
                    txtSrcPath.Text = string.Empty;
                    return;
                }

                if (!dirDest.Exists)
                {
                    txtDestPath.Text = string.Empty;
                    return;
                }

                // Ensure destination folder is not inside the source folder
                var dirs = from nestedDirs in dir.EnumerateDirectories("*")
                    where dirDest.FullName.Contains(nestedDirs.FullName)
                    select nestedDirs;
                if (dirs.Count() > 0)
                {
                    TaskDialog.Show(
                        "Abort Conversion",
                        "Selected Destination folder, " + dirDest.Name +
                        ", is contained in the Source folder. Please select a" +
                        " Destination folder outside the Source folder.");
                    txtDestPath.Text = string.Empty;
                    return;
                }

                // If paths are valid
                // Create log and initialize it

                writer = File.CreateText(
                    txtDestPath.Text + "\\" + "ConversionLog.txt"
                );

                // Clear list box 

                lstBxUpdates.Items.Clear();
                files.Clear();

                // Progress bar initialization

                bar.Minimum = 1;

                // Search the directory and create the 
                // list of files to be upgraded

                SearchDir(dir, true);

                // Set Progress bar base values for progression

                bar.Maximum = files.Count;
                bar.Value = 1;
                bar.Step = 1;

                // Traverse through source directory and upgrade
                // files which match the type criteria

                TraverseAll(
                    new DirectoryInfo(txtSrcPath.Text),
                    new DirectoryInfo(txtDestPath.Text));

                // In case no files were found to match 
                // the required criteria

                if (failed.Equals(0) && success.Equals(0))
                {
                    var msg = "No relevant files found for conversion!";
                    TaskDialog.Show("Incomplete", msg);
                    writer.WriteLine(msg);
                    writer.Flush();
                }
                else
                {
                    if (failures.Count > 0)
                    {
                        var msg = "-------------"
                                  + "List of files that "
                                  + "failed to be upgraded"
                                  + "--------------------";

                        // Log failed files information

                        writer.WriteLine("\n");
                        writer.WriteLine(msg);
                        writer.WriteLine("\n");
                        writer.Flush();

                        // Display the failed files information

                        lstBxUpdates.Items.Add("\n");
                        lstBxUpdates.Items.Add(msg);
                        lstBxUpdates.Items.Add("\n");
                        lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                        foreach (var str in failures)
                        {
                            writer.WriteLine(str);
                            lstBxUpdates.Items.Add("\n" + str);
                            lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                        }

                        failures.Clear();
                        writer.Flush();
                    }

                    // Display final completion dialog 
                    // with success rate

                    TaskDialog.Show("Completed",
                        success + "/" + (success + failed)
                        + " files have been successfully upgraded! "
                        + "\n\nA log file has been created at :\n"
                        + txtDestPath.Text);
                }
                // Reset the Progress bar

                bar.Value = 1;

                // Close the Writer object

                writer.Close();
            }
        }

        // Method which upgrades each file
        private void Upgrade(FileInfo file, string destPath)
        {
            addInfo = false;

            // Check if file type is what is expected to be upgraded
            // or is a text file which is for files which contain 
            // type information for certain family files

            if (fileTypes.Contains(file.Extension)
                || file.Extension.Equals(".txt"))
                try
                {
                    // If it is a text file
                    if (file.Extension.Equals(".txt"))
                    {
                        if (fileTypes.Contains(".rfa"))
                        {
                            var copy = false;

                            // Check each file from the list to see 
                            // if the text file has the same name as 
                            // any of the family files or if it is 
                            // just a standalone text file. In case 
                            // of standalone text file, ignore.
                            foreach (var rft in files)
                                if (
                                    rft.Name.Remove(rft.Name.Length - 4, 4).Equals(
                                        file.Name.Remove(file.Name.Length - 4, 4))
                                    && !rft.Extension.Equals(file.Extension)
                                )
                                {
                                    copy = true;
                                    break;
                                }

                            if (copy)
                            {
                                // Copy the text file into target 
                                // destination
                                File.Copy(file.DirectoryName +
                                          "\\" + file.Name, destPath +
                                                            "\\" + file.Name, true);
                                addInfo = true;
                            }
                        }
                    }

                    // For other file types other than text file
                    else
                    {
                        // This is the main function that opens and save  
                        // a given file. 
                        {
                            // Open a Revit file as an active document. 
                            var UIApp = cmdData.Application;
                            var UIDoc = UIApp.OpenAndActivateDocument(file.FullName);

                            var doc = UIDoc.Document;

                            // Try closing the previously opened document after 
                            // another one is opened. We are doing this because we 
                            // cannot explicitely close an active document
                            //  at a moment.  
                            if (previousDocument != null) previousDocument.SaveAndClose();

                            if (radioButtonMetric.Checked) BatchMetric(doc);

                            if (radioButtonImperial.Checked) BatchImperial(doc);

                            // Save the Revit file to the target destination.
                            // Since we are opening a file as an active document, 
                            // it takes care of preview. 
                            var destinationFile = destPath + "\\" + file.Name;
                            doc.SaveAs(destinationFile);

                            // Saving the current document to close it later.   
                            // If we had a method to close an active document, 
                            // we want to close it here. However, since we opened 
                            // it as an active document, we cannot do so.
                            // We'll close it after the next file is opened.
                            previousDocument = UIDoc;

                            // Set variable to know if upgrade 
                            // was successful - for status updates
                            addInfo = true;
                        }
                    }


                    var msgUnits = "";

                    if (radioButtonMetric.Checked)
                        msgUnits = "metric";

                    else
                        msgUnits = "imperial";

                    if (addInfo)
                    {
                        var msg = " has been converted to " + msgUnits;

                        // Log file and user interface updates
                        lstBxUpdates.Items.Add("\n" + file.Name + msg);
                        lstBxUpdates.TopIndex = lstBxUpdates.Items.Count - 1;
                        writer.WriteLine(file.FullName + msg);
                        writer.Flush();
                        bar.PerformStep();
                        ++success;
                    }
                }
                catch (Exception ex)
                {
                    failures.Add(file.FullName
                                 + " could not be upgraded: "
                                 + ex.Message);

                    bar.PerformStep();

                    ++failed;
                }
        }

        private void UpgraderForm_Load(object sender, EventArgs e)
        {
        }

        private void BatchMetric(Document doc)
        {
            //get the units in the document
            var units = doc.GetUnits();


            //UTLength
            var foUTLength = units.GetFormatOptions(UnitType.UT_Length);
            foUTLength.Accuracy = 1;
            foUTLength.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Length, foUTLength);
            //UTArea
            var foUTArea = units.GetFormatOptions(UnitType.UT_Area);
            foUTArea.Accuracy = 0.01;
            foUTArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_METERS;
            units.SetFormatOptions(UnitType.UT_Area, foUTArea);
            //UTVolume
            var foUTVolume = units.GetFormatOptions(UnitType.UT_Volume);
            foUTVolume.Accuracy = 0.01;
            foUTVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_METERS;
            units.SetFormatOptions(UnitType.UT_Volume, foUTVolume);
            //UTAngle
            var foUTAngle = units.GetFormatOptions(UnitType.UT_Angle);
            foUTAngle.Accuracy = 0.01;
            foUTAngle.DisplayUnits = DisplayUnitType.DUT_DECIMAL_DEGREES;
            units.SetFormatOptions(UnitType.UT_Angle, foUTAngle);
            //UTHVACDensity
            var foUTHVACDensity = units.GetFormatOptions(UnitType.UT_HVAC_Density);
            foUTHVACDensity.Accuracy = 0.0001;
            foUTHVACDensity.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Density, foUTHVACDensity);
            //UTHVACEnergy
            var foUTHVACEnergy = units.GetFormatOptions(UnitType.UT_HVAC_Energy);
            foUTHVACEnergy.Accuracy = 1;
            foUTHVACEnergy.DisplayUnits = DisplayUnitType.DUT_JOULES;
            units.SetFormatOptions(UnitType.UT_HVAC_Energy, foUTHVACEnergy);
            //UTHVACFriction
            var foUTHVACFriction = units.GetFormatOptions(UnitType.UT_HVAC_Friction);
            foUTHVACFriction.Accuracy = 0.01;
            foUTHVACFriction.DisplayUnits = DisplayUnitType.DUT_PASCALS_PER_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Friction, foUTHVACFriction);
            //UTHVACPower
            var foUTHVACPower = units.GetFormatOptions(UnitType.UT_HVAC_Power);
            foUTHVACPower.Accuracy = 1;
            foUTHVACPower.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Power, foUTHVACPower);
            //UTHVACPowerDensity
            var foUTHVACPowerDensity = units.GetFormatOptions(UnitType.UT_HVAC_Power_Density);
            foUTHVACPowerDensity.Accuracy = 0.01;
            foUTHVACPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Power_Density, foUTHVACPowerDensity);
            //UTHVACPressure
            var foUTHVACPressure = units.GetFormatOptions(UnitType.UT_HVAC_Pressure);
            foUTHVACPressure.Accuracy = 0.1;
            foUTHVACPressure.DisplayUnits = DisplayUnitType.DUT_PASCALS;
            units.SetFormatOptions(UnitType.UT_HVAC_Pressure, foUTHVACPressure);
            //UTHVACTemperature
            var foUTHVACTemperature = units.GetFormatOptions(UnitType.UT_HVAC_Temperature);
            foUTHVACTemperature.Accuracy = 1;
            foUTHVACTemperature.DisplayUnits = DisplayUnitType.DUT_CELSIUS;
            units.SetFormatOptions(UnitType.UT_HVAC_Temperature, foUTHVACTemperature);
            //UTHVACVelocity
            var foUTHVACVelocity = units.GetFormatOptions(UnitType.UT_HVAC_Velocity);
            foUTHVACVelocity.Accuracy = 0.1;
            foUTHVACVelocity.DisplayUnits = DisplayUnitType.DUT_METERS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_HVAC_Velocity, foUTHVACVelocity);
            //UTHVACAirflow
            var foUTHVACAirflow = units.GetFormatOptions(UnitType.UT_HVAC_Airflow);
            foUTHVACAirflow.Accuracy = 0.1;
            foUTHVACAirflow.DisplayUnits = DisplayUnitType.DUT_LITERS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow, foUTHVACAirflow);
            //UTHVACDuctSize
            var foUTHVACDuctSize = units.GetFormatOptions(UnitType.UT_HVAC_DuctSize);
            foUTHVACDuctSize.Accuracy = 1;
            foUTHVACDuctSize.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctSize, foUTHVACDuctSize);
            //UTHVACCrossSection
            var foUTHVACCrossSection = units.GetFormatOptions(UnitType.UT_HVAC_CrossSection);
            foUTHVACCrossSection.Accuracy = 1;
            foUTHVACCrossSection.DisplayUnits = DisplayUnitType.DUT_SQUARE_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_HVAC_CrossSection, foUTHVACCrossSection);
            //UTHVACHeatGain
            var foUTHVACHeatGain = units.GetFormatOptions(UnitType.UT_HVAC_HeatGain);
            foUTHVACHeatGain.Accuracy = 1;
            foUTHVACHeatGain.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_HeatGain, foUTHVACHeatGain);
            //UTElectricalCurrent
            var foUTElectricalCurrent = units.GetFormatOptions(UnitType.UT_Electrical_Current);
            foUTElectricalCurrent.Accuracy = 1;
            foUTElectricalCurrent.DisplayUnits = DisplayUnitType.DUT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Current, foUTElectricalCurrent);
            //UTElectricalPotential
            var foUTElectricalPotential = units.GetFormatOptions(UnitType.UT_Electrical_Potential);
            foUTElectricalPotential.Accuracy = 1;
            foUTElectricalPotential.DisplayUnits = DisplayUnitType.DUT_VOLTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Potential, foUTElectricalPotential);
            //UTElectricalFrequency
            var foUTElectricalFrequency = units.GetFormatOptions(UnitType.UT_Electrical_Frequency);
            foUTElectricalFrequency.Accuracy = 1;
            foUTElectricalFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Electrical_Frequency, foUTElectricalFrequency);
            //UTElectricalIlluminance
            var foUTElectricalIlluminance = units.GetFormatOptions(UnitType.UT_Electrical_Illuminance);
            foUTElectricalIlluminance.Accuracy = 1;
            foUTElectricalIlluminance.DisplayUnits = DisplayUnitType.DUT_LUX;
            units.SetFormatOptions(UnitType.UT_Electrical_Illuminance, foUTElectricalIlluminance);
            //UTElectricalLuminance
            var foUTElectricalLuminance = units.GetFormatOptions(UnitType.UT_Electrical_Luminance);
            foUTElectricalLuminance.Accuracy = 1;
            foUTElectricalLuminance.DisplayUnits = DisplayUnitType.DUT_CANDELAS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminance, foUTElectricalLuminance);
            //UTElectricalLuminousFlux
            var foUTElectricalLuminousFlux = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Flux);
            foUTElectricalLuminousFlux.Accuracy = 1;
            foUTElectricalLuminousFlux.DisplayUnits = DisplayUnitType.DUT_LUMENS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Flux, foUTElectricalLuminousFlux);
            //UTElectricalLuminousIntensity
            var foUTElectricalLuminousIntensity = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity);
            foUTElectricalLuminousIntensity.Accuracy = 1;
            foUTElectricalLuminousIntensity.DisplayUnits = DisplayUnitType.DUT_CANDELAS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity, foUTElectricalLuminousIntensity);
            //UTElectricalEfficacy
            var foUTElectricalEfficacy = units.GetFormatOptions(UnitType.UT_Electrical_Efficacy);
            foUTElectricalEfficacy.Accuracy = 1;
            foUTElectricalEfficacy.DisplayUnits = DisplayUnitType.DUT_LUMENS_PER_WATT;
            units.SetFormatOptions(UnitType.UT_Electrical_Efficacy, foUTElectricalEfficacy);
            //UTElectricalWattage
            var foUTElectricalWattage = units.GetFormatOptions(UnitType.UT_Electrical_Wattage);
            foUTElectricalWattage.Accuracy = 1;
            foUTElectricalWattage.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Wattage, foUTElectricalWattage);
            //UTColorTemperature
            var foUTColorTemperature = units.GetFormatOptions(UnitType.UT_Color_Temperature);
            foUTColorTemperature.Accuracy = 1;
            foUTColorTemperature.DisplayUnits = DisplayUnitType.DUT_KELVIN;
            units.SetFormatOptions(UnitType.UT_Color_Temperature, foUTColorTemperature);
            //UTElectricalPower
            var foUTElectricalPower = units.GetFormatOptions(UnitType.UT_Electrical_Power);
            foUTElectricalPower.Accuracy = 1;
            foUTElectricalPower.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Power, foUTElectricalPower);
            //UTHVACRoughness
            var foUTHVACRoughness = units.GetFormatOptions(UnitType.UT_HVAC_Roughness);
            foUTHVACRoughness.Accuracy = 0.01;
            foUTHVACRoughness.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_HVAC_Roughness, foUTHVACRoughness);
            //UTElectricalApparentPower
            var foUTElectricalApparentPower = units.GetFormatOptions(UnitType.UT_Electrical_Apparent_Power);
            foUTElectricalApparentPower.Accuracy = 1;
            foUTElectricalApparentPower.DisplayUnits = DisplayUnitType.DUT_VOLT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Apparent_Power, foUTElectricalApparentPower);
            //UTElectricalPowerDensity
            var foUTElectricalPowerDensity = units.GetFormatOptions(UnitType.UT_Electrical_Power_Density);
            foUTElectricalPowerDensity.Accuracy = 0.01;
            foUTElectricalPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_Electrical_Power_Density, foUTElectricalPowerDensity);
            //UTPipingDensity
            var foUTPipingDensity = units.GetFormatOptions(UnitType.UT_Piping_Density);
            foUTPipingDensity.Accuracy = 0.0001;
            foUTPipingDensity.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_Piping_Density, foUTPipingDensity);
            //UTPipingFlow
            var foUTPipingFlow = units.GetFormatOptions(UnitType.UT_Piping_Flow);
            foUTPipingFlow.Accuracy = 0.1;
            foUTPipingFlow.DisplayUnits = DisplayUnitType.DUT_LITERS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Piping_Flow, foUTPipingFlow);
            //UTPipingFriction
            var foUTPipingFriction = units.GetFormatOptions(UnitType.UT_Piping_Friction);
            foUTPipingFriction.Accuracy = 0.01;
            foUTPipingFriction.DisplayUnits = DisplayUnitType.DUT_PASCALS_PER_METER;
            units.SetFormatOptions(UnitType.UT_Piping_Friction, foUTPipingFriction);
            //UTPipingPressure
            var foUTPipingPressure = units.GetFormatOptions(UnitType.UT_Piping_Pressure);
            foUTPipingPressure.Accuracy = 0.1;
            foUTPipingPressure.DisplayUnits = DisplayUnitType.DUT_PASCALS;
            units.SetFormatOptions(UnitType.UT_Piping_Pressure, foUTPipingPressure);
            //UTPipingTemperature
            var foUTPipingTemperature = units.GetFormatOptions(UnitType.UT_Piping_Temperature);
            foUTPipingTemperature.Accuracy = 1;
            foUTPipingTemperature.DisplayUnits = DisplayUnitType.DUT_CELSIUS;
            units.SetFormatOptions(UnitType.UT_Piping_Temperature, foUTPipingTemperature);
            //UTPipingVelocity
            var foUTPipingVelocity = units.GetFormatOptions(UnitType.UT_Piping_Velocity);
            foUTPipingVelocity.Accuracy = 0.1;
            foUTPipingVelocity.DisplayUnits = DisplayUnitType.DUT_METERS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Piping_Velocity, foUTPipingVelocity);
            //UTPipingViscosity
            var foUTPipingViscosity = units.GetFormatOptions(UnitType.UT_Piping_Viscosity);
            foUTPipingViscosity.Accuracy = 0.1;
            foUTPipingViscosity.DisplayUnits = DisplayUnitType.DUT_PASCAL_SECONDS;
            units.SetFormatOptions(UnitType.UT_Piping_Viscosity, foUTPipingViscosity);
            //UTPipeSize
            var foUTPipeSize = units.GetFormatOptions(UnitType.UT_PipeSize);
            foUTPipeSize.Accuracy = 1;
            foUTPipeSize.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_PipeSize, foUTPipeSize);
            //UTPipingRoughness
            var foUTPipingRoughness = units.GetFormatOptions(UnitType.UT_Piping_Roughness);
            foUTPipingRoughness.Accuracy = 0.001;
            foUTPipingRoughness.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Piping_Roughness, foUTPipingRoughness);
            //UTPipingVolume
            var foUTPipingVolume = units.GetFormatOptions(UnitType.UT_Piping_Volume);
            foUTPipingVolume.Accuracy = 0.1;
            foUTPipingVolume.DisplayUnits = DisplayUnitType.DUT_LITERS;
            units.SetFormatOptions(UnitType.UT_Piping_Volume, foUTPipingVolume);
            //UTHVACViscosity
            var foUTHVACViscosity = units.GetFormatOptions(UnitType.UT_HVAC_Viscosity);
            foUTHVACViscosity.Accuracy = 0.1;
            foUTHVACViscosity.DisplayUnits = DisplayUnitType.DUT_PASCAL_SECONDS;
            units.SetFormatOptions(UnitType.UT_HVAC_Viscosity, foUTHVACViscosity);
            //UTHVACCoefficientOfHeatTransfer
            var foUTHVACCoefficientOfHeatTransfer = units.GetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer);
            foUTHVACCoefficientOfHeatTransfer.Accuracy = 0.0001;
            foUTHVACCoefficientOfHeatTransfer.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_METER_KELVIN;
            units.SetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer, foUTHVACCoefficientOfHeatTransfer);
            //UTHVACThermalResistance
            var foUTHVACThermalResistance = units.GetFormatOptions(UnitType.UT_HVAC_ThermalResistance);
            foUTHVACThermalResistance.Accuracy = 0.0001;
            foUTHVACThermalResistance.DisplayUnits = DisplayUnitType.DUT_SQUARE_METER_KELVIN_PER_WATT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalResistance, foUTHVACThermalResistance);
            //UTHVACThermalMass
            var foUTHVACThermalMass = units.GetFormatOptions(UnitType.UT_HVAC_ThermalMass);
            foUTHVACThermalMass.Accuracy = 0.01;
            foUTHVACThermalMass.DisplayUnits = DisplayUnitType.DUT_KILOJOULES_PER_KELVIN;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalMass, foUTHVACThermalMass);
            //UTHVACThermalConductivity
            var foUTHVACThermalConductivity = units.GetFormatOptions(UnitType.UT_HVAC_ThermalConductivity);
            foUTHVACThermalConductivity.Accuracy = 0.0001;
            foUTHVACThermalConductivity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_METER_KELVIN;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalConductivity, foUTHVACThermalConductivity);
            //UTHVACSpecificHeat
            var foUTHVACSpecificHeat = units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeat);
            foUTHVACSpecificHeat.Accuracy = 0.0001;
            foUTHVACSpecificHeat.DisplayUnits = DisplayUnitType.DUT_JOULES_PER_GRAM_CELSIUS;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeat, foUTHVACSpecificHeat);
            //UTHVACSpecificHeatOfVaporization
            var foUTHVACSpecificHeatOfVaporization =
                units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization);
            foUTHVACSpecificHeatOfVaporization.Accuracy = 0.0001;
            foUTHVACSpecificHeatOfVaporization.DisplayUnits = DisplayUnitType.DUT_JOULES_PER_GRAM;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization, foUTHVACSpecificHeatOfVaporization);
            //UTHVACPermeability
            var foUTHVACPermeability = units.GetFormatOptions(UnitType.UT_HVAC_Permeability);
            foUTHVACPermeability.Accuracy = 0.0001;
            foUTHVACPermeability.DisplayUnits = DisplayUnitType.DUT_NANOGRAMS_PER_PASCAL_SECOND_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Permeability, foUTHVACPermeability);
            //UTElectricalResistivity
            var foUTElectricalResistivity = units.GetFormatOptions(UnitType.UT_Electrical_Resistivity);
            foUTElectricalResistivity.Accuracy = 0.0001;
            foUTElectricalResistivity.DisplayUnits = DisplayUnitType.DUT_OHM_METERS;
            units.SetFormatOptions(UnitType.UT_Electrical_Resistivity, foUTElectricalResistivity);
            //UTHVACAirflowDensity
            var foUTHVACAirflowDensity = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Density);
            foUTHVACAirflowDensity.Accuracy = 0.0001;
            foUTHVACAirflowDensity.DisplayUnits = DisplayUnitType.DUT_LITERS_PER_SECOND_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Density, foUTHVACAirflowDensity);
            //UTSlope
            var foUTSlope = units.GetFormatOptions(UnitType.UT_Slope);
            foUTSlope.Accuracy = 0.01;
            foUTSlope.DisplayUnits = DisplayUnitType.DUT_SLOPE_DEGREES;
            units.SetFormatOptions(UnitType.UT_Slope, foUTSlope);
            //UTHVACCoolingLoad
            var foUTHVACCoolingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load);
            foUTHVACCoolingLoad.Accuracy = 1;
            foUTHVACCoolingLoad.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load, foUTHVACCoolingLoad);
            //UTHVACHeatingLoad
            var foUTHVACHeatingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load);
            foUTHVACHeatingLoad.Accuracy = 1;
            foUTHVACHeatingLoad.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load, foUTHVACHeatingLoad);
            //UTHVACCoolingLoadDividedByArea
            var foUTHVACCoolingLoadDividedByArea =
                units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area);
            foUTHVACCoolingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByArea.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area, foUTHVACCoolingLoadDividedByArea);
            //UTHVACHeatingLoadDividedByArea
            var foUTHVACHeatingLoadDividedByArea =
                units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area);
            foUTHVACHeatingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByArea.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area, foUTHVACHeatingLoadDividedByArea);
            //UTHVACCoolingLoadDividedByVolume
            var foUTHVACCoolingLoadDividedByVolume =
                units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume);
            foUTHVACCoolingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByVolume.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume, foUTHVACCoolingLoadDividedByVolume);
            //UTHVACHeatingLoadDividedByVolume
            var foUTHVACHeatingLoadDividedByVolume =
                units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume);
            foUTHVACHeatingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByVolume.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume, foUTHVACHeatingLoadDividedByVolume);
            //UTHVACAirflowDividedByVolume
            var foUTHVACAirflowDividedByVolume = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume);
            foUTHVACAirflowDividedByVolume.Accuracy = 0.01;
            foUTHVACAirflowDividedByVolume.DisplayUnits = DisplayUnitType.DUT_LITERS_PER_SECOND_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume, foUTHVACAirflowDividedByVolume);
            //UTHVACAirflowDividedByCoolingLoad
            var foUTHVACAirflowDividedByCoolingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load);
            foUTHVACAirflowDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAirflowDividedByCoolingLoad.DisplayUnits = DisplayUnitType.DUT_LITERS_PER_SECOND_KILOWATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load,
                foUTHVACAirflowDividedByCoolingLoad);
            //UTHVACAreaDividedByCoolingLoad
            var foUTHVACAreaDividedByCoolingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load);
            foUTHVACAreaDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAreaDividedByCoolingLoad.DisplayUnits = DisplayUnitType.DUT_SQUARE_METERS_PER_KILOWATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load, foUTHVACAreaDividedByCoolingLoad);
            //UTHVACAreaDividedByHeatingLoad
            var foUTHVACAreaDividedByHeatingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load);
            foUTHVACAreaDividedByHeatingLoad.Accuracy = 0.0001;
            foUTHVACAreaDividedByHeatingLoad.DisplayUnits = DisplayUnitType.DUT_SQUARE_METERS_PER_KILOWATTS;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load, foUTHVACAreaDividedByHeatingLoad);
            //UTWireSize
            var foUTWireSize = units.GetFormatOptions(UnitType.UT_WireSize);
            foUTWireSize.Accuracy = 0.01;
            foUTWireSize.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_WireSize, foUTWireSize);
            //UTHVACSlope
            var foUTHVACSlope = units.GetFormatOptions(UnitType.UT_HVAC_Slope);
            foUTHVACSlope.Accuracy = 0.01;
            foUTHVACSlope.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_HVAC_Slope, foUTHVACSlope);
            //UTPipingSlope
            var foUTPipingSlope = units.GetFormatOptions(UnitType.UT_Piping_Slope);
            foUTPipingSlope.Accuracy = 0.01;
            foUTPipingSlope.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_Piping_Slope, foUTPipingSlope);
            //UTCurrency
            var foUTCurrency = units.GetFormatOptions(UnitType.UT_Currency);
            foUTCurrency.Accuracy = 0.01;
            foUTCurrency.DisplayUnits = DisplayUnitType.DUT_CURRENCY;
            units.SetFormatOptions(UnitType.UT_Currency, foUTCurrency);
            //UTMassDensity
            var foUTMassDensity = units.GetFormatOptions(UnitType.UT_MassDensity);
            foUTMassDensity.Accuracy = 0.01;
            foUTMassDensity.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_MassDensity, foUTMassDensity);
            //UTHVACFactor
            var foUTHVACFactor = units.GetFormatOptions(UnitType.UT_HVAC_Factor);
            foUTHVACFactor.Accuracy = 0.01;
            foUTHVACFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_HVAC_Factor, foUTHVACFactor);
            //UTElectricalTemperature
            var foUTElectricalTemperature = units.GetFormatOptions(UnitType.UT_Electrical_Temperature);
            foUTElectricalTemperature.Accuracy = 1;
            foUTElectricalTemperature.DisplayUnits = DisplayUnitType.DUT_CELSIUS;
            units.SetFormatOptions(UnitType.UT_Electrical_Temperature, foUTElectricalTemperature);
            //UTElectricalCableTraySize
            var foUTElectricalCableTraySize = units.GetFormatOptions(UnitType.UT_Electrical_CableTraySize);
            foUTElectricalCableTraySize.Accuracy = 1;
            foUTElectricalCableTraySize.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Electrical_CableTraySize, foUTElectricalCableTraySize);
            //UTElectricalConduitSize
            var foUTElectricalConduitSize = units.GetFormatOptions(UnitType.UT_Electrical_ConduitSize);
            foUTElectricalConduitSize.Accuracy = 1;
            foUTElectricalConduitSize.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Electrical_ConduitSize, foUTElectricalConduitSize);
            //UTElectricalDemandFactor
            var foUTElectricalDemandFactor = units.GetFormatOptions(UnitType.UT_Electrical_Demand_Factor);
            foUTElectricalDemandFactor.Accuracy = 0.01;
            foUTElectricalDemandFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_Electrical_Demand_Factor, foUTElectricalDemandFactor);
            //UTHVACDuctInsulationThickness
            var foUTHVACDuctInsulationThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness);
            foUTHVACDuctInsulationThickness.Accuracy = 1;
            foUTHVACDuctInsulationThickness.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness, foUTHVACDuctInsulationThickness);
            //UTHVACDuctLiningThickness
            var foUTHVACDuctLiningThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness);
            foUTHVACDuctLiningThickness.Accuracy = 1;
            foUTHVACDuctLiningThickness.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness, foUTHVACDuctLiningThickness);
            //UTPipeInsulationThickness
            var foUTPipeInsulationThickness = units.GetFormatOptions(UnitType.UT_PipeInsulationThickness);
            foUTPipeInsulationThickness.Accuracy = 1;
            foUTPipeInsulationThickness.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_PipeInsulationThickness, foUTPipeInsulationThickness);
            //UTForce
            var foUTForce = units.GetFormatOptions(UnitType.UT_Force);
            foUTForce.Accuracy = 0.01;
            foUTForce.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS;
            units.SetFormatOptions(UnitType.UT_Force, foUTForce);
            //UTLinearForce
            var foUTLinearForce = units.GetFormatOptions(UnitType.UT_LinearForce);
            foUTLinearForce.Accuracy = 0.01;
            foUTLinearForce.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_METER;
            units.SetFormatOptions(UnitType.UT_LinearForce, foUTLinearForce);
            //UTAreaForce
            var foUTAreaForce = units.GetFormatOptions(UnitType.UT_AreaForce);
            foUTAreaForce.Accuracy = 0.01;
            foUTAreaForce.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_AreaForce, foUTAreaForce);
            //UTMoment
            var foUTMoment = units.GetFormatOptions(UnitType.UT_Moment);
            foUTMoment.Accuracy = 0.01;
            foUTMoment.DisplayUnits = DisplayUnitType.DUT_KILONEWTON_METERS;
            units.SetFormatOptions(UnitType.UT_Moment, foUTMoment);
            //UTLinearMoment
            var foUTLinearMoment = units.GetFormatOptions(UnitType.UT_LinearMoment);
            foUTLinearMoment.Accuracy = 0.01;
            foUTLinearMoment.DisplayUnits = DisplayUnitType.DUT_KILONEWTON_METERS_PER_METER;
            units.SetFormatOptions(UnitType.UT_LinearMoment, foUTLinearMoment);
            //UTStress
            var foUTStress = units.GetFormatOptions(UnitType.UT_Stress);
            foUTStress.Accuracy = 0.1;
            foUTStress.DisplayUnits = DisplayUnitType.DUT_MEGAPASCALS;
            units.SetFormatOptions(UnitType.UT_Stress, foUTStress);
            //UTUnitWeight
            var foUTUnitWeight = units.GetFormatOptions(UnitType.UT_UnitWeight);
            foUTUnitWeight.Accuracy = 0.1;
            foUTUnitWeight.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_UnitWeight, foUTUnitWeight);
            //UTWeight
            var foUTWeight = units.GetFormatOptions(UnitType.UT_Weight);
            foUTWeight.Accuracy = 0.01;
            foUTWeight.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS;
            units.SetFormatOptions(UnitType.UT_Weight, foUTWeight);
            //UTMass
            var foUTMass = units.GetFormatOptions(UnitType.UT_Mass);
            foUTMass.Accuracy = 0.01;
            foUTMass.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_MASS;
            units.SetFormatOptions(UnitType.UT_Mass, foUTMass);
            //UTMassPerUnitArea
            var foUTMassPerUnitArea = units.GetFormatOptions(UnitType.UT_MassPerUnitArea);
            foUTMassPerUnitArea.Accuracy = 0.01;
            foUTMassPerUnitArea.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_MASS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_MassPerUnitArea, foUTMassPerUnitArea);
            //UTThermalExpansion
            var foUTThermalExpansion = units.GetFormatOptions(UnitType.UT_ThermalExpansion);
            foUTThermalExpansion.Accuracy = 1E-05;
            foUTThermalExpansion.DisplayUnits = DisplayUnitType.DUT_INV_CELSIUS;
            units.SetFormatOptions(UnitType.UT_ThermalExpansion, foUTThermalExpansion);
            //UTForcePerLength
            var foUTForcePerLength = units.GetFormatOptions(UnitType.UT_ForcePerLength);
            foUTForcePerLength.Accuracy = 0.1;
            foUTForcePerLength.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_METER;
            units.SetFormatOptions(UnitType.UT_ForcePerLength, foUTForcePerLength);
            //UTLinearForcePerLength
            var foUTLinearForcePerLength = units.GetFormatOptions(UnitType.UT_LinearForcePerLength);
            foUTLinearForcePerLength.Accuracy = 0.1;
            foUTLinearForcePerLength.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_LinearForcePerLength, foUTLinearForcePerLength);
            //UTAreaForcePerLength
            var foUTAreaForcePerLength = units.GetFormatOptions(UnitType.UT_AreaForcePerLength);
            foUTAreaForcePerLength.Accuracy = 0.1;
            foUTAreaForcePerLength.DisplayUnits = DisplayUnitType.DUT_KILONEWTONS_PER_CUBIC_METER;
            units.SetFormatOptions(UnitType.UT_AreaForcePerLength, foUTAreaForcePerLength);
            //UTForceLengthPerAngle
            var foUTForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_ForceLengthPerAngle);
            foUTForceLengthPerAngle.Accuracy = 0.1;
            foUTForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KILONEWTON_METERS_PER_DEGREE;
            units.SetFormatOptions(UnitType.UT_ForceLengthPerAngle, foUTForceLengthPerAngle);
            //UTLinearForceLengthPerAngle
            var foUTLinearForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_LinearForceLengthPerAngle);
            foUTLinearForceLengthPerAngle.Accuracy = 0.1;
            foUTLinearForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KILONEWTON_METERS_PER_DEGREE_PER_METER;
            units.SetFormatOptions(UnitType.UT_LinearForceLengthPerAngle, foUTLinearForceLengthPerAngle);
            //UTDisplacementDeflection
            var foUTDisplacementDeflection = units.GetFormatOptions(UnitType.UT_Displacement_Deflection);
            foUTDisplacementDeflection.Accuracy = 0.01;
            foUTDisplacementDeflection.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Displacement_Deflection, foUTDisplacementDeflection);
            //UTRotation
            var foUTRotation = units.GetFormatOptions(UnitType.UT_Rotation);
            foUTRotation.Accuracy = 0.001;
            foUTRotation.DisplayUnits = DisplayUnitType.DUT_RADIANS;
            units.SetFormatOptions(UnitType.UT_Rotation, foUTRotation);
            //UTPeriod
            var foUTPeriod = units.GetFormatOptions(UnitType.UT_Period);
            foUTPeriod.Accuracy = 0.1;
            foUTPeriod.DisplayUnits = DisplayUnitType.DUT_SECONDS;
            units.SetFormatOptions(UnitType.UT_Period, foUTPeriod);
            //UTStructuralFrequency
            var foUTStructuralFrequency = units.GetFormatOptions(UnitType.UT_Structural_Frequency);
            foUTStructuralFrequency.Accuracy = 0.1;
            foUTStructuralFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Structural_Frequency, foUTStructuralFrequency);
            //UTPulsation
            var foUTPulsation = units.GetFormatOptions(UnitType.UT_Pulsation);
            foUTPulsation.Accuracy = 0.1;
            foUTPulsation.DisplayUnits = DisplayUnitType.DUT_RADIANS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Pulsation, foUTPulsation);
            //UTStructuralVelocity
            var foUTStructuralVelocity = units.GetFormatOptions(UnitType.UT_Structural_Velocity);
            foUTStructuralVelocity.Accuracy = 0.1;
            foUTStructuralVelocity.DisplayUnits = DisplayUnitType.DUT_METERS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Structural_Velocity, foUTStructuralVelocity);
            //UTAcceleration
            var foUTAcceleration = units.GetFormatOptions(UnitType.UT_Acceleration);
            foUTAcceleration.Accuracy = 0.1;
            foUTAcceleration.DisplayUnits = DisplayUnitType.DUT_METERS_PER_SECOND_SQUARED;
            units.SetFormatOptions(UnitType.UT_Acceleration, foUTAcceleration);
            //UTEnergy
            var foUTEnergy = units.GetFormatOptions(UnitType.UT_Energy);
            foUTEnergy.Accuracy = 0.1;
            foUTEnergy.DisplayUnits = DisplayUnitType.DUT_KILOJOULES;
            units.SetFormatOptions(UnitType.UT_Energy, foUTEnergy);
            //UTReinforcementVolume
            var foUTReinforcementVolume = units.GetFormatOptions(UnitType.UT_Reinforcement_Volume);
            foUTReinforcementVolume.Accuracy = 0.01;
            foUTReinforcementVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Volume, foUTReinforcementVolume);
            //UTReinforcementLength
            var foUTReinforcementLength = units.GetFormatOptions(UnitType.UT_Reinforcement_Length);
            foUTReinforcementLength.Accuracy = 1;
            foUTReinforcementLength.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Length, foUTReinforcementLength);
            //UTReinforcementArea
            var foUTReinforcementArea = units.GetFormatOptions(UnitType.UT_Reinforcement_Area);
            foUTReinforcementArea.Accuracy = 0.01;
            foUTReinforcementArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area, foUTReinforcementArea);
            //UTReinforcementAreaperUnitLength
            var foUTReinforcementAreaperUnitLength =
                units.GetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length);
            foUTReinforcementAreaperUnitLength.Accuracy = 0.01;
            foUTReinforcementAreaperUnitLength.DisplayUnits = DisplayUnitType.DUT_SQUARE_CENTIMETERS_PER_METER;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length, foUTReinforcementAreaperUnitLength);
            //UTReinforcementSpacing
            var foUTReinforcementSpacing = units.GetFormatOptions(UnitType.UT_Reinforcement_Spacing);
            foUTReinforcementSpacing.Accuracy = 1;
            foUTReinforcementSpacing.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Spacing, foUTReinforcementSpacing);
            //UTReinforcementCover
            var foUTReinforcementCover = units.GetFormatOptions(UnitType.UT_Reinforcement_Cover);
            foUTReinforcementCover.Accuracy = 1;
            foUTReinforcementCover.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Cover, foUTReinforcementCover);
            //UTBarDiameter
            var foUTBarDiameter = units.GetFormatOptions(UnitType.UT_Bar_Diameter);
            foUTBarDiameter.Accuracy = 1;
            foUTBarDiameter.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Bar_Diameter, foUTBarDiameter);
            //UTCrackWidth
            var foUTCrackWidth = units.GetFormatOptions(UnitType.UT_Crack_Width);
            foUTCrackWidth.Accuracy = 0.01;
            foUTCrackWidth.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Crack_Width, foUTCrackWidth);
            //UTSectionDimension
            var foUTSectionDimension = units.GetFormatOptions(UnitType.UT_Section_Dimension);
            foUTSectionDimension.Accuracy = 0.1;
            foUTSectionDimension.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Section_Dimension, foUTSectionDimension);
            //UTSectionProperty
            var foUTSectionProperty = units.GetFormatOptions(UnitType.UT_Section_Property);
            foUTSectionProperty.Accuracy = 0.1;
            foUTSectionProperty.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Section_Property, foUTSectionProperty);
            //UTSectionArea
            var foUTSectionArea = units.GetFormatOptions(UnitType.UT_Section_Area);
            foUTSectionArea.Accuracy = 0.1;
            foUTSectionArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Section_Area, foUTSectionArea);
            //UTSectionModulus
            var foUTSectionModulus = units.GetFormatOptions(UnitType.UT_Section_Modulus);
            foUTSectionModulus.Accuracy = 0.1;
            foUTSectionModulus.DisplayUnits = DisplayUnitType.DUT_CUBIC_CENTIMETERS;
            units.SetFormatOptions(UnitType.UT_Section_Modulus, foUTSectionModulus);
            //UTMomentofInertia
            var foUTMomentofInertia = units.GetFormatOptions(UnitType.UT_Moment_of_Inertia);
            foUTMomentofInertia.Accuracy = 0.01;
            foUTMomentofInertia.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS_TO_THE_FOURTH_POWER;
            units.SetFormatOptions(UnitType.UT_Moment_of_Inertia, foUTMomentofInertia);
            //UTWarpingConstant
            var foUTWarpingConstant = units.GetFormatOptions(UnitType.UT_Warping_Constant);
            foUTWarpingConstant.Accuracy = 0.1;
            foUTWarpingConstant.DisplayUnits = DisplayUnitType.DUT_CENTIMETERS_TO_THE_SIXTH_POWER;
            units.SetFormatOptions(UnitType.UT_Warping_Constant, foUTWarpingConstant);
            //UTMassperUnitLength
            var foUTMassperUnitLength = units.GetFormatOptions(UnitType.UT_Mass_per_Unit_Length);
            foUTMassperUnitLength.Accuracy = 0.01;
            foUTMassperUnitLength.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_MASS_PER_METER;
            units.SetFormatOptions(UnitType.UT_Mass_per_Unit_Length, foUTMassperUnitLength);
            //UTWeightperUnitLength
            var foUTWeightperUnitLength = units.GetFormatOptions(UnitType.UT_Weight_per_Unit_Length);
            foUTWeightperUnitLength.Accuracy = 0.01;
            foUTWeightperUnitLength.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_FORCE_PER_METER;
            units.SetFormatOptions(UnitType.UT_Weight_per_Unit_Length, foUTWeightperUnitLength);
            //UTSurfaceArea
            var foUTSurfaceArea = units.GetFormatOptions(UnitType.UT_Surface_Area);
            foUTSurfaceArea.Accuracy = 0.01;
            foUTSurfaceArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_METERS_PER_METER;
            units.SetFormatOptions(UnitType.UT_Surface_Area, foUTSurfaceArea);
            //UTPipeDimension
            var foUTPipeDimension = units.GetFormatOptions(UnitType.UT_Pipe_Dimension);
            foUTPipeDimension.Accuracy = 0.01;
            foUTPipeDimension.DisplayUnits = DisplayUnitType.DUT_MILLIMETERS;
            units.SetFormatOptions(UnitType.UT_Pipe_Dimension, foUTPipeDimension);
            //UTPipeMass
            var foUTPipeMass = units.GetFormatOptions(UnitType.UT_PipeMass);
            foUTPipeMass.Accuracy = 0.01;
            foUTPipeMass.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_MASS;
            units.SetFormatOptions(UnitType.UT_PipeMass, foUTPipeMass);
            //UTPipeMassPerUnitLength
            var foUTPipeMassPerUnitLength = units.GetFormatOptions(UnitType.UT_PipeMassPerUnitLength);
            foUTPipeMassPerUnitLength.Accuracy = 0.01;
            foUTPipeMassPerUnitLength.DisplayUnits = DisplayUnitType.DUT_KILOGRAMS_MASS_PER_METER;
            units.SetFormatOptions(UnitType.UT_PipeMassPerUnitLength, foUTPipeMassPerUnitLength);
        }

        private void BatchImperial(Document doc)
        {
            var units = doc.GetUnits();

            //UTLength
            var foUTLength = units.GetFormatOptions(UnitType.UT_Length);
            foUTLength.Accuracy = 0.00260416666666667;
            foUTLength.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Length, foUTLength);
            //UTArea
            var foUTArea = units.GetFormatOptions(UnitType.UT_Area);
            foUTArea.Accuracy = 0.01;
            foUTArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET;
            units.SetFormatOptions(UnitType.UT_Area, foUTArea);
            //UTVolume
            var foUTVolume = units.GetFormatOptions(UnitType.UT_Volume);
            foUTVolume.Accuracy = 0.01;
            foUTVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET;
            units.SetFormatOptions(UnitType.UT_Volume, foUTVolume);
            //UTAngle
            var foUTAngle = units.GetFormatOptions(UnitType.UT_Angle);
            foUTAngle.Accuracy = 0.01;
            foUTAngle.DisplayUnits = DisplayUnitType.DUT_DECIMAL_DEGREES;
            units.SetFormatOptions(UnitType.UT_Angle, foUTAngle);
            //UTHVACDensity
            var foUTHVACDensity = units.GetFormatOptions(UnitType.UT_HVAC_Density);
            foUTHVACDensity.Accuracy = 0.0001;
            foUTHVACDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Density, foUTHVACDensity);
            //UTHVACEnergy
            var foUTHVACEnergy = units.GetFormatOptions(UnitType.UT_HVAC_Energy);
            foUTHVACEnergy.Accuracy = 1;
            foUTHVACEnergy.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS;
            units.SetFormatOptions(UnitType.UT_HVAC_Energy, foUTHVACEnergy);
            //UTHVACFriction
            var foUTHVACFriction = units.GetFormatOptions(UnitType.UT_HVAC_Friction);
            foUTHVACFriction.Accuracy = 0.01;
            foUTHVACFriction.DisplayUnits = DisplayUnitType.DUT_INCHES_OF_WATER_PER_100FT;
            units.SetFormatOptions(UnitType.UT_HVAC_Friction, foUTHVACFriction);
            //UTHVACPower
            var foUTHVACPower = units.GetFormatOptions(UnitType.UT_HVAC_Power);
            foUTHVACPower.Accuracy = 1;
            foUTHVACPower.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Power, foUTHVACPower);
            //UTHVACPowerDensity
            var foUTHVACPowerDensity = units.GetFormatOptions(UnitType.UT_HVAC_Power_Density);
            foUTHVACPowerDensity.Accuracy = 0.01;
            foUTHVACPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Power_Density, foUTHVACPowerDensity);
            //UTHVACPressure
            var foUTHVACPressure = units.GetFormatOptions(UnitType.UT_HVAC_Pressure);
            foUTHVACPressure.Accuracy = 0.01;
            foUTHVACPressure.DisplayUnits = DisplayUnitType.DUT_INCHES_OF_WATER;
            units.SetFormatOptions(UnitType.UT_HVAC_Pressure, foUTHVACPressure);
            //UTHVACTemperature
            var foUTHVACTemperature = units.GetFormatOptions(UnitType.UT_HVAC_Temperature);
            foUTHVACTemperature.Accuracy = 1;
            foUTHVACTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_Temperature, foUTHVACTemperature);
            //UTHVACVelocity
            var foUTHVACVelocity = units.GetFormatOptions(UnitType.UT_HVAC_Velocity);
            foUTHVACVelocity.Accuracy = 1;
            foUTHVACVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_HVAC_Velocity, foUTHVACVelocity);
            //UTHVACAirflow
            var foUTHVACAirflow = units.GetFormatOptions(UnitType.UT_HVAC_Airflow);
            foUTHVACAirflow.Accuracy = 1;
            foUTHVACAirflow.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow, foUTHVACAirflow);
            //UTHVACDuctSize
            var foUTHVACDuctSize = units.GetFormatOptions(UnitType.UT_HVAC_DuctSize);
            foUTHVACDuctSize.Accuracy = 1;
            foUTHVACDuctSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctSize, foUTHVACDuctSize);
            //UTHVACCrossSection
            var foUTHVACCrossSection = units.GetFormatOptions(UnitType.UT_HVAC_CrossSection);
            foUTHVACCrossSection.Accuracy = 0.01;
            foUTHVACCrossSection.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_CrossSection, foUTHVACCrossSection);
            //UTHVACHeatGain
            var foUTHVACHeatGain = units.GetFormatOptions(UnitType.UT_HVAC_HeatGain);
            foUTHVACHeatGain.Accuracy = 0.1;
            foUTHVACHeatGain.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_HeatGain, foUTHVACHeatGain);
            //UTElectricalCurrent
            var foUTElectricalCurrent = units.GetFormatOptions(UnitType.UT_Electrical_Current);
            foUTElectricalCurrent.Accuracy = 1;
            foUTElectricalCurrent.DisplayUnits = DisplayUnitType.DUT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Current, foUTElectricalCurrent);
            //UTElectricalPotential
            var foUTElectricalPotential = units.GetFormatOptions(UnitType.UT_Electrical_Potential);
            foUTElectricalPotential.Accuracy = 1;
            foUTElectricalPotential.DisplayUnits = DisplayUnitType.DUT_VOLTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Potential, foUTElectricalPotential);
            //UTElectricalFrequency
            var foUTElectricalFrequency = units.GetFormatOptions(UnitType.UT_Electrical_Frequency);
            foUTElectricalFrequency.Accuracy = 1;
            foUTElectricalFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Electrical_Frequency, foUTElectricalFrequency);
            //UTElectricalIlluminance
            var foUTElectricalIlluminance = units.GetFormatOptions(UnitType.UT_Electrical_Illuminance);
            foUTElectricalIlluminance.Accuracy = 1;
            foUTElectricalIlluminance.DisplayUnits = DisplayUnitType.DUT_LUX;
            units.SetFormatOptions(UnitType.UT_Electrical_Illuminance, foUTElectricalIlluminance);
            //UTElectricalLuminance
            var foUTElectricalLuminance = units.GetFormatOptions(UnitType.UT_Electrical_Luminance);
            foUTElectricalLuminance.Accuracy = 1;
            foUTElectricalLuminance.DisplayUnits = DisplayUnitType.DUT_CANDELAS_PER_SQUARE_METER;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminance, foUTElectricalLuminance);
            //UTElectricalLuminousFlux
            var foUTElectricalLuminousFlux = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Flux);
            foUTElectricalLuminousFlux.Accuracy = 1;
            foUTElectricalLuminousFlux.DisplayUnits = DisplayUnitType.DUT_LUMENS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Flux, foUTElectricalLuminousFlux);
            //UTElectricalLuminousIntensity
            var foUTElectricalLuminousIntensity = units.GetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity);
            foUTElectricalLuminousIntensity.Accuracy = 1;
            foUTElectricalLuminousIntensity.DisplayUnits = DisplayUnitType.DUT_CANDELAS;
            units.SetFormatOptions(UnitType.UT_Electrical_Luminous_Intensity, foUTElectricalLuminousIntensity);
            //UTElectricalEfficacy
            var foUTElectricalEfficacy = units.GetFormatOptions(UnitType.UT_Electrical_Efficacy);
            foUTElectricalEfficacy.Accuracy = 1;
            foUTElectricalEfficacy.DisplayUnits = DisplayUnitType.DUT_LUMENS_PER_WATT;
            units.SetFormatOptions(UnitType.UT_Electrical_Efficacy, foUTElectricalEfficacy);
            //UTElectricalWattage
            var foUTElectricalWattage = units.GetFormatOptions(UnitType.UT_Electrical_Wattage);
            foUTElectricalWattage.Accuracy = 1;
            foUTElectricalWattage.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Wattage, foUTElectricalWattage);
            //UTColorTemperature
            var foUTColorTemperature = units.GetFormatOptions(UnitType.UT_Color_Temperature);
            foUTColorTemperature.Accuracy = 1;
            foUTColorTemperature.DisplayUnits = DisplayUnitType.DUT_KELVIN;
            units.SetFormatOptions(UnitType.UT_Color_Temperature, foUTColorTemperature);
            //UTElectricalPower
            var foUTElectricalPower = units.GetFormatOptions(UnitType.UT_Electrical_Power);
            foUTElectricalPower.Accuracy = 1;
            foUTElectricalPower.DisplayUnits = DisplayUnitType.DUT_WATTS;
            units.SetFormatOptions(UnitType.UT_Electrical_Power, foUTElectricalPower);
            //UTHVACRoughness
            var foUTHVACRoughness = units.GetFormatOptions(UnitType.UT_HVAC_Roughness);
            foUTHVACRoughness.Accuracy = 0.0001;
            foUTHVACRoughness.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
            units.SetFormatOptions(UnitType.UT_HVAC_Roughness, foUTHVACRoughness);
            //UTElectricalApparentPower
            var foUTElectricalApparentPower = units.GetFormatOptions(UnitType.UT_Electrical_Apparent_Power);
            foUTElectricalApparentPower.Accuracy = 1;
            foUTElectricalApparentPower.DisplayUnits = DisplayUnitType.DUT_VOLT_AMPERES;
            units.SetFormatOptions(UnitType.UT_Electrical_Apparent_Power, foUTElectricalApparentPower);
            //UTElectricalPowerDensity
            var foUTElectricalPowerDensity = units.GetFormatOptions(UnitType.UT_Electrical_Power_Density);
            foUTElectricalPowerDensity.Accuracy = 0.01;
            foUTElectricalPowerDensity.DisplayUnits = DisplayUnitType.DUT_WATTS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_Electrical_Power_Density, foUTElectricalPowerDensity);
            //UTPipingDensity
            var foUTPipingDensity = units.GetFormatOptions(UnitType.UT_Piping_Density);
            foUTPipingDensity.Accuracy = 0.0001;
            foUTPipingDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_Piping_Density, foUTPipingDensity);
            //UTPipingFlow
            var foUTPipingFlow = units.GetFormatOptions(UnitType.UT_Piping_Flow);
            foUTPipingFlow.Accuracy = 1;
            foUTPipingFlow.DisplayUnits = DisplayUnitType.DUT_GALLONS_US_PER_MINUTE;
            units.SetFormatOptions(UnitType.UT_Piping_Flow, foUTPipingFlow);
            //UTPipingFriction
            var foUTPipingFriction = units.GetFormatOptions(UnitType.UT_Piping_Friction);
            foUTPipingFriction.Accuracy = 0.01;
            foUTPipingFriction.DisplayUnits = DisplayUnitType.DUT_FEET_OF_WATER_PER_100FT;
            units.SetFormatOptions(UnitType.UT_Piping_Friction, foUTPipingFriction);
            //UTPipingPressure
            var foUTPipingPressure = units.GetFormatOptions(UnitType.UT_Piping_Pressure);
            foUTPipingPressure.Accuracy = 0.01;
            foUTPipingPressure.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_SQUARE_INCH;
            units.SetFormatOptions(UnitType.UT_Piping_Pressure, foUTPipingPressure);
            //UTPipingTemperature
            var foUTPipingTemperature = units.GetFormatOptions(UnitType.UT_Piping_Temperature);
            foUTPipingTemperature.Accuracy = 1;
            foUTPipingTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_Piping_Temperature, foUTPipingTemperature);
            //UTPipingVelocity
            var foUTPipingVelocity = units.GetFormatOptions(UnitType.UT_Piping_Velocity);
            foUTPipingVelocity.Accuracy = 1;
            foUTPipingVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Piping_Velocity, foUTPipingVelocity);
            //UTPipingViscosity
            var foUTPipingViscosity = units.GetFormatOptions(UnitType.UT_Piping_Viscosity);
            foUTPipingViscosity.Accuracy = 0.01;
            foUTPipingViscosity.DisplayUnits = DisplayUnitType.DUT_CENTIPOISES;
            units.SetFormatOptions(UnitType.UT_Piping_Viscosity, foUTPipingViscosity);
            //UTPipeSize
            var foUTPipeSize = units.GetFormatOptions(UnitType.UT_PipeSize);
            foUTPipeSize.Accuracy = 1;
            foUTPipeSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_PipeSize, foUTPipeSize);
            //UTPipingRoughness
            var foUTPipingRoughness = units.GetFormatOptions(UnitType.UT_Piping_Roughness);
            foUTPipingRoughness.Accuracy = 1E-05;
            foUTPipingRoughness.DisplayUnits = DisplayUnitType.DUT_DECIMAL_FEET;
            units.SetFormatOptions(UnitType.UT_Piping_Roughness, foUTPipingRoughness);
            //UTPipingVolume
            var foUTPipingVolume = units.GetFormatOptions(UnitType.UT_Piping_Volume);
            foUTPipingVolume.Accuracy = 0.1;
            foUTPipingVolume.DisplayUnits = DisplayUnitType.DUT_GALLONS_US;
            units.SetFormatOptions(UnitType.UT_Piping_Volume, foUTPipingVolume);
            //UTHVACViscosity
            var foUTHVACViscosity = units.GetFormatOptions(UnitType.UT_HVAC_Viscosity);
            foUTHVACViscosity.Accuracy = 0.01;
            foUTHVACViscosity.DisplayUnits = DisplayUnitType.DUT_CENTIPOISES;
            units.SetFormatOptions(UnitType.UT_HVAC_Viscosity, foUTHVACViscosity);
            //UTHVACCoefficientOfHeatTransfer
            var foUTHVACCoefficientOfHeatTransfer = units.GetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer);
            foUTHVACCoefficientOfHeatTransfer.Accuracy = 0.0001;
            foUTHVACCoefficientOfHeatTransfer.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_CoefficientOfHeatTransfer, foUTHVACCoefficientOfHeatTransfer);
            //UTHVACThermalResistance
            var foUTHVACThermalResistance = units.GetFormatOptions(UnitType.UT_HVAC_ThermalResistance);
            foUTHVACThermalResistance.Accuracy = 0.0001;
            foUTHVACThermalResistance.DisplayUnits =
                DisplayUnitType.DUT_HOUR_SQUARE_FOOT_FAHRENHEIT_PER_BRITISH_THERMAL_UNIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalResistance, foUTHVACThermalResistance);
            //UTHVACThermalMass
            var foUTHVACThermalMass = units.GetFormatOptions(UnitType.UT_HVAC_ThermalMass);
            foUTHVACThermalMass.Accuracy = 0.0001;
            foUTHVACThermalMass.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNIT_PER_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalMass, foUTHVACThermalMass);
            //UTHVACThermalConductivity
            var foUTHVACThermalConductivity = units.GetFormatOptions(UnitType.UT_HVAC_ThermalConductivity);
            foUTHVACThermalConductivity.Accuracy = 0.0001;
            foUTHVACThermalConductivity.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_FOOT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_ThermalConductivity, foUTHVACThermalConductivity);
            //UTHVACSpecificHeat
            var foUTHVACSpecificHeat = units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeat);
            foUTHVACSpecificHeat.Accuracy = 0.0001;
            foUTHVACSpecificHeat.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_POUND_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeat, foUTHVACSpecificHeat);
            //UTHVACSpecificHeatOfVaporization
            var foUTHVACSpecificHeatOfVaporization =
                units.GetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization);
            foUTHVACSpecificHeatOfVaporization.Accuracy = 0.0001;
            foUTHVACSpecificHeatOfVaporization.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_POUND;
            units.SetFormatOptions(UnitType.UT_HVAC_SpecificHeatOfVaporization, foUTHVACSpecificHeatOfVaporization);
            //UTHVACPermeability
            var foUTHVACPermeability = units.GetFormatOptions(UnitType.UT_HVAC_Permeability);
            foUTHVACPermeability.Accuracy = 0.0001;
            foUTHVACPermeability.DisplayUnits = DisplayUnitType.DUT_GRAINS_PER_HOUR_SQUARE_FOOT_INCH_MERCURY;
            units.SetFormatOptions(UnitType.UT_HVAC_Permeability, foUTHVACPermeability);
            //UTElectricalResistivity
            var foUTElectricalResistivity = units.GetFormatOptions(UnitType.UT_Electrical_Resistivity);
            foUTElectricalResistivity.Accuracy = 0.0001;
            foUTElectricalResistivity.DisplayUnits = DisplayUnitType.DUT_OHM_METERS;
            units.SetFormatOptions(UnitType.UT_Electrical_Resistivity, foUTElectricalResistivity);
            //UTHVACAirflowDensity
            var foUTHVACAirflowDensity = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Density);
            foUTHVACAirflowDensity.Accuracy = 0.01;
            foUTHVACAirflowDensity.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Density, foUTHVACAirflowDensity);
            //UTSlope
            var foUTSlope = units.GetFormatOptions(UnitType.UT_Slope);
            foUTSlope.Accuracy = 0.01;
            foUTSlope.DisplayUnits = DisplayUnitType.DUT_SLOPE_DEGREES;
            units.SetFormatOptions(UnitType.UT_Slope, foUTSlope);
            //UTHVACCoolingLoad
            var foUTHVACCoolingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load);
            foUTHVACCoolingLoad.Accuracy = 0.1;
            foUTHVACCoolingLoad.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load, foUTHVACCoolingLoad);
            //UTHVACHeatingLoad
            var foUTHVACHeatingLoad = units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load);
            foUTHVACHeatingLoad.Accuracy = 0.1;
            foUTHVACHeatingLoad.DisplayUnits = DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load, foUTHVACHeatingLoad);
            //UTHVACCoolingLoadDividedByArea
            var foUTHVACCoolingLoadDividedByArea =
                units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area);
            foUTHVACCoolingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByArea.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Area, foUTHVACCoolingLoadDividedByArea);
            //UTHVACHeatingLoadDividedByArea
            var foUTHVACHeatingLoadDividedByArea =
                units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area);
            foUTHVACHeatingLoadDividedByArea.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByArea.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Area, foUTHVACHeatingLoadDividedByArea);
            //UTHVACCoolingLoadDividedByVolume
            var foUTHVACCoolingLoadDividedByVolume =
                units.GetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume);
            foUTHVACCoolingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACCoolingLoadDividedByVolume.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Cooling_Load_Divided_By_Volume, foUTHVACCoolingLoadDividedByVolume);
            //UTHVACHeatingLoadDividedByVolume
            var foUTHVACHeatingLoadDividedByVolume =
                units.GetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume);
            foUTHVACHeatingLoadDividedByVolume.Accuracy = 0.01;
            foUTHVACHeatingLoadDividedByVolume.DisplayUnits =
                DisplayUnitType.DUT_BRITISH_THERMAL_UNITS_PER_HOUR_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Heating_Load_Divided_By_Volume, foUTHVACHeatingLoadDividedByVolume);
            //UTHVACAirflowDividedByVolume
            var foUTHVACAirflowDividedByVolume = units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume);
            foUTHVACAirflowDividedByVolume.Accuracy = 0.01;
            foUTHVACAirflowDividedByVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Volume, foUTHVACAirflowDividedByVolume);
            //UTHVACAirflowDividedByCoolingLoad
            var foUTHVACAirflowDividedByCoolingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load);
            foUTHVACAirflowDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAirflowDividedByCoolingLoad.DisplayUnits =
                DisplayUnitType.DUT_CUBIC_FEET_PER_MINUTE_TON_OF_REFRIGERATION;
            units.SetFormatOptions(UnitType.UT_HVAC_Airflow_Divided_By_Cooling_Load,
                foUTHVACAirflowDividedByCoolingLoad);
            //UTHVACAreaDividedByCoolingLoad
            var foUTHVACAreaDividedByCoolingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load);
            foUTHVACAreaDividedByCoolingLoad.Accuracy = 0.01;
            foUTHVACAreaDividedByCoolingLoad.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET_PER_TON_OF_REFRIGERATION;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Cooling_Load, foUTHVACAreaDividedByCoolingLoad);
            //UTHVACAreaDividedByHeatingLoad
            var foUTHVACAreaDividedByHeatingLoad =
                units.GetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load);
            foUTHVACAreaDividedByHeatingLoad.Accuracy = 0.0001;
            foUTHVACAreaDividedByHeatingLoad.DisplayUnits =
                DisplayUnitType.DUT_SQUARE_FEET_PER_THOUSAND_BRITISH_THERMAL_UNITS_PER_HOUR;
            units.SetFormatOptions(UnitType.UT_HVAC_Area_Divided_By_Heating_Load, foUTHVACAreaDividedByHeatingLoad);
            //UTWireSize
            var foUTWireSize = units.GetFormatOptions(UnitType.UT_WireSize);
            foUTWireSize.Accuracy = 1E-06;
            foUTWireSize.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_WireSize, foUTWireSize);
            //UTHVACSlope
            var foUTHVACSlope = units.GetFormatOptions(UnitType.UT_HVAC_Slope);
            foUTHVACSlope.Accuracy = 0.03125;
            foUTHVACSlope.DisplayUnits = DisplayUnitType.DUT_RISE_OVER_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_Slope, foUTHVACSlope);
            //UTPipingSlope
            var foUTPipingSlope = units.GetFormatOptions(UnitType.UT_Piping_Slope);
            foUTPipingSlope.Accuracy = 0.03125;
            foUTPipingSlope.DisplayUnits = DisplayUnitType.DUT_RISE_OVER_INCHES;
            units.SetFormatOptions(UnitType.UT_Piping_Slope, foUTPipingSlope);
            //UTCurrency
            var foUTCurrency = units.GetFormatOptions(UnitType.UT_Currency);
            foUTCurrency.Accuracy = 0.01;
            foUTCurrency.DisplayUnits = DisplayUnitType.DUT_CURRENCY;
            units.SetFormatOptions(UnitType.UT_Currency, foUTCurrency);
            //UTMassDensity
            var foUTMassDensity = units.GetFormatOptions(UnitType.UT_MassDensity);
            foUTMassDensity.Accuracy = 0.01;
            foUTMassDensity.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_MassDensity, foUTMassDensity);
            //UTHVACFactor
            var foUTHVACFactor = units.GetFormatOptions(UnitType.UT_HVAC_Factor);
            foUTHVACFactor.Accuracy = 0.01;
            foUTHVACFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_HVAC_Factor, foUTHVACFactor);
            //UTElectricalTemperature
            var foUTElectricalTemperature = units.GetFormatOptions(UnitType.UT_Electrical_Temperature);
            foUTElectricalTemperature.Accuracy = 1;
            foUTElectricalTemperature.DisplayUnits = DisplayUnitType.DUT_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_Electrical_Temperature, foUTElectricalTemperature);
            //UTElectricalCableTraySize
            var foUTElectricalCableTraySize = units.GetFormatOptions(UnitType.UT_Electrical_CableTraySize);
            foUTElectricalCableTraySize.Accuracy = 0.25;
            foUTElectricalCableTraySize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Electrical_CableTraySize, foUTElectricalCableTraySize);
            //UTElectricalConduitSize
            var foUTElectricalConduitSize = units.GetFormatOptions(UnitType.UT_Electrical_ConduitSize);
            foUTElectricalConduitSize.Accuracy = 0.125;
            foUTElectricalConduitSize.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Electrical_ConduitSize, foUTElectricalConduitSize);
            //UTElectricalDemandFactor
            var foUTElectricalDemandFactor = units.GetFormatOptions(UnitType.UT_Electrical_Demand_Factor);
            foUTElectricalDemandFactor.Accuracy = 0.01;
            foUTElectricalDemandFactor.DisplayUnits = DisplayUnitType.DUT_PERCENTAGE;
            units.SetFormatOptions(UnitType.UT_Electrical_Demand_Factor, foUTElectricalDemandFactor);
            //UTHVACDuctInsulationThickness
            var foUTHVACDuctInsulationThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness);
            foUTHVACDuctInsulationThickness.Accuracy = 1;
            foUTHVACDuctInsulationThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctInsulationThickness, foUTHVACDuctInsulationThickness);
            //UTHVACDuctLiningThickness
            var foUTHVACDuctLiningThickness = units.GetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness);
            foUTHVACDuctLiningThickness.Accuracy = 1;
            foUTHVACDuctLiningThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_HVAC_DuctLiningThickness, foUTHVACDuctLiningThickness);
            //UTPipeInsulationThickness
            var foUTPipeInsulationThickness = units.GetFormatOptions(UnitType.UT_PipeInsulationThickness);
            foUTPipeInsulationThickness.Accuracy = 1;
            foUTPipeInsulationThickness.DisplayUnits = DisplayUnitType.DUT_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_PipeInsulationThickness, foUTPipeInsulationThickness);
            //UTForce
            var foUTForce = units.GetFormatOptions(UnitType.UT_Force);
            foUTForce.Accuracy = 0.01;
            foUTForce.DisplayUnits = DisplayUnitType.DUT_KIPS;
            units.SetFormatOptions(UnitType.UT_Force, foUTForce);
            //UTLinearForce
            var foUTLinearForce = units.GetFormatOptions(UnitType.UT_LinearForce);
            foUTLinearForce.Accuracy = 0.001;
            foUTLinearForce.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForce, foUTLinearForce);
            //UTAreaForce
            var foUTAreaForce = units.GetFormatOptions(UnitType.UT_AreaForce);
            foUTAreaForce.Accuracy = 0.0001;
            foUTAreaForce.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_AreaForce, foUTAreaForce);
            //UTMoment
            var foUTMoment = units.GetFormatOptions(UnitType.UT_Moment);
            foUTMoment.Accuracy = 0.01;
            foUTMoment.DisplayUnits = DisplayUnitType.DUT_KIP_FEET;
            units.SetFormatOptions(UnitType.UT_Moment, foUTMoment);
            //UTLinearMoment
            var foUTLinearMoment = units.GetFormatOptions(UnitType.UT_LinearMoment);
            foUTLinearMoment.Accuracy = 0.01;
            foUTLinearMoment.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearMoment, foUTLinearMoment);
            //UTStress
            var foUTStress = units.GetFormatOptions(UnitType.UT_Stress);
            foUTStress.Accuracy = 0.01;
            foUTStress.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_INCH;
            units.SetFormatOptions(UnitType.UT_Stress, foUTStress);
            //UTUnitWeight
            var foUTUnitWeight = units.GetFormatOptions(UnitType.UT_UnitWeight);
            foUTUnitWeight.Accuracy = 0.01;
            foUTUnitWeight.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_UnitWeight, foUTUnitWeight);
            //UTWeight
            var foUTWeight = units.GetFormatOptions(UnitType.UT_Weight);
            foUTWeight.Accuracy = 0.01;
            foUTWeight.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE;
            units.SetFormatOptions(UnitType.UT_Weight, foUTWeight);
            //UTMass
            var foUTMass = units.GetFormatOptions(UnitType.UT_Mass);
            foUTMass.Accuracy = 0.01;
            foUTMass.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS;
            units.SetFormatOptions(UnitType.UT_Mass, foUTMass);
            //UTMassPerUnitArea
            var foUTMassPerUnitArea = units.GetFormatOptions(UnitType.UT_MassPerUnitArea);
            foUTMassPerUnitArea.Accuracy = 0.01;
            foUTMassPerUnitArea.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_MassPerUnitArea, foUTMassPerUnitArea);
            //UTThermalExpansion
            var foUTThermalExpansion = units.GetFormatOptions(UnitType.UT_ThermalExpansion);
            foUTThermalExpansion.Accuracy = 1E-05;
            foUTThermalExpansion.DisplayUnits = DisplayUnitType.DUT_INV_FAHRENHEIT;
            units.SetFormatOptions(UnitType.UT_ThermalExpansion, foUTThermalExpansion);
            //UTForcePerLength
            var foUTForcePerLength = units.GetFormatOptions(UnitType.UT_ForcePerLength);
            foUTForcePerLength.Accuracy = 0.1;
            foUTForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_INCH;
            units.SetFormatOptions(UnitType.UT_ForcePerLength, foUTForcePerLength);
            //UTLinearForcePerLength
            var foUTLinearForcePerLength = units.GetFormatOptions(UnitType.UT_LinearForcePerLength);
            foUTLinearForcePerLength.Accuracy = 0.1;
            foUTLinearForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_SQUARE_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForcePerLength, foUTLinearForcePerLength);
            //UTAreaForcePerLength
            var foUTAreaForcePerLength = units.GetFormatOptions(UnitType.UT_AreaForcePerLength);
            foUTAreaForcePerLength.Accuracy = 0.1;
            foUTAreaForcePerLength.DisplayUnits = DisplayUnitType.DUT_KIPS_PER_CUBIC_FOOT;
            units.SetFormatOptions(UnitType.UT_AreaForcePerLength, foUTAreaForcePerLength);
            //UTForceLengthPerAngle
            var foUTForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_ForceLengthPerAngle);
            foUTForceLengthPerAngle.Accuracy = 0.1;
            foUTForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_DEGREE;
            units.SetFormatOptions(UnitType.UT_ForceLengthPerAngle, foUTForceLengthPerAngle);
            //UTLinearForceLengthPerAngle
            var foUTLinearForceLengthPerAngle = units.GetFormatOptions(UnitType.UT_LinearForceLengthPerAngle);
            foUTLinearForceLengthPerAngle.Accuracy = 0.1;
            foUTLinearForceLengthPerAngle.DisplayUnits = DisplayUnitType.DUT_KIP_FEET_PER_DEGREE_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_LinearForceLengthPerAngle, foUTLinearForceLengthPerAngle);
            //UTDisplacementDeflection
            var foUTDisplacementDeflection = units.GetFormatOptions(UnitType.UT_Displacement_Deflection);
            foUTDisplacementDeflection.Accuracy = 0.01;
            foUTDisplacementDeflection.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Displacement_Deflection, foUTDisplacementDeflection);
            //UTRotation
            var foUTRotation = units.GetFormatOptions(UnitType.UT_Rotation);
            foUTRotation.Accuracy = 0.001;
            foUTRotation.DisplayUnits = DisplayUnitType.DUT_DECIMAL_DEGREES;
            units.SetFormatOptions(UnitType.UT_Rotation, foUTRotation);
            //UTPeriod
            var foUTPeriod = units.GetFormatOptions(UnitType.UT_Period);
            foUTPeriod.Accuracy = 0.1;
            foUTPeriod.DisplayUnits = DisplayUnitType.DUT_SECONDS;
            units.SetFormatOptions(UnitType.UT_Period, foUTPeriod);
            //UTStructuralFrequency
            var foUTStructuralFrequency = units.GetFormatOptions(UnitType.UT_Structural_Frequency);
            foUTStructuralFrequency.Accuracy = 0.1;
            foUTStructuralFrequency.DisplayUnits = DisplayUnitType.DUT_HERTZ;
            units.SetFormatOptions(UnitType.UT_Structural_Frequency, foUTStructuralFrequency);
            //UTPulsation
            var foUTPulsation = units.GetFormatOptions(UnitType.UT_Pulsation);
            foUTPulsation.Accuracy = 0.1;
            foUTPulsation.DisplayUnits = DisplayUnitType.DUT_RADIANS_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Pulsation, foUTPulsation);
            //UTStructuralVelocity
            var foUTStructuralVelocity = units.GetFormatOptions(UnitType.UT_Structural_Velocity);
            foUTStructuralVelocity.Accuracy = 0.1;
            foUTStructuralVelocity.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND;
            units.SetFormatOptions(UnitType.UT_Structural_Velocity, foUTStructuralVelocity);
            //UTAcceleration
            var foUTAcceleration = units.GetFormatOptions(UnitType.UT_Acceleration);
            foUTAcceleration.Accuracy = 0.1;
            foUTAcceleration.DisplayUnits = DisplayUnitType.DUT_FEET_PER_SECOND_SQUARED;
            units.SetFormatOptions(UnitType.UT_Acceleration, foUTAcceleration);
            //UTEnergy
            var foUTEnergy = units.GetFormatOptions(UnitType.UT_Energy);
            foUTEnergy.Accuracy = 0.1;
            foUTEnergy.DisplayUnits = DisplayUnitType.DUT_POUND_FORCE_FEET;
            units.SetFormatOptions(UnitType.UT_Energy, foUTEnergy);
            //UTReinforcementVolume
            var foUTReinforcementVolume = units.GetFormatOptions(UnitType.UT_Reinforcement_Volume);
            foUTReinforcementVolume.Accuracy = 0.01;
            foUTReinforcementVolume.DisplayUnits = DisplayUnitType.DUT_CUBIC_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Volume, foUTReinforcementVolume);
            //UTReinforcementLength
            var foUTReinforcementLength = units.GetFormatOptions(UnitType.UT_Reinforcement_Length);
            foUTReinforcementLength.Accuracy = 0.00260416666666667;
            foUTReinforcementLength.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Length, foUTReinforcementLength);
            //UTReinforcementArea
            var foUTReinforcementArea = units.GetFormatOptions(UnitType.UT_Reinforcement_Area);
            foUTReinforcementArea.Accuracy = 0.01;
            foUTReinforcementArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area, foUTReinforcementArea);
            //UTReinforcementAreaperUnitLength
            var foUTReinforcementAreaperUnitLength =
                units.GetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length);
            foUTReinforcementAreaperUnitLength.Accuracy = 0.01;
            foUTReinforcementAreaperUnitLength.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Area_per_Unit_Length, foUTReinforcementAreaperUnitLength);
            //UTReinforcementSpacing
            var foUTReinforcementSpacing = units.GetFormatOptions(UnitType.UT_Reinforcement_Spacing);
            foUTReinforcementSpacing.Accuracy = 0.00260416666666667;
            foUTReinforcementSpacing.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Spacing, foUTReinforcementSpacing);
            //UTReinforcementCover
            var foUTReinforcementCover = units.GetFormatOptions(UnitType.UT_Reinforcement_Cover);
            foUTReinforcementCover.Accuracy = 0.00260416666666667;
            foUTReinforcementCover.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Reinforcement_Cover, foUTReinforcementCover);
            //UTBarDiameter
            var foUTBarDiameter = units.GetFormatOptions(UnitType.UT_Bar_Diameter);
            foUTBarDiameter.Accuracy = 0.00260416666666667;
            foUTBarDiameter.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Bar_Diameter, foUTBarDiameter);
            //UTCrackWidth
            var foUTCrackWidth = units.GetFormatOptions(UnitType.UT_Crack_Width);
            foUTCrackWidth.Accuracy = 0.01;
            foUTCrackWidth.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Crack_Width, foUTCrackWidth);
            //UTSectionDimension
            var foUTSectionDimension = units.GetFormatOptions(UnitType.UT_Section_Dimension);
            foUTSectionDimension.Accuracy = 0.00520833333333333;
            foUTSectionDimension.DisplayUnits = DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Dimension, foUTSectionDimension);
            //UTSectionProperty
            var foUTSectionProperty = units.GetFormatOptions(UnitType.UT_Section_Property);
            foUTSectionProperty.Accuracy = 0.001;
            foUTSectionProperty.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Property, foUTSectionProperty);
            //UTSectionArea
            var foUTSectionArea = units.GetFormatOptions(UnitType.UT_Section_Area);
            foUTSectionArea.Accuracy = 0.01;
            foUTSectionArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Area, foUTSectionArea);
            //UTSectionModulus
            var foUTSectionModulus = units.GetFormatOptions(UnitType.UT_Section_Modulus);
            foUTSectionModulus.Accuracy = 0.01;
            foUTSectionModulus.DisplayUnits = DisplayUnitType.DUT_CUBIC_INCHES;
            units.SetFormatOptions(UnitType.UT_Section_Modulus, foUTSectionModulus);
            //UTMomentofInertia
            var foUTMomentofInertia = units.GetFormatOptions(UnitType.UT_Moment_of_Inertia);
            foUTMomentofInertia.Accuracy = 0.01;
            foUTMomentofInertia.DisplayUnits = DisplayUnitType.DUT_INCHES_TO_THE_FOURTH_POWER;
            units.SetFormatOptions(UnitType.UT_Moment_of_Inertia, foUTMomentofInertia);
            //UTWarpingConstant
            var foUTWarpingConstant = units.GetFormatOptions(UnitType.UT_Warping_Constant);
            foUTWarpingConstant.Accuracy = 0.01;
            foUTWarpingConstant.DisplayUnits = DisplayUnitType.DUT_INCHES_TO_THE_SIXTH_POWER;
            units.SetFormatOptions(UnitType.UT_Warping_Constant, foUTWarpingConstant);
            //UTMassperUnitLength
            var foUTMassperUnitLength = units.GetFormatOptions(UnitType.UT_Mass_per_Unit_Length);
            foUTMassperUnitLength.Accuracy = 0.01;
            foUTMassperUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Mass_per_Unit_Length, foUTMassperUnitLength);
            //UTWeightperUnitLength
            var foUTWeightperUnitLength = units.GetFormatOptions(UnitType.UT_Weight_per_Unit_Length);
            foUTWeightperUnitLength.Accuracy = 0.01;
            foUTWeightperUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_FORCE_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Weight_per_Unit_Length, foUTWeightperUnitLength);
            //UTSurfaceArea
            var foUTSurfaceArea = units.GetFormatOptions(UnitType.UT_Surface_Area);
            foUTSurfaceArea.Accuracy = 0.01;
            foUTSurfaceArea.DisplayUnits = DisplayUnitType.DUT_SQUARE_FEET_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_Surface_Area, foUTSurfaceArea);
            //UTPipeDimension
            var foUTPipeDimension = units.GetFormatOptions(UnitType.UT_Pipe_Dimension);
            foUTPipeDimension.Accuracy = 0.001;
            foUTPipeDimension.DisplayUnits = DisplayUnitType.DUT_DECIMAL_INCHES;
            units.SetFormatOptions(UnitType.UT_Pipe_Dimension, foUTPipeDimension);
            //UTPipeMass
            var foUTPipeMass = units.GetFormatOptions(UnitType.UT_PipeMass);
            foUTPipeMass.Accuracy = 0.01;
            foUTPipeMass.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS;
            units.SetFormatOptions(UnitType.UT_PipeMass, foUTPipeMass);
            //UTPipeMassPerUnitLength
            var foUTPipeMassPerUnitLength = units.GetFormatOptions(UnitType.UT_PipeMassPerUnitLength);
            foUTPipeMassPerUnitLength.Accuracy = 0.01;
            foUTPipeMassPerUnitLength.DisplayUnits = DisplayUnitType.DUT_POUNDS_MASS_PER_FOOT;
            units.SetFormatOptions(UnitType.UT_PipeMassPerUnitLength, foUTPipeMassPerUnitLength);

            using (var t = new Transaction(doc, "Convert to Imperial"))
            {
                t.Start();

                doc.SetUnits(units);

                t.Commit();
            }
        }

        private void radioButtonMetric_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.revit.com.au");
        }
    }
}