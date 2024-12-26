using System;
using System.Windows.Forms;
using Syntec.OpenCNC; // Пространство имен для OpenCNC
using Syntec.RemoteCNC; // Пространство имен для RemoteCNC
using Syntec.RemoteObj; // Пространство имен для RemoteObj

namespace CNCControllerApp
{
    public partial class MillApiForm : Form, ICNCController
    {
        private RemoteCNC remoteCNC; // Объект для управления удалённым CNC
        private bool isConnected = false; // Флаг состояния подключения

        // Элементы управления
        private TextBox txtCommand; // Поле для ввода команды
        private Button btnSendCommand; // Кнопка для отправки команды
        private Button btnConnect; // Кнопка для подключения

        // Конструктор формы
        public MillApiForm()
        {
            InitializeComponent(); // Инициализация компонентов формы
            UpdateUI(); // Обновляем интерфейс при инициализации
            InitializePorts(); // Инициализация портов
        }

        // Метод для инициализации компонентов формы
        private void InitializeComponent()
        {
            this.txtCommand = new TextBox();
            this.btnSendCommand = new Button();
            this.btnConnect = new Button();

            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(12, 12);
            this.txtCommand.Size = new System.Drawing.Size(260, 20);
            this.txtCommand.TabIndex = 0;

            // 
            // btnSendCommand
            // 
            this.btnSendCommand.Location = new System.Drawing.Point(12, 38);
            this.btnSendCommand.Size = new System.Drawing.Size(75, 23);
            this.btnSendCommand.Text = "Отправить";
            this.btnSendCommand.Click += new EventHandler(btnSendCommand_Click);

            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(197, 38);
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.Text = "Подключиться";
            this.btnConnect.Click += new EventHandler(btnConnect_Click);

            // 
            // MillApiForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 81);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.btnSendCommand);
            this.Controls.Add(this.btnConnect);
            this.Text = "CNC Controller";
        }

        // Метод для инициализации портов
        private void InitializePorts()
        {
            try
            {
                remoteCNC = new RemoteCNC();
                remoteCNC.InitializePorts("A1"); // Предположим, что есть такой метод
                MessageBox.Show("Порты A1 успешно инициализированы.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации портов: {ex.Message}");
            }
        }

        // Обработчик события нажатия кнопки подключения
        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (isConnected)
                {
                    MessageBox.Show("Вы уже подключены.");
                    return;
                }

