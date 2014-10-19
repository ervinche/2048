using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Game.Solver.Interfaces;
using Game2048.Game.ViewModels.Interfaces;
using Game2048.Infrastructure;
using Game2048.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace Game2048.Game.ViewModels
{   
    public class GameViewModel : IGameViewModel
    {
        private enum GamePlayState
        {
            NotRunning,
            RunningBeforeElapsed,
            RunningBeforeFinished,
            TimeElapsed,
            PausedBeforeElapsed,            
            PausedBeforeFinished,                        
        }

        private GamePlayState playState;
        private IGameInteractor game;
        private IGameStateRetriever gameStateRetriever;
        private ICellTracker cellTracker;
        private IMediaPlayer mediaPlayer;
        private IGameSolver gameSolver;
        private INotificationService notificationService;
        private IViewModelStateUpdater stateUpdater;

        private Timer timer;
        private DateTime startTime;        

        private ICommand startCommand;
        private ICommand pauseCommand;
        private ICommand continueCommand;
        private ICommand finishCommand;        
        
        public GameViewModel(IGameInteractor game, 
            IGameStateRetriever gameStateRetriever, 
            ICellTracker cellTracker, 
            IMediaPlayer mediaPlayer, 
            IGameSolver gameSolver, 
            INotificationService notificationService,
            IViewModelStateUpdater stateUpdater)
        {            
            this.game = game;
            this.gameStateRetriever = gameStateRetriever;
            this.cellTracker = cellTracker;
            this.mediaPlayer = mediaPlayer;
            this.gameSolver = gameSolver;
            this.notificationService = notificationService;
            this.stateUpdater = stateUpdater;

            cellTracker.NewCellAction = (cell) =>
                {
                    mediaPlayer.Play(@"Resources\beep.wav");                                        
                };
            cellTracker.WinAction = () =>
                {
                    mediaPlayer.Play(@"Resources\applause.wav");                    
                    gameSolver.SetDepth(3);
                };

            timer = new Timer();
            timer.Elapsed += (sender, e) =>
                {                    
                    timer.Stop();
                    playState = GamePlayState.TimeElapsed;
                    stateUpdater.UpdateStates();
                    notificationService.ShowNotification("Your time has elapsed!");
                };
        }

        public ICommand StartCommand
        {
            get
            {
                if (startCommand == null)
                {
                    startCommand = new RelayCommand(param => this.Start(), param => this.CanStart);
                }
                return startCommand;
            }
        }

        public bool CanStart
        {
            get
            {
                return playState == GamePlayState.NotRunning || playState == GamePlayState.TimeElapsed;
            }
        }

        public ICommand PauseCommand
        {
            get
            {
                if (pauseCommand == null)
                {
                    pauseCommand = new RelayCommand(param => this.Pause(), param => this.CanPause);
                }
                return pauseCommand;
            }
        }

        public bool CanPause
        {
            get
            {
                return playState == GamePlayState.RunningBeforeElapsed || playState == GamePlayState.RunningBeforeFinished;
            }
        }

        public ICommand ContinueCommand
        {
            get
            {
                if (continueCommand == null)
                {
                    continueCommand = new RelayCommand(param => this.Continue(), param => this.CanContinue);
                }
                return continueCommand;
            }
        }

        public bool CanContinue
        {
            get
            {
                return playState == GamePlayState.PausedBeforeElapsed || playState == GamePlayState.PausedBeforeFinished;
            }
        }

        public ICommand FinishCommand
        {
            get
            {
                if (finishCommand == null)
                {
                    finishCommand = new RelayCommand(param => this.Finish(), param => this.CanFinish);
                }
                return finishCommand;
            }
        }

        public bool CanFinish
        {
            get
            {
                return playState == GamePlayState.TimeElapsed;
            }
        }

        public void Initialize()
        {
            gameSolver.InitHeuristics();
        }

        public void Start()
        {            
            if (playState == GamePlayState.NotRunning || playState == GamePlayState.TimeElapsed)
            {
                Initialize();
                playState = GamePlayState.RunningBeforeElapsed;
                stateUpdater.UpdateStates();
                timer.Interval = 60 * 1000;
                timer.Start();
                startTime = DateTime.Now;                
                cellTracker.Initialize();
            }                                    
            Move();
        }

        public void Continue()
        {
            if (playState == GamePlayState.PausedBeforeElapsed)
            {
                playState = GamePlayState.RunningBeforeElapsed;
                stateUpdater.UpdateStates();
                timer.Start();
                startTime = DateTime.Now; // because of decreased interval                
            }
            else
            {
                if (playState == GamePlayState.PausedBeforeFinished)
                {
                    playState = GamePlayState.RunningBeforeFinished;
                    stateUpdater.UpdateStates();
                }
            }                        
            Move();
        }

        public void Pause()
        {
            if (playState == GamePlayState.RunningBeforeElapsed)
            {
                timer.Stop();
                TimeSpan elapsed = DateTime.Now.Subtract(startTime);
                timer.Interval = timer.Interval - elapsed.TotalMilliseconds;
                playState = GamePlayState.PausedBeforeElapsed;
            }
            else
            {
                if (playState == GamePlayState.RunningBeforeFinished)
                {
                    playState = GamePlayState.PausedBeforeFinished;
                }
            }
            stateUpdater.UpdateStates();
        }

        public void Finish()
        {            
            playState = GamePlayState.RunningBeforeFinished;
            stateUpdater.UpdateStates();
            Move();
        }        

        private async void Move()
        {                        
            if (playState == GamePlayState.TimeElapsed || playState == GamePlayState.PausedBeforeElapsed || playState == GamePlayState.PausedBeforeFinished)
            {
                return;
            }

            GameState gameStateValue = gameStateRetriever.GetState();
            // am murit
            if (gameStateValue == null)
            {
                if (playState == GamePlayState.RunningBeforeElapsed)
                {
                    timer.Stop();                    
                }
                playState = GamePlayState.NotRunning;
                stateUpdater.UpdateStates();
                return;
            }

            //keep playing popout
            if (!gameStateValue.KeepPlaying && gameStateValue.Won)
            {
                await Task.Delay(100);
                game.Continue();
            }

            ulong stateValue = gameStateRetriever.TransformStateValue(gameStateValue);
            cellTracker.RegisterState(stateValue);

            MoveDirection best = 0;
            await Task.Factory.StartNew(() =>
            {
                best = gameSolver.GetBestMove(stateValue);
            });            

            game.Move(best);
            Move();
        }
    }
}
