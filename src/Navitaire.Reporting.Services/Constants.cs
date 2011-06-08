using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Navitaire.Reporting.Services
{
    public static class Constants
    {
        public static class AvailableApplicationSettings
        {
            public const string CSVDelimiter = "CSVDelimiter";
            public const string FTPPassword = "FTPPassword";
            public const string FTPPath = "FTPPath";
            public const string FTPServer = "FTPServer";
            public const string FTPUser = "FTPUser";
            public const string MinimumUIYear = "MinimumUIYear";
            public const string SMTPServer = "SMTPServer";
            public const string StartupPageAddress = "StartupPageAddress";
            public const string SupportEmail = "SupportEmail";
            public const string BaseUrl = "BaseUrl";
            public const string SRSBaseUrl = "SRSBaseUrl";
            public const string SRSReportExecutionPage = "SRSReportExecutionPage";
            public const string SRSReportExecutionServicePage = "SRSReportExecutionServicePage";
            public const string SRSReportServicePage = "SRSReportServicePage";
            public const string SRSTargetUrl = "SRSTargetUrl";
        }

        public static class AvailableVersionSettings
        {
            public const string NewSkiesReportsVersion = "NewSkiesReportsVersion";
            public const string NewSkiesDWVersion = "NewSkiesDWVersion";
            public const string SkyLedgerVersion = "SkyLedgerVersion";
            public const string RevenueAccntVersion = "RevenueAccntVersion";
            public const string LoyaltyVersion = "LoyaltyVersion";
            public const string TravelCommerceVersion = "TravelCommerceVersion";
        }

        public static class AvailableSubscriptionSettings
        {
            public const string HeadquartersTimeZone = "ServerTimeZone";
            public const string MaxEntriesPerSubscription = "MaxEntriesPerSubscription";
            public const string MaxReportFileAge = "MaxReportFileAge";
            public const string MaxScheduleFailCount = "MaxScheduleFailCount";
            public const string MaxScheduleQueueAge = "MaxScheduleQueueAge";
            public const string MaxScheduleThreads = "MaxScheduleThreads";
            public const string ScheduleServiceSignature = "Navitaire.Reporting.ScheduleService.exe";
            public const string SubscriptionEmailAttachmentThreshold = "SubscriptionEmailAttachmentThreshold";
            public const string SubscriptionNameFormat = "SubscriptionNameFormat";
            public const string SubscriptionReportTimeout = "SubscriptionReportTimeout";
            public const string SubscriptionZipThreshold = "SubscriptionZipThreshold";
            public const string TimerMaintenancePollingInterval = "TimerMaintenancePollingInterval";
            public const string TimerQueueLoadingInterval = "TimerQueueLoadingInterval";
            public const string TimerQueuePollingInterval = "TimerQueuePollingInterval";
            public const string SubscriptionLogLevel = "SubscriptionLogLevel";
        }

        public static class CatalogItemExtensions
        {
            public const string RDL = ".rdl";
            public const string MSAccess = ".mdb";
            public const string MSExcel = ".xls";
        }

        public static class CatalogItemTypes
        {
            public const string None = "None";
            public const string DataSource = "DataSource";
            public const string Folder = "Folder";
            public const string Home = "Home";
            public const string LinkedReport = "LinkedReport";
            public const string MSAccess = "MSAccess";
            public const string MSExcel = "MSExcel";
            public const string Report = "Report";
        }

        public static class CatalogItemTypeIcons
        {
            public const string Folder = "FolderIcon.gif";
            public const string Datasource = "DataSourceIcon.gif";
            public const string LinkedReport = "LinkedReportIcon.gif";
            public const string Report = "ReportIcon.gif";
        }

        public static class ContextDataKeys
        {
            public const string AutoRefreshEnabled = "AutoRefreshEnabled";
            public const string AutoRefreshInterval = "AutoRefreshInterval";
            public const string AvailableGridColumns = "AvailableGridColumns";
            public const string CurrentCatalogItemInfo = "CurrentCatalogItemInfo";
            public const string CurrentCatalogItemName = "CurrentCatalogItemName";
            public const string FilterColumnName = "FilterColumnName";
            public const string FilterValue = "FilterValue";
            public const string SelectedCatalogItemPath = "SelectedCatalogItemPath";
            public const string ShowCompletedSubscriptions = "ShowCompletedSubscriptions";
            public const string ShowDisabledSubscriptions = "ShowDisabledSubscriptions";
            public const string ShowSuccessfulQueueEntries = "ShowSuccessfulQueueEntries";
            public const string SubscriptionId = "SubscriptionId";
            public const string SubscriptionName = "SubscriptionName";
            public const string SubscriptionUserId = "SubscriptionUserId";
        }

        public static class DayOfMonthAdjustmentTimes
        {
            public const string EndOfDay = "EndOfDay";
            public const string StartOfDay = "StartOfDay";
        }

        public static class EmailTemplates
        {
            public const string EmailDiagnostics = "EmailDiagnostics";
            public const string SubscriptionNotificationAttachment = "SubscriptionNotificationAttachment";
            public const string SubscriptionNotificationAttachmentNoLink = "SubscriptionNotificationAttachmentNoLink";
            public const string SubscriptionNotificationAvailable = "SubscriptionNotificationAvailable";
            public const string SubscriptionNotificationAvailableNoLink = "SubscriptionNotificationAvailableNoLink";
            public const string SubscriptionNotificationDisabled = "SubscriptionNotificationDisabled";
            public const string SubscriptionNotificationDisabledNoLink = "SubscriptionNotificationDisabledNoLink";
            public const string SubscriptionNotificationDisabledSupport = "SubscriptionNotificationDisabledSupport";
            public const string SubscriptionNotificationFtp = "SubscriptionNotificationFtp";
            public const string SubscriptionNotificationFtpNoLink = "SubscriptionNotificationFtpNoLink";
            public const string SubscriptionNotificationEmailBody = "SubscriptionNotificationEmailBody";
            public const string SubscriptionNotificationEmailBodyNoLink = "SubscriptionNotificationEmailBodyNoLink";
            public const string SubscriptionNotificationExpiredAccount = "SubscriptionNotificationExpiredAccount";
        }

        public static class ExportMapCatalogOptions
        {
            public const string ExportOnly = "ExportOnly";
            public const string ExportAndView = "ExportAndView";
        }

        public static class MappingAdjustmentTypes
        {
            public const string Specific = "Specific";
            public const string ScalarNext = "Next";
            public const string ScalarPrevious = "Previous";
            public const string RollingStartOf = "StartOf";
            public const string RollingEndOf = "EndOf";
            public const string DayOfMonth = "DayOfMonth";
        }

        public static class MimeTypes
        {
            public const string MSExcel = "application/vnd.ms-excel";
            public const string MSAccess = "application/x-msaccess";
            public const string RDL = "application/rdl";
            public const string ZIP = "application/zip";
            public const string PDF = "application/pdf";
            public const string CSV = "text/csv";
            public const string MHTML = "multipart/related";
            public const string XML = "text/xml";
            public const string HTML = "text/html";
        }

        public static class RollingAdjustmentPeriods
        {
            public const string Day = "Day";
            public const string Week = "Week";
            public const string Month = "Month";
            public const string Quarter = "Quarter";
            public const string Year = "Year";
        }

        public static class ScalarAdjustmentPeriods
        {
            public const string Days = "Days";
            public const string Weeks = "Weeks";
            public const string Months = "Months";
            public const string Quarters = "Quarters";
            public const string Years = "Years";
        }

        public static class SortDirection
        {
            public const string Ascending = "ASC";
            public const string Descending = "DESC";
        }

        public static class SpecialParameterValues
        {
            public const string NavitaireNull = "_NULL_";
        }

        public static class SubscriptionFileExtensions
        {
            public const string ZIP = "zip";
            public const string PDF = "pdf";
            public const string CSV = "csv";
            public const string MHTML = "mhtml";
            public const string XML = "xml";
            public const string HTML = "mht";
        }

        public static class SubscriptionNameFormats
        {
            public const string ReportDate = "ReportDate";
            public const string ReportNameDate = "ReportNameDate";
            public const string NameDate = "NameDate";
        }
    }
}
