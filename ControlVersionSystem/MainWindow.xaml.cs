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
        

        private void button1_Click(object sender, RoutedEventArgs e) //кнопка ДОБАВИТЬ
        {
            OpenFileDialog _openFileDialog1 = new OpenFileDialog();
            if (_openFileDialog1.ShowDialog() == true)
            {
                MessageBox.Show(_openFileDialog1.FileName);
            }
            string _fullName = _openFileDialog1.FileName;
            string _fileName=System.IO.Path.GetFileName(_fullName);
            string _sourcePath = SOURCEPATH; 
            var _dlg = new System.Windows.Forms.FolderBrowserDialog();
            _dlg.ShowDialog();
            string _targetPath = _dlg.SelectedPath;
            string _sourceFile = System.IO.Path.Combine(_sourcePath, _fileName);
            string _destFile = System.IO.Path.Combine(_targetPath, _fileName);

            if (!System.IO.Directory.Exists(_targetPath))
            {
                System.IO.Directory.CreateDirectory(_targetPath);
            }

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
            }
            else
            {
                MessageBox.Show("Такой путь не существует!", "Ошибка!", MessageBoxButton.OK);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e) //кнопка добавления новой версии
        {
            OpenFileDialog _openFileDialog2 = new OpenFileDialog();
            if (_openFileDialog2.ShowDialog() == true)
            {
                MessageBox.Show(_openFileDialog2.FileName);
            }
            string _fullName = _openFileDialog2.FileName;
            string _fileName = System.IO.Path.GetFileName(_fullName);
            string _sourcePath = SOURCEPATH; // @"D:\_builds\прочее";
            var _d = new System.Windows.Forms.FolderBrowserDialog();
            _d.ShowDialog();
            string _targetPath = _d.SelectedPath;  // @"D:\_builds\1\files";
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

        private void button4_Click(object sender, RoutedEventArgs e) // кнопка добавления новой сборки
        {
            string _activeDir = DIR;                               //указание активной папки
            string _newPath = System.IO.Path.Combine(_activeDir, textBox1.Text); //подпапка для сборки
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
            listView1.UpdateLayout();
            listView1.Items.Clear();
            textBox2.Text = "";
            if (pathToFile == "") //если значение пути пустое
            {
                string[] _folders = System.IO.Directory.GetDirectories(DIR);
                foreach (string _fldFull in _folders)
                {
                    string _fld = System.IO.Path.GetFileName(_fldFull);
                    listView1.Items.Add(_fld);
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
                        listView1.Items.Add(_dirname);
                    }
                    //получаем список файлов
                    string[] _files = System.IO.Directory.GetFiles(pathToFile);
                    foreach (var _s in _files)
                    {
                        string _filename = System.IO.Path.GetFileName(_s);
                        listView1.Items.Add(_filename);
                    }
                }
                catch
                {
                    MessageBox.Show("Error", "");
                }
            }
            //конец обновления
            listView1.UpdateLayout();
        }

        void getUpdateListFS() //обновленный список сборок после добавления новой сборки
        {
            listView1.UpdateLayout();
            listView1.Items.Clear();
            listView2.UpdateLayout();
            listView2.Items.Clear();
            textBox2.Text = "";
            string[] _folders = System.IO.Directory.GetDirectories(DIR);
            foreach (string _fldFull in _folders)
            {
                string _fld = System.IO.Path.GetFileName(_fldFull);
                listView1.Items.Add(_fld);
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

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listView2.UpdateLayout();
            listView2.Items.Clear();
            textBox2.Text = "";

            try
            {
                if (listView1.SelectedItems.Count == 0)
                    return;
                item = listView1.SelectedItem as string;
                pathToFile = DIR+"\\" + item.ToString() + "\\files";
                string[] _folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
                foreach (string _fldFull in _folders)
                {
                    string _fld = System.IO.Path.GetFileName(_fldFull);
                    listView2.Items.Add(_fld);
                }
                listView2.UpdateLayout();
                try
                {
                    readLogFile();
                }
                catch
                {
                    MessageBox.Show("Ошибка!Файл не возможно отобразить!", "");
                }
            }
            catch
            {
                MessageBox.Show("Ошибка!Необходимо выбрать сборку!", ""); 
            }
        }

        private void readLogFile()
        {
            string _selFld = listView1.SelectedItem as string;
            string _text = System.IO.File.ReadAllText(DIR+"\\"+_selFld+"\\changelog");
            var _list=new List<VersionText>();
            var _pair=new VersionText(){ VersName="",VersText=""};
            bool flag=false;
            bool firstEquals=false; //первое вхождение "===="
            foreach (var _line in Tools.ReadFrom(DIR+"\\"+_selFld+"\\changelog"))
            {
                if (_line=="====")
                {
                    if (flag)
                    {
                        _list.Add(_pair);
                    }                 
                    _pair=new VersionText() { VersName="",VersText=""};
                    firstEquals=false;                    
                }               
                else
                {
                    if (firstEquals)
                    {
                        _pair.VersText +="\n"+_line;
                    }
                    else
                    {
                        _pair.VersName+=_line;
                        firstEquals=true;
                    }                
                }
            }
            string _tmp=DIR+"\\"+_selFld+"\\changelog";
            textBox4.Text = _text;         
        }

        private void listView2_SelectionChanged(object sender, SelectionChangedEventArgs e) //изменение названия выбранной версии
        {
            listView2.UpdateLayout();
            if (listView2.SelectedItems.Count == 0)
                return;
            itemVers = listView2.SelectedItem as string;
            textBox2.Text = itemVers;
            //поиск в changelog по выбранной версии
            string _tmp = "====\r";
            string _tmpVer = "Версия " + itemVers;
            string _a = textBox4.Text;
            string[] _split = _a.Split(new Char[] { '\n' });
            bool flag = false;

            CommandBinding bind = new CommandBinding();
            
            

            
        }
           
        private void button5_Click(object sender, RoutedEventArgs e) //функция изменения названия версии
        {
            string _item = listView2.SelectedItem as string;
            pathToFile = DIR+"\\" + item.ToString() + "\\files\\" + itemVers.ToString();
            string _pathToFile2 = DIR+"\\" + item.ToString() + "\\files\\" + textBox2.Text;
            try
            {
                System.IO.File.Move(pathToFile, _pathToFile2);
                MessageBox.Show("Версия переименована!", "");
            }
            catch
            {
                MessageBox.Show("Не удалось!", "");
            }
            textBox2.Text = "";
            listView2.UpdateLayout();
            listView2.Items.Clear();
            textBox2.Text = "";
            pathToFile = DIR+"\\" + item.ToString() + "\\files";

            string[] _folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
            foreach (string _fldFull in _folders)
            {
                string _fld = System.IO.Path.GetFileName(_fldFull);
                listView2.Items.Add(_fld);
            }
            listView2.UpdateLayout();           
        }

        private void textBox4_TextChanged(object sender, TextChangedEventArgs e) //если что-то поменяли в текстбоксе, то записываем в логфайл
        {
            try
            {
                string _selFld = listView1.SelectedItem as string;
                string _tmp = @"D:\_builds\\" + _selFld + "\\changelog";
                string _changeText = textBox4.Text;
                System.IO.File.WriteAllText(_tmp, _changeText);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить изменения в changlog!", "");
            }
        }



        

        //private void button6_Click(object sender, RoutedEventArgs e) // для кнопки НАЗАД
        //{
        //    if (pathToFile == "")
        //    {
        //        MessageBox.Show("Вы находитесь в корне файловой системы", "0_о");
        //    }
        //    else
        //    {
        //        try
        //        {
        //            System.IO.DirectoryInfo dirinfo = System.IO.Directory.GetParent(pathToFile);
        //            pathToFile = dirinfo.ToString();
        //        }
        //        catch (NullReferenceException)
        //        {
        //            pathToFile = "";
        //        }
        //        getListFS();
        //    }
        //}

        
    }
}
