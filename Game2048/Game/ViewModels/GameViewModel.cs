using Game2048.Game.Models;
using Game2048.Game.Services.Interfaces;
using Game2048.Game.Solver.Interfaces;
using Game2048.Game.ViewModels.Interfaces;
using Game2048.Infrastructure;
using Game2048.Infrastructure.Interfaces;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace Game2048.Game.ViewModels
{
    public class GameViewModel : IGameViewModel, INotifyPropertyChanged
    {
        /// <summary>
        /// The Game stats
        /// </summary>
        private enum GamePlayState
        {
            NotRunning,
            RunningBeforeElapsed,
            RunningBeforeFinished,
            TimeElapsed,
            PausedBeforeElapsed,
            PausedBeforeFinished,
        }

        #region Fields

        private GamePlayState playState;
        private IGameInteractor game;
        private IGameStateRetriever gameStateRetriever;
        private ICellTracker cellTracker;
        private IMediaPlayer mediaPlayer;
        private IGameSolver gameSolver;
        private INotificationService notificationService;
        private IViewModelStateUpdater stateUpdater;

        private Timer timer;
        private int timerCount = 60;

        private ICommand startCommand;
        private ICommand pauseCommand;
        private ICommand continueCommand;
        private ICommand finishCommand;

        #endregion

        #region Constrcutor

        /// <summary>
        /// Initializes a new instance of the <see cref="GameViewModel"/> class.
        /// </summary>
        /// <param name="game">The game.</param>
        /// <param name="gameStateRetriever">The game state retriever.</param>
        /// <param name="cellTracker">The cell tracker.</param>
        /// <param name="mediaPlayer">The media player.</param>
        /// <param name="gameSolver">The game solver.</param>
        /// <param name="notificationService">The notification service.</param>
        /// <param name="stateUpdater">The state updater.</param>
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
                    if (cell == 13)
                    {
                        gameSolver.SetDepth(3);
                    }
                };
            cellTracker.WinAction = () =>
                {
                    mediaPlayer.Play(@"Resources\applause.wav");
                    gameSolver.SetDepth(4);
                };

            timer = new Timer();
            timer.Elapsed += (sender, e) =>
                {
                    timerCount--;
                    RaisePropertyChanged("ElapsedTime");
                    if (timerCount == 0)
                    {
                        timer.Stop();
                        playState = GamePlayState.TimeElapsed;
                        stateUpdater.UpdateStates();
                        notificationService.ShowNotification("Your time has elapsed!");
                    }
                };
        }

        #endregion

        #region Commands

        /// <summary>
        /// Gets the start command.
        /// </summary>
        /// <value>
        /// The start command.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance can start.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can start; otherwise, <c>false</c>.
        /// </value>
        public bool CanStart
        {
            get
            {
                return playState == GamePlayState.NotRunning || playState == GamePlayState.TimeElapsed;
            }
        }

        /// <summary>
        /// Gets the pause command.
        /// </summary>
        /// <value>
        /// The pause command.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance can pause.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can pause; otherwise, <c>false</c>.
        /// </value>
        public bool CanPause
        {
            get
            {
                return playState == GamePlayState.RunningBeforeElapsed || playState == GamePlayState.RunningBeforeFinished;
            }
        }

        /// <summary>
        /// Gets the continue command.
        /// </summary>
        /// <value>
        /// The continue command.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance can continue.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can continue; otherwise, <c>false</c>.
        /// </value>
        public bool CanContinue
        {
            get
            {
                return playState == GamePlayState.PausedBeforeElapsed || playState == GamePlayState.PausedBeforeFinished;
            }
        }

        /// <summary>
        /// Gets the finish command.
        /// </summary>
        /// <value>
        /// The finish command.
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this instance can finish.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can finish; otherwise, <c>false</c>.
        /// </value>
        public bool CanFinish
        {
            get
            {
                return playState == GamePlayState.TimeElapsed;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the elapsed time.
        /// </summary>
        /// <value>
        /// The elapsed time.
        /// </value>
        public int ElapsedTime
        {
            get
            {
                return timerCount;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            gameSolver.InitHeuristics();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (playState == GamePlayState.NotRunning || playState == GamePlayState.TimeElapsed)
            {
                Initialize();
                gameSolver.SetDepth(3);
                playState = GamePlayState.RunningBeforeElapsed;
                stateUpdater.UpdateStates();
                timer.Interval = 1000;
                timerCount = 60;
                timer.Start();
                cellTracker.Initialize();
            }
            Move();
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            if (playState == GamePlayState.RunningBeforeElapsed)
            {
                timer.Stop();
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

        /// <summary>
        /// Continues this instance.
        /// </summary>
        public void Continue()
        {
            if (playState == GamePlayState.PausedBeforeElapsed)
            {
                playState = GamePlayState.RunningBeforeElapsed;
                stateUpdater.UpdateStates();
                timer.Start();
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

        /// <summary>
        /// Finishes this instance.
        /// </summary>
        public void Finish()
        {
            playState = GamePlayState.RunningBeforeFinished;
            stateUpdater.UpdateStates();
            gameSolver.SetDepth(2);
            Move();
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Moves this instance.
        /// </summary>
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
                    timerCount = 0;
                    RaisePropertyChanged("ElapsedTime");                    
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

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
