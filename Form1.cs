/*
 * This program will search for files based on an extension.
 * You do not need to use a '.' in the extension.
 * It will not automatically delete a file, it will require you to press Delete after.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ZetaLongPaths;

namespace FindExtensionAndDelete
{
    public partial class form1 : Form
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        List<String> stringList = new List<string>();
        StreamWriter sw;
        string fileExtension, location;
        long counter = 0;
        bool deleting = false;

        public form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerSupportsCancellation = true;

            buttonStop.Hide();
            buttonDelete.Hide();
            saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                boxLocation.Text = folderBrowserDialog.SelectedPath;

                if (boxLocation.Text[boxLocation.Text.Length - 1] != '\\')
                {
                    boxLocation.AppendText("\\");
                }
            }
        }

        private void buttonSt_Click(object sender, EventArgs e)
        {
            if((string.IsNullOrEmpty(boxLocation.Text) || string.IsNullOrEmpty(boxExtension.Text)))
            {
                MessageBox.Show("You must fill in all fields!");
            }
            else
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((sw = new StreamWriter(saveFileDialog.OpenFile())) != null)
                    {
                        sw.Close();
                        buttonSt.Hide();
                        buttonStop.Show();
                        boxLocation.ReadOnly = true;
                        boxExtension.ReadOnly = true;

                        fileExtension = boxExtension.Text;
                        buttonLocation.Enabled = false;
                        location = boxLocation.Text;

                        backgroundWorker1.RunWorkerAsync();
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            ZlpDirectoryInfo rootDir;
            rootDir = new ZlpDirectoryInfo(folderBrowserDialog.SelectedPath);
            WalkDirectoryTree(rootDir);
        }

        private void WalkDirectoryTree(ZlpDirectoryInfo root)
        {
            ZlpFileInfo[] files = null;
            ZlpDirectoryInfo[] subDirs = null;

            // First, process all the files directly under this folder 
            try
            {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e)
            {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
                stringList.Add(e.Message);
            }

            catch (System.IO.DirectoryNotFoundException e)
            {
                stringList.Add(e.Message);
            }

            if (files != null)
            {
                //foreach (System.IO.FileInfo fi in files)
                foreach (ZlpFileInfo fi in files)
                {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().

                    this.Invoke(new Action(() =>
                    {
                        boxStatus.Text = fi.DirectoryName;
                    }));

                    string[] fileName = fi.Name.Split('.');
                    string fileNameOnly = fi.Name;

                    if (fileName[fileName.Length - 1] == fileExtension && fileNameOnly[0] != '~')
                    {
                        try
                        {
                            //stringList.Add(fi.FullName);

                            counter++;
                            this.Invoke(new Action(() =>
                            {
                                labelFound.Text = counter.ToString() + " objects found!";
                            }));

                            stringList.Add(fi.FullName);

                        }
                        catch (Exception ex)
                        {
                            stringList.Add(ex.Message);
                        }
                    }
                }

                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();

                //foreach (System.IO.DirectoryInfo dirInfo in subDirs)
                foreach (ZlpDirectoryInfo dirInfo in subDirs)
                {
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree(dirInfo);
                    if (backgroundWorker1.CancellationPending)
                    {
                        break;
                    }
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Run finished!");
            buttonStop.Hide();
            buttonSt.Show();

            boxExtension.ReadOnly = false;
            boxLocation.ReadOnly = false;

            buttonLocation.Enabled = true;

            if ((sw = new StreamWriter(saveFileDialog.OpenFile())) != null)
            {
                foreach (String t in stringList)
                {
                    sw.WriteLine(t);
                }
                sw.Close();
            }

            this.Size = new System.Drawing.Size(248, 285);
            buttonDelete.Enabled = true;
            buttonDelete.Show();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (!deleting)
            {
                backgroundWorker2.RunWorkerAsync();
                buttonDelete.Text = "Stop";
            }
            else
            {
                backgroundWorker2.CancelAsync();
                buttonDelete.Enabled = false;
            }
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            foreach (String t in stringList)
            {
                this.Invoke(new Action(() =>
                {
                    boxStatus.Text = t;
                }));
                File.Delete(t);         
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Finished Deleting!");
        }

    }
}
