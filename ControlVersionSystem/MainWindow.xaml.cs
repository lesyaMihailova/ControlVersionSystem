using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string folderName;
        const string SOURCEPATH = @"D:\_builds\прочее";
        const string DIR = @"D:\_builds";
        Dictionary<string, string> openLogFile = new Dictionary<string, string>();
            
        public MainWindow()
        {
            InitializeComponent();
            getListFS();
            addElemInDictionary();
        }
        string pathToFile = "";
        string item; //для сборки
        string itemVers; //для версии

        public VersionText SelectedVersion
        {
            get { return (VersionText)GetValue(SelectedVersionProperty); }
            set { SetValue(SelectedVersionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedVersion.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedVersionProperty =
            DependencyProperty.Register("SelectedVersion", typeof(VersionText), typeof(MainWindow), new UIPropertyMetadata(null));


        public List<VersionText> VersionList
        {
            get { return (List<VersionText>)GetValue(VersionListProperty); }
            set { SetValue(VersionListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionListProperty =
            DependencyProperty.Register("VersionList", typeof(List<VersionText>), typeof(MainWindow), new UIPropertyMetadata(null));


        private void v_btnAddNewVers_Click(object sender, RoutedEventArgs e) //кнопка добавления новой версии
        {
            OpenFileDialog _openFileDialog2 = new OpenFileDialog();
            if (_openFileDialog2.ShowDialog() == true)
            {
                MessageBox.Show(_openFileDialog2.FileName);
            }
            string _fullName = _openFileDialog2.FileName;
            string _fileName = System.IO.Path.GetFileName(_fullName);
            string _sourcePath = SOURCEPATH; 
            var _d = new System.Windows.Forms.FolderBrowserDialog();
            _d.ShowDialog();
            string _targetPath = _d.SelectedPath;  
            string _sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
            string _destFile = System.IO.Path.Combine(_targetPath, _fileName);
            System.IO.File.Copy(_sourceFile, _destFile, true);

            if (System.IO.Directory.Exists(_sourcePath))
            {
                string[] _files = System.IO.Directory.GetFiles(_sourcePath);
                foreach (string _s in _files)
                {
                    _fullName = System.IO.Path.GetFileName(_s);
                    _destFile = System.IO.Path.Combine(_targetPath, _fullName);
                    System.IO.File.Copy(_s, _destFile, true);
                }
                MessageBox.Show("Сборки успешно добавлены!", " ", MessageBoxButton.OK);
                getUpdateListFS();
            }          
        }

        private void v_btnAddSbor_Click(object sender, RoutedEventArgs e) // кнопка добавления новой сборки
        {
            string _activeDir = DIR;                               //указание активной папки
            string _newPath = System.IO.Path.Combine(_activeDir, v_tbSborName.Text); //подпапка для сборки
            System.IO.Directory.CreateDirectory(_newPath);                   //создание подпапки для сборки
            string _newPath1 = System.IO.Path.Combine(_newPath, "files");     //подпапка "files" папки "сборка N" 
            System.IO.Directory.CreateDirectory(_newPath1);                  //создание подпапки files
            if (System.IO.Directory.Exists(_newPath1))
            {
                MessageBox.Show("Новые папки созданы!"," ",MessageBoxButton.OK);
                getUpdateListFS();
            }
            string _newFileName = "changelog";    
            _newPath = System.IO.Path.Combine(_newPath, _newFileName);          
        }

        void getListFS() // получение списка папок заданной директории
        {
            v_listViewSbor.UpdateLayout();
            v_listViewSbor.Items.Clear();
            v_tbVersName.Text = "";
            if (pathToFile == "") //если значение пути пустое
            {
                string[] _folders = System.IO.Directory.GetDirectories(DIR);
                foreach (string _fldFull in _folders)
                {
                    string _fld = System.IO.Path.GetFileName(_fldFull);
                    v_listViewSbor.Items.Add(_fld);
                }
            }
            else
            {
                try
                {
                    //получаем список директорий
                    string[] _dirs = System.IO.Directory.GetDirectories(pathToFile);
                    foreach (string _s in _dirs)
                    {
                        string _dirname = System.IO.Path.GetFileName(_s);
                        v_listViewSbor.Items.Add(_dirname);
                    }
                    //получаем список файлов
                    string[] _files = System.IO.Directory.GetFiles(pathToFile);
                    foreach (var _s in _files)
                    {
                        string _filename = System.IO.Path.GetFileName(_s);
                        v_listViewSbor.Items.Add(_filename);
                    }
                }
                catch
                {
                    MessageBox.Show("Error", "");
                }
            }
            //конец обновления
            v_listViewSbor.UpdateLayout();
        }

        void getUpdateListFS() //обновленный список сборок после добавления новой сборки
        {
            v_listViewSbor.UpdateLayout();
            v_listViewSbor.Items.Clear();
            v_listViewVers.UpdateLayout();
            v_listViewVers.Items.Clear();
            v_tbVersName.Text = "";
            string[] _folders = System.IO.Directory.GetDirectories(DIR);
            foreach (string _fldFull in _folders)
            {
                string _fld = System.IO.Path.GetFileName(_fldFull);
                v_listViewSbor.Items.Add(_fld);
            }
        }

        void addElemInDictionary()
        {
            //добавление в словарь сброки и соответствующего логфайла
            string[] _fldl = System.IO.Directory.GetDirectories(DIR);
            foreach (string _f in _fldl)
            {
                string _fldname = System.IO.Path.GetFileName(_f);
                string _logFileName = "changelog";
                openLogFile.Add(_fldname, _logFileName);
            }
        }

        private void v_listViewSbor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //v_listViewVers.UpdateLayout();
            //v_listViewVers.Items.Clear();
            //v_tbVersName.Text = "";

            ////try
            ////{
            //    if (v_listViewSbor.SelectedItems.Count == 0)
            //        return;
            //    item = v_listViewSbor.SelectedItem as string;
            //    pathToFile = DIR+"\\" + item.ToString() + "\\files";
            //    string[] _folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
            //    //foreach (string _fldFull in _folders)
            //    //{
            //    //    string _fld = System.IO.Path.GetFileName(_fldFull);
            //    //    v_listViewVers.Items.Add(_fld);
            //    //}
            //    v_listViewVers.UpdateLayout();
            //    //try
            //    //{
                    readLogFile();
                //}
                //catch
                //{
                    //MessageBox.Show("Ошибка!Файл не возможно отобразить!", "");
            //    }
            //}
            //catch
            //{
            //    MessageBox.Show("Ошибка!Необходимо выбрать сборку!", "");
            //}
        }

        private void readLogFile()
        {
            string _selFld = v_listViewSbor.SelectedItem as string;
            //string _text = System.IO.File.ReadAllText(DIR+"\\"+_selFld+"\\changelog");
            var _list = new List<VersionText>();
            var _pair = new VersionText() { VersName = "", VersText = "" };
            bool flag = false;
            bool firstEquals = false; //первое вхождение "===="
            foreach (var _line in Tools.ReadFrom(DIR + "\\" + _selFld + "\\changelog"))
            {
                if (_line == "====")
                {
                    if (flag)
                    {
                        _list.Add(_pair);
                    }
                    _pair = new VersionText() { VersName = "", VersText = "" };
                    flag = true;
                    firstEquals = false;
                }
                else
                {
                    if (firstEquals)
                    {
                        _pair.VersText += "\n" + _line;
                    }
                    else
                    {
                        _pair.VersName += _line;
                        firstEquals = true;
                    }
                }
            }
            if (flag)
            {
                _list.Add(_pair);
            }
            string _tmp = DIR + "\\" + _selFld + "\\changelog";
            VersionList = _list;
        }

        private void v_listViewVers_SelectionChanged(object sender, SelectionChangedEventArgs e) //изменение названия выбранной версии
        {
            v_listViewVers.UpdateLayout();
            if (v_listViewVers.SelectedItems.Count == 0)
                return;
            itemVers = v_listViewVers.SelectedItem as string;
            v_tbVersName.Text = itemVers;    
        }
           
        private void v_btnChangeVersName_Click(object sender, RoutedEventArgs e) //функция изменения названия версии
        {
            string _item = v_listViewVers.SelectedItem as string;
            pathToFile = DIR+"\\" + item.ToString() + "\\files\\" + itemVers.ToString();
            string _pathToFile2 = DIR + "\\" + item.ToString() + "\\files\\" + v_tbVersName.Text;
            try
            {
                System.IO.File.Move(pathToFile, _pathToFile2);
                MessageBox.Show("Версия переименована!", "");
            }
            catch
            {
                MessageBox.Show("Не удалось!", "");
            }
            v_tbVersName.Text = "";
            v_listViewVers.UpdateLayout();
            v_listViewVers.Items.Clear();
            v_tbVersName.Text = "";
            pathToFile = DIR+"\\" + item.ToString() + "\\files";

            string[] _folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
            foreach (string _fldFull in _folders)
            {
                string _fld = System.IO.Path.GetFileName(_fldFull);
                v_listViewVers.Items.Add(_fld);
            }
            v_listViewVers.UpdateLayout();           
        }

        private void v_btnSaveTextChange_Click(object sender, RoutedEventArgs e)
        {
        }

        private void v_tbDisplayFile_TextChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                string _selFld = v_listViewSbor.SelectedItem as string;
                string _tmp = DIR + "\\" + _selFld + "\\changelog";
                string _changeText = "";
                var _tmpList = VersionList.ToArray();
                _tmpList.Reverse();
                foreach (var _s in _tmpList)
                {
                    _changeText += "====\r\n";
                    _changeText += _s.VersName;//+Environment.NewLine;
                    _changeText += _s.VersText + Environment.NewLine;

                }
                System.IO.File.WriteAllText(_tmp, _changeText);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить изменения в changlog!", "");
            }

        }
    }
}
