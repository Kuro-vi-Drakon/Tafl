using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.ComponentModel;

namespace Tafl
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            MouseLeftButtonDown += new MouseButtonEventHandler(Mouse1);
            this.Board.ItemsSource = this.Pieces;
            this.Log_.ItemsSource = Log1;
            Setupgame();
        }
        
        ObservableCollection<Piece> Pieces = new ObservableCollection<Piece>();

        ObservableCollection<Log> Log1 = new ObservableCollection<Log>();

        List<Point> Moves = new List<Point>();

        public char[,] board = new char[11, 11];

        private Piece Origin;

        private bool Turn;

        public int TurnCount;

        public bool GameWon;

        private void Mouse1(object sender, MouseEventArgs e)
        {
            if (GameWon)
                GameWon = false;
            else
                GameWon = true;
            Point a = Mouse.GetPosition(Board);
            a.X = (int)Math.Floor(a.X);
            a.Y = (int)Math.Floor(a.Y);
            DoMove(a);
        }
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (TurnCount > 0)
            {
                for (int i = Pieces.Count - 1; i >= 0; i--)// clear target squares
                {
                    if (Pieces[i].Type == PieceType.TargetSquare)
                        Pieces.RemoveAt(i);
                }
                foreach (Piece a in Log1[Log1.Count - 1].Taken)
                {
                    Pieces.Add(a);
                    switch (a.Type)
                    {
                        case PieceType.BlackPiece:
                            board[(int)a.Pos.Y, (int)a.Pos.X] = 'B';
                            break;
                        case PieceType.WhitePiece:
                            board[(int)a.Pos.Y, (int)a.Pos.X] = 'W';
                            break;
                    }
                }
                Piece c = Log1[Log1.Count - 1].Pos1;
                Point d = Log1[Log1.Count - 1].Pos2;
                switch (c.Type)
                {
                    case PieceType.BlackPiece:
                        board[(int)d.Y, (int)d.X] = ' ';
                        board[(int)c.Pos.Y, (int)c.Pos.X] = 'B';
                        break;
                    case PieceType.WhitePiece:
                        board[(int)d.Y, (int)d.X] = ' ';
                        board[(int)c.Pos.Y, (int)c.Pos.X] = 'W';
                        break;
                    case PieceType.WhiteKing:
                        if (c.Pos.Y == 5 && c.Pos.X == 5)
                            board[(int)d.Y, (int)d.X] = 'k';
                        else
                            board[(int)d.Y, (int)d.X] = ' ';
                        board[(int)c.Pos.Y, (int)c.Pos.X] = 'K';
                        break;
                }
                for (int i = Pieces.Count - 1; i >= 0; i--)
                {
                    if (Pieces[i].Pos == Log1[Log1.Count - 1].Pos2)
                        Pieces.RemoveAt(i);
                }

                Pieces.Add(Log1[Log1.Count - 1].Pos1);
                Log1.RemoveAt(Log1.Count - 1);
                TurnCount--;
                if (Turn)
                    Turn = false;
                else
                    Turn = true;
            }
            
        }
        private void ResetButtonClick(object sender, RoutedEventArgs e)
        {
            Pieces.Clear();
            Log1.Clear();
            Setupgame();
        }
        private void DoMove(Point a)
        {
            Piece z = new Piece(PieceType.Blank, new Point(-1, -1));
            bool PieceClicked = false;
            foreach (Piece b in Pieces)// check what is at click
            {
                if (b.Pos == a)
                {
                    PieceClicked = true;
                    z = b;
                }
            }
            if (PieceClicked)// if piece at click
            {
                Moves.Clear();
                for (int i = Pieces.Count - 1; i >= 0; i--)// clear target squares
                {
                    if (Pieces[i].Type == PieceType.TargetSquare)
                        Pieces.RemoveAt(i);
                }
                if (z.Type == PieceType.TargetSquare)// if target square move to square
                {
                    MovePiece(a);
                    foreach (Piece b in Pieces)//CheckWin
                    {
                        if (b.Type == PieceType.WhiteKing)
                        {
                            if ((b.Pos.X == 0 || b.Pos.X == 10) && (b.Pos.Y == 0 || b.Pos.Y == 10))
                                MessageBox.Show("White Wins");
                            if(b.Pos.X > 0 && b.Pos.X < 10 && b.Pos.Y > 0 && b.Pos.Y < 10)
                                if ((board[(int)b.Pos.Y + 1, (int)b.Pos.X] == 'B' || (b.Pos.Y == 4 && b.Pos.X == 5)) &&
                                    (board[(int)b.Pos.Y - 1, (int)b.Pos.X] == 'B' || (b.Pos.Y == 6 && b.Pos.X == 5)) &&
                                    (board[(int)b.Pos.Y, (int)b.Pos.X + 1] == 'B' || (b.Pos.Y == 5 && b.Pos.X == 4)) &&
                                    (board[(int)b.Pos.Y, (int)b.Pos.X - 1] == 'B' || (b.Pos.Y == 5 && b.Pos.X == 6)))
                                    MessageBox.Show("Black Wins");
                        }
                    }
                }
                else// select new piece instead
                {
                    SelectPiece(a);
                }
            }
        }
        private void MovePiece(Point a)
        {
            Pieces.Add(new Piece(Origin.Type, a));
            switch (Origin.Type)
            {
                case PieceType.BlackPiece:
                    board[(int)a.Y, (int)a.X] = 'B';
                    board[(int)Origin.Pos.Y, (int)Origin.Pos.X] = ' ';
                    break;
                case PieceType.WhitePiece:
                    board[(int)a.Y, (int)a.X] = 'W';
                    board[(int)Origin.Pos.Y, (int)Origin.Pos.X] = ' ';
                    break;
                case PieceType.WhiteKing:
                    board[(int)a.Y, (int)a.X] = 'K';
                    if (Origin.Pos.Y == 5 && Origin.Pos.X == 5)
                        board[(int)Origin.Pos.Y, (int)Origin.Pos.X] = 'k';
                    else
                        board[(int)Origin.Pos.Y, (int)Origin.Pos.X] = ' ';
                    break;
            }
            
            for (int i = Pieces.Count - 1; i >= 0; i--)
            {
                if (Pieces[i] == Origin)
                    Pieces.RemoveAt(i);
            }

            if (Turn)
                Turn = false;
            else
                Turn = true;

            Log1.Add(new Log(Origin,a));
            TakePiece(a, Origin.Type);
            TurnCount++;
        }
        private void SelectPiece(Point a)
        {
            if (Turn)
            {
                foreach (Piece b in Pieces)
                {
                    if (b.Pos == a && b.Type == PieceType.BlackPiece)
                    {
                        Origin = b;
                        PossibleMoves1(a, false);
                    }
                }
            }
            else
            {
                foreach (Piece b in Pieces)
                {
                    if (b.Pos == a && (b.Type == PieceType.WhitePiece))
                    {
                        Origin = b;
                        PossibleMoves1(a, false);
                    }
                    if (b.Pos == a && (b.Type == PieceType.WhiteKing))
                    {
                        Origin = b;
                        PossibleMoves1(a, true);
                    }
                }
            }
            
            foreach (Point c in Moves)
            {
                Pieces.Add(new Piece(PieceType.TargetSquare, c));
            }
        }
        private void TakePiece(Point a,PieceType z)
        {
            for (int i = Pieces.Count - 1; i >= 0; i--)
            {
                switch(z)
                {
                    case PieceType.BlackPiece:// take enemy for black
                        if(a.X <= 8)
                            if (Pieces[i].Type == PieceType.WhitePiece && Pieces[i].Pos.X == a.X + 1 && Pieces[i].Pos.Y == a.Y && board[(int)a.Y,(int)a.X + 2] == 'B')
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y, (int)a.X + 1] = ' ';
                            }
                        if(a.X >= 2)
                            if (Pieces[i].Type == PieceType.WhitePiece && Pieces[i].Pos.X == a.X - 1 && Pieces[i].Pos.Y == a.Y && board[(int)a.Y, (int)a.X - 2] == 'B')
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y, (int)a.X - 1] = ' ';
                            }
                        if(a.Y <= 8)
                            if (Pieces[i].Type == PieceType.WhitePiece && Pieces[i].Pos.X == a.X && Pieces[i].Pos.Y == a.Y + 1 && board[(int)a.Y + 2, (int)a.X] == 'B')
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y + 1, (int)a.X] = ' ';
                            }
                        if(a.Y >= 2)
                            if (Pieces[i].Type == PieceType.WhitePiece && Pieces[i].Pos.X == a.X && Pieces[i].Pos.Y == a.Y - 1 && board[(int)a.Y - 2, (int)a.X] == 'B')
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y - 1, (int)a.X] = ' ';
                            }
                        break;
                    case PieceType.WhitePiece:
                    case PieceType.WhiteKing:// take enemy for white
                        if(a.X <= 8)
                            if (Pieces[i].Type == PieceType.BlackPiece && Pieces[i].Pos.X == a.X + 1 && Pieces[i].Pos.Y == a.Y && (board[(int)a.Y, (int)a.X + 2] == 'W' || board[(int)a.Y, (int)a.X + 2] == 'K' || board[(int)a.Y, (int)a.X + 2] == 'k'))
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y, (int)a.X + 1] = ' ';
                            }
                        if(a.X >= 2)
                            if (Pieces[i].Type == PieceType.BlackPiece && Pieces[i].Pos.X == a.X - 1 && Pieces[i].Pos.Y == a.Y && (board[(int)a.Y, (int)a.X - 2] == 'W' || board[(int)a.Y, (int)a.X - 2] == 'K' || board[(int)a.Y, (int)a.X - 2] == 'k'))
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y, (int)a.X - 1] = ' ';
                            }
                        if(a.Y <= 8)
                            if (Pieces[i].Type == PieceType.BlackPiece && Pieces[i].Pos.X == a.X && Pieces[i].Pos.Y == a.Y + 1 && (board[(int)a.Y + 2, (int)a.X] == 'W' || board[(int)a.Y + 2, (int)a.X] == 'K' || board[(int)a.Y + 2, (int)a.X] == 'k'))
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y + 1, (int)a.X] = ' ';
                            }
                        if(a.Y >= 2)
                            if (Pieces[i].Type == PieceType.BlackPiece && Pieces[i].Pos.X == a.X && Pieces[i].Pos.Y == a.Y - 1 && (board[(int)a.Y - 2, (int)a.X] == 'W' || board[(int)a.Y - 2, (int)a.X] == 'K' || board[(int)a.Y - 2, (int)a.X] == 'k'))
                            {
                                Log1[TurnCount].Taken.Add(Pieces[i]);
                                Pieces.RemoveAt(i);
                                board[(int)a.Y - 1, (int)a.X] = ' ';
                            }
                        break;
                }
            }
        }
        private void Setupgame()
        {
            //Ask to Load game or start new one
            bool Load = false;
            GameWon = false;
            TurnCount = 0;
            Turn = true;
            string a;
            if (Load)
            {//Load
                System.IO.StreamReader LoadGame = new System.IO.StreamReader("GameSave.txt");
                a = LoadGame.ReadToEnd();
            }
            else
            {//Make new game
                a = "k  BBBBB  k" +
                    "     B     " +
                    "           " +
                    "B    W    B" +
                    "B   WWW   B" +
                    "BB WWKWW BB" +
                    "B   WWW   B" +
                    "B    W    B" +
                    "           " +
                    "     B     " +
                    "k  BBBBB  k";
            }
            char[] b = a.ToCharArray();
            for (int row = 0; row < 11; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    char c = b[row * 11 + column];
                    board[row, column] = c;
                    switch (c)
                    {
                        case 'B':
                            Pieces.Add(new Piece(PieceType.BlackPiece, new Point(column, row)));
                            break;
                        case 'W':
                            Pieces.Add(new Piece(PieceType.WhitePiece, new Point(column, row)));
                            break;
                        case 'K':
                            Pieces.Add(new Piece(PieceType.WhiteKing, new Point(column, row)));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void PossibleMoves1(Point Pos, bool King)
        {
            PossibleMoves2(Pos, 1, 0, King);
            PossibleMoves2(Pos, -1, 0, King);
            PossibleMoves2(Pos, 0, 1, King);
            PossibleMoves2(Pos, 0, -1, King);
        }
        private void PossibleMoves2(Point Pos, int x, int y, bool King)
        {
            int a = 1;
            while (Pos.X + a * x < 11 && Pos.X + a * x >= 0 && Pos.Y + a * y < 11 && Pos.Y + a * y >= 0)
            {
                if (board[(int)Pos.Y + a * y, (int)Pos.X + a * x] == ' ' || (King && board[(int)Pos.Y + a * y, (int)Pos.X + a * x] == 'k'))
                {
                    Moves.Add(new Point(Pos.X + a * x, Pos.Y + a * y));
                }
                else if(board[(int)Pos.Y + a * y, (int)Pos.X + a * x] == 'k')
                {

                }
                else
                {
                    break;
                }
                a++;
            }
        }
        private void DrawBoardToFile()
        {
            System.IO.StreamWriter Save = new System.IO.StreamWriter("GameSave.txt");
            for (int row = 0; row < 11; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    Save.Write(board[row, column]);
                }
                Save.Write("\r\n");
            }
            Save.Close();
        }
    }
    public class Piece : INotifyPropertyChanged
    {
        public Piece(PieceType F, Point C)
        {
            Type = F;
            Pos = C;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void Changed(string property)
        {
            if(this.PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private Point pos;
        public Point Pos
        {
            get
            {
                return pos;
            }
            set
            {
                pos = value;
                Changed("Pos");
            }
        }

        public PieceType Type { get; set; }
    }
    public class Log
    {
        public Log(Piece pos1, Point pos2)
        {
            Pos1 = pos1;
            Pos2 = pos2;
            Taken = new List<Piece>();
        }
        public Piece Pos1;
        public Point Pos2;
        public List<Piece> Taken;
        public string Record
        {
            get
            {
                string a = (Pos1.Pos.X + 1) + "," + (Pos1.Pos.Y + 1) + " -->" + (Pos2.X + 1) + "," + (Pos2.Y + 1);
                string c = "  X  ";
                foreach(Piece b in Taken)
                {
                    c = c + b.Pos.X + "," + b.Pos.Y + " / ";
                }
                c = c.Substring(0, c.Length - 3);
                return a + c;
            }
        }
    }
    public enum PieceType
    {
        Blank,
        WhitePiece,
        BlackPiece,
        WhiteKing,
        TargetSquare,
    }
}
