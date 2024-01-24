using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace UnitsConverter
{
    class ExternalApplication : IExternalApplication
    {

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public BitmapImage Convert(Image img)
        {
            using (var memory = new MemoryStream())
            {
                img.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public Result OnStartup(UIControlledApplication application)
        {

            //get the file path of the current assembly
            string path = Assembly.GetExecutingAssembly().Location;
            string tabName = "Revit.AU";

            //create ribbon tab
            application.CreateRibbonTab(tabName);

            //create button for metric for current document
            PushButtonData metricDocButton = new PushButtonData("metricDocButton", "Metric", path, "UnitsConverter.ConvertCurrentDocumetToMetric");
            //create button for imperialfor current document
            PushButtonData imperialDocButton = new PushButtonData("imperialDocButton", "Imperial", path, "UnitsConverter.ConvertCurrentDocumetToImperial");

            PushButtonData batchDocButton = new PushButtonData("batchDocButton", "Batch Convert", path, "UnitsConverter.BatchConverter");



            //create ribbon panel
            RibbonPanel panel = application.CreateRibbonPanel(tabName, "Unit Conversion Tools");

            /*
            //get images for buttons
            string imagePath = @"C:\Users\leniharp\source\repos\UnitsConverter\UnitsConverter\";
            string metric = "metric_16.png";
            string imperial = "imperial_16.png";
            string batch = "batch_32.png";
            */

            /*
            Uri mImagePath = new Uri(imagePath+metric);
            Uri iImagePath = new Uri(imagePath+imperial);
            Uri bImagePath = new Uri(imagePath+batch);
            */

            Image bResource = Properties.Resources.batch_32;
            Image mResource = Properties.Resources.metric_16;
            Image iResource = Properties.Resources.imperial_16;


            /*
            BitmapImage mImage = new BitmapImage(mImagePath);
            BitmapImage iImage = new BitmapImage(iImagePath);
            BitmapImage bImage = new BitmapImage(bImagePath);
            */

            //BitmapImage bImage = Properties.Resources.batch_32;

            //PushButton mDocButton = panel.AddItem(metricDocButton) as PushButton;
            //PushButton iDocButton = panel.AddItem(imperialDocButton) as PushButton;

            //metricDocButton.Image = mImage;
            //imperialDocButton.Image = iImage;

            metricDocButton.Image = Convert(mResource);
            imperialDocButton.Image = Convert(iResource);
            batchDocButton.LargeImage = Convert(bResource);




            List<RibbonItem> unitsButtons = new List<RibbonItem>();
            unitsButtons.AddRange(panel.AddStackedItems(metricDocButton, imperialDocButton));

            PushButton bDocButton = panel.AddItem(batchDocButton) as PushButton;




            return Result.Succeeded;
        }

    }
}
