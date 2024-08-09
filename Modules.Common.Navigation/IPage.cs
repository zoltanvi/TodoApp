namespace Modules.Common.Navigation;

public interface IPage;

public interface ICategoryListPage : IPage;
public interface IEmptyPage : IPage;
public interface ISettingsPage : IPage;
public interface IRecycleBinPage : IPage;
public interface ITagSelectorPage : IPage;
public interface ITaskHistoryPage : IPage;
public interface ITaskPage : IPage;

// Setting pages
public interface IApplicationSettingsPage : IPage;
public interface IThemeSettingsPage : IPage;
public interface IPageTitleSettingsPage : IPage;
public interface ITaskPageSettingsPage : IPage;
public interface ITaskItemSettingsPage : IPage;
public interface ITagSettingsPage : IPage;
public interface ITaskQuickActionsSettingsPage : IPage;
public interface ITextEditorQuickActionsSettingsPage : IPage;
public interface IDateTimeSettingsPage : IPage;
public interface IShortcutsPage : IPage;