                remoteCNC.Connect("192.168.1.100", 5568); // Укажите IP-адрес контроллера
                isConnected = true;
                MessageBox.Show("Подключение успешно");
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Неправильный формат IP-адреса: {ex.Message}");
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                MessageBox.Show($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}");
            }
            finally
            {
                UpdateUI(); // Обновляем интерфейс в любом случае
            }
        }

        // Обработчик события нажатия кнопки отправки команды
        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("Сначала подключитесь к контроллеру.");
                return;
            }

            string command = txtCommand.Text; // Получаем текст команды из текстового поля

            if (string.IsNullOrWhiteSpace(command))
            {
                MessageBox.Show("Введите команду для отправки.");
                return;
            }

            try
            {
                short returnValue = remoteCNC.SendCommand(command); // Предположим, что метод возвращает short
                HandleReturnValue(returnValue);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show($"Ошибка выполнения команды: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке команды: {ex.Message}");
            }
        }

        // Обработка возвращаемого значения после отправки команды
        private void HandleReturnValue(short returnValue)
        {
            switch (returnValue)
            {
                case -18:
                    MessageBox.Show("Ошибка: Not supported - Контроллер не поддерживает эту функцию.");
                    break;
                case -17:
                    MessageBox.Show("Ошибка: Protocol error - Данные от Ethernet платы неверные.");
                    break;
                case -16:
                    MessageBox.Show("Ошибка: Socket error - Проверьте питание CNC, Ethernet-кабель и I/F плату.");
                    break;
                case -15:
                    MessageBox.Show("Ошибка: DLL file error - Не найден DLL файл для каждой серии CNC.");
                    break;
                case -14:
                    MessageBox.Show("Ошибка: USBEmpty - USB-порт пуст.");
                    break;
                case -13:
                    MessageBox.Show("Ошибка: NoUSB - USB не подключен.");
                    break;
                case -8:
                    MessageBox.Show("Ошибка: Handle number error - Ошибка при получении номера дескриптора библиотеки.");
                    break;
                case -7:
                    MessageBox.Show("Ошибка: Version mismatch - Версия CNC/PMC не соответствует версии библиотеки.");
                    break;
                case -6:
                    MessageBox.Show("Ошибка: Abnormal library state - Произошла непредвиденная ошибка.");
                    break;
                case -2:
                    MessageBox.Show("Ошибка: Reset or stop request - Кнопка RESET или STOP была нажата.");
                    break;
                case -1:
                    MessageBox.Show("Ошибка: CNC Busy - Подождите завершения обработки CNC или повторите попытку.");
                    break;
                case 0:
                    MessageBox.Show("Операция завершена успешно, ошибок не произошло!");
                    break;
                case 1:
                    MessageBox.Show("Ошибка: Error - Специфическая функция не была выполнена.");
                    break;
                case 2:
                    MessageBox.Show("Ошибка: Error - Ошибка длины блока данных, ошибка количества данных.");
                    break;
                case 3:
                    MessageBox.Show("Ошибка: Error - Ошибка номера данных.");
                    break;
                case 4:
                    MessageBox.Show("Ошибка: Error - Ошибка атрибута данных.");
                    break;
                case 5:
                    MessageBox.Show("Ошибка: Error - Ошибка данных.");
                    break;
                case 6:
                    MessageBox.Show("Ошибка: Error - Соответствующая опция CNC отсутствует.");
                    break;
                case 7:
                    MessageBox.Show("Ошибка: Error - Запись запрещена.");
                    break;
                case 8:
                    MessageBox.Show("Ошибка: Error - Память ленты CNC переполнена.");
                    break;
                case 9:
                    MessageBox.Show("Ошибка: Error - Параметры CNC установлены неверно.");
                    break;
                case 10:
                    MessageBox.Show("Ошибка: Error - Буфер пуст или полон.");
                    break;
                case 11:
                    MessageBox.Show("Ошибка: Error - Номер пути неверен.");
                    break;
                case 12:
                    MessageBox.Show("Ошибка: Error - Режим CNC неверен.");
                    break;
                case 13:
                    MessageBox.Show("Ошибка: Error - Выполнение в CNC отклонено.");
                    break;
                case 14:
                    MessageBox.Show("Ошибка: Error - Произошли некоторые ошибки на сервере данных.");
                    break;
                case 15:
                    MessageBox.Show("Ошибка: Error - Функция не может быть выполнена из-за тревоги в CNC.");
                    break;
                case 16:
                    MessageBox.Show("Ошибка: Error - Статус CNC - остановка или авария.");
                    break;
                case 17:
                    MessageBox.Show("Ошибка: Error - Данные защищены функцией защиты данных CNC.");
                    break;
                case 18:
                    MessageBox.Show("Ошибка: Error - Не найден идентификатор машины.");
                    break;
                case 19:
                    MessageBox.Show("Ошибка: Error - Пожалуйста, проверьте номер NO.");
                    break;
                case 20:
                    MessageBox.Show("Ошибка: Error - Произошло из-за превышения времени ожидания.");
                    break;
                default:
                    MessageBox.Show("Неизвестный код ошибки.");
                    break;
            }
        }

        // Обновление состояния интерфейса
        private void UpdateUI()
        {
            btnConnect.Enabled = !isConnected; // Кнопка подключения активна, если не подключены
            btnSendCommand.Enabled = isConnected; // Кнопка отправки команды активна, если подключены
            txtCommand.Enabled = isConnected; // Поле для ввода команды активно, если подключены
        }

        // Проверка наличия подключенного USB
        public bool IsUSBExist()
        {
            return remoteCNC.HasUSB(); // Предположим, что метод HasUSB() проверяет наличие USB
        }

        // Получение серийного номера контроллера
        public string GetSerialNumber()
        {
            return remoteCNC.GetSerialNumber(); // Предположим, что метод GetSerialNumber() возвращает серийный номер
        }

        // Получение модели главной платы контроллера
        public string GetMainBoardModel()
        {
            return remoteCNC.GetMainBoardModel(); // Предположим, что метод GetMainBoardModel() возвращает модель платы
        }

        // Реализация функции READ_information
        public short READ_information(out short Axes, out string CncType, out short MaxAxes, out string Series, out string Nc_Ver, out string[] AxisName)
        {
            // Логика получения основной информации о контроллере CNC
            Axes = 0;
            CncType = "18"; // Пример значения
            MaxAxes = 6; // Пример значения
            Series = "M"; // Пример значения
            Nc_Ver = "1.0"; // Пример значения
            AxisName = new string[] { "X", "Y", "Z", "A", "B", "C" }; // Пример названий осей

            return 0; // Успешное завершение операции
        }

        // Реализация функции READ_status
        public short READ_status(out string MainProg, out string CurProg, out int CurSeq, out string Mode, out string Status, out string Alarm, out string EMG)
        {
            // Логика получения статусной информации о контроллере CNC
            MainProg = "MainProgram.nc"; // Пример имени основного файла программы
            CurProg = "CurrentProgram.nc"; // Пример имени текущего исполняемого файла
            CurSeq = -1; // По умолчанию -1
            Mode = "MDI"; // Пример значения
            Status = "START"; // Пример значения
            Alarm = "****"; // Пример значения
            EMG = "****"; // Пример значения

            return 0; // Успешное завершение операции
        }
    }
}
