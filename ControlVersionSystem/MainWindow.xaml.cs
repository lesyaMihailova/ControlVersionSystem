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
using System.Collections.ObjectModel;
using System.IO;

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
            checkDirectoryExist(DIR);
            getListFS();

        }
        string pathToFile = "";
        string item; //для сборки
        string itemVers; //для версии

        public ObservableCollection<VersionText> VersionList
        {
            get { return (ObservableCollection<VersionText>)GetValue(VersionListProperty); }
            set { SetValue(VersionListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionListProperty =
            DependencyProperty.Register("VersionList", typeof(ObservableCollection<VersionText>), typeof(MainWindow), new UIPropertyMetadata(null));

        void checkDirectoryExist(string target)
        {
            bool _create = false;
            if (Directory.Exists(target) == false)
            {
                var _mbResult=MessageBox.Show("Директория _builds не существует! Создать?", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error);
                _create = _mbResult == MessageBoxResult.Yes;
                try
                {
                    Directory.CreateDirectory(DIR);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void v_btnAddNewVers_Click(object sender, RoutedEventArgs e) //кнопка добавления новой версии
        {
            if (v_listViewSbor.SelectedItem != null)
            {

                OpenFileDialog _openFileDialog2 = new OpenFileDialog();
                if (_openFileDialog2.ShowDialog() == true)
                {
                    //MessageBox.Show(_openFileDialog2.FileName);
                    string _fullName = _openFileDialog2.FileName;
                    var _fileName = System.IO.Path.GetFileName(_fullName);
                    //string _sourcePath = SOURCEPATH;
                    bool _overwrite = false;
                    //var _d = new System.Windows.Forms.FolderBrowserDialog();
                    //_d.ShowDialog();

                    string _targetPath = System.IO.Path.Combine(DIR, v_listViewSbor.SelectedItem.ToString(), "files", _fileName);
                    string _sourceFile = _fullName;
                    // string _sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
                    //string _destFile = System.IO.Path.Combine(_targetPath, _fileName);
                    if (System.IO.Directory.Exists(_targetPath))
                    {
                        var _mbResult = MessageBox.Show("Файл уже существует! Заменить? ", " ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        _overwrite = _mbResult == MessageBoxResult.Yes;
                    }
                    try
                    {
                        System.IO.File.Copy(_sourceFile, _targetPath, _overwrite);
                        VersionList.Add(new VersionText() { VersName = "Версия " + v_tbVersName.Text, VersText = "" });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString(), "Произошла ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        private void v_btnAddSbor_Click(object sender, RoutedEventArgs e) // кнопка добавления новой сборки
        {
            string _activeDir = DIR;                               //указание активной папки
            if (v_tbSborName.Text == "")
            {
                MessageBox.Show("Не указано название новой сборки!", " ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            string _newPath = System.IO.Path.Combine(_activeDir, v_tbSborName.Text); //подпапка для сборки
            try
            {
                if (v_tbSborName.Text != "")
                {
                    System.IO.Directory.CreateDirectory(_newPath);                   //создание подпапки для сборки
                    string _newPath1 = System.IO.Path.Combine(_newPath, "files");     //подпапка "files" папки "сборка N" 
                    System.IO.Directory.CreateDirectory(_newPath1);                  //создание подпапки files
                    string _newFileName = "changelog";
                    _newPath = System.IO.Path.Combine(_newPath, _newFileName);
                    System.IO.File.Create(_newPath).Close();

                    if (System.IO.Directory.Exists(_newPath1))
                    {
                        MessageBox.Show("Новые папки созданы!", " ", MessageBoxButton.OK);
                        getUpdateListFS();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Новые папки добавлены не корректно!Повторите попытку!", " ", MessageBoxButton.OK, MessageBoxImage.Error);
                getUpdateListFS();
            }
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
            v_listViewSbor.UpdateLayout();
            v_listViewVers.UpdateLayout();
        }

        //void addElemInDictionary()
        //{
        //    //добавление в словарь сброки и соответствующего логфайла
        //    string[] _fldl = System.IO.Directory.GetDirectories(DIR);
        //    foreach (string _f in _fldl)
        //    {
        //        string _fldname = System.IO.Path.GetFileName(_f);
        //        string _logFileName = "changelog";
        //        openLogFile.Add(_fldname, _logFileName);
        //    }
        //}

        private void v_listViewSbor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            readLogFile();
        }

        private void readLogFile()
        {
            string _selFld = v_listViewSbor.SelectedItem as string;
            var _list = new ObservableCollection<VersionText>();
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
                        _pair.VersText += _line + "\n";
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
            try
            {
                if (v_listViewVers.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите версию для изменения названия!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    if (v_tbVersName.Text != "")
                    {
                        string _txt = v_tbVersName.Text;
                        var _curVers = v_listViewVers.SelectedItem as VersionText;
                        _curVers.VersName = _txt;
                    }
                    else
                    {
                        MessageBox.Show("Введите новое название версии!", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Не удалось изменить название версии!", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    _changeText += _s.VersName + Environment.NewLine;
                    _changeText += _s.VersText;// +Environment.NewLine;
                }
                System.IO.File.WriteAllText(_tmp, _changeText);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить изменения в changlog!", "");
            }

        }

        private void v_btnChangeVersName_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}
