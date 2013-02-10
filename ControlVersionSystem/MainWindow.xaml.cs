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
        public MainWindow()
        {
            InitializeComponent();
            getListFS();        
        }
        string pathToFile = "";
        string item; //для сборки
        string itemVers; //для версии

        private void button1_Click(object sender, RoutedEventArgs e) //кнопка ДОБАВИТЬ
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == true)
            {
                MessageBox.Show(openFileDialog1.FileName);
            }
            string _fullName = openFileDialog1.FileName;
            string _fileName=System.IO.Path.GetFileName(_fullName);
            string sourcePath = @"D:\_builds\прочее";
            var _dlg = new System.Windows.Forms.FolderBrowserDialog();
            _dlg.ShowDialog();
            string targetPath = _dlg.SelectedPath;

            //string targetPath = @"D:\_builds\сборка2";
            string sourceFile = System.IO.Path.Combine(sourcePath, _fileName);
            string destFile = System.IO.Path.Combine(targetPath, _fileName);

            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }

            System.IO.File.Copy(sourceFile, destFile, true);

            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);
                foreach (string s in files)
                {
                    _fullName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, _fullName);
                    System.IO.File.Copy(s, destFile, true);
                }
            }
            else
            {
                MessageBox.Show("Такой путь не существует!", "Ошибка!", MessageBoxButton.OK);
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e) //кнопка добавления новой версии
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == true)
            {
                MessageBox.Show(openFileDialog2.FileName);
            }
            string _fullName = openFileDialog2.FileName;
            string _fileName = System.IO.Path.GetFileName(_fullName);
            string sourcePath = @"D:\_builds\прочее";

            var _d = new System.Windows.Forms.FolderBrowserDialog();
            _d.ShowDialog();
            string targetPath = _d.SelectedPath;  // @"D:\_builds\1\files";

            string sourceFile = System.IO.Path.Combine(sourcePath, _fileName);
            string destFile = System.IO.Path.Combine(targetPath, _fileName);

            System.IO.File.Copy(sourceFile, destFile, true);

            if (System.IO.Directory.Exists(sourcePath))
            {
                string[] files = System.IO.Directory.GetFiles(sourcePath);
                foreach (string s in files)
                {
                    _fullName = System.IO.Path.GetFileName(s);
                    destFile = System.IO.Path.Combine(targetPath, _fullName);
                    System.IO.File.Copy(s, destFile, true);
                }
                MessageBox.Show("Сборки успешно добавлены!", " ", MessageBoxButton.OK);
                getUpdateListFS();
            }
           
        }

        private void button4_Click(object sender, RoutedEventArgs e) // кнопка добавления новой сборки
        {
            string activeDir = @"D:\_builds";                               //указание активной папки
            string newPath = System.IO.Path.Combine(activeDir, textBox1.Text); //подпапка для сборки
            System.IO.Directory.CreateDirectory(newPath);                   //создание подпапки для сборки
            string newPath1 = System.IO.Path.Combine(newPath, "files");     //подпапка "files" папки "сборка N" 
            System.IO.Directory.CreateDirectory(newPath1);                  //создание подпапки files
            if (System.IO.Directory.Exists(newPath1))
            {
                MessageBox.Show("Новые папки созданы!"," ",MessageBoxButton.OK);
                getUpdateListFS();
            }
            string newFileName = System.IO.Path.GetRandomFileName();    //создание файла
            newPath = System.IO.Path.Combine(newPath, newFileName);     //создание нового пути для файла
            if (!System.IO.File.Exists(newPath))                        //создание и запись в файл
            {
                using (System.IO.FileStream fs = System.IO.File.Create(newPath))
                {
                    for (byte i = 0; i < 100; i++)
                    {
                        fs.WriteByte(i);
                    }
                }
            }
            

            //обзор папок
            //var _d = new System.Windows.Forms.FolderBrowserDialog();
            //_d.ShowDialog();
        }

        void getListFS() // получение списка папок заданной директории
        {
            listView1.UpdateLayout();
            listView1.Items.Clear();
            textBox2.Text = "";
            if (pathToFile == "") //если значение пути пустое
            {
                string[] folders = System.IO.Directory.GetDirectories(@"D:\_builds");
                //получаем имена логических дисков в компе              
                //string[] drivers = System.IO.Directory.GetLogicalDrives();
                foreach (string fldFull in folders)
                {
                    string fld = System.IO.Path.GetFileName(fldFull);
                    listView1.Items.Add(fld);
                }
            }
            else
            {
                try
                {
                    //получаем список директорий
                    string[] dirs = System.IO.Directory.GetDirectories(pathToFile);
                    foreach (string s in dirs)
                    {
                        string dirname = System.IO.Path.GetFileName(s);
                        listView1.Items.Add(dirname);
                    }
                    //получаем список файлов
                    string[] files = System.IO.Directory.GetFiles(pathToFile);
                    foreach (var s in files)
                    {
                        string filename = System.IO.Path.GetFileName(s);
                        listView1.Items.Add(filename);
                    }
                }
                catch
                {
                    MessageBox.Show("Error", "");
                }
            }
            //выводим полный путь к директории в которой находимся
            label5.Content=pathToFile;
            //конец обновления
            listView1.UpdateLayout();// EndUpdate();
        }

        void getUpdateListFS() //обновленный список сборок после добавления новой сборки
        {
            listView1.UpdateLayout();
            listView1.Items.Clear();
            listView2.UpdateLayout();
            listView2.Items.Clear();
            textBox2.Text = "";
            string[] folders = System.IO.Directory.GetDirectories(@"D:\_builds");
            //получаем имена логических дисков в компе              
            //string[] drivers = System.IO.Directory.GetLogicalDrives();
            foreach (string fldFull in folders)
            {
                string fld = System.IO.Path.GetFileName(fldFull);
                listView1.Items.Add(fld);
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
                pathToFile = @"D:\_builds\" + item.ToString() + "\\files";
                string[] folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
                foreach (string fldFull in folders)
                {
                    string fld = System.IO.Path.GetFileName(fldFull);
                    listView2.Items.Add(fld);
                }
                listView2.UpdateLayout();
            }
            catch
            {
                MessageBox.Show("Error!", ""); 
            }
        }

        private void listView2_SelectionChanged(object sender, SelectionChangedEventArgs e) //изменение названия выбранной версии
        {
            listView2.UpdateLayout();
            if (listView2.SelectedItems.Count == 0)
                return;
            itemVers = listView2.SelectedItem as string;
            textBox2.Text = itemVers;
        }

        private void button5_Click(object sender, RoutedEventArgs e) //функция изменения названия версии
        {
            string _item = listView2.SelectedItem as string;
            pathToFile = @"D:\_builds\" + item.ToString() + "\\files\\" + itemVers.ToString();
            string _pathToFile2 = @"D:\_builds\" + item.ToString() + "\\files\\" + textBox2.Text;
            try
            {
                System.IO.File.Move(pathToFile, _pathToFile2);
                MessageBox.Show("Файл переименован!", "");
            }
            catch
            {
                MessageBox.Show("Не удалось!", "");
            }
            textBox2.Text = "";
            listView2.UpdateLayout();
            listView2.Items.Clear();
            textBox2.Text = "";
            pathToFile = @"D:\_builds\" + item.ToString() + "\\files";

            string[] folders = System.IO.Directory.GetFiles(pathToFile, "*.rar");
            foreach (string fldFull in folders)
            {
                string fld = System.IO.Path.GetFileName(fldFull);
                listView2.Items.Add(fld);
            }
            listView2.UpdateLayout();           
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
