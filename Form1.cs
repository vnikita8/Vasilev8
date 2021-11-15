using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vasilev8
{
    public struct Symbols
    {
        public int Red;
        public int Blue;
        public int Lower;
        public int Upper;
        public int Other;
    }

    public partial class Form1 : Form
    {
        Temperature temp;
        TemperatureDict temperatureDict;
        Matrix matrix1;
        Matrix matrix2;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) 
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            textBox1.Text = filename;

            TextReader textReader = new TextReader(filename);
            Symbols symbolsCount = textReader.GetSymbolsCount();

            OneBlue.Text = symbolsCount.Blue.ToString();
            OneRed.Text = symbolsCount.Red.ToString();
            OneLower.Text = symbolsCount.Lower.ToString();
            OneUpper.Text = symbolsCount.Upper.ToString();
            OneOther.Text = symbolsCount.Other.ToString();

            OneText.Text = textReader.GetText();
        }

        private void tabPage1_Click(object sender, EventArgs e) {}
        
        private void Form1_Load(object sender, EventArgs e) 
        { 
            openFileDialog1.Filter = "Text files(*.txt)|*.txt"; 
            temp = new Temperature(); 
            temperatureDict = new TemperatureDict(); 
        }

        private void button3_Click(object sender, EventArgs e) 
        {
            ThreeOne.Text = "";
            temp.RandomGenerate();
            for (int i = 0; i<12; i++)
            {
                ThreeOne.Text += $"\n\tMonth: {i+1}\n\n";
                int day = 1;
                for (int days = 0; days < 30; days++)
                {
                    ThreeOne.Text += $"Day: {day}, Temp: {temp.temperature[i, days]}\n";
                    day++;
                }
            }

            ThreeTwo.Text = "";
            for (int i = 0; i < 12; i++)
            {
                ThreeTwo.Text += $" Month: {i + 1}, TempAvg: {temp.middleTemperature[i]}\n";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ThreeThree.Text = "";
            for (int i = 0; i < 12; i++)
            {
                ThreeThree.Text += $"{temp.middleTemperatureSort[i]}\n";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            temperatureDict.RandomGenerate();
            SixOne.Text = "";
            SixThree.Text = "";
            SixTwo.Text = "";
            Rich();
            
                foreach (KeyValuePair<string, double> keyValuePair in temperatureDict.middleTemperature)
                {
                    SixTwo.Text += $"{keyValuePair.Key}:  {keyValuePair.Value}\n";
                }
            

            
            foreach (KeyValuePair<string, double> keyValuePair in temperatureDict.middleTemperatureSort)
            {
                SixThree.Text += $"{keyValuePair.Key}:  {keyValuePair.Value}\n";
            }

        }

        private async void Rich()
        {
            await Task.Run(() => Temp());
        }

        private void Temp()
        {
            foreach (KeyValuePair<string, int[]> keyValuePair in temperatureDict.temperature)
            {
                Invoke((MethodInvoker)delegate
                {
                    SixOne.Text += $"\t{keyValuePair.Key}\n";
                    int dayCount = 1;
                    foreach (int days in keyValuePair.Value)
                    {
                        SixOne.Text += $"{dayCount}:  {days}\n";
                        dayCount++;
                    }
                });
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            textBox6.Text = filename;
            TextReaderList text = new TextReaderList(filename);
            Symbols symbolsCount = text.GetSymbolsCount();

            FourBlue.Text = symbolsCount.Blue.ToString();
            FourRed.Text = symbolsCount.Red.ToString();
            FourDown.Text = symbolsCount.Lower.ToString();
            FourUp.Text = symbolsCount.Upper.ToString();
            FourOther.Text = symbolsCount.Other.ToString();

            FourText.Text = text.GetText();
        }

        private void GenButton_Click(object sender, EventArgs e)
        {
            if (Rows1.Value > 0 & Columns1.Value>0 & Columns2.Value > 0 & Rows2.Value > 0)
            {
                matrix1 = new Matrix((int)Rows1.Value, (int)Columns1.Value, (int)MaxValues.Value);
                matrix1.RandomGenerate();
                matrix1.MatrixGui(dataMatrix1);
                matrix2 = new Matrix((int)Rows2.Value, (int)Columns2.Value, (int)MaxValues.Value);
                matrix2.RandomGenerate();
                matrix2.MatrixGui(dataMatrix2);
            }
           
        }

        private void buttonMultiply_Click(object sender, EventArgs e)
        {
            
                bool mult = Matrix.TryMultiply(matrix1, matrix2, out Matrix multiplyResult);
                if (mult)
                    multiplyResult.MatrixGui(dataMatrixRes);
                else
                    MessageBox.Show("Ошибка умножения");
            
           
        }

        private void dataMatrixRes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }

    public class TextReader //Ex1 
    {
        private static string RedLowerSymbols = "аеёиоуыэюя"; //Гласные строчные
        private static string BlueLowerSymbols = "бвгджзйклмнпрстфхцчшщ"; //Согласные

        private char[] RedLowerSymbolsChar; //Потом будет символьный массив из глассных строчных
        private char[] BlueLowerSymbolsChar; //Потом будет символьный массив из соглассных строчных
        private char[] RedUpperSymbolsChar; //Потом будет символьный массив из глассных строчных
        private char[] BlueUpperSymbolsChar; //Потом будет символьный массив из соглассных строчных
        private string filename; //Путь к файлу

        private Symbols symbols; //Структура с количеством символов

        public TextReader (string filepath) //Конструктор с путём к файлу
        {
            RedLowerSymbolsChar = RedLowerSymbols.ToCharArray();  
            BlueLowerSymbolsChar = BlueLowerSymbols.ToCharArray();
            RedUpperSymbolsChar = RedLowerSymbols.ToUpper().ToCharArray();
            BlueUpperSymbolsChar = BlueLowerSymbols.ToUpper().ToCharArray();

            filename = filepath; 
        }

        public Symbols GetSymbolsCount() //В целом выдает ответ
        {
            char[] charSymbols = ReadFile(); //Функция 
            SymbolsCount(charSymbols); //Функция 
            return symbols;
        }
        
        private char[] ReadFile() //Функция чтения файла
        {
            StreamReader sr = new StreamReader(filename);
            char[] all_text = sr.ReadToEnd().ToCharArray();
            return all_text;
        }

        private void SymbolsCount(char[] symbolsChar) //Функция нахождения количества символов
        {
            foreach (char sym in symbolsChar)
            {
                if (RedLowerSymbolsChar.Contains(sym))
                {
                    symbols.Red++;
                    symbols.Lower++;
                }
                else if (RedUpperSymbolsChar.Contains(sym))
                {
                    symbols.Red++;
                    symbols.Upper++;
                }
                    
                else if (BlueLowerSymbols.Contains(sym))
                {
                    symbols.Blue++;
                    symbols.Lower++;
                }
                else if (BlueUpperSymbolsChar.Contains(sym))
                {
                    symbols.Blue++;
                    symbols.Upper++;
                }
                else
                {
                    symbols.Other++;
                }
            }
        }

        public string GetText()
        {
            StreamReader sr = new StreamReader(filename);
            return sr.ReadToEnd();
        }
    }

    public class TextReaderList //Ex5 
    {
        private static string RedLowerSymbols = "аеёиоуыэюя"; //Гласные строчные
        private static string BlueLowerSymbols = "бвгджзйклмнпрстфхцчшщ"; //Согласные
        private string filename; //Путь к файлу

        private Symbols symbols; //Структура с количеством символов

        public TextReaderList(string filepath) //Конструктор с путём к файлу
        {

            filename = filepath;
        }

        public Symbols GetSymbolsCount() //В целом выдает ответ
        {
            List<char> charSymbols = ReadFile(); //Функция 
            SymbolsCount(charSymbols); //Функция 
            return symbols;
        }

        private List<char> ReadFile() //Функция чтения файла
        {
            StreamReader sr = new StreamReader(filename);
            List<char> all_text = new List<char>();
            all_text.AddRange(sr.ReadToEnd().ToCharArray());
            return all_text;
        }

        private void SymbolsCount(List<char> symbolsChar) //Функция нахождения количества символов
        {
            foreach (char sym in symbolsChar)
            {
                if (RedLowerSymbols.Contains(sym))
                {
                    symbols.Red++;
                    symbols.Lower++;
                }
                else if (RedLowerSymbols.ToUpper().Contains(sym))
                {
                    symbols.Red++;
                    symbols.Upper++;
                }

                else if (BlueLowerSymbols.Contains(sym))
                {
                    symbols.Blue++;
                    symbols.Lower++;
                }
                else if (BlueLowerSymbols.ToUpper().Contains(sym))
                {
                    symbols.Blue++;
                    symbols.Upper++;
                }
                else{symbols.Other++;}
            }
        }

        public string GetText()
        {
            StreamReader sr = new StreamReader(filename);
            return sr.ReadToEnd();
        }
    }

    public class Matrix //Ex2
    {
        public int[,] matrix;
        private int rows;
        private int columns;
        public int maxValue = 2;

        public Matrix(int rows, int columns, int MaxValue)
        {
            matrix = new int[rows,columns];
            this.rows = rows;
            this.columns = columns;
            maxValue = MaxValue + 1;
        }

        public Matrix(int rows, int columns) {matrix = new int[rows, columns]; this.rows = rows; this.columns = columns;}

        public void RandomGenerate()
        {
            Random rnd = new Random();
            for (int row = 0; row< rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    matrix[row, column] = rnd.Next(0, maxValue);
                }
            }
        }

        static public bool TryMultiply (Matrix matrix1, Matrix matrix2, out Matrix matrixRes)
        {
            if (matrix1.columns == matrix2.rows)
            {
                matrixRes = new Matrix(matrix1.rows, matrix2.columns);
                for (int rowMatrix1 = 0; rowMatrix1 < matrix1.rows; rowMatrix1++)
                {
                    for (int columnsMatrix2 = 0; columnsMatrix2 < matrix2.columns; columnsMatrix2++)
                    {
                        matrixRes.matrix[rowMatrix1, columnsMatrix2] = 0;
                        for (int column = 0; column < matrix1.columns; column++)
                            matrixRes.matrix[rowMatrix1, columnsMatrix2] += matrix1.matrix[rowMatrix1, column] * matrix2.matrix[column, columnsMatrix2];
                    }
                }
                return true;
            }
            else
            {
                matrixRes = null;
                return false;
            }
        }

        public void MatrixGui(DataGridView table)
        {
            table.Rows.Clear();
            table.Columns.Clear();
            for (int col = 0; col < columns; col++)
                table.Columns.Add("", "");

            for (int row = 0; row < rows; row++)
            {
                table.Rows.Add();
                for (int cell = 0; cell < columns; cell++)
                    table[cell, row].Value = this.matrix[row, cell];
            }
        }
    }

    public class MatrixLinkedList
    {
        public LinkedList<LinkedList<int>> matrix;

        public MatrixLinkedList(int rows, int columns)
        {

        }
    }

    public class Temperature //Ex3 
    {
        public int[,] temperature;
        public double[] middleTemperature;
        public double[] middleTemperatureSort;

        public Temperature()
        {
            temperature = new int[12, 30];
            middleTemperature = new double[12];
            middleTemperatureSort = new double[12];
        }

        public void RandomGenerate()
        {
            Random rnd = new Random();
            for (int month = 0; month < 12; month++)
            {
                if (month ==  (0 | 1 | 2 | 9 | 10 | 11))
                {
                    for (int day = 0; day<30; day++)
                    {
                        temperature[month, day] = rnd.Next(-20, 5);
                    }
                }
                else
                {
                    for (int day = 0; day < 30; day++)
                    {
                        temperature[month, day] = rnd.Next(5, 30);
                    }
                }
            }
            MiddleTempCalc();
        }
    
        public void MiddleTempCalc()
        {
            for (int month = 0; month < 12; month++)
            {
                int daysSumm = 0;
                for (int day = 0; day < 30; day++){ daysSumm += temperature[month, day];}
                middleTemperature[month] = Math.Round((double)daysSumm / 30, 1);
            }
            SortMiddleTemp();
        }

        public void SortMiddleTemp()
        {
            middleTemperature.CopyTo(middleTemperatureSort, 0);
            middleTemperatureSort = SortClass.QuickSort(middleTemperatureSort);
        }
    }

    public class TemperatureDict //Ex6 
    {
        public Dictionary<string,int[]> temperature;
        public Dictionary<string, double> middleTemperature;
        public Dictionary<string, double> middleTemperatureSort;

        public void RandomGenerate()
        {
            temperature = new Dictionary<string, int[]>(12);
            temperature.Add("Январь", RandomTemoForMonth(31, -30, -5));
            temperature.Add("Февраль", RandomTemoForMonth(31, -15, 0));
            temperature.Add("Март", RandomTemoForMonth(31, -10, 5));
            temperature.Add("Апрель", RandomTemoForMonth(31, -5, 5));
            temperature.Add("Май", RandomTemoForMonth(31, -5, 10));
            temperature.Add("Июнь", RandomTemoForMonth(31, 5, 15));
            temperature.Add("Июль", RandomTemoForMonth(31, 8, 30));
            temperature.Add("Август", RandomTemoForMonth(31, 10, 30));
            temperature.Add("Сентябрь", RandomTemoForMonth(31, 8, 25));
            temperature.Add("Октябрь", RandomTemoForMonth(31, 0, 10));
            temperature.Add("Ноябрь", RandomTemoForMonth(31, -15, 0));
            temperature.Add("Декабрь", RandomTemoForMonth(31, -30, -5));
            MiddleTempCalc();
        }

        private int[] RandomTemoForMonth(int days, int min, int max)
        {
            Random rnd = new Random();
            int[] getDown = new int[days];
            for (int day = 0; day < days; day++)
            {
                 getDown[day] = rnd.Next(min, max);
            }
            return getDown;
        }

        private void MiddleTempCalc()
        {
            middleTemperature = new Dictionary<string, double>(12);
            foreach (KeyValuePair<string, int[]> month in temperature) { middleTemperature.Add(month.Key, AvgTempForMonth(month.Value)); }

            SortMiddleTemp();
        }

        private double AvgTempForMonth(int[] days)
        {
            double middle = 0;
            foreach (int day in days) {middle += day;}
            return Math.Round(middle / days.Length);
        }

        private void SortMiddleTemp()
        {
            middleTemperatureSort = new Dictionary<string, double>(12);
            middleTemperatureSort = middleTemperature.OrderBy(pair => pair.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
    }

    public static class SortClass 
    {
        private static void Swap(ref double value1, ref double value2)
        {
            double value3 = value1;
            value1 = value2;
            value2 = value3;
        }

        private static int PivotIndex(double[] array, int minId, int maxId)
        {
            int index = minId - 1;
            for (int i = minId; i < maxId; i++)
            {
                if (array[i]< array[maxId])
                {
                    index++;
                    Swap(ref array[index], ref array[i]);
                }
            }
            index++;
            Swap(ref array[index], ref array[maxId]);
            return index;
        }

        private static double[] QuickSort(double[] array, int minId, int maxId)
        {
            if (minId >= maxId)
                return array;
            int pivotPoint = PivotIndex(array, minId, maxId);
            QuickSort(array, minId, pivotPoint - 1);
            QuickSort(array, pivotPoint + 1, maxId);
            return array;
        }
        public static double[] QuickSort(double[] array)
        {
            return QuickSort(array, 0, array.Length - 1);
        }
    }
}
