﻿using ChatCorporaAnnotator.Data.Dialogs;
using ChatCorporaAnnotator.Data.Indexing;
using ChatCorporaAnnotator.Data.Windows;
using ChatCorporaAnnotator.Infrastructure.Commands;
using ChatCorporaAnnotator.Infrastructure.Exceptions.Indexing;
using ChatCorporaAnnotator.Models.Indexing;
using ChatCorporaAnnotator.Models.Messages;
using ChatCorporaAnnotator.ViewModels.Base;
using ChatCorporaAnnotator.ViewModels.Chat;
using ChatCorporaAnnotator.Views.Windows;
using IndexEngine;
using IndexEngine.Indexes;
using IndexEngine.Paths;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChatCorporaAnnotator.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        public ChatViewModel ChatVM { get; }
        public IndexFileWindow IndexFileWindow { get; set; }

        private bool _isFileLoaded = false;
        public bool IsFileLoaded
        {
            get => _isFileLoaded;
            set
            {
                if (!SetValue(ref _isFileLoaded, value))
                    return;

                TabControlGridsVisibility = _isFileLoaded
                    ? Visibility.Visible
                    : Visibility.Hidden;

                BottomMenuVisibility = TabControlGridsVisibility;
                CurrentTagsetVisibility = TabControlGridsVisibility;
            }
        }

        #region SelectedItems

        private object _selectedTagset;
        public object SelectedTagset
        {
            get => _selectedTagset;
            set => SetValue(ref _selectedTagset, value);
        }

        #endregion

        #region ItemsVisibilities

        private Visibility _currentTagsetVisibility = Visibility.Hidden;
        public Visibility CurrentTagsetVisibility
        {
            get => _currentTagsetVisibility;
            set => SetValue(ref _currentTagsetVisibility, value);
        }

        private Visibility _bottomMenuVisibility = Visibility.Hidden;
        public Visibility BottomMenuVisibility
        {
            get => _bottomMenuVisibility;
            set => SetValue(ref _bottomMenuVisibility, value);
        }

        private Visibility _tabControlGridsVisibility = Visibility.Hidden;
        public Visibility TabControlGridsVisibility
        {
            get => _tabControlGridsVisibility;
            set => SetValue(ref _tabControlGridsVisibility, value);
        }

        #endregion

        #region BottomBarItems

        private string _loadedFileInfo = "Not loaded";
        public string LoadedFileInfo
        {
            get => _loadedFileInfo;
            set => SetValue(ref _loadedFileInfo, value);
        }

        private string _tagsetName = "No tagset";
        public string TagsetName
        {
            get => _tagsetName;
            set => SetValue(ref _tagsetName, value);
        }

        private int _messagesCount = 0;
        public int MessagesCount
        {
            get => _messagesCount;
            set
            {
                if (!SetValue(ref _messagesCount, value) || !IsFileLoaded)
                    return;

                LoadedFileInfo = $"{_messagesCount} messages";
            }
        }

        private int _situationsCount = 0;
        public int SituationsCount
        {
            get => _situationsCount;
            set => SetValue(ref _situationsCount, value);
        }

        #endregion

        #region BottomBarCommands

        #region ItemsCommands

        public ICommand SetTagsetNameCommand { get; }
        public bool CanSetTagsetNameCommandExecute(object parameter)
        {
            return parameter != null;
        }
        public void OnSetTagsetNameCommandExecuted(object parameter)
        {
            if (!CanSetTagsetNameCommandExecute(parameter))
                return;

            TagsetName = parameter.ToString();
        }

        public ICommand UpdateSituationCountCommand { get; }
        public bool CanUpdateSituationCountCommandExecute(object parameter)
        {
            return true;
        }
        public void OnUpdateSituationCountCommandExecuted(object parameter)
        {
            if (!CanUpdateSituationCountCommandExecute(parameter))
                return;

            int newCount = parameter is int count
                ? count
                : SituationIndex.GetInstance().ItemCount;

            SituationsCount = newCount;
        }

        #endregion

        #region FilterCommands

        public ICommand ChooseTagForFilterCommand { get; }
        public bool CanChooseTagForFilterCommandExecute(object parameter)
        {
            return false;
        }
        public void OnChooseTagForFilterCommandExecuted(object parameter)
        {
            if (!CanChooseTagForFilterCommandExecute(parameter))
                return;
        }

        public ICommand SetTaggedOnlyParamForFilterCommand { get; }
        public bool CanSetTaggedOnlyParamForFilterCommandExecute(object parameter)
        {
            return false;
        }
        public void OnSetTaggedOnlyParamForFilterCommandExecuted(object parameter)
        {
            if (!CanSetTaggedOnlyParamForFilterCommandExecute(parameter))
                return;
        }

        #endregion

        #region SaveFileCommands

        public ICommand SaveFileCommand { get; }
        public bool CanSaveFileCommandExecute(object parameter)
        {
            return false;
        }
        public void OnSaveFileCommandExecuted(object parameter)
        {
            if (!CanSaveFileCommandExecute(parameter))
                return;
        }

        public ICommand WriteFileToDiskCommand { get; }
        public bool CanWriteFileToDiskCommandExecute(object parameter)
        {
            return false;
        }
        public void OnWriteFileToDiskCommandExecuted(object parameter)
        {
            if (!CanWriteFileToDiskCommandExecute(parameter))
                return;
        }

        #endregion

        #region SuggesterCommands

        public ICommand ShowSuggesterCommand { get; }
        public bool CanShowSuggesterCommandExecute(object parameter)
        {
            return false;
        }
        public void OnShowSuggesterCommandExecuted(object parameter)
        {
            if (!CanShowSuggesterCommandExecute(parameter))
                return;
        }

        #endregion

        #region TagsetEditorCommands

        public ICommand ShowTagsetEditorCommand { get; }
        public bool CanShowTagsetEditorCommandExecute(object parameter)
        {
            return false;
        }
        public void OnShowTagsetEditorCommandExecuted(object parameter)
        {
            if (!CanShowTagsetEditorCommandExecute(parameter))
                return;
        }

        #endregion

        #endregion

        #region TopBarCommands

        #region IndexFileCommands

        public ICommand IndexNewFileCommand { get; }
        public bool CanIndexNewFileCommandExecute(object parameter)
        {
            return true;
        }
        public void OnIndexNewFileCommandExecuted(object parameter)
        {
            if (!CanIndexNewFileCommandExecute(parameter))
                return;

            if (IndexFileWindow != null)
            {
                new WindowInteract(IndexFileWindow).MoveToForeground();
                return;
            }

            if (!DialogProvider.GetCsvFilePath(out string path))
                return;

            IndexFileWindowViewModel indexFileWindowVM;

            try
            {
                indexFileWindowVM = new IndexFileWindowViewModel(path)
                {
                    FinishAction = () => FileLoaded(),
                    DeactivateAction = () => IndexFileWindow = null
                };
            }
            catch (OneProjectOnlyException ex)
            {
                new QuickMessage(ex.Message).ShowInformation();
                return;
            }
            catch (Exception ex)
            {
                new QuickMessage($"Failed to upload the file.\nComment: {ex.Message}").ShowError();
                return;
            }

            IndexFileWindow = new IndexFileWindow(indexFileWindowVM);
            IndexFileWindow.Show();
        }

        public ICommand OpenCorpusCommand { get; }
        public bool CanOpenCorpusCommandExecute(object parameter)
        {
            return true;
        }
        public void OnOpenCorpusCommandExecuted(object parameter)
        {
            if (!CanOpenCorpusCommandExecute(parameter))
                return;

            if (!DialogProvider.GetFolderPath(out string path))
                return;

            SituationIndex.GetInstance().UnloadData();
            ProjectInfo.LoadProject(path);

            if (!LuceneService.OpenIndex())
            {
                new QuickMessage("No index").ShowError();
                return;
            }

            string dirName = System.IO.Path.GetDirectoryName(path);
            ProjectInteraction.ProjectInfo = new ProjectInformation(dirName, path);

            FileLoaded();
        }

        #endregion

        #region PlotCommands

        public ICommand ShowPlotCommand { get; }
        public bool CanShowPlotCommandExecute(object parameter)
        {
            return false;
        }
        public void OnShowPlotCommandExecuted(object parameter)
        {
            if (!CanShowPlotCommandExecute(parameter))
                return;
        }

        #endregion

        #region HeatmapCommands

        public ICommand ShowHeatmapCommand { get; }
        public bool CanShowHeatmapCommandExecute(object parameter)
        {
            return false;
        }
        public void OnShowHeatmapCommandExecuted(object parameter)
        {
            if (!CanShowHeatmapCommandExecute(parameter))
                return;
        }

        #endregion

        #region ExtractFileCommands

        public ICommand ExtractFileCommand { get; }
        public bool CanExtractFileCommandExecute(object parameter)
        {
            return false;
        }
        public void OnExtractFileCommandExecuted(object parameter)
        {
            if (!CanExtractFileCommandExecute(parameter))
                return;
        }

        #endregion

        #endregion

        #region WindowsCommands

        public ICommand MainWindowClosingCommand { get; }
        public bool CanMainWindowClosingCommandExecute(object parameter)
        {
            return true;
        }
        public void OnMainWindowClosingCommandExecuted(object parameter)
        {
            if (!CanMainWindowClosingCommandExecute(parameter))
                return;

            CloseIndexFileWindowCommand?.Execute(null);
            CloseMessageExplorerWindowsCommand?.Execute(null);
        }

        public ICommand CloseIndexFileWindowCommand { get; }
        public bool CanCloseIndexFileWindowCommandExecute(object parameter)
        {
            return IndexFileWindow != null;
        }
        public void OnCloseIndexFileWindowCommandExecuted(object parameter)
        {
            if (!CanCloseIndexFileWindowCommandExecute(parameter))
                return;

            IndexFileWindow.Close();
        }

        public ICommand CloseMessageExplorerWindowsCommand { get; }
        public bool CanCloseMessageExplorerWindowsCommandExecute(object parameter)
        {
            return true;
        }
        public void OnCloseMessageExplorerWindowsCommandExecuted(object parameter)
        {
            if (!CanCloseMessageExplorerWindowsCommandExecute(parameter))
                return;

            MessageExplorerWindowViewModel.CloseAllExplorers();
        }

        #endregion

        public MainWindowViewModel()
        {
            ChatVM = new ChatViewModel(this);

            IndexNewFileCommand = new RelayCommand(OnIndexNewFileCommandExecuted, CanIndexNewFileCommandExecute);
            OpenCorpusCommand = new RelayCommand(OnOpenCorpusCommandExecuted, CanOpenCorpusCommandExecute);

            ShowPlotCommand = new RelayCommand(OnShowPlotCommandExecuted, CanShowPlotCommandExecute);
            ShowHeatmapCommand = new RelayCommand(OnShowHeatmapCommandExecuted, CanShowHeatmapCommandExecute);
            ExtractFileCommand = new RelayCommand(OnExtractFileCommandExecuted, CanExtractFileCommandExecute);

            SetTagsetNameCommand = new RelayCommand(OnSetTagsetNameCommandExecuted, CanSetTagsetNameCommandExecute);
            UpdateSituationCountCommand = new RelayCommand(OnUpdateSituationCountCommandExecuted, CanUpdateSituationCountCommandExecute);

            ChooseTagForFilterCommand = new RelayCommand(OnChooseTagForFilterCommandExecuted, CanChooseTagForFilterCommandExecute);
            SetTaggedOnlyParamForFilterCommand = new RelayCommand(OnSetTaggedOnlyParamForFilterCommandExecuted, CanSetTaggedOnlyParamForFilterCommandExecute);

            WriteFileToDiskCommand = new RelayCommand(OnWriteFileToDiskCommandExecuted, CanWriteFileToDiskCommandExecute);
            SaveFileCommand = new RelayCommand(OnSaveFileCommandExecuted, CanSaveFileCommandExecute);

            ShowSuggesterCommand = new RelayCommand(OnShowSuggesterCommandExecuted, CanShowSuggesterCommandExecute);
            ShowTagsetEditorCommand = new RelayCommand(OnShowTagsetEditorCommandExecuted, CanShowTagsetEditorCommandExecute);

            MainWindowClosingCommand = new RelayCommand(OnMainWindowClosingCommandExecuted, CanMainWindowClosingCommandExecute);
            CloseIndexFileWindowCommand = new RelayCommand(OnCloseIndexFileWindowCommandExecuted, CanCloseIndexFileWindowCommandExecute);
            CloseMessageExplorerWindowsCommand = new RelayCommand(OnCloseMessageExplorerWindowsCommandExecuted, CanCloseMessageExplorerWindowsCommandExecute);
        }

        private void FileLoaded()
        {
            IsFileLoaded = true;
            ResetChatData();
        }

        private void ResetChatData()
        {
            ChatVM.ClearData();
            ChatVM.SetChatColumnsCommand?.Execute(null);

            ChatVM.MessagesVM.SetMessagesCommand?.Execute(null);

            ChatVM.TagsVM.SetTagsCommand?.Execute(null);
            ChatVM.DatesVM.SetDatesCommand?.Execute(null);
            ChatVM.SituationsVM.SetSituationsCommand?.Execute(null);
            ChatVM.UsersVM.SetUsersCommand?.Execute(null);

            MessagesCount = ProjectInfo.Data.LineCount;
        }
    }
}
