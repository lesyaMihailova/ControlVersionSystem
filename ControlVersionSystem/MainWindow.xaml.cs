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
using System.Xml.Linq;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public string folderName;
        string itemVers; //для версии
        string DIR = "";
        string textTemplate = @"template.xml";
        string[] TextTemplate = {"Включает сборки:", "Изменения:", "     +", "     *", "     -", "Группа изменений", "     +", "     *", "     -", "Примечания", "     *" };

        public MainWindow()
        {
            InitializeComponent();
            string _fileName = @"config.xml";
            DIR=readConfig(_fileName);
            //readTemplate(textTemplate);
            checkDirectoryExist(DIR);
            getListFS();          
        }

        public ObservableCollection<VersionText> VersionList
        {
            get { return (ObservableCollection<VersionText>)GetValue(VersionListProperty); }
            set { SetValue(VersionListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VersionList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VersionListProperty =
            DependencyProperty.Register("VersionList", typeof(ObservableCollection<VersionText>), typeof(MainWindow), new UIPropertyMetadata(null));

        public ObservableCollection<string> AssemblyList
        {
            get { return (ObservableCollection<string>)GetValue(AssemblyListProperty); }
            set { SetValue(AssemblyListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssemblyList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssemblyListProperty =
            DependencyProperty.Register("AssemblyList", typeof(ObservableCollection<string>), typeof(MainWindow), new UIPropertyMetadata(null));

        private string readConfig(string fileName)
        {
            string _s = "";
            string _fileName = fileName;
            XmlDocument xd = new XmlDocument();
            FileStream fs = new FileStream(_fileName, FileMode.Open);
            xd.Load(fs);

            foreach (XmlNode _node in xd.DocumentElement.ChildNodes)
            {
                foreach (XmlAttribute attr in _node.Attributes)
                {
                    _s = attr.Value;
                }
            }
            return _s;
        }

        void readTemplate(string path)
        {
            string _path = path;
            XmlDocument _doc = new XmlDocument();
            FileStream fs = new FileStream(_path, FileMode.Open);
            _doc.Load(fs);
            foreach (XmlNode _tml in _doc.DocumentElement.ChildNodes)
            {
                foreach (XmlAttribute _attr in _tml.Attributes)
                {
                    string _s = _attr.Name + ":" + _attr.Value;
                }
                foreach (XmlNode ch in _tml.ChildNodes)
                {
                    string _s1 = ch.Name + "-" + ch.Value;
                }
                MessageBox.Show(_tml.InnerText);
            }
        }

        void checkDirectoryExist(string target)
        {
            bool _create = false;
            if (Directory.Exists(target) == false)
            {
                var _mbResult = MessageBox.Show("Директория _builds не существует! Создать?", "Ошибка", MessageBoxButton.YesNo, MessageBoxImage.Error);
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
                    string _fullName = _openFileDialog2.FileName;
                    var _fileName = System.IO.Path.GetFileName(_fullName);
                    bool _overwrite = false;

                    string _targetPath = System.IO.Path.Combine(DIR, v_listViewSbor.SelectedItem.ToString(), "files", _fileName);
                    string _sourceFile = _fullName;
                    if (System.IO.Directory.Exists(_targetPath))
                    {
                        var _mbResult = MessageBox.Show("Файл уже существует! Заменить? ", " ", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        _overwrite = _mbResult == MessageBoxResult.Yes;
                    }
                    try
                    {
                        System.IO.File.Copy(_sourceFile, _targetPath, _overwrite);

                        for (var i = 0; i < TextTemplate.Length; i++)
                        {
                            v_tbDisplayFile.AppendText(TextTemplate[i] + Environment.NewLine);
                        }
                        VersionList.Add(new VersionText() { VersName = "Версия " + v_tbVersName.Text, VersText = v_tbDisplayFile.Text });
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
                        getListFS();
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString(), " ", MessageBoxButton.OK, MessageBoxImage.Error);
                getListFS();
            }
        }

        void getListFS(object sender, RoutedEventArgs e) // получение списка папок заданной директории
        {
            v_tbVersName.Text = "";
            var _aslist = new ObservableCollection<string>();

            string[] _folders = System.IO.Directory.GetDirectories(DIR);
            foreach (string _fldFull in _folders)
            {
                string _fld = System.IO.Path.GetFileName(_fldFull);
                _aslist.Add(_fld);
            }
            AssemblyList = _aslist;
        }

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

        private void getListFS()
        {
        }       
    }
}
