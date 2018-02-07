using OWOrganizerApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculatrice.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string _current;
        public string Current
        {
            get { return _current; }
            set { SetProperty(ref _current, value); }
        }

        private string _historic;
        public string Historic
        {
            get { return _historic; }
            set { SetProperty(ref _historic, value); }
        }

        private string _result;
        public string Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }
        
        public char CurrentOperation { get; set; }

        public static List<Tuple<double, char>> HistoricList { get; set; }

        #region Binding Commands
        private RelayCommand _numberCommand;
        public RelayCommand NumberCommand
        {
            get { return _numberCommand; }
            set { SetProperty(ref _numberCommand, value); }
        }

        private RelayCommand _operationCommand;
        public RelayCommand OperationCommand
        {
            get { return _operationCommand; }
            set { SetProperty(ref _operationCommand, value); }
        }

        private RelayCommand _equalCommand;
        public RelayCommand EqualCommand
        {
            get { return _equalCommand; }
            set { SetProperty(ref _equalCommand, value); }
        }

        private RelayCommand _correctCommand;
        public RelayCommand CorrectCommand
        {
            get { return _correctCommand; }
            set { SetProperty(ref _correctCommand, value); }
        }
        #endregion

        public MainViewModel()
        {
            Reset();
            NumberCommand = new RelayCommand(o => HittingNumber(o.ToString()), o => true);
            OperationCommand = new RelayCommand(o => HittingOperation(o.ToString()), o => true);
            EqualCommand = new RelayCommand(o => HittingEqual(), o => true);
            CorrectCommand = new RelayCommand(o => HittingCorrect(), o => true);
        }

        public void Reset()
        {
            Current = string.Empty;
            Historic = string.Empty;
            HistoricList = new List<Tuple<double, char>>();
            CurrentOperation = '+';
        }

        public string UpdateHistoric()
        {
            if (HistoricList.Count == 0) return string.Empty;

            string histo = string.Join(" ", HistoricList.Select(x => $"{x.Item2} {x.Item1}"));
            return histo.Remove(0,2) + " " + CurrentOperation;
        }

        public string UpdateResult()
        {
            double r = 0;
            try
            {
                foreach (var item in HistoricList)
                {
                    switch(item.Item2)
                    {
                        case '+': r += item.Item1; break;
                        case '-': r -= item.Item1; break;
                        case 'x': r *= item.Item1; break;
                        case '/': r /= item.Item1; break;
                    }
                }
                return r.ToString();
            }
            catch(Exception e)
            {
                return e.Message;
            }
        }

        #region Methods Commands
        public void HittingNumber(string number)
        {
            Current += number;
        }

        public void HittingOperation(string operation)
        {
            if (string.IsNullOrEmpty(Current))
                return;

            HistoricList.Add(new Tuple<double, char>(double.Parse(Current),CurrentOperation));

            CurrentOperation = operation.First();
            Historic = UpdateHistoric();
            Current = string.Empty;
            Result = UpdateResult();
        }

        public void HittingEqual()
        {
            HistoricList.Add(new Tuple<double, char>(double.Parse(Current), CurrentOperation));
            Result = UpdateResult();

            Reset();
        }

        public void HittingCorrect()
        {
            if (Current.Length > 0)
                Current = Current.Remove(Current.Length - 1);
        }
        #endregion

    }
}