
using A1QSystem.Commands;
using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.Machine;
using A1QSystem.Model.Meta;
using A1QSystem.Model.Orders;
using A1QSystem.Model.Other;
using A1QSystem.View;
using Microsoft.Office.Interop.Excel;
using Microsoft.Practices.Prism.Commands;
using MsgBox;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace A1QSystem.ViewModel.Machine.MachineHistory
{
    public class FileUploadViewModel : ViewModelBase
    {
        private string pathFrom ; // this is the path that you are checking.
        private string pathTo;
        private ObservableCollection<FileUpload> _fileUploadedList;
        private string userName;
        private int machineID;
        private Int64 workOrderNo;
        
        private List<MetaData> metaData;
        public event System.Action<int> Closed;
        private DelegateCommand _closeCommand;
        private ICommand _addNewRowCommand;
        private ICommand _uploadAndCloseCommand;
        private ICommand _command;
        private ICommand _uploadDocumentCommand;
        public FileUploadViewModel(string un, int mid, Int64 wod)
        {
            FileUploadedList = new ObservableCollection<FileUpload>();
            userName = un;
            machineID = mid;
            workOrderNo = wod;
            metaData = DBAccess.GetMetaData();            
            
            if (metaData != null)
            {
                var data1 = metaData.FirstOrDefault(x => x.KeyName.Equals("machine_file_upload_location"));
                if (data1 != null)
                {
                    pathFrom = data1.Description.Replace("\\\\", @"\");
                }

                var data2 = metaData.FirstOrDefault(x => x.KeyName.Equals("machine_download_location"));
                if (data2 != null)
                {
                    pathTo = data2.Description.Replace("\\\\", @"\");
                }
            }

            _closeCommand = new DelegateCommand(CloseForm);
        }
                
        private void UploadAndClose()
        {

            if (FileUploadedList == null || FileUploadedList.Count == 0)
            {
                Msg.Show("Enter description and upload files", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if(FileUploadedList.Any(x=> string.IsNullOrWhiteSpace(x.Description)))
            {
                Msg.Show("Please enter description", "Description Required", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
            }
            else if (FileUploadedList.Any(x => string.IsNullOrWhiteSpace(x.FileName)))
            {
                var data = FileUploadedList.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.FileName));
                if (data != null)
                {
                    Msg.Show("Upload file for the description " + data.Description, "Upload File", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
            }
            else
            {
                bool y = false;
                List<FileUpload> newList = FileUploadedList.Distinct(new FileUploadComparer()).ToList();

                if (newList.Count != FileUploadedList.Count)
                {
                    y = true;
                }

                if (y == true)
                {
                    Msg.Show("Duplicate descriptions exist. Please change the description", "Duplicate Descriptions", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }
                else
                {

                    BackgroundWorker worker = new BackgroundWorker();
                    ChildWindowView LoadingScreen;
                    LoadingScreen = new ChildWindowView();
                    LoadingScreen.ShowWaitingScreen("Uploading");
                    int errNo = 0;                    

                    worker.DoWork += (_, __) =>
                    {
                        if (FileUploadedList.Count > 0)
                        {                            
                            foreach (var item in FileUploadedList)
                            {
                                string name = Path.GetFileName(item.FileName);

                                try
                                {
                                    File.Copy(item.FilePathFrom, pathTo + @"\" + item.FileName);
                                }
                                catch ( IOException rEx)
                                {                                    
                                    errNo = 1;                                    
                                }
                            }

                            if (errNo == 0)
                            {
                                DBAccess.InsertToUploadFiles(userName, machineID, workOrderNo, FileUploadedList);
                            }
                        }

                    };

                    worker.RunWorkerCompleted += delegate (object s, RunWorkerCompletedEventArgs args)
                    {
                        LoadingScreen.CloseWaitingScreen();

                        if (errNo == 0)
                        {
                            CloseForm();
                        }
                        else if(errNo == 1)
                        {
                            Msg.Show("Could not access the server location to upload file" + System.Environment.NewLine + "The file path not found!", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);

                        }
                    };
                    worker.RunWorkerAsync();
                }
            }
        }

        private void CloseForm()
        {
            if (Closed != null)
            {
                FileUploadedList.Clear();

                Closed(1);
            }
        }

        private void AddNewRow()
        {
            if (FileUploadedList != null)
            {
                bool has = FileUploadedList.Any(x => string.IsNullOrWhiteSpace(x.Description));
                if (has == false)
                {
                    FileUpload od = null;
                    if (FileUploadedList.Count > 0)
                    {
                        od = FileUploadedList[FileUploadedList.Count - 1];
                    }

                    FileUploadedList.Add(new FileUpload() { Description = String.Empty });
                }
            }
        }

        private void RemoveRecod(object parameter)
        {
            int index = FileUploadedList.IndexOf(parameter as FileUpload);
            if (index > -1 && index < FileUploadedList.Count)
            {
                FileUploadedList.RemoveAt(index);
               
            }
        }

        private void UploadDocuments(object parameter)
        {
            int index = FileUploadedList.IndexOf(parameter as FileUpload);
            if (index > -1 && index < FileUploadedList.Count)
            {
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

                if (Directory.Exists(pathFrom))
                {
                    openFileDialog.InitialDirectory = pathFrom;
                }
                else
                {
                    openFileDialog.InitialDirectory = @"C:\";
                }

                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = openFileDialog.FileName;
                    if (File.Exists(filename))
                    {
                        string name = Path.GetFileName(filename);
                        string destinationFilename = Path.Combine(pathTo, name);

                        var filenNme = Path.GetFileName(name);

                        if (!File.Exists(pathTo + @"\" + filenNme))
                        {

                            FileUploadedList[index].FilePathFrom = filename;
                            FileUploadedList[index].FileName = name;
                            FileUploadedList[index].UploadedDateTime = DateTime.Now;
                            FileUploadedList[index].UploadedBy = userName;    
                        }
                        else
                        {
                            Msg.Show("This file already exists", "", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                        }
                    }
                }


                FileUploadedList[index].FilePathTo = pathTo;
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

        private bool CanExecute(object parameter)
        {
            return true;
        }

        public DelegateCommand CloseCommand
        {
            get { return _closeCommand; }
        }

        
        public ICommand UploadAndCloseCommand
        {
            get
            {
                return _uploadAndCloseCommand ?? (_uploadAndCloseCommand = new A1QSystem.Commands.LogOutCommandHandler(() => UploadAndClose(), true));
            }
        }


        public ICommand AddNewRowCommand
        {
            get
            {
                return _addNewRowCommand ?? (_addNewRowCommand = new LogOutCommandHandler(() => AddNewRow(), true));
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                if (_command == null)
                {
                    _command = new A1QSystem.Commands.DelegateCommand(CanExecute, RemoveRecod);
                }
                return _command;
            }
        }

        public ICommand UploadDocumentCommand
        {
            get
            {
                if (_uploadDocumentCommand == null)
                {
                    _uploadDocumentCommand = new A1QSystem.Commands.DelegateCommand(CanExecute, UploadDocuments);
                }
                return _uploadDocumentCommand;
            }
        }
        
    }
}
