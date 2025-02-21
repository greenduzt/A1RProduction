

using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Other;
using Microsoft.Practices.Prism.Commands;
using Microsoft.WindowsAPICodePack.Dialogs;
using MsgBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interactivity;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;


namespace A1QSystem.ViewModel.Machine.MachineHistory
{
    public class OpenUploadedFilesViewModel: ViewModelBase
    {
        public event System.Action<int> Closed;
        private string _machineName; // this is the path that you are checking.
        private Int64 _workOrderNo;
        private ObservableCollection<FileUpload> _fileUploadedList;

        private ICommand _downloadDocumentCommand;
        private ICommand _downloadAllCommand;
        private DelegateCommand _closeCommand;
        public OpenUploadedFilesViewModel(Int64 workOrderNo, string machineName)
        {
            WorkOrderNo = workOrderNo;
            MachineName = machineName;
            FileUploadedList = new ObservableCollection<FileUpload>();
            FileUploadedList=DBAccess.GetUploadedFilesByWorkOrderNo(workOrderNo);

            _closeCommand = new DelegateCommand(CloseForm);
        }


        private void DownloadAllFiles()
        {
            if (FileUploadedList != null && FileUploadedList.Count > 0)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                dialog.Title = "Select a folder to download";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    //MessageBox.Show("You selected: " + dialog.FileName);
                }
                foreach (var item in FileUploadedList)
                {
                    string fromFile = (item.FilePathTo + "/" + item.FileName).Replace("\\", "/");
                    string toFile = dialog.FileName + @"\" + item.FileName;
                    WebClient webClient = new WebClient();

                    if (!File.Exists(toFile))
                    {
                        
                        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                        //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        webClient.DownloadFile(new Uri(fromFile), toFile);
                    }
                    else
                    {
                        if (Msg.Show("This file already exist in the folder" + System.Environment.NewLine + "Would you like to replace it with the same name? " + System.Environment.NewLine +"or NO to enter a new file name", "", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                            webClient.DownloadFile(new Uri(fromFile), dialog.FileName + @"\" + item.FileName);
                        }
                        else
                        {
                            string newFileName = Microsoft.VisualBasic.Interaction.InputBox("Enter new file name to save file " + item.FileName, "Enter File Name", "");

                            if (!string.IsNullOrWhiteSpace(newFileName))
                            {
                                FileInfo fi = new FileInfo(item.FileName);
                                string extn = fi.Extension;

                                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                                webClient.DownloadFile(new Uri(fromFile), dialog.FileName + @"\" + newFileName+ extn);
                            }
                            else
                            {
                                Msg.Show("File name required", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                            }
                        }
                    }
                }
                CloseForm();
            }
            else
            {
                Msg.Show("File not available to download", "", MsgBoxButtons.OK, MsgBoxImage.Warning, MsgBoxResult.OK);

            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Msg.Show("Download Completed!", "", MsgBoxButtons.OK, MsgBoxImage.OK, MsgBoxResult.OK);
        }

        //private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    //progressBar.Value = e.ProgressPercentage;

        //    Console.WriteLine(e.ProgressPercentage);
        //}

        private void CloseForm()
        {
            if (Closed != null)
            {                

                Closed(1);
            }
        }

        private bool CanExecute(object parameter)
        {
            return true;
        }

        private void DownloadDocuments(object parameter)
        {
            int index = FileUploadedList.IndexOf(parameter as FileUpload);
            if (index > -1 && index < FileUploadedList.Count)
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                dialog.Title = "Select a folder to download";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    //MessageBox.Show("You selected: " + dialog.FileName);                           
                
                    string fromFile = (FileUploadedList[index].FilePathTo + "/" + FileUploadedList[index].FileName).Replace("\\", "/");
                    string toFile = dialog.FileName + @"\" + FileUploadedList[index].FileName;
                    WebClient webClient = new WebClient();

                    if (!File.Exists(toFile))
                    {

                        webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                        //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                        webClient.DownloadFile(new Uri(fromFile), toFile);
                    }
                    else
                    {
                        if (Msg.Show("This file already exist in the folder" + System.Environment.NewLine + "Would you like to replace it with the same name? " + System.Environment.NewLine + "or NO to enter a new file name", "", MsgBoxButtons.YesNo, MsgBoxImage.Warning, MsgBoxResult.Yes) == MsgBoxResult.Yes)
                        {
                            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                            webClient.DownloadFile(new Uri(fromFile), dialog.FileName + @"\" + FileUploadedList[index].FileName);
                        }
                        else
                        {
                            string newFileName = Microsoft.VisualBasic.Interaction.InputBox("Enter new file name to save file " + FileUploadedList[index].FileName, "Enter File Name", "");

                            if (!string.IsNullOrWhiteSpace(newFileName))
                            {
                                FileInfo fi = new FileInfo(FileUploadedList[index].FileName);
                                string extn = fi.Extension;

                                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                                webClient.DownloadFile(new Uri(fromFile), dialog.FileName + @"\" + newFileName + extn);
                            }
                            else
                            {
                                Msg.Show("File name required", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                            }
                        }
                    }
                
                    CloseForm();
                }
            }
        }

        public ObservableCollection<FileUpload> FileUploadedList
        {
            get
            {
                return _fileUploadedList;
            }
            set
            {
                _fileUploadedList = value;
                RaisePropertyChanged(() => this.FileUploadedList);
            }
        }

        public string MachineName
        {
            get
            {
                return _machineName;
            }
            set
            {
                _machineName = value;
                RaisePropertyChanged(() => this.MachineName);
            }
        }

        public Int64 WorkOrderNo
        {
            get
            {
                return _workOrderNo;
            }
            set
            {
                _workOrderNo = value;
                RaisePropertyChanged(() => this.WorkOrderNo);
            }
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }


        public ICommand DownloadAllCommand
        {
            get
            {
                return _downloadAllCommand ?? (_downloadAllCommand = new Commands.LogOutCommandHandler(() => DownloadAllFiles(), true));
            }
        }

        public ICommand DownloadDocumentCommand
        {
            get
            {
                if (_downloadDocumentCommand == null)
                {
                    _downloadDocumentCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, DownloadDocuments);
                }
                return _downloadDocumentCommand;
            }
        }
    }
}
