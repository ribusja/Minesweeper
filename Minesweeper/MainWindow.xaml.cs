using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{

    enum GameElementState
    {
        Inactive = 0,
        Checked,
        Flag
    }

    public enum GameDifficulty
    {
        Easy,
        Normal,
        Hard,
        Custom
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameSettings Settings;
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GameWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Instance = this;
            Settings = new GameSettings();
        }

        public void EndGameHandle(bool isWin)
        {
            if (isWin)
            {
                MessageBox.Show("Hooraaa");
            }
            else
            {
                MessageBox.Show("ohhh...");
            }
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            Grid toDel = (Grid)LogicalTreeHelper.FindLogicalNode(Main_Container, "GameField");
            Main_Container.Children.Remove(toDel);
            Settings.SetStartValuables(GameDifficulty.Normal);
            GameField gf = new GameField(Main_Container);
        }
    }

    public class GameSettings
    {
        public static GameSettings Instance;
        GridLength FieldWidth;
        GridLength FieldHeight;
        int ElementUI_Size;
        const int MaxRowCount = 20;
        const int MinRowCount = 10;
        const int MaxColCount = 50;
        const int MinColCount = 10;
        const int MaxMinePercent = 75;
        const int MinMinePercent = 5;
        int RowCount;
        int ColCount;
        int MinePercent;
        double MineCount;

        public int GetRowCount()
        {
            return RowCount;
        }

        public int GetColCount()
        {
            return ColCount;
        }

        public double GetMineCount()
        {
            return MineCount;
        }

        public GameSettings()
        {
            Instance = this;
            ElementUI_Size = 25;
            RowCount = 0;
            ColCount = 0;
            MinePercent = 0;
            MineCount = 0;
        }

        public int GetUISize()
        {
            return ElementUI_Size;
        }

        public void SetStartValuables(GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    RowCount = 10;
                    ColCount = 10;
                    MinePercent = 10;
                    break;
                case GameDifficulty.Normal:
                    RowCount = 16;
                    ColCount = 16;
                    MinePercent = 20;
                    break;
                case GameDifficulty.Hard:
                    RowCount = 20;
                    ColCount = 50;
                    MinePercent = 35;
                    break;
                case GameDifficulty.Custom:
                    //Instantiate new Window
                    break;
            }

            MineCount = (double)RowCount * (double)ColCount / 100 * MinePercent;
            FieldWidth = new GridLength(ColCount * ElementUI_Size);
            FieldHeight = new GridLength(RowCount * ElementUI_Size);

            SetFieldSize();
        }

        public void SetFieldSize()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            MainWindow.Instance.GameWindow.Left = (screenWidth / 2) - ((FieldWidth.Value + 50) / 2);
            MainWindow.Instance.GameWindow.Top = (screenHeight / 2) - ((FieldHeight.Value + 100) / 2);

            MainWindow.Instance.GameWindow.Width = FieldWidth.Value + 50;
            MainWindow.Instance.GameWindow.Height = FieldHeight.Value + 100;
            MainWindow.Instance.Game_Column.Width = FieldWidth;
            MainWindow.Instance.Game_Row.Height = FieldHeight;
            MainWindow.Instance.GameWindow.HorizontalAlignment = HorizontalAlignment.Center;
            MainWindow.Instance.GameWindow.VerticalAlignment = VerticalAlignment.Center;
        }
    }

    class GameField
    {
        public static GameField Game_Instance;
        Grid Game_Container;
        int Game_RowCount;
        int Game_ColumnCount;
        List<GameElement> Game_Elementlist;
        List<GameElement> Game_Minelist;

        public GameField(Grid Container)
        {
            Game_Instance = this;
            Game_Container = Container;
            Game_RowCount = GameSettings.Instance.GetRowCount();
            Game_ColumnCount = GameSettings.Instance.GetColCount();
            Game_Elementlist = new List<GameElement>();
            Game_Minelist = new List<GameElement>();
            InitializeField();
        }

        void InitializeField()
        {
            Grid Game_Field = new Grid();
            Game_Field.Name = "GameField";
            Grid.SetColumn(Game_Field, 1);
            Grid.SetRow(Game_Field, 1);

            Game_Field.Background = Brushes.DarkGray;

            for (int i = 0; i < Game_RowCount; i++)
                Game_Field.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < Game_ColumnCount; i++)
                Game_Field.ColumnDefinitions.Add(new ColumnDefinition());

            for (int row = 0; row < Game_RowCount; row++)
                for (int column = 0; column < Game_ColumnCount; column++)
                    Game_Elementlist.Add(new GameElement(Game_Field, row, column));

            Random rnd = new Random();
            for (int mine = 0; mine < GameSettings.Instance.GetMineCount(); mine++)
            {
                int element = rnd.Next(0, Game_Elementlist.Count - 1);
                if (Game_Elementlist[element].SetMine())
                {
                    Game_Minelist.Add(Game_Elementlist[element]);
                }
                else mine--;
            }

            Game_Container.Children.Add(Game_Field);
        }

    }

    class GameElement
    {
        Button Element_Ui;
        int Element_Row;
        int Element_Column;
        bool Element_isMine;
        GameElementState element_State;
        GameElementState Element_State
        {
            get { return element_State; }
            set
            {
                if (element_State != value)
                {
                    element_State = value;
                    State_Changed();
                }
            }
        }

        public GameElement(Grid Game_Field, int Row, int Column)
        {
            Element_Column = Column;
            Element_Row = Row;
            Element_State = GameElementState.Inactive;
            Element_isMine = false;

            Element_Ui = new Button();
            Element_Ui.Width = Element_Ui.Height = GameSettings.Instance.GetUISize(); 
            Grid.SetColumn(Element_Ui, Column);
            Grid.SetRow(Element_Ui, Row);
            Element_Ui.Click += new RoutedEventHandler(Element_Click);
            Element_Ui.MouseRightButtonDown += new MouseButtonEventHandler(Element_RightClick);

            Game_Field.Children.Add(Element_Ui);
        }

        public bool SetMine()
        {
            if (Element_isMine)
                return false;
            Element_isMine = true;
            return true;
        }

        void Element_Click(object sender, RoutedEventArgs e)
        {
            if (Element_State == GameElementState.Inactive)
                Element_State = GameElementState.Checked;
        }

        void Element_RightClick(object sender, RoutedEventArgs e)
        {
            switch (Element_State)
            {
                case GameElementState.Inactive:
                    Element_State = GameElementState.Flag;
                    break;
                case GameElementState.Flag:
                    Element_State = GameElementState.Inactive;
                    break;
            }
        }

        void State_Changed()
        {
            switch (Element_State)
            {
                case GameElementState.Inactive:
                    break;
                case GameElementState.Checked:
                    if (Element_isMine)
                    {
                        Element_Ui.Focusable = false;
                        Element_Ui.Background = Brushes.Red;
                        MainWindow.Instance.EndGameHandle(false);
                    }
                    else
                    {
                        Element_Ui.Focusable = false;
                        Element_Ui.Background = Brushes.Gray;
                    }
                    
                    break;
                case GameElementState.Flag:
                    Element_Ui.Focusable = false;
                    Element_Ui.Background = Brushes.Yellow;
                    break;
            }
        }
    } 
}
