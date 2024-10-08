﻿namespace Modules.Common
{
    public static class Constants
    {
        public const string Transparent = "Transparent";
        public const string Zero = "0";
        public const string DatabaseFileExtension = "db";
        public const string CurrentVersion = "CurrentVersion";
        public const string SortableDateFormat = "yyyy-MM-dd__HH:mm:ss__ffffff";
        public const int RecycleBinCategoryId = -1000;
        public const int DefaultListOrder = 0;

        public struct CategoryName
        {
            public const string RecycleBin = "Recycle bin";
            public const string Today = "Today";
        }

        public struct ColorName
        {
            public const string Transparent = "Transparent";
        }

        public struct BrushName
        {
            public const string HatchBrush = "HatchBrush";
            public const string TaskBgBrush = "Surface1";
            public const string TransparentPatternBrush = "TransparentPatternBrush";
            public const string ForegroundBrush = "OnBackground";
            public const string Surface1 = "Surface1";
            public const string Surface2 = "Surface2";
            public const string Surface3 = "Surface3";
        }

        public struct FontFamily
        {
            public const string Calibri = "Calibri";
            public const string Consolas = "Consolas";
            public const string CourierNew = "Courier New";
            public const string SegoeUILight = "Segoe UI Light";
            public const string SegoeUI = "Segoe UI";
            public const string SegoeUIBold = "Segoe UI Bold";
            public const string Tahoma = "Tahoma";
            public const string TimesNewRoman = "Times New Roman";
            public const string Verdana = "Verdana";
        }

        public struct ResourceNames
        {
            public const string TextEditorToolbar = "TextEditorToolbar";
            public const string ColorPickerPopup = "ColorPickerPopup";
            public const string OutlineVariant = "OutlineVariant";
        }

        public struct TableNames
        {
            public const string Task = "Task";
            public const string Category = "Category";
            public const string Note = "Note";
            public const string Setting = "Setting";
        }

        // Keys in the App.config
        public struct ConfigKeys
        {
            public const string DatabaseDirectory = "DatabaseDirectory";
            public const string DatabaseFileName = "DatabaseFileName";
        }
    }
}
