﻿using Modules.Common.DataBinding;
using Modules.Common.Helpers;
using Modules.Common.ViewModel;
using PropertyChanged;
using System.Windows.Input;

namespace Modules.Common.Views.Controls;

[AddINotifyPropertyChangedInterface]
public class SearchBoxViewModel : BaseViewModel
{
    private string _searchText = string.Empty;
    private List<string> _searchTerms = [];
    private bool _isSearchBoxOpen;

    public SearchBoxViewModel()
    {
        CloseSearchBoxCommand = new RelayCommand(() =>
        {
            IsSearchBoxOpen = false;
            SearchText = string.Empty;
        });
    }

    public event Action? SearchTermsChanged;

    public HashSet<string> SearchTerms { get; private set; } = new();

    public ICommand CloseSearchBoxCommand { get; }

    public bool IsSearchBoxOpen
    {
        get => _isSearchBoxOpen;
        set
        {
            if (value == false)
            {
                SearchText = string.Empty;
            }

            _isSearchBoxOpen = value;
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (!IsSearchBoxOpen) return;

            _searchText = value;

            var newSearchTerms = _searchText.GetSearchTermsList();
            if (!newSearchTerms.SequenceEqual(_searchTerms))
            {
                SearchTerms = SearchText.GetSearchTerms();
                SearchTermsChanged?.Invoke();
            }

            _searchTerms = newSearchTerms;
        }
    }

}