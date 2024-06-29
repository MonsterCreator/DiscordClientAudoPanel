using Avalonia.Controls;
using DynamicData;
using NAudio.Wave;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace DiscordClientAudoPanel.Views
{
    public partial class MainView : UserControl
    {
        public string localPath = "";

        public MainView()
        {
            InitializeComponent();
            localPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            LoadAudiofiles();
        }

        private void LoadAudiofiles()
        {
            string[] fileArray = Directory.GetFiles($@"{localPath}\\Audio", "*.mp3");
            addStackPanelElement(fileArray);
        }

        private void addStackPanelElement(string[] fileArray)
        {
            foreach (string filePath in fileArray)
            {
                // Получаем имя файла без расширения
                string fullName = Path.GetFileNameWithoutExtension(filePath);

                var file = new FileInfo(filePath, FileInfo.GetName(fullName), FileInfo.GetFileName(fullName));

                // Добавляем имя файла в элемент StackPanel
                FilesPanel.Children.Add(getCutomPanel(file));
            }
        }

        private StackPanel getCutomPanel(FileInfo file)
        {
            var panel = new StackPanel() { Orientation = Avalonia.Layout.Orientation.Horizontal };

            var localButton = new Button { Content = "►", Tag = file.filePath };

            var discordButton = new Button { Content = "Discord Play", Tag = file.filePath };

            localButton.Click += (sender, e) =>
            {
                var clickedButton = (Button)sender;
                string clickedFilePath = clickedButton.Tag.ToString();
                LocalAudioPlayAsync(clickedFilePath, localButton);
            };

            discordButton.Click += (sender, e) =>
            {
                var clickedButton = (Button)sender;
                PlayInDiscord(file);
            };

            panel.Children.Add(localButton);
            panel.Children.Add(discordButton);
            panel.Children.Add(new Label() { Content = file.name, Name = $"{file.fileName}" });
            return panel;
        }

        private async Task LocalAudioPlayAsync(string filePath, Button button)
        {
            var reader = new Mp3FileReader(filePath);
            var waveOut = new WaveOutEvent();
            waveOut.Init(reader);
            waveOut.Play();
        }

        private async void PlayInDiscord(FileInfo file)
        {
            label1.Content = "fuck";
            await Task.Run(() =>
            {
                
                // Замените на IP-адрес и порт вашего бота
                string serverIp = "217.20.75.230";
                int serverPort = 9007;

                // Создаем TcpClient для подключения к серверу
                TcpClient client = new TcpClient(serverIp, serverPort);

                // Получаем поток для чтения и записи данных
                NetworkStream stream = client.GetStream();

                // Строка, которую мы хотим отправить
                string dataToSend = file.fileName;

                // Преобразуем строку в байты
                byte[] dataBytes = Encoding.ASCII.GetBytes(dataToSend);

                // Отправляем данные
                stream.Write(dataBytes, 0, dataBytes.Length);

                Console.WriteLine("Данные отправлены!");

                // Закрываем соединение
                stream.Close();
                client.Close();
            });
        }



    }

    public class FileInfo
    {
        public FileInfo(string _filePath, string _fileName, string _name)
        {
            filePath = _filePath;
            fileName = _fileName;
            name = _name;
        }

        public string fileName { get; set; }
        public string name { get; set; }
        public string filePath { get; set; }

        public static string GetName(string fullName)
        {
            return fullName.Split('+')[0];
        }

        public static string GetFileName(string fullName)
        {
            return fullName.Split('+')[1];
        }
    }


}

