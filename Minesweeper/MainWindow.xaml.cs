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
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            /*Grid toDel = (Grid)LogicalTreeHelper.FindLogicalNode(Main_Container, "GameField");
            Main_Container.Children.Remove(toDel);
            GameField gf = new GameField(Main_Container);*/
           MessageBox.Show(this.ActualWidth.ToString());
        }
    }

    class GameField
    {
        public static GameField Game_Instance;
        Grid Game_Container;
        int Game_RowCount;
        int Game_ColumnCount;

        public GameField(Grid Container)
        {
            Game_Instance = this;
            Game_Container = Container;
            Game_RowCount = 3;
            Game_ColumnCount = 3;

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
                {
                    GameElement NewElement = new GameElement(Game_Field, row, column);
                }

            Game_Container.Children.Add(Game_Field);
        }
    }

    enum GameElementState
    {
        Inactive = 0,
        Checked,
        Flag
    }

    class GameElement
    {
        Button Element_Ui;
        int Element_Row;
        int Element_Column;
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

            Element_Ui = new Button();
            Element_Ui.Width = 25;
            Element_Ui.Height = 25;
            Grid.SetColumn(Element_Ui, Column);
            Grid.SetRow(Element_Ui, Row);
            Element_Ui.Click += new RoutedEventHandler(Element_Click);
            Element_Ui.MouseRightButtonDown += new MouseButtonEventHandler(Element_RightClick);

            Game_Field.Children.Add(Element_Ui);
            MessageBox.Show(Element_State.ToString());
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
                    Element_Ui.Focusable = false;
                    Element_Ui.Background = Brushes.Gray;
                    break;
                case GameElementState.Flag:
                    Element_Ui.Focusable = false;
                    Element_Ui.Background = Brushes.Yellow;
                    break;
            }
            MessageBox.Show(Element_State.ToString());
        }
    } 
}
